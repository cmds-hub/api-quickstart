using System;
using System.Text.Json;

namespace BearerAuthDemo;

public class ExportDemo
{
    private ApiClient _client;

    public void StartExport(ApiClient client, Action<string> output, string criteria)
    {
        _client = client;
        
        var export = ExportGradebooks(criteria);

        var downloadUrl = $"{_client.BaseUrl}/{export.DownloadUrl}";

        var lifetime = (int)(new DateTimeOffset(export.Expiry) - DateTimeOffset.UtcNow).TotalMinutes;

        output($"Here is the link to download your gradebooks: {downloadUrl}");

        output($"Please note the link requires authentication, and it expires in {lifetime} minutes.");
    }

    private Export ExportGradebooks(string title)
    {
        var query = string.Empty;

        if (!string.IsNullOrEmpty(title))
            query = $"?GradebookTitle={title}";

        var json = _client.Get("progress/gradebooks/export" + query).Data;

        return JsonSerializer.Deserialize<Export>(json);
    }
}