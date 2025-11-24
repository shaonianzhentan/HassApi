using Xunit;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using HassApi;
using HassApi.Models;
using System.Collections.Generic;

namespace HassApi.Tests;

public class HassAuthTests
{

    private const string FakeBaseUrl = "http://localhost:8123";
    private const string FakeClientId = "AndroidTV";

    [Fact]
    public async Task Main()
    {
        var code = "";
        Assert.NotEmpty(code);

        var hassAuth = new HassAuth(FakeBaseUrl, FakeClientId);
        var rt = hassAuth.GetRefreshTokenAsync(code);
        Assert.NotNull(rt);

        var refreshToken = rt.RefreshToken;
        Assert.NotEmpty(refreshToken);
        Assert.NotEmpty(rt.AccessToken);

        var at = hassAuth.GetAccessTokenAsync(refreshToken);
        Assert.NotNull(at);
        Assert.NotEmpty(at.AccessToken);
    }
}