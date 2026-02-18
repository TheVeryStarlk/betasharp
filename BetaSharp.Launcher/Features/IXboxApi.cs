using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Refit;

namespace BetaSharp.Launcher.Features;

internal interface IXboxApi
{
    [Post("/user/authenticate")]
    Task<XboxUserAuthResponse> AuthenticateUserAsync([Body] XboxUserAuthRequest request);

    [Post("/xsts/authorize")]
    Task<XstsAuthResponse> AuthorizeXstsAsync([Body] XstsAuthRequest request);
}

internal record XboxUserAuthRequest(
    object Properties,
    string RelyingParty,
    string TokenType
);

internal record XboxUserAuthResponse(
    string Token,
    DisplayClaims DisplayClaims
);

internal record DisplayClaims(
    [property: JsonPropertyName("xui")] Xui[] Xui
);

internal record Xui(
    [property: JsonPropertyName("uhs")] string Uhs
);

internal record XstsAuthRequest(
    object Properties,
    string RelyingParty,
    string TokenType
);

internal record XstsAuthResponse(
    string Token
);
