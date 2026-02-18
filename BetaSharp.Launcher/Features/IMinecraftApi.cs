using System.Text.Json.Serialization;
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
    [property: JsonPropertyName("identityToken")] string IdentityToken
);

internal record MinecraftAuthResponse(
    [property: JsonPropertyName("access_token")] string AccessToken
);

internal record MinecraftProfileResponse(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("skins")] MinecraftSkin[] Skins
);

internal record MinecraftSkin(
    [property: JsonPropertyName("url")] string Url
);
