using System;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace BetaSharp.Launcher.Features;

internal sealed class XboxService
{
    private readonly IXboxApi _userAuthApi;
    private readonly IXboxApi _xstsAuthApi;

    public XboxService(IHttpClientFactory httpClientFactory)
    {
        _userAuthApi = RestService.For<IXboxApi>(httpClientFactory.CreateClient("XboxUserAuth"));
        _xstsAuthApi = RestService.For<IXboxApi>(httpClientFactory.CreateClient("XboxXsts"));
    }

    public async Task<(string Token, string Hash)> GetAsync(string microsoft)
    {
        var userAuthRequest = new XboxUserAuthRequest(
            Properties: new { AuthMethod = "RPS", SiteName = "user.auth.xboxlive.com", RpsTicket = $"d={microsoft}" },
            RelyingParty: "http://auth.xboxlive.com",
            TokenType: "JWT"
        );

        var userAuthResponse = await _userAuthApi.AuthenticateUserAsync(userAuthRequest);

        string token = userAuthResponse.Token;
        string hash = userAuthResponse.DisplayClaims.xui[0].uhs;

        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(hash);

        var xstsAuthRequest = new XstsAuthRequest(
            Properties: new { SandboxId = "RETAIL", UserTokens = new[] { token } },
            RelyingParty: "rp://api.minecraftservices.com/",
            TokenType: "JWT"
        );

        var xstsAuthResponse = await _xstsAuthApi.AuthorizeXstsAsync(xstsAuthRequest);

        return (xstsAuthResponse.Token, hash);
    }
}
