using System.Threading.Tasks;
using Refit;

namespace BetaSharp.Launcher.Features;

internal interface IMinecraftApi
{
    [Post("/authentication/login_with_xbox")]
    Task<MinecraftAuthResponse> LoginWithXboxAsync([Body] MinecraftAuthRequest request);

    [Get("/minecraft/profile")]
    Task<MinecraftProfileResponse> GetProfileAsync([Header("Authorization")] string authorization);
}

internal record MinecraftAuthRequest(
    string identityToken
);

internal record MinecraftAuthResponse(
    string access_token
);

internal record MinecraftProfileResponse(
    string name,
    string id,
    MinecraftSkin[] skins
);

internal record MinecraftSkin(
    string url
);
