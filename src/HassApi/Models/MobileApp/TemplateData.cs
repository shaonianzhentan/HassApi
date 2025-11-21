using System.Collections.Generic;

namespace HassApi.Models.MobileApp;

/// <summary>
/// Webhook 消息中用于定义单个模板及其变量的负载结构。
/// 对应 JSON 中 "data" 字典内的值。
/// </summary>
public record TemplateData(
    string Template // 要渲染的 Jinja2 模板字符串
)
{
    /// <summary>
    /// 可选：额外的模板变量。对应 JSON 中的 "variables"。
    /// </summary>
    public Dictionary<string, object>? Variables { get; init; }
}