using Common;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;

namespace StatelessSvc.Controllers
{
	[Route("api/[controller]")]
	public class CountController : Controller
	{
		private readonly ILogger _logger;

		public CountController(ILogger logger)
		{
			_logger = logger;
		}

		// GET api/count/dns
		[HttpGet("dns")]
		public IActionResult CountWithDns()
		{
			var correlationId = HttpContext.Request.GetCorrelationId();
			_logger.Information("{MethodName} completed in {ElapsedTime} ms. {CorrelationId}", "api/count/dns", 0, correlationId);
			return Ok(17);
		}

		// GET api/count/reverse-proxy
		[HttpGet("reverse-proxy")]
		public IActionResult CountWithReverseProxy()
		{
			var correlationId = HttpContext.Request.GetCorrelationId();
			_logger.Information("{MethodName} completed in {ElapsedTime} ms. {CorrelationId}", "api/count/reverse-proxy", 0, correlationId);
			return Ok(17);
		}
	}
}
