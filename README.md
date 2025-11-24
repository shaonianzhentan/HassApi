# HassApi
Home Assistant REST API


**授权**
```cs
using HassApi;

var hassAuth = new HassAuth(
    "http://192.168.1.5:8123", 
    "http://192.168.1.5:8123/android-tv" // clientId
);

// 1. 页面跳转
Console.WriteLine($"授权链接，获取code: {hassAuth.AuthorizeUri}");

// 2. 获取 RefreshToken
webview.Navigating += async (sender, ev) => {
    // 重定向匹配
    if (ev.Url.StartsWith(hassAuth.RedirectUri))
    {
        var code = "...";

        var auth = await hassAuth.GetRefreshTokenAsync(code);

        Console.WriteLine($"RefreshToken: {auth.RefreshToken}");
        Console.WriteLine($"AccessToken: {auth.AccessToken}");
        Console.WriteLine($"ExpiresIn: {auth.ExpiresIn}");
        Console.WriteLine($"TokenType: {auth.TokenType}");
    }
};


// 3. 过期后换取新的AccessToken

var refreshToken = "...";

var auth = await hassAuth.GetAccessTokenAsync(refreshToken);

Console.WriteLine($"RefreshToken: {auth.RefreshToken}");
Console.WriteLine($"AccessToken: {auth.AccessToken}");
Console.WriteLine($"ExpiresIn: {auth.ExpiresIn}");
Console.WriteLine($"TokenType: {auth.TokenType}");
```

**REST API**
```cs
using HassApi;

// 1. 初始化客户端
var hassClient = new HassClient(
    "http://192.168.1.5:8123", 
    "eyJhGci..." // 你的 Long-Lived Token
);

// 2. 获取某个灯的状态
var light = await hassClient.GetStateAsync("light.living_room");
Console.WriteLine($"客厅灯状态: {light?.State}");

// 3. 开灯 (调用服务)
await hassClient.CallServiceAsync("light", "turn_on", new { entity_id = "light.living_room", brightness = 255 });
```

**MobileApp**
```cs
// 1. 注册设备
var res = await hassClient.RegisterMobileAppAsync(new MobileAppRegistrationRequest{
    ...
})
Console.WriteLine($"WebhookId: {res.WebhookId}");

var mobileApp = new MobileApp(
    "http://192.168.1.5:8123",
    "xxxxxxxxxxxxx" // WebhookId
);

// 2. 更新设备信息
await mobileApp.UpdateRegistrationAsync(new UpdateRegistrationRequest{
    ...
})

// 3. 上报传感器
...
```