using Xunit;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using HassApi;
using HassApi.Models;
using System.Collections.Generic;

namespace HassApi.Tests;

public class MobileAppTests
{
    private const string BaseUrl = "http://localhost:8123";
    private const string WebhookId = "xxxxxxxxxxxx";

    private MobileApp CreateMobileApp()
    {
        return new MobileApp(BaseUrl, WebhookId);
    }

    [Fact]
    public async Task UpdateRegistrationAsync()
    {
        var mobileApp = CreateMobileApp();
        
        await mobileApp.UpdateRegistrationAsync(new UpdateRegistrationRequest{
            
        });
    }
}