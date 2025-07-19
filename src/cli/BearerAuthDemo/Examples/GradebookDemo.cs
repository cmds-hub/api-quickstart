using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace BearerAuthDemo;

public class GradebookDemo
{
    private ApiClient _client;

    public void Run(ApiClient client, Action<string> output, string criteria)
    {
        _client = client;

        var count = CountGradebooks();

        var gradebooks = GetGradebooks(criteria);

        var report = GradebookReport.Generate(count, criteria, gradebooks);

        output(report);
    }

    private int CountGradebooks()
    {
        var data = _client.Get($"progress/gradebooks/count").Data;
        return JsonSerializer.Deserialize<CountResult>(data).Count;
    }

    private int CountGradebooks(string title)
    {
        // Count using an HTTP GET request:
        var data = _client.Get($"progress/gradebooks/count?GradebookTitle={title}").Data;
        var a = JsonSerializer.Deserialize<CountResult>(data).Count;

        // Count using an HTTP POST request:
        data = _client.Post("progress/gradebooks/count", new { GradebookTitle = title }).Data;
        var b = JsonSerializer.Deserialize<CountResult>(data).Count;

        Debug.Assert(a == b);

        return b;
    }

    private Gradebook[] GetGradebooks(string title)
    {
        var query = string.Empty;

        if (!string.IsNullOrEmpty(title))
            query = $"?GradebookTitle={title}";

        var json = _client.Get("progress/gradebooks" + query).Data;

        return JsonSerializer.Deserialize<Gradebook[]>(json)
            .OrderBy(x => x.GradebookTitle)
            .ToArray();
    }
}