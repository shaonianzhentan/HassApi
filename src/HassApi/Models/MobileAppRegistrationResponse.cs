namespace HassApi.Models;

/// <summary>
/// Home Assistant 移动应用设备注册成功后的响应模型。
/// 对应 POST /api/mobile_app/registrations 接口的成功响应。
/// </summary>
public record MobileAppRegistrationResponse
{
    /// <summary>
    /// Home Assistant Cloud 提供的 cloudhook URL。
    /// 仅当用户订阅了 Nabu Casa 服务时才会提供 (可能为 null)。
    /// 对应 JSON 中的 "cloudhook_url"。
    /// </summary>
    public string? CloudhookUrl { get; init; }

    /// <summary>
    /// Home Assistant Cloud 提供的远程 UI URL。
    /// 仅当用户订阅了 Nabu Casa 服务时才会提供 (可能为 null)。
    /// 对应 JSON 中的 "remote_ui_url"。
    /// </summary>
    public string? RemoteUiUrl { get; init; }

    /// <summary>
    /// 用于加密通信的密钥。
    /// 仅当客户端和服务端都支持加密时才会提供 (可能为 null)。
    /// 对应 JSON 中的 "secret"。
    /// </summary>
    public string? Secret { get; init; }

    /// <summary>
    /// 可用于向 Home Assistant 发送数据的 Webhook ID。
    /// 对应 JSON 中的 "webhook_id"。
    /// </summary>
    public required string WebhookId { get; init; }
}