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

                while (Connection.writePacket(connection))
                {
                }

                connection.waitForSignal(10);

                try
                {
                    Connection.getOutputStream(connection)?.Flush();
                }
                catch (java.io.IOException ex)
                {
                    if (!Connection.isDisconnected(connection))
                    {
                        Connection.disconnect(connection, ex);
                        ex.printStackTrace();
                    }
                }
            }
            finally
            {
            }
        }
    }
}
