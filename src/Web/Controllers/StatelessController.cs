using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	public class StatelessController : Controller
	{
		private static readonly HttpClient _statelessSvc = new HttpClient { BaseAddress = new Uri("http://stateless.app:8546") };

		// GET api/stateless/dns
		[HttpGet("dns")]
		public async Task<IActionResult> Get()
		{
			var response = await _statelessSvc.GetAsync($"api/value").ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
				return StatusCode((int)response.StatusCode);

			string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			return Ok(long.Parse(content));
		}
	}
}
