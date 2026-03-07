using System.Net;
using BetaSharp.Network.Packets;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Network;

public class InternalConnection : Connection
{
    public InternalConnection RemoteConnection { get; set; }

    public string Name { get; set; }

    private readonly ILogger<InternalConnection> _logger = Log.Instance.For<InternalConnection>();

    public InternalConnection(NetworkHandler? networkHandler, string name)
    {
        this.NetworkHandler = networkHandler;
        Name = name;
    }

    public void AssignRemote(InternalConnection remote)
    {
        RemoteConnection = remote;
    }

    public override void sendPacket(Packet packet)
    {
        if (!IsDisconnected)
        {
            packet.ProcessForInternal();

            if (RemoteConnection != null && !RemoteConnection.IsDisconnected)
            {
                RemoteConnection.ReceivePacket(packet);
            }
        }
    }

    protected void ReceivePacket(Packet packet)
    {
        ReadQueue.Enqueue(packet);
    }

    protected override void processPackets()
    {
        if (NetworkHandler == null)
        {
            throw new Exception($"InternalConnection is not initialized");
        }

        int count = 0;
        while (!ReadQueue.IsEmpty)
        {
            if (!ReadQueue.TryDequeue(out var packet))
            {
                continue;
            }

            packet.Apply(NetworkHandler);
            packet.Return();

            count++;
        }
        if (count > 0)
        {
            // _logger.LogInformation($"[{Name}] Processed {count} packets");
        }
    }

    public override bool write()
    {
        return false;
    }

    public override bool read()
    {
        return false;
    }

    public override void disconnect(string disconnectedReason, Exception? disconnectedException = null)
    {
        if (!IsDisconnected)
        {
            IsDisconnected = true;

            DisconnectedReason = disconnectedReason;
            DisconnectedException = disconnectedException;

            _logger.LogInformation($"[{Name}] Disconnected: {disconnectedReason}");

            if (RemoteConnection != null && !RemoteConnection.IsDisconnected)
            {
                RemoteConnection.OnRemoteDisconnect(disconnectedReason, disconnectedException);
            }
        }
    }

    public void OnRemoteDisconnect(string reason, Exception? disconnectedException)
    {
        if (!IsDisconnected)
        {
            IsDisconnected = true;
            DisconnectedReason = reason;
            DisconnectedException = disconnectedException;
            _logger.LogInformation($"[{Name}] Remote disconnected: {reason}");
        }
    }

    public override void disconnect()
    {
        disconnect("Disconnecting");
    }

    public override void interrupt()
    {
    }

    public override void tick()
    {
        processPackets();
        if (IsDisconnected && ReadQueue.IsEmpty)
        {
            NetworkHandler?.onDisconnected(DisconnectedReason, DisconnectedException);
        }
    }

    public override IPEndPoint? Address { get; } = new (IPAddress.Parse("127.0.0.1"), 12345);
}
