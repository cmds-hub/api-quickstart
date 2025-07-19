using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace BearerAuthDemo
{
    /// <summary>
    /// A bare-metal HTTP client that includes client secret in bearer authorization header. Uses modern HttpClient with 
    /// proper disposal patterns.
    /// </summary>
    public partial class ApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        private readonly string _baseUrl;

        private bool _disposed = false;

        public string BaseUrl => _baseUrl;
        
        /// <summary>
        /// Initialize the HTTP client with a client secret.
        /// </summary>
        public ApiClient(string clientSecret, string baseUrl, string userAgent, int timeoutInSeconds = 30)
        {
            if (string.IsNullOrEmpty(clientSecret))
                throw new ArgumentException("Client secret cannot be null or empty", nameof(clientSecret));

            _baseUrl = baseUrl?.TrimEnd('/');

            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {clientSecret}");

            _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);

            SetTimeout(TimeSpan.FromSeconds(timeoutInSeconds));
        }

        private string BuildUrl(string endpoint)
        {
            if (string.IsNullOrEmpty(_baseUrl))
                return endpoint;

            return $"{_baseUrl}/{endpoint.TrimStart('/')}";
        }

        private string BuildUrlWithParams(string url, Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return url;

            var queryParams = new List<string>();
            foreach (var param in parameters)
            {
                var encodedKey = HttpUtility.UrlEncode(param.Key);
                var encodedValue = HttpUtility.UrlEncode(param.Value);
                queryParams.Add($"{encodedKey}={encodedValue}");
            }

            var separator = url.Contains("?") ? "&" : "?";
            return $"{url}{separator}{string.Join("&", queryParams)}";
        }

        private HttpContent PrepareContent(object data)
        {
            if (data == null)
                return null;

            string jsonString;
            if (data is string stringData)
            {
                jsonString = stringData;
            }
            else
            {
                jsonString = JsonSerializer.Serialize(data);
            }

            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        private void AddCustomHeaders(HttpRequestMessage request, Dictionary<string, string> headers)
        {
            if (headers == null) return;

            foreach (var header in headers)
            {
                // Try to add to request headers first, then content headers if it fails
                if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        private async Task<ApiResponse> MakeRequestAsync(HttpMethod method, string url, object data = null,
            Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null)
        {
            try
            {
                // Build full URL
                var fullUrl = BuildUrl(url);
                fullUrl = BuildUrlWithParams(fullUrl, parameters);

                // Create request
                var request = new HttpRequestMessage(method, fullUrl);
                
                // Add content if present
                if (data != null)
                {
                    request.Content = PrepareContent(data);
                }

                // Add custom headers
                AddCustomHeaders(request, headers);

                // Send request
                var response = await _httpClient.SendAsync(request);

                // Process response
                var responseData = await response.Content.ReadAsStringAsync();
                
                var result = new ApiResponse
                {
                    StatusCode = (int)response.StatusCode,
                    Data = responseData,
                    Success = response.IsSuccessStatusCode
                };

                // Extract headers
                foreach (var header in response.Headers)
                {
                    result.Headers[header.Key] = string.Join(", ", header.Value);
                }

                if (response.Content?.Headers != null)
                {
                    foreach (var header in response.Content.Headers)
                    {
                        result.Headers[header.Key] = string.Join(", ", header.Value);
                    }
                }

                if (!response.IsSuccessStatusCode)
                {
                    result.Error = $"HTTP {result.StatusCode}: {response.ReasonPhrase}";
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Error = $"HTTP Request Exception: {ex.Message}"
                };
            }
            catch (TaskCanceledException ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Error = ex.InnerException is TimeoutException ? "Request timeout" : $"Request cancelled: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Error = $"Request failed: {ex.Message}"
                };
            }
        }

        // Synchronous wrapper methods
        private ApiResponse MakeRequest(HttpMethod method, string url, object data = null,
            Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null)
        {
            return MakeRequestAsync(method, url, data, headers, parameters).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Make a GET request.
        /// </summary>
        public ApiResponse Get(string url, Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null)
        {
            return MakeRequest(HttpMethod.Get, url, null, headers, parameters);
        }

        /// <summary>
        /// Make a GET request asynchronously.
        /// </summary>
        public Task<ApiResponse> GetAsync(string url, Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null)
        {
            return MakeRequestAsync(HttpMethod.Get, url, null, headers, parameters);
        }

        /// <summary>
        /// Make a POST request.
        /// </summary>
        public ApiResponse Post(string url, object data = null,
            Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null)
        {
            return MakeRequest(HttpMethod.Post, url, data, headers, parameters);
        }

        /// <summary>
        /// Make a POST request asynchronously.
        /// </summary>
        public Task<ApiResponse> PostAsync(string url, object data = null,
            Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null)
        {
            return MakeRequestAsync(HttpMethod.Post, url, data, headers, parameters);
        }

        /// <summary>
        /// Update the client secret used in bearer authorization.
        /// </summary>
        public void UpdateClientSecret(string newSecret)
        {
            if (string.IsNullOrEmpty(newSecret))
                throw new ArgumentException("Client secret cannot be null or empty", nameof(newSecret));

            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {newSecret}");
        }

        /// <summary>
        /// Set timeout for HTTP requests.
        /// </summary>
        public void SetTimeout(TimeSpan timeout)
        {
            _httpClient.Timeout = timeout;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }
}