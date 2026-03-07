using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using BetaSharp.Network.Packets;
using Microsoft.Extensions.Logging;
using Thread = java.lang.Thread;

namespace BetaSharp.Network;

public class Connection
{
    public int DelayedSendQueueLength => _delayedSendQueue.Count;

    public virtual IPEndPoint? Address { get; }
    public Thread? Reader { get; }
    public Thread? Writer { get; }
    public NetworkStream? NetworkStream { get; }

    public bool BetaSharpClient { get; set; }
    public bool IsDisconnected { get; set; }
    public NetworkHandler? NetworkHandler { get; set; }

    protected ConcurrentQueue<Packet> ReadQueue { get; } = [];
    protected string DisconnectedReason { get; set; } = string.Empty;
    protected Exception? DisconnectedException { get; set; }

    private int _timeout;
    private int _delay;

    private readonly Socket? _socket;
    private readonly ConcurrentQueue<Packet> _sendQueue = [];
    private readonly ConcurrentQueue<Packet> _delayedSendQueue = [];
    private readonly ILogger<Connection> _logger = Log.Instance.For<Connection>();

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
    }

    public virtual void queuePacket(Packet packet)
    {
        if (IsDisconnected || !BetaSharpClient && packet is ExtendedProtocolPacket)
        {
            return;
        }

        if (Packet.Registry[packet.Id]!.WorldPacket)
        {
            _delayedSendQueue.Enqueue(packet);
        }
        else
        {
            _sendQueue.Enqueue(packet);
        }
    }

    public virtual bool write()
    {
        ArgumentNullException.ThrowIfNull(NetworkStream);

        bool wrote = false;

        try
        {
            Packet? packet;

            if (!_sendQueue.IsEmpty)
            {
                if (!_sendQueue.TryDequeue(out packet))
                {
                    return false;
                }

                Packet.Write(packet, NetworkStream);

                wrote = true;
                packet.Return();
            }

            if (!_delayedSendQueue.IsEmpty && _delay-- <= 0)
            {
                if (!_delayedSendQueue.TryDequeue(out packet))
                {
                    return false;
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
                disconnect(exception: exception);
            }

            return false;
        }
    }

    public virtual bool read()
    {
        ArgumentNullException.ThrowIfNull(NetworkStream);
        ArgumentNullException.ThrowIfNull(NetworkHandler);

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
                disconnect(exception: exception);
            }

            return false;
        }
    }

    public virtual void tick()
    {
        if (_sendQueue.Count > 1048576)
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

        if (IsDisconnected && ReadQueue.IsEmpty)
        {
            NetworkHandler?.onDisconnected(DisconnectedReason, DisconnectedException);
        }
    }

    protected virtual void processPackets()
    {
        ArgumentNullException.ThrowIfNull(NetworkHandler);

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

    public virtual void disconnect(string reason = "disconnect.genericReason", Exception? exception = null)
    {
        if (exception is not null)
        {
            _logger.LogError(exception, "An exception has occurred and connection had to be terminated");
        }

        exception ??= new Exception("disconnect.closed");

        if (IsDisconnected)
        {
            return;
        }

        IsDisconnected = true;

        DisconnectedReason = reason;
        DisconnectedException = exception;

        try
        {
            NetworkStream?.Close();
            _socket?.Close();
        }
        catch (Exception)
        {
            // Ignore.
        }
    }
}
