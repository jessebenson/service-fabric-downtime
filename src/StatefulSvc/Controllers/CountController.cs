using Common;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace StatefulSvc.Controllers
{
	[Route("api/[controller]")]
	public class CountController : Controller
	{
		private readonly IStatefulSvc _statefulSvc;
		private readonly ILogger _logger;

		public CountController(IStatefulSvc statefulSvc, ILogger logger)
		{
			_statefulSvc = statefulSvc;
			_logger = logger;
		}

		// GET api/count/reverse-proxy
		[HttpGet("reverse-proxy")]
		public async Task<IActionResult> Count(CancellationToken token)
		{
			var timer = Stopwatch.StartNew();
			var correlationId = HttpContext.Request.GetCorrelationId();
			try
			{
				long value = await _statefulSvc.IncrementAsync(token).ConfigureAwait(false);

				_logger.Information("{MethodName} completed in {ElapsedTime} ms. {CorrelationId}", "api/count/reverse-proxy", timer.ElapsedMilliseconds, correlationId);

				return Ok(value);
			}
			catch (Exception e)
			{
				_logger.Error(e, "{MethodName} failed in {ElapsedTime} ms. {CorrelationId}", "api/count/reverse-proxy", timer.ElapsedMilliseconds, correlationId);
				return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
			}
		}
	}
}
