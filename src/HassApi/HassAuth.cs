using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HassApi.Models; 

namespace HassApi;

/// <summary>
/// Home Assistant OAuth2 授权服务客户端。
/// </summary>
public class HassAuth 
{
    // 关键修改 1: 使用 static readonly 确保在整个应用程序中只创建一个 HttpClient 实例。
    private static readonly HttpClient _httpClient = HassDefaults.UnauthenticatedClient;
    private readonly JsonSerializerOptions _jsonOptions = HassDefaults.DefaultJsonOptions;

    private readonly string baseUrl;

    /// <summary>
    /// 初始化 HassAuth。
    /// </summary>
    public HassAuth(string baseUrl)
    {
        this.baseUrl = baseUrl.TrimEnd('/');
    }

    /// <summary>
    /// 内部通用方法，用于向 /auth/token 端点发送 POST 请求。
    /// </summary>
    private async Task<AuthorizationResult?> PostAuthTokenAsync(string queryString)
    {
        // 1. 使用静态的 _httpClient
        using var httpContent = new StringContent(queryString, Encoding.UTF8, "application/x-www-form-urlencoded");
        
        string fullUrl = $"{baseUrl}/auth/token";
        
        // 2. 使用静态客户端发送请求
        HttpResponseMessage response = await _httpClient.PostAsync(fullUrl, httpContent);

        response.EnsureSuccessStatusCode(); 

        string responseContent = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<AuthorizationResult>(responseContent, _jsonOptions);
    }
    
    // --- 公共授权接口 (保持不变) ---

    /// <summary>
    /// 使用授权码 (Code) 交换访问令牌 (Access Token) 和刷新令牌 (Refresh Token)。
    /// </summary>
    public Task<AuthorizationResult?> GetAccessTokenAsync(string code, string hassUrl, string clientId)
    {
        string queryString = $"grant_type=authorization_code&code={code}&client_id={clientId}";
        return PostAuthTokenAsync(queryString);
    }

    /// <summary>
    /// 使用刷新令牌 (Refresh Token) 获取新的访问令牌 (Access Token)。
    /// </summary>
    public Task<AuthorizationResult?> GetRefreshTokenAsync(string hassUrl, string refreshToken, string clientId)
    {
        string queryString = $"grant_type=refresh_token&refresh_token={refreshToken}&client_id={clientId}";
        return PostAuthTokenAsync(queryString);
    }

    /// <summary>
    /// 撤销指定的刷新令牌 (Refresh Token)。
    /// </summary>
    public async Task RevokeRefreshTokenAsync(string hassUrl, string refreshToken)
    {
        string queryString = $"token={refreshToken}&action=revoke";
        await PostAuthTokenAsync(queryString); 
    }
}