namespace HassApi.Models;

/// <summary>
/// Home Assistant 移动应用设备注册请求体模型。
/// 对应 POST /api/mobile_app/registrations 接口的请求 JSON Payload。
/// </summary>
/// <param name="DeviceId">必填：设备的唯一标识符。映射到 JSON "device_id"。</param>
/// <param name="AppId">必填：移动应用的唯一标识符。映射到 JSON "app_id"。</param>
/// <param name="AppName">必填：移动应用的名称。映射到 JSON "app_name"。</param>
/// <param name="AppVersion">必填：移动应用的当前版本。映射到 JSON "app_version"。</param>
/// <param name="DeviceName">必填：设备的名称。映射到 JSON "device_name"。</param>
/// <param name="Manufacturer">必填：设备制造商。</param>
/// <param name="Model">必填：设备型号。</param>
/// <param name="OsName">必填：操作系统名称。映射到 JSON "os_name"。</param>
/// <param name="OsVersion">必填：操作系统版本。映射到 JSON "os_version"。</param>
/// <param name="SupportsEncryption">必填：应用是否支持加密。映射到 JSON "supports_encryption"。</param>
/// <param name="AppData">必填：应用扩展数据 (如推送通知密钥)。映射到 JSON "app_data"。</param>
public record MobileAppRegistrationRequest(
    string DeviceId,
    string AppId,
    string AppName,
    string AppVersion,
    string DeviceName,
    string Manufacturer,
    string Model,
    string OsName,
    string OsVersion,
    MobileAppData AppData,
    bool SupportsEncryption = false
);