using Common;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	public class StatelessController : Controller
	{
		private static readonly HttpClient _reverseProxy = new HttpClient { BaseAddress = new Uri("http://localhost:19081/App/StatelessSvc/") };
		private static readonly HttpClient _statelessSvc = new HttpClient { BaseAddress = new Uri("http://stateless.app:8546") };

		private readonly ILogger _logger;

		public StatelessController(ILogger logger)
		{
			_logger = logger;
		}

		// GET api/stateless/dns
		[HttpGet("dns")]
		public async Task<IActionResult> GetWithDns()
		{
			var timer = Stopwatch.StartNew();
			var correlationId = HttpContext.Request.GetCorrelationId();
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, "api/count")
					.AddCorrelationId(correlationId);

				var response = await _statelessSvc.SendAsync(request).ConfigureAwait(false);
				if (!response.IsSuccessStatusCode)
				{
					_logger.Error("{MethodName} failed with {StatusCode} in {ElapsedTime} ms. {CorrelationId}", "api/stateless/dns", (int)response.StatusCode, timer.ElapsedMilliseconds, correlationId);
					return StatusCode((int)response.StatusCode);
				}

				string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				long value = long.Parse(content);

				_logger.Information("{MethodName} completed with {StatusCode} in {ElapsedTime} ms. {CorrelationId}", "api/stateless/dns", (int)HttpStatusCode.OK, timer.ElapsedMilliseconds, correlationId);

				return Ok(value);
			}
			catch (Exception e)
			{
				_logger.Error(e, "{MethodName} failed with {StatusCode} in {ElapsedTime} ms. {CorrelationId}", "api/stateless/dns", (int)HttpStatusCode.InternalServerError, timer.ElapsedMilliseconds, correlationId);
				return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
			}
		}

		// GET api/stateless/reverse-proxy
		[HttpGet("reverse-proxy")]
		public async Task<IActionResult> GetWithReverseProxy()
		{
			var timer = Stopwatch.StartNew();
			var correlationId = HttpContext.Request.GetCorrelationId();
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Get, "api/count")
					.AddCorrelationId(correlationId);

				// Default timeout is 60 seconds.
				var response = await _reverseProxy.SendAsync(request).ConfigureAwait(false);
				if (!response.IsSuccessStatusCode)
				{
					_logger.Error("{MethodName} failed with {StatusCode} in {ElapsedTime} ms. {CorrelationId}", "api/stateless/reverse-proxy", (int)response.StatusCode, timer.ElapsedMilliseconds, correlationId);
					return StatusCode((int)response.StatusCode);
				}

				string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				long value = long.Parse(content);

				_logger.Information("{MethodName} completed with {StatusCode} in {ElapsedTime} ms. {CorrelationId}", "api/stateless/reverse-proxy", (int)HttpStatusCode.OK, timer.ElapsedMilliseconds, correlationId);

				return Ok(value);
			}
			catch (Exception e)
			{
				_logger.Error(e, "{MethodName} failed with {StatusCode} in {ElapsedTime} ms. {CorrelationId}", "api/stateless/reverse-proxy", (int)HttpStatusCode.InternalServerError, timer.ElapsedMilliseconds, correlationId);
				return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
			}
		}
	}
}
