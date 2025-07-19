using System;
using System.Collections.Generic;
using System.Text.Json;

namespace BearerAuthDemo
{
    /// <summary>
    /// Response object containing all relevant HTTP response information.
    /// </summary>
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string Data { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }

        public T GetJsonData<T>()
        {
            if (string.IsNullOrEmpty(Data))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(Data);
            }
            catch (JsonException)
            {
                throw new InvalidOperationException("Response data is not valid JSON");
            }
        }
    }
}