using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using BetaSharp.Network.Packets;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Network;

public class Connection
{
    public int DelayedSendQueueLength => _delayedSendQueue.Count;

    public virtual IPEndPoint? Address { get; }

    public bool BetaSharpClient { get; set; }
    public NetworkHandler? NetworkHandler { get; set; }

    protected ConcurrentQueue<Packet> ReadQueue { get; } = [];
    protected bool IsDisconnected { get; set; }
    protected string DisconnectedReason { get; set; } = string.Empty;
    protected Exception? DisconnectedException { get; set; }

    private int _timeout;
    private int _delay;

    private readonly Socket? _socket;
    private readonly ConcurrentQueue<Packet> _sendQueue = [];
    private readonly ConcurrentQueue<Packet> _delayedSendQueue = [];
    private readonly ILogger<Connection> _logger = Log.Instance.For<Connection>();
    private readonly NetworkStream? _networkStream;

    public Connection(Socket socket, NetworkHandler networkHandler)
    {
        _socket = socket;

        Address = (IPEndPoint?)socket.RemoteEndPoint;
        _networkStream = new NetworkStream(socket);
        NetworkHandler = networkHandler;

        socket.ReceiveTimeout = 30000;

        Task.Factory.StartNew(Reading, TaskCreationOptions.LongRunning);
        Task.Factory.StartNew(Writing, TaskCreationOptions.LongRunning);
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

        ProcessPackets();

        if (IsDisconnected && ReadQueue.IsEmpty)
        {
            NetworkHandler?.onDisconnected(DisconnectedReason, DisconnectedException);
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
            _networkStream?.Close();
            _socket?.Close();
        }
        catch (Exception)
        {
            // Ignore.
        }
    }

    protected virtual void ProcessPackets()
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

    private void Reading()
    {
        while (!IsDisconnected)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_networkStream);
                ArgumentNullException.ThrowIfNull(NetworkHandler);

                Packet? packet = Packet.Read(_networkStream, NetworkHandler.isServerSide());

                if (packet is not null)
                {
                    ReadQueue.Enqueue(packet);
                }
                else
                {
                    disconnect("disconnect.endOfStream");
                    break;
                }

                Task.Delay(10);
            }
            catch (Exception exception)
            {
                disconnect(exception: exception);
                break;
            }
        }
    }

    private void Writing()
    {
        while (!IsDisconnected)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(_networkStream);

                Packet? packet;

                if (!_sendQueue.IsEmpty)
                {
                    if (!_sendQueue.TryDequeue(out packet))
                    {
                        continue;
                    }

                    packet.UseCount++;

                    Packet.Write(packet, _networkStream);
                    packet.Return();
                }

                if (!_delayedSendQueue.IsEmpty && _delay-- <= 0)
                {
                    if (!_delayedSendQueue.TryDequeue(out packet))
                    {
                        continue;
                    }

                    packet.UseCount++;

                    _delay = 0;

                    Packet.Write(packet, _networkStream);
                    packet.Return();
                }

                _networkStream.Flush();
            }
            catch (Exception exception)
            {
                if (!IsDisconnected)
                {
                    disconnect(exception: exception);
                }

                break;
            }
        }
    }
}
