using System.Net;
using BetaSharp.Network.Packets;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Network;

public class InternalConnection : Connection
{
    public override IPEndPoint? Address { get; } = new(IPAddress.Parse("127.0.0.1"), 12345);

    public InternalConnection? RemoteConnection { get; set; }

    private readonly string _name;
    private readonly ILogger<InternalConnection> _logger = Log.Instance.For<InternalConnection>();

    public InternalConnection(NetworkHandler? networkHandler, string name)
    {
        NetworkHandler = networkHandler;
        _name = name;
    }

    public override void queuePacket(Packet packet)
    {
        if (IsDisconnected)
        {
            return;
        }

        packet.ProcessForInternal();

        if (RemoteConnection is { IsDisconnected: false })
        {
            RemoteConnection.ReadQueue.Enqueue(packet);
        }
    }

    public override void disconnect(string reason = "disconnect.genericReason", Exception? exception = null)
    {
        if (IsDisconnected)
        {
            return;
        }

        IsDisconnected = true;

        DisconnectedReason = reason;
        DisconnectedException = exception;

        _logger.LogInformation("{Name}: Disconnected: {Reason}", _name, reason);

        if (RemoteConnection is { IsDisconnected: false })
        {
            RemoteConnection.OnRemoteDisconnect(reason, exception);
        }
    }

    public override void tick()
    {
        ProcessPackets();

        if (IsDisconnected && ReadQueue.IsEmpty)
        {
            NetworkHandler?.onDisconnected(DisconnectedReason, DisconnectedException);
        }
    }

    protected override void ProcessPackets()
    {
        ArgumentNullException.ThrowIfNull(NetworkHandler);

        while (!ReadQueue.IsEmpty)
        {
            if (!ReadQueue.TryDequeue(out var packet))
            {
                continue;
            }

            packet.Apply(NetworkHandler);
            packet.Return();
        }
    }

    private void OnRemoteDisconnect(string reason, Exception? disconnectedException)
    {
        if (IsDisconnected)
        {
            return;
        }

        IsDisconnected = true;

        DisconnectedReason = reason;
        DisconnectedException = disconnectedException;

        _logger.LogInformation("{Name}: Disconnected: {Reason}", _name, reason);
    }
}
