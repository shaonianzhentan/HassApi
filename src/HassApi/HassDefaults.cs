using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HassApi;

/// <summary>
/// 内部类：为所有 Home Assistant API 客户端提供默认的、可复用的资源。
/// </summary>
internal static class HassDefaults
{
    /// <summary>
    /// 共享的、未认证的 HttpClient 实例，用于 OAuth 和 Webhook 访问。
    /// 确保在应用程序生命周期内复用，且不含 Bearer Token。
    /// </summary>
    internal static readonly HttpClient UnauthenticatedClient = new HttpClient();

    /// <summary>
    /// Home Assistant API 所需的默认 JSON 序列化选项，包括 SnakeCase 命名策略。
    /// </summary>
    internal static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        // 确保我们能正确处理 HistoryState 中的稀疏对象
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
    };
}