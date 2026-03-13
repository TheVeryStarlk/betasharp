using java.io;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Server;

internal class DedicatedPlayerManager : PlayerManager
{
    private readonly ILogger<DedicatedPlayerManager> _logger = Log.Instance.For<DedicatedPlayerManager>();
    private readonly java.io.File BANNED_PLAYERS_FILE;
    private readonly java.io.File BANNED_IPS_FILE;
    private readonly java.io.File OPERATORS_FILE;
    private readonly java.io.File WHITELIST_FILE;

    public DedicatedPlayerManager(BetaSharpServer server) : base(server)
    {
        BANNED_PLAYERS_FILE = server.getFile("banned-players.txt");
        BANNED_IPS_FILE = server.getFile("banned-ips.txt");
        OPERATORS_FILE = server.getFile("ops.txt");
        WHITELIST_FILE = server.getFile("white-list.txt");

        loadBannedPlayers();
        loadBannedIps();
        loadOperators();
        loadWhitelist();
        saveBannedPlayers();
        saveBannedIps();
        saveOperators();
        saveWhitelist();
    }

    protected override void loadBannedPlayers()
    {
        try
        {
            bannedPlayers.Clear();
            BufferedReader reader = new(new FileReader(BANNED_PLAYERS_FILE));
            string line = "";

            while ((line = reader.readLine()) != null)
            {
                bannedPlayers.Add(line.Trim().ToLower());
            }

            reader.close();
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to load ban list: {exception}");
        }
    }

    protected override void saveBannedPlayers()
    {
        try
        {
            PrintWriter writer = new(new FileWriter(BANNED_PLAYERS_FILE, false));

            foreach (string player in bannedPlayers)
            {
                writer.println(player);
            }

            writer.close();
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to save ban list: {exception}");
        }
    }

    protected override void loadBannedIps()
    {
        try
        {
            bannedIps.Clear();
            BufferedReader reader = new(new FileReader(BANNED_IPS_FILE));
            string line = "";

            while ((line = reader.readLine()) != null)
            {
                bannedIps.Add(line.Trim().ToLower());
            }

            reader.close();
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to load ip ban list: {exception}");
        }
    }

    protected override void saveBannedIps()
    {
        try
        {
            PrintWriter writer = new(new FileWriter(BANNED_IPS_FILE, false));

            foreach (string ip in bannedIps)
            {
                writer.println(ip);
            }

            writer.close();
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to save ip ban list: {exception}");
        }
    }

    protected override void loadOperators()
    {
        try
        {
            ops.Clear();
            BufferedReader reader = new(new FileReader(OPERATORS_FILE));
            string line = "";

            while ((line = reader.readLine()) != null)
            {
                ops.Add(line.Trim().ToLower());
            }

            reader.close();
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to load ip ban list: {exception}");
        }
    }

    protected override void saveOperators()
    {
        try
        {
            PrintWriter writer = new(new FileWriter(OPERATORS_FILE, false));

            foreach (string op in ops)
            {
                writer.println(op);
            }

            writer.close();
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to save ip ban list: {exception}");
        }
    }

    protected override void loadWhitelist()
    {
        try
        {
            whitelist.Clear();
            BufferedReader reader = new(new FileReader(WHITELIST_FILE));
            string line = "";

            while ((line = reader.readLine()) != null)
            {
                whitelist.Add(line.Trim().ToLower());
            }

            reader.close();
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to load white-list: {exception}");
        }
    }

    protected override void saveWhitelist()
    {
        try
        {
            PrintWriter writer = new(new FileWriter(WHITELIST_FILE, false));

            foreach (string name in whitelist)
            {
                writer.println(name);
            }

            writer.close();
        }
        catch (Exception exception)
        {
            _logger.LogWarning($"Failed to save white-list: {exception}");
        }
    }
}
