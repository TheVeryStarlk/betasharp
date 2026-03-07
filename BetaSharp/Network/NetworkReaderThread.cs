namespace BetaSharp.Network;

class NetworkReaderThread(Connection connection, string name) : java.lang.Thread(name)
{
    public override void run()
    {

        while (true)
        {
            try
            {
                if (connection.IsDisconnected)
                {
                    break;
                }

                while (connection.read())
                {
                }

                // connection.waitForSignal(10);
            }
            finally
            {
            }
        }
    }
}
