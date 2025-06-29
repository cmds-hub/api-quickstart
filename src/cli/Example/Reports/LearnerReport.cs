using System;
using System.Linq;
using System.Text;

namespace Example;

public class LearnerReport
{
    internal static string Generate(Learner[] learners, string criteria)
    {
        var output = new StringBuilder();

        var total = learners.Count();

        output.AppendLine("## LEARNER SUMMARY");
            
        output.AppendLine($"Your organization contains {total:n0} user accounts for learners.");

        var matches = learners.Where(x => x.LastName.StartsWith(criteria, StringComparison.OrdinalIgnoreCase)).ToList();

        var count = matches.Count();

        var percent = total > 0 ? (decimal)count / total : 0m;

        output.AppendLine($"{count} people ({percent:p2}) have a last name that starts with the letters `{criteria}`:");

        foreach (var match in matches)
        {
            output.AppendLine($"  - {match.LastName}, {match.FirstName} ({match.Email})");
        }
        
        return output.ToString();
    }
}
