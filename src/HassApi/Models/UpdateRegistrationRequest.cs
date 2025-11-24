namespace HassApi.Models;

/// <summary>
/// Webhook 消息的 'update_registration' 类型中嵌套的 'data' 负载。
/// 用于更新设备信息或 App 数据。所有字段都是必填项，且依赖于全局 SnakeCase 策略。
/// </summary>
/// <param name="AppData">必填：用于更新推送信息的 App 数据。对应 JSON 中的 "app_data"。</param>
/// <param name="AppVersion">必填：移动 App 的版本。对应 JSON 中的 "app_version"。</param>
/// <param name="DeviceName">必填：设备的名称。对应 JSON 中的 "device_name"。</param>
/// <param name="Manufacturer">必填：设备制造商。</param>
/// <param name="Model">必填：设备型号。</param>
/// <param name="OsVersion">必填：操作系统版本。对应 JSON 中的 "os_version"。</param>
public record UpdateRegistrationRequest(
    MobileAppData AppData,
    string AppVersion,
    string DeviceName,
    string Manufacturer,
    string Model,
    string OsVersion
);