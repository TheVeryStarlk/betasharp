using System.Net;
using System.Net.Sockets;
using BetaSharp.Server.Network;

namespace BetaSharp.Server.Threading;

public class AcceptConnectionThread : java.lang.Thread
{
    private readonly ConnectionListener _listener;

    public AcceptConnectionThread(ConnectionListener listener, string name) : base(name)
    {
        this._listener = listener;
    }

    public override void run()
    {
        Dictionary<EndPoint, long> map = [];

        while (_listener.open)
        {
            try
            {
                Socket socket = _listener.socket.Accept();
                if (socket != null)
                {
                    EndPoint addr = socket.RemoteEndPoint;
                    if (map.ContainsKey(addr) && !((IPEndPoint)addr).Address.Equals("127.0.0.1") && java.lang.System.currentTimeMillis() - map[addr] < 5000L)
                    {
                        map[addr] = java.lang.System.currentTimeMillis();
                        socket.Close();
                    }
                    else
                    {
                        map[addr] = java.lang.System.currentTimeMillis();
                        ServerLoginNetworkHandler handler = new(_listener.server, socket, "Connection # " + _listener.connectionCounter);
                        _listener.AddPendingConnection(handler);
                    }
                }
            }
            catch (java.io.IOException var5)
            {
                var5.printStackTrace();
            }
        }
    }
}
