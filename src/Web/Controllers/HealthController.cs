using System;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	public class HealthController : Controller
	{
		private readonly IWebService _webService;

		public HealthController(IWebService webService)
		{
			_webService = webService;
		}

		// GET api/health
		[HttpGet]
		public IActionResult Get()
		{
			return StatusCode((int)_webService.GetHealth());
		}
	}
}
