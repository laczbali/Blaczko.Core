using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Blaczko.Core.Wrappers
{
	public static class HttpClientWrapper
	{
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
		public static async Task<string> MakeRequestAsync(
			string url,
			HttpMethod method,
			HttpContent content)
		{
			var client = GetBaseClient(url);
			var message = CreateMessage(method, content);
			return await client.SendRequest(message);
		}

		/// <inheritdoc cref="MakeRequestAsync(string, HttpMethod, HttpContent)"/>
		/// <remarks>
		/// The response is deserialized into the specified type. If not successful, it will throw an exception.
		/// </remarks>
		/// <typeparam name="ResponseType">Target type to deserialize the result into</typeparam>
		public static async Task<ResponseType> MakeRequestAsync<ResponseType>(
			string url,
			HttpMethod method,
			HttpContent content)
		{
			var rawResponse = await MakeRequestAsync(url, method, content);
			return rawResponse.DeserializeResponse<ResponseType>();
		}

		/// <inheritdoc cref="MakeRequestAsync(string, HttpMethod, HttpContent)"/>
		/// <param name="authScheme">Eg "Bearer" or "Bot"</param>
		/// <param name="authValue">Eg the bearer token value</param>
		public static async Task<string> MakeRequestAsync(
			string url,
			HttpMethod method,
			HttpContent content,
			string authScheme,
			string authValue)
		{
			var client = GetBaseClient(url);
			var message = CreateMessage(method, content, new AuthenticationHeaderValue(authScheme, authValue));
			return await client.SendRequest(message);
		}

		/// <inheritdoc cref="MakeRequestAsync{ResponseType}(string, HttpMethod, HttpContent)"/>
		/// <inheritdoc cref="MakeRequestAsync(string, HttpMethod, HttpContent, string, string)"/>
		public static async Task<ResponseType> MakeRequestAsync<ResponseType>(
			string url,
			HttpMethod method,
			HttpContent content,
			string authScheme,
			string authValue)
		{
			var rawResponse = await MakeRequestAsync(url, method, content, authScheme, authValue);
			return rawResponse.DeserializeResponse<ResponseType>();
		}

		/// <summary>
		/// Formats the object into a HttpContent object, to be used in a request
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static HttpContent ToHttpJsonContent(this object obj)
		{
			return JsonContent.Create(obj);
		}

		private static HttpClient GetBaseClient(string url)
		{
			var client = new HttpClient();
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

		private static async Task<string> SendRequest(this HttpClient client, HttpRequestMessage requestMessage)
		{
			var result = await client.SendAsync(requestMessage);
			string responseBody = result.Content.ReadAsStringAsync().Result;
			if (!result.IsSuccessStatusCode)
			{
				throw new Exception($"HTTP ERROR - {result.StatusCode} - {responseBody}");
			}

			return responseBody;
		}

		private static ResponseType DeserializeResponse<ResponseType>(this string rawResponse)
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
