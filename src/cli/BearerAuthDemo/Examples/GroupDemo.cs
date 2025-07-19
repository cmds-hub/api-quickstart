using System;
using System.Text.Json;

namespace BearerAuthDemo;

public class GroupDemo
{
    private ApiClient _client;

    public void Run(ApiClient client, Action<string> output)
    {
        _client = client;

        var groups = GetGroups();

        var report = GroupReport.Generate(groups);

        output(report);
    }

    private Group[] GetGroups()
    {
        var json = _client.Get("directory/groups").Data;

        return JsonSerializer.Deserialize<Group[]>(json);
    }
}