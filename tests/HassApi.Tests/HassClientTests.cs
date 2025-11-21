using Xunit;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using HassApi;
using HassApi.Models;
using System.Collections.Generic;

namespace HassApi.Tests;

public class HassClientTests
{
    private const string FakeBaseUrl = "http://fake.hass.io/";
    private const string FakeToken = "fake_token_123";

    // 辅助方法：创建一个带 Mock HttpHandler 的 HassClient 实例
    private HassClient CreateMockClient(MockHttpMessageHandler handler)
    {
        var mockHttpClient = new HttpClient(handler) { BaseAddress = new Uri(FakeBaseUrl) };
        return new HassClient(mockHttpClient, FakeBaseUrl, FakeToken);
    }

    /// <summary>
    /// 测试 1: 验证客户端是否能正确获取 API 状态信息 (/api/)
    /// </summary>
    [Fact]
    public async Task GetApiMessageAsync_ShouldReturnApiRunningMessage()
    {
        // 1. Arrange (准备)
        const string expectedMessage = "API running.";
        // HA 的 /api/ 响应是一个 JSON 对象，里面包含一个 message 字段
        string successJson = $"{{\"message\": \"{expectedMessage}\"}}"; // Mock JSON

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, $"{FakeBaseUrl}api/")
            .Respond("application/json", successJson);

        var client = CreateMockClient(mockHttp);

        // 2. Act (执行)
        // 注意：现在方法返回 ApiStatusResponse? 类型
        var response = await client.GetApiStatusAsync();

        // 3. Assert (断言)
        Assert.NotNull(response);
        // 断言对象的 Message 属性是否等于预期值
        Assert.Equal(expectedMessage, response!.Message);
        mockHttp.VerifyNoOutstandingRequest();
    }

    /// <summary>
    /// 测试 2: 验证客户端是否能正确反序列化单个实体状态 (GetStateAsync)
    /// </summary>
    [Fact]
    public async Task GetStateAsync_ShouldDeserializeHassStateCorrectly()
    {
        // 1. Arrange (准备)
        const string entityId = "light.kitchen";

        // 使用 C# 11 的原始字符串字面量 (Raw String Literals) 来定义 JSON，非常方便！
        string mockJson = $$"""
            {
                "entity_id": "{{entityId}}",
                "state": "on",
                "last_changed": "2025-11-20T10:00:00+00:00",
                "attributes": {
                    "friendly_name": "Kitchen Light",
                    "brightness": 255
                }
            }
            """;

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, $"{FakeBaseUrl}api/states/{entityId}")
            .Respond("application/json", mockJson);

        var client = CreateMockClient(mockHttp);

        // 2. Act (执行)
        var state = await client.GetStateAsync(entityId);

        // 3. Assert (断言)
        Assert.NotNull(state);
        Assert.Equal(entityId, state.EntityId);
        Assert.Equal("on", state.State);

        // 验证 Attributes 字典是否正确解析
        Assert.True(state.Attributes.ContainsKey("friendly_name"));
        Assert.Equal("Kitchen Light", state.Attributes["friendly_name"].ToString());
    }

    /// <summary>
    /// 测试 3: 验证当 API 返回 401 Unauthorized 时，客户端是否抛出 HttpRequestException
    /// </summary>
    [Fact]
    public async Task GetStateAsync_ShouldThrowExceptionOnUnauthorized()
    {
        // 1. Arrange (准备)
        var mockHttp = new MockHttpMessageHandler();

        // 配置 Mock: 返回 401 Unauthorized 状态码
        mockHttp.When(HttpMethod.Get, $"{FakeBaseUrl}api/states/light.invalid")
            .Respond(System.Net.HttpStatusCode.Unauthorized);

        var client = CreateMockClient(mockHttp);

        // 2. Act & 3. Assert (执行并断言)
        // 断言 client.GetStateAsync 必须抛出 HttpRequestException
        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.GetStateAsync("light.invalid")
        );
    }
}