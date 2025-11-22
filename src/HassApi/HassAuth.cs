using HassApi.Models; 
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace HassApi;

/// <summary>
/// Home Assistant OAuth2 授权服务客户端。
/// </summary>
public class HassAuth 
{
    // 关键修改 1: 使用 static readonly 确保在整个应用程序中只创建一个 HttpClient 实例。
    private static readonly HttpClient _httpClient = HassDefaults.UnauthenticatedClient;
    private readonly JsonSerializerOptions _jsonOptions = HassDefaults.DefaultJsonOptions;

    readonly string baseUrl;
    readonly string clientId;
    public readonly string redirectUri;

    /// <summary>
    /// 初始化 HassAuth。
    /// </summary>
    public HassAuth(string baseUrl, string clientId)
    {
        this.baseUrl = baseUrl.TrimEnd('/');
        this.clientId = HttpUtility.UrlEncode(clientId);
        redirectUri = $"{baseUrl}/?external_auth=1";
    }

    /// <summary>
    /// 获取授权链接
    /// </summary>
    /// <returns></returns>
    public string GetAuthUrl()
    {
        return $"{baseUrl}/auth/authorize?client_id={clientId}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}";
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
    public Task<AuthorizationResult?> GetAccessTokenAsync(string code)
    {
        string queryString = $"grant_type=authorization_code&code={code}&client_id={clientId}";
        return PostAuthTokenAsync(queryString);
    }

    /// <summary>
    /// 使用刷新令牌 (Refresh Token) 获取新的访问令牌 (Access Token)。
    /// </summary>
    public Task<AuthorizationResult?> GetRefreshTokenAsync(string refreshToken)
    {
        string queryString = $"grant_type=refresh_token&refresh_token={refreshToken}&client_id={clientId}";
        return PostAuthTokenAsync(queryString);
    }

    /// <summary>
    /// 撤销指定的刷新令牌 (Refresh Token)。
    /// </summary>
    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        string queryString = $"token={refreshToken}&action=revoke";
        await PostAuthTokenAsync(queryString); 
    }
}