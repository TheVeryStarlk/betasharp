using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using BetaSharp.Network.Packets;
using BetaSharp.Threading;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Network;

public class Connection
{
    public virtual IPEndPoint? Address { get; }
    public java.lang.Thread Reader { get; }
    public java.lang.Thread Writer { get; }
    public NetworkStream NetworkStream { get; }

    public bool BetaSharpClient { get; set; }
    public bool IsDisconnected { get; set; }

    protected ConcurrentQueue<Packet> ReadQueue { get; } = [];
    protected NetworkHandler? NetworkHandler { get; set; }
    protected string DisconnectedReason { get; set; } = string.Empty;
    protected object[]? DisconnectedReasonArgs { get; set; }

    private readonly Lock _lock = new();
    private Socket? _socket;
    private readonly ConcurrentQueue<Packet> _sendQueue = [];
    private readonly ConcurrentQueue<Packet> _delayedSendQueue = [];
    private int _timeout;
    private int _delay;
    private int _sendQueueSize;

    private readonly ILogger<Connection> _logger = Log.Instance.For<Connection>();
    private readonly ManualResetEventSlim _wakeSignal = new(false);

    public Connection(Socket socket, string address, NetworkHandler networkHandler)
    {
        _socket = socket;

        Address = (IPEndPoint?)socket.RemoteEndPoint;
        NetworkHandler = networkHandler;

        socket.ReceiveTimeout = 30000;

        NetworkStream = new NetworkStream(socket);

        Reader = new NetworkReaderThread(this, address + " read thread");
        Writer = new NetworkWriterThread(this, address + " write thread");

        Reader.start();
        Writer.start();
    }

    protected Connection()
    {
        Address = null;
    }

    public void setNetworkHandler(NetworkHandler networkHandler)
    {
        NetworkHandler = networkHandler;
    }

    public virtual void sendPacket(Packet packet)
    {
        if (IsDisconnected || !BetaSharpClient && packet is ExtendedProtocolPacket)
        {
            return;
        }

        lock (_lock)
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

    public virtual bool write()
    {
        if (NetworkStream == null)
        {
            throw new Exception("Connection not initialized");
        }

        bool wrote = false;

        try
        {
            Packet? packet;

            if (!_sendQueue.IsEmpty)
            {
                lock (_lock)
                {
                    if (!_sendQueue.TryDequeue(out packet))
                    {
                        return false;
                    }

                    _sendQueueSize -= packet.Size() + 1;
                }

                Packet.Write(packet, NetworkStream);

                wrote = true;
                packet.Return();
            }

            if (!_delayedSendQueue.IsEmpty && _delay-- <= 0)
            {
                lock (_lock)
                {
                    if (!_delayedSendQueue.TryDequeue(out packet))
                    {
                        return false;
                    }

                    _sendQueueSize -= packet.Size() + 1;
                }

                Packet.Write(packet, NetworkStream);

                _delay = 0;
                wrote = true;
                packet.Return();
            }

            return wrote;
        }
        catch (Exception exception)
        {
            if (!IsDisconnected)
            {
                disconnect(exception);
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

    public virtual bool read()
    {
        if (NetworkHandler == null || NetworkStream == null)
        {
            throw new Exception("Connection not initialized");
        }

        bool received = false;

        try
        {
            Packet? packet = Packet.Read(NetworkStream, NetworkHandler.isServerSide());

            if (packet is not null)
            {
                ReadQueue.Enqueue(packet);
                received = true;
            }
            else
            {
                disconnect("disconnect.endOfStream");
            }

            return received;
        }
        catch (Exception exception)
        {
            if (!IsDisconnected)
            {
                disconnect(exception);
            }

            return false;
        }
    }

    public void disconnect(Exception exception)
    {
        _logger.LogError(exception, "An exception has occured and connection had to be terminated");
        disconnect("disconnect.genericReason", $"Internal exception: {exception}");
    }

    public virtual void disconnect(string disconnectedReason, params object[] disconnectedReasonArgs)
    {
        if (IsDisconnected)
        {
            return;
        }

        IsDisconnected = true;

        DisconnectedReason = disconnectedReason;
        DisconnectedReasonArgs = disconnectedReasonArgs;

        new NetworkMasterThread(this).start();

        try
        {
            NetworkStream.Close();

            _socket?.Close();
            _socket = null;
        }
        catch (Exception)
        {
            // Ignore.
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
            NetworkHandler?.onDisconnected(DisconnectedReason, DisconnectedReasonArgs);
        }
    }

    protected virtual void processPackets()
    {
        if (NetworkHandler is null)
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

    public virtual void disconnect()
    {
        interrupt();
        new ThreadCloseConnection(this).Start();
    }

    public int getDelayedSendQueueSize()
    {
        return _delayedSendQueue.Count;
    }
}
