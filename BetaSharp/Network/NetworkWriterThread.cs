namespace BetaSharp.Network;

internal class NetworkWriterThread(Connection connection, string name) : java.lang.Thread(name)
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

                while (connection.write())
                {
                }

                connection.waitForSignal(10);

                try
                {
                    connection.NetworkStream.Flush();
                }
                catch (java.io.IOException exception)
                {
                    if (!connection.IsDisconnected)
                    {
                        connection.disconnect(exception);
                        exception.printStackTrace();
                    }
                }
            }
            finally
            {
            }
        }
    }
}
