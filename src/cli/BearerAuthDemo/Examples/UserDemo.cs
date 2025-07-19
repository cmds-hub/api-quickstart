using System;
using System.Linq;
using System.Text.Json;

namespace BearerAuthDemo;

public class UserDemo
{
    private ApiClient _client;

    public void Run(ApiClient client, Action<string> output)
    {
        _client = client;

        var users = GetUsers();

        var report = UserReport.Generate(users);

        output(report);
    }

    private User[] GetUsers()
    {
        var json = _client.Get("security/users").Data;

        return [.. JsonSerializer.Deserialize<User[]>(json)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)];
    }
}