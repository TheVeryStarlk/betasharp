using java.lang;

namespace BetaSharp.Network;

internal class NetworkMasterThread(Connection connection) : java.lang.Thread
{

    public override void run()
    {
        try
        {
            sleep(5000L);

            if (connection.Reader.isAlive())
            {
                try
                {
                    connection.Reader.stop();
                }
                catch
                {
                }
            }

            if (connection.Writer.isAlive())
            {
                try
                {
                    connection.Writer.stop();
                }
                catch
                {
                }
            }
        }
        catch (InterruptedException ex)
        {
            ex.printStackTrace();
        }
    }
}
