using System.Net.Http;

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
}