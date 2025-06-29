using System;
using System.Linq;
using System.Text;

namespace Example;

public class ProfileReport
{
    internal static string Generate(Profile[] profiles)
    {
        var output = new StringBuilder();

        var distinctCompetencyCount = profiles.SelectMany(p => p.Competencies)
            .Select(c => c.Identifier)
            .Distinct()
            .Count();

        output.AppendLine("## PROFILE SUMMARY");
            
        output.Append($"Your organization contains {distinctCompetencyCount:n0} distinct competencies");

        output.Append($" in {profiles.Length} occupation profiles.");

        output.AppendLine();

        foreach (var profile in profiles)
        {
            output.AppendLine("  - " + profile.Name);
        }

        return output.ToString();
    }
}
