using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Net.Http;

namespace Common
{
	public static class HttpRequestExtensions
	{
		public static Guid GetCorrelationId(this HttpRequest request)
		{
			Guid correlationId = Guid.Empty;
			if (request.Headers.TryGetValue("correlation-id", out StringValues header))
				correlationId = Guid.Parse(header);

			return correlationId;
		}

		public static HttpRequestMessage AddCorrelationId(this HttpRequestMessage request, Guid correlationId)
		{
			request.Headers.Add("correlation-id", correlationId.ToString());
			return request;
		}
	}
}
