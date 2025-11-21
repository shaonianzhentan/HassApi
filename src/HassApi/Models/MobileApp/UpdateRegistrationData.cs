using System.Text.Json.Serialization;

namespace HassApi.Models.MobileApp;

/// <summary>
/// Webhook 消息的 'update_registration' 类型中嵌套的 'data' 负载。
/// 用于更新设备信息或 App 数据。所有字段都是可选的。
/// </summary>
public record UpdateRegistrationData
{
    /// <summary>
    /// 可选：用于更新推送信息的 App 数据。对应 JSON 中的 "app_data"。
    /// </summary>
    [JsonPropertyName("app_data")]
    public AppUpdateData? AppData { get; init; }

    /// <summary>
    /// 可选：移动 App 的版本。
    /// </summary>
    [JsonPropertyName("app_version")]
    public string? AppVersion { get; init; }

    /// <summary>
    /// 可选：设备的名称。
    /// </summary>
    [JsonPropertyName("device_name")]
    public string? DeviceName { get; init; }

    /// <summary>
    /// 可选：设备制造商。
    /// </summary>
    public string? Manufacturer { get; init; }

    /// <summary>
    /// 可选：设备型号。
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// 可选：操作系统版本。
    /// </summary>
    [JsonPropertyName("os_version")]
    public string? OsVersion { get; init; }
}