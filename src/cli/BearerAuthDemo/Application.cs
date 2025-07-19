using System;

namespace BearerAuthDemo
{
    internal class Application
    {
        private readonly ApiClient _client;

        private readonly string _criteria;

        public Application(ApiClient client, string criteria)
        {
            _client = client;
            _criteria = criteria;
        }

        public void Run(Action<string> output)
        {
            new GroupDemo().Run(_client, output);

            new UserDemo().Run(_client, output);

            new GradebookDemo().Run(_client, output, _criteria);

            new PaginationDemo().Run(_client, output);
        }
    }
}