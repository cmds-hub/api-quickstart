using System;
using System.Text;
using System.Text.Json;

namespace BearerAuthDemo;

public class PaginationDemo
{
    private ApiClient _client;

    public void Run(ApiClient client, Action<string> output)
    {
        _client = client;

        var report = GetGradebooksInPages();

        output(report);
    }

    private string GetGradebooksInPages()
    {
        var report = new StringBuilder();

        var pageIndex = 1;

        var pageSize = 20;

        report.AppendLine($"# PAGINATION DEMO ");

        var (pagination, items) = GetGradebooksPage(pageIndex, pageSize);

        var itemTotal = pagination.TotalCount;

        var totalPageCount = pagination.CountPages();

        report.AppendLine($"Watch me page through {itemTotal} gradebooks using a page size of {pageSize} items per page!");

        while (items.Length != 0)
        {
            var currentPageIndex = pagination.Page;

            var currentPageItemCount = items.Length;

            report.AppendLine($"  - Page {currentPageIndex} of {totalPageCount} has {currentPageItemCount} items ({(pagination.HasMore() ? "more" : "no more")})");

            currentPageIndex++;

            (pagination, items) = GetGradebooksPage(currentPageIndex, pageSize);
        }

        return report.ToString();
    }

    private (Pagination, Gradebook[]) GetGradebooksPage(int page, int take)
    {
        var query = $"filter.page={page}&filter.pagesize={take}";

        var response = _client.Get("progress/gradebooks?" + query);

        var data = response.Data;

        var header = response.Headers[Pagination.HeaderKey];

        var items = JsonSerializer.Deserialize<Gradebook[]>(data);

        var pagination = JsonSerializer.Deserialize<Pagination>(header);

        return (pagination, items);
    }
}