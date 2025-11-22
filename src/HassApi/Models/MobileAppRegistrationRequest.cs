using System.Collections.Generic;

namespace HassApi.Models;

/// <summary>
/// Home Assistant 移动应用设备注册请求体模型。
/// 对应 POST /api/mobile_app/registrations 接口的请求 JSON Payload。
/// </summary>
public record MobileAppRegistrationRequest
{
    /// <summary>
    /// 设备的唯一标识符 (device_id)。必填。
    /// </summary>
    public required string DeviceId { get; init; }

    /// <summary>
    /// 移动应用的唯一标识符 (app_id)。必填。
    /// </summary>
    public required string AppId { get; init; }

    /// <summary>
    /// 移动应用的名称 (app_name)。必填。
    /// </summary>
    public required string AppName { get; init; }

    /// <summary>
    /// 移动应用的当前版本 (app_version)。必填。
    /// </summary>
    public required string AppVersion { get; init; }

    /// <summary>
    /// 设备的名称 (device_name)。必填。
    /// </summary>
    public required string DeviceName { get; init; }

    /// <summary>
    /// 设备制造商 (manufacturer)。必填。
    /// </summary>
    public required string Manufacturer { get; init; }

    /// <summary>
    /// 设备型号 (model)。必填。
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// 操作系统名称 (os_name)。必填。
    /// </summary>
    public required string OsName { get; init; }

    /// <summary>
    /// 操作系统版本 (os_version)。必填。
    /// </summary>
    public required string OsVersion { get; init; }

    /// <summary>
    /// 应用是否支持加密 (supports_encryption)。必填。
    /// </summary>
    public required bool SupportsEncryption { get; init; }

    /// <summary>
    /// 应用扩展数据 (app_data)。包含如推送通知密钥等自定义数据。
    /// </summary>
    public Dictionary<string, object> AppData { get; init; } = new();
}