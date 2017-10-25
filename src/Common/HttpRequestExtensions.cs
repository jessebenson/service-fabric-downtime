using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Net.Http;

namespace Common
{
	public static class HttpRequestExtensions
	{
		public static string GetCorrelationId(this HttpRequest request)
		{
			if (request.Headers.TryGetValue("correlation-id", out StringValues header))
				return header.ToString();
			return Guid.Empty.ToString();
		}

		public static HttpRequestMessage AddCorrelationId(this HttpRequestMessage request, string correlationId)
		{
			request.Headers.Add("correlation-id", correlationId);
			return request;
		}
	}
}
