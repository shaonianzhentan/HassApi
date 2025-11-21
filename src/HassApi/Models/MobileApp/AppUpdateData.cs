using System.Text.Json.Serialization;

namespace HassApi.Models.MobileApp;

/// <summary>
/// Webhook 消息中用于更新推送令牌和 URL 的负载结构。
/// 对应 JSON 中 "app_data"。
/// </summary>
public record AppUpdateData
{
    /// <summary>
    /// 推送通知令牌。
    /// </summary>
    [JsonPropertyName("push_token")]
    public string? PushToken { get; init; }

    /// <summary>
    /// 推送通知 URL。
    /// </summary>
    [JsonPropertyName("push_url")]
    public string? PushUrl { get; init; }
}