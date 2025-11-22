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

    private const string FakeBaseUrl = "http://192.168.0.100:8123";
    private const string FakeClientId = "AndroidTV";

    [Fact]
    public async Task GetAccessTokenAsync()
    {
        HassAuth hassAuth = new HassAuth(FakeBaseUrl, FakeClientId);
        var result = hassAuth.GetAccessTokenAsync("");
        Assert.NotNull(result);
    }
}