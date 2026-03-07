using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using BetaSharp.Network.Packets;
using BetaSharp.Threading;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Network;

public class Connection
{
    private static readonly int[] s_totalReadSize = new int[256];
    private static readonly int[] s_totalSendSize = new int[256];

    public bool BetaSharpClient { get; set; }

    protected bool IsOpen { get; set; } = true;
    protected ConcurrentQueue<Packet> ReadQueue { get; } = [];
    protected NetHandler? NetworkHandler { get; set; }
    protected bool IsClosed { get; set; }
    protected bool IsDisconnected { get; set; }
    protected string DisconnectedReason { get; set; } = string.Empty;
    protected object[]? DisconnectReasonArgs { get; set; }

    private readonly object _lock = new();
    private Socket? _socket;
    private readonly ConcurrentQueue<Packet> _sendQueue = [];
    private readonly ConcurrentQueue<Packet> _delayedSendQueue = [];
    private int _timeout;
    private int _delay;
    private int _sendQueueSize;
    private NetworkStream? _networkStream;

    private readonly ILogger<Connection> _logger = Log.Instance.For<Connection>();
    private readonly IPEndPoint? _address;
    private readonly java.lang.Thread _writer;
    private readonly java.lang.Thread _reader;
    private readonly ManualResetEventSlim _wakeSignal = new(false);

    public Connection(Socket socket, string address, NetHandler networkHandler)
    {
        _socket = socket;
        _address = (IPEndPoint?)socket.RemoteEndPoint;
        NetworkHandler = networkHandler;

        socket.ReceiveTimeout = 30000;
        // setTrafficClass doesn't have a direct .NET equivalent and can be omitted

        _networkStream = new NetworkStream(socket);

        _reader = new NetworkReaderThread(this, address + " read thread");
        _writer = new NetworkWriterThread(this, address + " write thread");
        _reader.start();
        _writer.start();
    }

    protected Connection()
    {
        _address = null;
    }

    public void setNetworkHandler(NetHandler netHandler)
    {
        NetworkHandler = netHandler;
    }

    public virtual void sendPacket(Packet packet)
    {
        if (packet is ExtendedProtocolPacket && !BetaSharpClient) return;

        if (!IsClosed)
        {
            object lockObj = _lock;
            lock (lockObj)
            {
                _sendQueueSize += packet.Size() + 1;
                if (Packet.Registry[packet.Id]!.WorldPacket)
                {
                    _delayedSendQueue.Enqueue(packet);
                }
                else
                {
                    _sendQueue.Enqueue(packet);
                }
            }
        }
    }

    protected virtual bool write()
    {
        if (_networkStream == null)
        {
            throw new Exception("Connection not initialized");
        }

        bool wrotePacket = false;

        try
        {
            int[] sizeStats;
            Packet? packet;
            object lockObj;

            if (!_sendQueue.IsEmpty)
            {
                lockObj = _lock;
                lock (lockObj)
                {
                    if (!_sendQueue.TryDequeue(out packet))
                    {
                        return false;
                    }

                    _sendQueueSize -= packet.Size() + 1;
                }

                Packet.Write(packet, _networkStream);
                sizeStats = s_totalSendSize;
                sizeStats[packet.Id] += packet.Size() + 1;
                wrotePacket = true;
                packet.Return();
            }

            if (!_delayedSendQueue.IsEmpty &&  _delay-- <= 0)
            {
                lockObj = _lock;
                lock (lockObj)
                {
                    if (!_delayedSendQueue.TryDequeue(out packet))
                    {
                        return false;
                    }

                    _sendQueueSize -= packet.Size() + 1;
                }

                Packet.Write(packet, _networkStream);
                sizeStats = s_totalSendSize;
                sizeStats[packet.Id] += packet.Size() + 1;
                _delay = 0;
                wrotePacket = true;
                packet.Return();
            }

            return wrotePacket;
        }
        catch (Exception ex)
        {
            if (!IsDisconnected)
            {
                disconnect(ex);
            }

            return false;
        }
    }

    public virtual void interrupt()
    {
        _wakeSignal.Set();
    }

    public void waitForSignal(int timeoutMs)
    {
        _wakeSignal.Wait(timeoutMs);
        _wakeSignal.Reset();
    }

    protected virtual bool read()
    {
        if (NetworkHandler == null || _networkStream == null)
        {
            throw new Exception("Connection not initialized");
        }

        bool receivedPacket = false;

        try
        {
            Packet? packet = Packet.Read(_networkStream, NetworkHandler.isServerSide());
            if (packet != null)
            {
                int[] sizeStats = s_totalReadSize;
                int packetId = packet.Id;
                sizeStats[packetId] += packet.Size() + 1;
                ReadQueue.Enqueue(packet);
                receivedPacket = true;
            }
            else
            {
                disconnect("disconnect.endOfStream");
            }

            return receivedPacket;
        }
        catch (Exception ex)
        {
            if (!IsDisconnected)
            {
                disconnect(ex);
            }

            return false;
        }
    }

    private void disconnect(Exception e)
    {
        _logger.LogError(e, e.Message);
        disconnect("disconnect.genericReason", "Internal exception: " + e);
    }

    public virtual void disconnect(string disconnectedReason, params object[] disconnectReasonArgs)
    {
        if (IsOpen)
        {
            IsDisconnected = true;
            this.DisconnectedReason = disconnectedReason;
            this.DisconnectReasonArgs = disconnectReasonArgs;
            new NetworkMasterThread(this).start();
            IsOpen = false;

            try
            {
                _networkStream?.Close();
                _networkStream = null;

                _socket?.Close();
                _socket = null;
            }
            catch (Exception)
            {
                // Ignore.
            }
        }
    }

    public virtual void tick()
    {
        if (_sendQueueSize > 1048576)
        {
            disconnect("disconnect.overflow");
        }

        if (ReadQueue.IsEmpty)
        {
            if (_timeout++ == 1200)
            {
                disconnect("disconnect.timeout");
            }
        }
        else
        {
            _timeout = 0;
        }

        processPackets();

        interrupt();
        if (IsDisconnected && ReadQueue.IsEmpty)
        {
            NetworkHandler?.onDisconnected(DisconnectedReason, DisconnectReasonArgs);
        }
    }

    protected virtual void processPackets()
    {
        if (NetworkHandler == null)
        {
            throw new Exception("networkHandler is null");
        }

        int maxPacketsPerTick = 100;

        while (!ReadQueue.IsEmpty && maxPacketsPerTick-- >= 0)
        {
            if (!ReadQueue.TryDequeue(out var packet))
            {
                continue;
            }

            packet.Apply(NetworkHandler);
            packet.Return();
        }
    }

    public virtual IPEndPoint? getAddress()
    {
        return _address;
    }

    public virtual void disconnect()
    {
        interrupt();
        IsClosed = true;
        new ThreadCloseConnection(this).Start();
    }

    public int getDelayedSendQueueSize()
    {
        return _delayedSendQueue.Count;
    }

    public static bool isOpen(Connection conn)
    {
        return conn.IsOpen;
    }

    public static bool isClosed(Connection conn)
    {
        return conn.IsClosed;
    }

    public static bool readPacket(Connection conn)
    {
        return conn.read();
    }

    public static bool writePacket(Connection conn)
    {
        return conn.write();
    }

    public static NetworkStream? getOutputStream(Connection conn)
    {
        return conn._networkStream;
    }

    public static bool isDisconnected(Connection conn)
    {
        return conn.IsDisconnected;
    }

    public static void disconnect(Connection conn, Exception ex)
    {
        conn.disconnect(ex);
    }

    public static java.lang.Thread getReader(Connection conn)
    {
        return conn._reader;
    }

    public static java.lang.Thread getWriter(Connection conn)
    {
        return conn._writer;
    }
}
