using System.Collections.Generic;

namespace HassApi.Models.MobileApp;

/// <summary>
/// Webhook 消息中用于渲染模板的顶级 'data' 负载。
/// 它包含一个或多个命名模板的字典。
/// </summary>
public record RenderTemplateRequestData 
{
    // 使用 Dictionary<string, TemplateData> 来表示 key: dictionary 的结构，
    // 允许在单个请求中渲染多个模板。
    public required Dictionary<string, TemplateData> Templates { get; init; }
}