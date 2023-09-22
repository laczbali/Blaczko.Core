using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Blaczko.Core.Wrappers
{
    public class HttpClientWrapper
    {
        private readonly IHttpClientFactory clientFactory;

        public HttpClientWrapper(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        /// <summary>
        /// Send a request to the specified endpoint
        /// </summary>
        /// <param name="url">Where to send the request</param>
        /// <param name="method">What method to user</param>
        /// <param name="content">
        ///		Content to send
        ///		(if needed, use extension methods in the HttpClientWrapper class to create)
        /// </param>
        /// <returns></returns>
        public async Task<string> MakeRequestAsync(
            string url,
            HttpMethod method,
            HttpContent content)
        {
            using (var client = GetBaseClient(url))
            {
                var message = CreateMessage(method, content);
                return await SendRequest(client, message);
            }
        }

        /// <inheritdoc cref="MakeRequestAsync(string, HttpMethod, HttpContent)"/>
        /// <remarks>
        /// The response is deserialized into the specified type. If not successful, it will throw an exception.
        /// </remarks>
        /// <typeparam name="ResponseType">Target type to deserialize the result into</typeparam>
        public async Task<ResponseType> MakeRequestAsync<ResponseType>(
            string url,
            HttpMethod method,
            HttpContent content)
        {
            var rawResponse = await MakeRequestAsync(url, method, content);
            return DeserializeResponse<ResponseType>(rawResponse);
        }

        /// <inheritdoc cref="MakeRequestAsync(string, HttpMethod, HttpContent)"/>
        /// <param name="authScheme">Eg "Bearer" or "Bot"</param>
        /// <param name="authValue">Eg the bearer token value</param>
        public async Task<string> MakeRequestAsync(
            string url,
            HttpMethod method,
            HttpContent content,
            string authScheme,
            string authValue)
        {
            using (var client = GetBaseClient(url))
            {
                var message = CreateMessage(method, content, new AuthenticationHeaderValue(authScheme, authValue));
                return await SendRequest(client, message);
            }
        }

        /// <inheritdoc cref="MakeRequestAsync{ResponseType}(string, HttpMethod, HttpContent)"/>
        /// <inheritdoc cref="MakeRequestAsync(string, HttpMethod, HttpContent, string, string)"/>
        public async Task<ResponseType> MakeRequestAsync<ResponseType>(
            string url,
            HttpMethod method,
            HttpContent content,
            string authScheme,
            string authValue)
        {
            var rawResponse = await MakeRequestAsync(url, method, content, authScheme, authValue);
            return DeserializeResponse<ResponseType>(rawResponse);
        }

        /// <summary>
        /// Formats the object into a HttpContent object, to be used in a request
        /// </summary>
        public static HttpContent ToHttpJsonContent(object obj)
        {
            return JsonContent.Create(obj);
        }

        private HttpClient GetBaseClient(string url)
        {
            var client = clientFactory.CreateClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.ConnectionClose = true;
            return client;
        }

        private static HttpRequestMessage CreateMessage(HttpMethod method, HttpContent content = null, AuthenticationHeaderValue auth = null)
        {
            var requestMessage = new HttpRequestMessage();
            requestMessage.Method = method;

            if (content is not null)
            {
                requestMessage.Content = content;
            }

            if (auth is not null)
            {
                requestMessage.Headers.Authorization = auth;
            }

            return requestMessage;
        }

        private static async Task<string> SendRequest(HttpClient client, HttpRequestMessage requestMessage)
        {
            var result = await client.SendAsync(requestMessage);
            var responseBody = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP ERROR - {result.StatusCode} - {responseBody}");
            }

            return responseBody;
        }

        private static ResponseType DeserializeResponse<ResponseType>(string rawResponse)
        {
            try
            {
                return JsonConvert.DeserializeObject<ResponseType>(rawResponse);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to deserialize [{rawResponse}] into [{typeof(ResponseType).Name}]", e);
            }
        }

    }
}
