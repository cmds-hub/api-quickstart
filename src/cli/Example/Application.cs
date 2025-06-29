using System;
using System.Linq;
using System.Text.Json;

using Example;

namespace ApiQuickstartExample
{
    internal class Application
    {
        private ApiClient _client;

        public Application(ApiClient client)
        {
            _client = client;
        }

        public void Run()
        {
            var departments = GetDepartments();
            var departmentReport = DepartmentReport.Generate(departments);
            Console.WriteLine(departmentReport);

            var profiles = GetProfiles();
            var profileReport = ProfileReport.Generate(profiles);
            Console.WriteLine(profileReport);

            var learners = GetLearners();
            var learnerReport = LearnerReport.Generate(learners, "Ab");
            Console.WriteLine(learnerReport);
        }

        private Department[] GetDepartments()
        {
            var json = _client.Get("cmds/contacts/departments").Data;

            return JsonSerializer.Deserialize<Department[]>(json)
                .OrderBy(x => x.Name)
                .ToArray();
        }

        private Learner[] GetLearners()
        {
            var json = _client.Get("cmds/contacts/users").Data;

            return JsonSerializer.Deserialize<Learner[]>(json)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToArray();
        }

        private Profile[] GetProfiles()
        {
            var json = _client.Get("cmds/templates/profiles").Data;

            return JsonSerializer.Deserialize<Profile[]>(json)
                .OrderBy(x => x.Name)
                .ToArray();
        }
    }
}