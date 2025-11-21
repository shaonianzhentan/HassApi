# HassApi
Home Assistant REST API




```cs
using HassApi;
using System.Net.Http;

// 1. 创建 HttpClient (实际项目中建议使用 IHttpClientFactory)
var httpClient = new HttpClient();

// 2. 初始化客户端
var client = new HassClient(
    httpClient, 
    "http://192.168.1.5:8123", 
    "eyJhGci..." // 你的 Long-Lived Token
);

// 3. 获取某个灯的状态
var light = await client.GetStateAsync("light.living_room");
Console.WriteLine($"客厅灯状态: {light?.State}");

// 4. 开灯 (调用服务)
await client.CallServiceAsync("light", "turn_on", new { entity_id = "light.living_room", brightness = 255 });
```