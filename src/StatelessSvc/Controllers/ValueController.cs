using System;
using Microsoft.AspNetCore.Mvc;

namespace StatelessSvc.Controllers
{
	[Route("api/[controller]")]
	public class ValueController : Controller
	{
		// GET api/value
		[HttpGet]
		public IActionResult Get()
		{
			return Ok(17);
		}
	}
}
