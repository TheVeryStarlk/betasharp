namespace BetaSharp.Network;

class NetworkReaderThread : java.lang.Thread
{
    public readonly Connection netManager;

    public NetworkReaderThread(Connection var1, string var2) : base(var2)
    {
        this.netManager = var1;
    }

    public override void run()
    {

        while (true)
        {
            try
            {
                if (!Connection.isOpen(this.netManager))
                {
                    break;
                }

                if (Connection.isClosed(this.netManager))
                {
                    break;
                }

                while (Connection.readPacket(this.netManager))
                {
                }

                netManager.waitForSignal(10);
            }
            finally
            {
            }
        }
    }
}
