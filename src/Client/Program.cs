using Common;
using Serilog.Core.Enrichers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
	class Program
	{
		static async Task MainAsync(string[] args)
		{
			int taskCount = args.Length > 0 ? int.Parse(args[0]) : 3;
			string clientName = args.Length > 1 ? args[1] : "Client";

			// Create Elasticsearch logger.
			var logger = LogConfig.CreateLogger()
				.ForContext(new PropertyEnricher("ClientName", clientName));

			// HttpClient to service.
			var client = new HttpClient { BaseAddress = new Uri("http://service-fabric.eastus.cloudapp.azure.com") };

			// Spawn concurrent tasks.
			var tasks = Enumerable.Range(0, taskCount).Select(i => Task.Run(async () =>
			{
				string endpoint = GetEndpoint(i);
				while (true)
				{
					await Task.Delay(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false);

					var task = Task.Run(async () =>
					{
						var timer = Stopwatch.StartNew();
						var correlationId = Guid.NewGuid().ToString();
						try
						{
							// GET from service.
							var request = new HttpRequestMessage(HttpMethod.Get, endpoint)
								.AddCorrelationId(correlationId);

							var response = await client.SendAsync(request).ConfigureAwait(false);

							if (response.IsSuccessStatusCode)
							{
								logger.Information("{MethodName} completed with {StatusCode} in {ElapsedTime} ms. {CorrelationId}", endpoint, (int)response.StatusCode, timer.ElapsedMilliseconds, correlationId);
							}
							else
							{
								logger.Error("{MethodName} failed with {StatusCode} in {ElapsedTime} ms. {CorrelationId}", endpoint, (int)response.StatusCode, timer.ElapsedMilliseconds, correlationId);
							}
						}
						catch (Exception e)
						{
							logger.Error(e, "{MethodName} failed in {ElapsedTime} ms. {CorrelationId}", endpoint, timer.ElapsedMilliseconds, correlationId);
						}
					});
				}
			}));

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		private static string GetEndpoint(int i)
		{
			if (i % 3 == 0)
				return "api/stateless/dns";
			if (i % 3 == 1)
				return "api/stateless/reverse-proxy";

			return "api/stateful/reverse-proxy";
		}

		static void Main(string[] args)
		{
			MainAsync(args).GetAwaiter().GetResult();
		}
	}
}
