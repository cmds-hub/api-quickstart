using System.Text;

namespace Example;

public class DepartmentReport
{
    internal static string Generate(Department[] departments)
    {
        var output = new StringBuilder();

        output.AppendLine("## DEPARTMENT SUMMARY");

        output.AppendLine($"Your organization contains {departments.Length} departments.");

        foreach (var department in departments)
        {
            output.AppendLine("  - " + department.Name);
        }

        return output.ToString();
    }
}