using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Refit;

namespace BetaSharp.Launcher.Features;

internal sealed class MinecraftService
{
    private readonly IMinecraftApi _api;
    private readonly HttpClient _httpClient;

    public MinecraftService(IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("MinecraftApi");
        _api = RestService.For<IMinecraftApi>(client);
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<(string Name, string ID, CroppedBitmap Image)> GetProfileAsync(string token)
    {
        var profile = await _api.GetProfileAsync($"Bearer {token}");

        string name = profile.name;
        string id = profile.id;
        string skin = profile.skins[0].url;

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(skin);

        var image = await _httpClient.GetStreamAsync(skin);
        var memory = new MemoryStream();
        await image.CopyToAsync(memory);
        memory.Position = 0;
        var bitmap = new CroppedBitmap(new Bitmap(memory), new PixelRect(8, 8, 8, 8));
        return (name, id, bitmap);
    }

    public async Task<string> GetTokenAsync(string token, string hash)
    {
        var request = new MinecraftAuthRequest(identityToken: $"XBL3.0 x={hash};{token}");
        var response = await _api.LoginWithXboxAsync(request);

        return response.access_token;
    }

    public async Task DownloadAsync()
    {
        await using var stream = await _httpClient.GetStreamAsync("https://launcher.mojang.com/v1/objects/43db9b498cb67058d2e12d394e6507722e71bb45/client.jar");
        await using var file = File.OpenWrite("b1.7.3.jar");

        await stream.CopyToAsync(file);
    }
}
