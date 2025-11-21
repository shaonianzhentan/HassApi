namespace HassApi.Models;

/// <summary>
/// 对应于 GET /api/ 端点的响应结构。
/// </summary>
// 使用 record 保持不可变性
public record ApiStatusResponse
{
    /// <summary>
    /// Message
    /// </summary>
    public required string Message { get; init; }
}