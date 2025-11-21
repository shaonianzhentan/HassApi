using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using HassApi.Models; // 引用你的 Models 命名空间

namespace HassApi;

/// <summary>
/// Home Assistant REST API 客户端
/// </summary>
public class HassClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// 初始化 HassClient
    /// </summary>
    /// <param name="httpClient">推荐通过 IHttpClientFactory 注入实例</param>
    /// <param name="baseUrl">HA 地址 (例如: http://192.168.1.5:8123)</param>
    /// <param name="accessToken">长期访问令牌 (Long-Lived Access Token)</param>
    public HassClient(HttpClient httpClient, string baseUrl, string accessToken)
    {
        // 基础校验
        if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));
        if (string.IsNullOrWhiteSpace(accessToken)) throw new ArgumentNullException(nameof(accessToken));

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        // 配置 BaseAddress
        // 确保 URL 以 / 结尾，防止相对路径拼接出错
        var normalizedUrl = baseUrl.TrimEnd('/') + "/";
        _httpClient.BaseAddress = new Uri(normalizedUrl);

        // 配置默认请求头
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // 配置 JSON 序列化选项 (关键：处理 HA 的 snake_case)
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            // .NET 8 的 System.Text.Json 自带 SnakeCaseLower，如果用旧版可能需要自定义 Policy
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, 
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// 检查 API 是否连通
    /// GET /api/
    /// </summary>
    public async Task<ApiStatusResponse?> GetApiStatusAsync(CancellationToken cancellationToken = default)
    {
        // 关键修改：反序列化到 ApiStatusResponse? 类型
        return await GetJsonAsync<ApiStatusResponse>("api/", cancellationToken);
    }

    /// <summary>
    /// 获取所有设备的状态
    /// GET /api/states
    /// </summary>
    public async Task<List<HassState>> GetAllStatesAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetJsonAsync<List<HassState>>("api/states", cancellationToken);
        return result ?? new List<HassState>();
    }

    /// <summary>
    /// 获取指定设备的状态
    /// GET /api/states/{entity_id}
    /// </summary>
    public async Task<HassState?> GetStateAsync(string entityId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(entityId)) throw new ArgumentException("EntityId cannot be empty", nameof(entityId));
        
        return await GetJsonAsync<HassState>($"api/states/{entityId}", cancellationToken);
    }

    /// <summary>
    /// 调用服务 (执行动作)
    /// POST /api/services/{domain}/{service}
    /// </summary>
    /// <param name="domain">领域 (如 light, switch)</param>
    /// <param name="service">服务名 (如 turn_on, toggle)</param>
    /// <param name="payload">参数对象 (可以是匿名对象)</param>
    /// <param name="cancellationToken">用于取消长时间运行的操作的令牌。</param>
    public async Task<List<HassState>> CallServiceAsync(string domain, string service, object? payload = null, CancellationToken cancellationToken = default)
    {
        var endpoint = $"api/services/{domain}/{service}";
        
        // 序列化请求体
        var jsonContent = payload is not null 
            ? JsonSerializer.Serialize(payload, _jsonOptions) 
            : "{}";

        using var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(endpoint, httpContent, cancellationToken);
        
        // HA 调用服务如果失败通常会返回 400 或 500
        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error calling service {domain}.{service}: {response.StatusCode} - {errorMsg}");
        }

        // 调用服务通常会返回受影响实体的最新状态列表
        using var responseStream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<List<HassState>>(responseStream, _jsonOptions, cancellationToken);
        return result ?? new List<HassState>();
    }

    // --- 私有辅助方法 ---

    private async Task<T?> GetJsonAsync<T>(string endpoint, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            // 处理 404 Not Found (有些 API 实体不存在返回 404)
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
            
            var errorMsg = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API Error {endpoint}: {response.StatusCode} - {errorMsg}");
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions, cancellationToken);
    }
}