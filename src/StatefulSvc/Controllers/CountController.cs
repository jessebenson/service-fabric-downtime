using Common;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Diagnostics;
using System.Fabric;
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

				_logger.Information("{MethodName} completed with {StatusCode} in {ElapsedTime} ms. {CorrelationId}", "api/count/reverse-proxy", (int)HttpStatusCode.OK, timer.ElapsedMilliseconds, correlationId);

				return Ok(value);
			}
			catch (Exception e)
			{
				// NotFound will allow Service Fabric's reverse proxy to re-resolve the new primary.
				var status = IsFailoverException(e) ? HttpStatusCode.NotFound : HttpStatusCode.InternalServerError;

				_logger.Error(e, "{MethodName} failed with {StatusCode} in {ElapsedTime} ms. {CorrelationId}", "api/count/reverse-proxy", (int)status, timer.ElapsedMilliseconds, correlationId);
				return StatusCode((int)status, e.Message);
			}
		}

		private static bool IsFailoverException(Exception e)
		{
			if (e is FabricNotPrimaryException || e is FabricNotReadableException)
				return true;

			var agg = e as AggregateException;
			if (agg != null)
			{
				foreach (var ex in agg.Flatten().InnerExceptions)
				{
					if (ex is FabricNotPrimaryException || ex is FabricNotReadableException)
						return true;
				}
			}

			return false;
		}
	}
}
