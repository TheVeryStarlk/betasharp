using DSharpPlus;
using DSharpPlus.Entities;

namespace BetaSharp;

public static class Discord
{
    public static ulong Id { get; set; }

    public static DiscordClient? Client { get; set; }

    public static DiscordChannel? Channel { get; set; }
}
