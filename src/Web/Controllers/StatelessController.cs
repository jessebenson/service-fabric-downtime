using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	public class StatelessController : Controller
	{
		private static readonly HttpClient _reverseProxy = new HttpClient { BaseAddress = new Uri("http://localhost:19081/App/StatelessSvc") };
		private static readonly HttpClient _statelessSvc = new HttpClient { BaseAddress = new Uri("http://stateless.app:8546") };

		// GET api/stateless/dns
		[HttpGet("dns")]
		public async Task<IActionResult> GetWithDns()
		{
			try
			{
				var response = await _statelessSvc.GetAsync("api/value").ConfigureAwait(false);
				if (!response.IsSuccessStatusCode)
					return StatusCode((int)response.StatusCode);

				string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				return Ok(long.Parse(content));
			}
			catch (Exception e)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
			}
		}

		// GET api/stateless/reverse-proxy
		[HttpGet("reverse-proxy")]
		public async Task<IActionResult> GetWithReverseProxy()
		{
			try
			{
				// Default timeout is 60 seconds.
				var response = await _reverseProxy.GetAsync("api/value").ConfigureAwait(false);
				if (!response.IsSuccessStatusCode)
					return StatusCode((int)response.StatusCode);

				string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				return Ok(long.Parse(content));
			}
			catch (Exception e)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
			}
		}
	}
}
