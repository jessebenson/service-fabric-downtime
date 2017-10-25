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

		// GET api/count
		[HttpGet]
		public IActionResult Count()
		{
			Guid correlationId = HttpContext.Request.GetCorrelationId();
			_logger.Information("{MethodName} completed in {ElapsedTime} ms. {CorrelationId}", "StatelessSvc.Count", 0, correlationId);
			return Ok(17);
		}
	}
}
