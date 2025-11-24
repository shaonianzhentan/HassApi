using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HassApi.Models;

namespace HassApi;

/// <summary>
/// Home Assistant 移动应用 Webhook 交互处理类。
/// 负责发送未认证的 Webhook 消息，使用持久化存储的配置进行实例化。
/// </summary>
public class MobileApp
{
    // 专用于未认证 Webhook 的 HttpClient 实例。
    private readonly HttpClient _httpClient = HassDefaults.UnauthenticatedClient;
    private readonly string _initialBaseUrl;
    private readonly string _webhookId;

    // 私有属性：用于构造 Webhook 完整 URL
    private string WebhookUrl => $"{_initialBaseUrl.TrimEnd('/')}/api/webhook/{_webhookId}";

    /// <summary>
    /// 初始化 MobileApp，用于后续的 Webhook 交互。
    /// 构造函数为 internal，强制通过 HassClient 工厂方法进行实例化。
    /// </summary>
    /// <param name="initialBaseUrl">Home Assistant 的基础 URL。</param>
    /// <param name="webhookId">设备注册成功后持久化存储的 Webhook ID。</param>
    public MobileApp(string initialBaseUrl, string webhookId)
    {
        // --- 兼容旧框架的参数检查 ---
        if (string.IsNullOrWhiteSpace(initialBaseUrl)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(initialBaseUrl));
        if (string.IsNullOrWhiteSpace(webhookId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(webhookId));

        _initialBaseUrl = initialBaseUrl;
        _webhookId = webhookId;
    }

    // --- 低层级通用发送方法 ---

    /// <summary>
    /// 通过 Webhook 发送结构化数据到 Home Assistant，用于无需返回响应的 API (如 fire_event)。
    /// </summary>
    /// <param name="structuredPayload">要发送的结构化 Webhook Payload。</param>
    public Task SendWebhookDataAsync(object structuredPayload, CancellationToken cancellationToken = default)
    {
        // 调用底层带响应的方法，并设置 expectResponse=false
        return PostJsonUnauthenticatedWithResponseAsync<object>(structuredPayload, cancellationToken, expectResponse: false);
    }

    /// <summary>
    /// 执行 POST 请求，发送 JSON Payload，并处理返回的 JSON 响应（未认证）。
    /// 这是所有高层级方法的最终执行者。
    /// </summary>
    /// <typeparam name="TResponse">期望的 JSON 响应类型。</typeparam>
    private async Task<TResponse?> PostJsonUnauthenticatedWithResponseAsync<TResponse>(
        object payload,
        CancellationToken cancellationToken,
        bool expectResponse = true)
        where TResponse : class
    {
        var jsonContent = HassJsonHelper.Serialize(payload);
        using var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsync(WebhookUrl, httpContent, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Webhook Error POST {WebhookUrl}: Connection failed or unexpected error.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Webhook Error POST {WebhookUrl}: {response.StatusCode} - {errorMsg}");
        }

        if (!expectResponse)
        {
            return null;
        }

        var responseStream = await response.Content.ReadAsStreamAsync();
        return await HassJsonHelper.DeserializeAsync<TResponse>(responseStream, cancellationToken);
    }

    // --- 高层级 API (发送/void) ---

    /// <summary>
    /// 发送设备的位置更新信息。
    /// </summary>
    public Task SendLocationUpdateAsync(
        LocationUpdateRequest locationData,
        CancellationToken cancellationToken = default)
    {
        // --- 兼容旧框架的参数检查 ---
        if (locationData is null) throw new ArgumentNullException(nameof(locationData));

        var locationPayload = new WebhookRequest<LocationUpdateRequest>("update_location", locationData);
        return SendWebhookDataAsync(locationPayload, cancellationToken);
    }

    /// <summary>
    /// 通过 Webhook 调用 Home Assistant 中的一个服务操作。
    /// </summary>
    public Task CallServiceActionAsync(
        CallServiceRequest serviceData,
        CancellationToken cancellationToken = default)
    {
        // --- 兼容旧框架的参数检查 ---
        if (serviceData is null) throw new ArgumentNullException(nameof(serviceData));

        var servicePayload = new WebhookRequest<CallServiceRequest>("call_service", serviceData);
        return SendWebhookDataAsync(servicePayload, cancellationToken);
    }

    /// <summary>
    /// 通过 Webhook 触发 Home Assistant 中的一个事件。
    /// </summary>
    public Task FireEventAsync(
        FireEventRequest eventData,
        CancellationToken cancellationToken = default)
    {
        // --- 兼容旧框架的参数检查 ---
        if (eventData is null) throw new ArgumentNullException(nameof(eventData));

        var eventPayload = new WebhookRequest<FireEventRequest>("fire_event", eventData);
        return SendWebhookDataAsync(eventPayload, cancellationToken);
    }

    /// <summary>
    /// 通过 Webhook 更新已注册设备的信息 (如 App 版本、设备名称或推送令牌)。
    /// </summary>
    public Task UpdateRegistrationAsync(
        UpdateRegistrationRequest updateData,
        CancellationToken cancellationToken = default)
    {
        // --- 兼容旧框架的参数检查 ---
        if (updateData is null) throw new ArgumentNullException(nameof(updateData));

        var updatePayload = new WebhookRequest<UpdateRegistrationRequest>("update_registration", updateData);
        return SendWebhookDataAsync(updatePayload, cancellationToken);
    }

    // --- 高层级 API (获取/带响应) ---

    /// <summary>
    /// 通过 Webhook 渲染一个或多个 Home Assistant 模板。
    /// </summary>
    /// <returns>返回包含渲染结果的字典。</returns>
    public Task<Dictionary<string, string>?> RenderTemplatesAsync(
        Dictionary<string, TemplateData> templatesData,
        CancellationToken cancellationToken = default)
    {
        if (templatesData is null || templatesData.Count == 0)
        {
            throw new ArgumentException("Templates data cannot be null or empty.", nameof(templatesData));
        }

        var renderData = new RenderTemplateRequest(templatesData);
        var renderPayload = new WebhookRequest<RenderTemplateRequest>("render_template", renderData);

        return PostJsonUnauthenticatedWithResponseAsync<Dictionary<string, string>>(renderPayload, cancellationToken);
    }

    /// <summary>
    /// 通过 Webhook 获取所有启用的区域（Zones）。
    /// </summary>
    /// <returns>返回 Zone 实体列表（通常为 List<object> 或自定义 Zone 模型）。</returns>
    public Task<List<object>?> GetZonesAsync(CancellationToken cancellationToken = default)
    {
        var request = new WebhookBaseRequest("get_zones");
        return PostJsonUnauthenticatedWithResponseAsync<List<object>>(request, cancellationToken);
    }


    /// <summary>
    /// 通过 Webhook 获取 Home Assistant 的配置信息。
    /// </summary>
    /// <returns>返回配置实体 (object 或自定义 HassConfig 模型)。</returns>
    public Task<object?> GetConfigAsync(CancellationToken cancellationToken = default)
    {
        var request = new WebhookBaseRequest("get_config");
        return PostJsonUnauthenticatedWithResponseAsync<object>(request, cancellationToken);
    }

    /// <summary>
    /// 通过 Webhook 注册一个新的传感器或二进制传感器。
    /// 注意：一次只能注册一个传感器。
    /// </summary>
    /// <param name="sensorData">包含传感器配置和初始状态的实体负载。</param>
    /// <param name="cancellationToken">用于取消长时间运行的操作的令牌。</param>
    public Task RegisterSensorAsync(
        RegisterSensorRequest sensorData,
        CancellationToken cancellationToken = default)
    {
        // --- 兼容旧框架的参数检查 ---
        if (sensorData is null) throw new ArgumentNullException(nameof(sensorData));

        // 1. 构造完整的 Webhook 请求实体：使用泛型基类，手动传入 type: "register_sensor"
        var registerPayload = new WebhookRequest<RegisterSensorRequest>("register_sensor", sensorData);

        // 2. 调用低级别的通用发送方法
        return SendWebhookDataAsync(registerPayload, cancellationToken);
    }

    /// <summary>
    /// 通过 Webhook 批量更新一个或多个已注册传感器的状态和属性。
    /// </summary>
    /// <param name="updates">包含要更新的传感器数据的列表。</param>
    /// <param name="cancellationToken">用于取消长时间运行的操作的令牌。</param>
    /// <returns>返回一个字典，其中键是 unique_id，值是更新结果。</returns>
    public Task<Dictionary<string, UpdateSensorResult>?> UpdateSensorsAsync(
        List<UpdateSensorRequest> updates,
        CancellationToken cancellationToken = default)
    {
        if (updates is null || updates.Count == 0)
        {
            throw new ArgumentException("Updates list cannot be null or empty.", nameof(updates));
        }

        // 1. 构造完整的 Webhook 请求实体：data 就是 updates 列表本身
        // WebhookRequest<TData> 接收 TData，这里 TData 是 List<UpdateSensorRequest>
        var updatePayload = new WebhookRequest<List<UpdateSensorRequest>>("update_sensor_states", updates);

        // 2. 调用带响应的底层方法，并指定返回类型
        return PostJsonUnauthenticatedWithResponseAsync<Dictionary<string, UpdateSensorResult>>(updatePayload, cancellationToken);
    }
}