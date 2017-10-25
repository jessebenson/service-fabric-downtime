using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Net;

namespace Web
{
	/// <summary>
	/// The FabricRuntime creates an instance of this class for each service type instance. 
	/// </summary>
	internal sealed class Web : StatelessService, IWebService
	{
		private const bool HealthCheck = false;

		private HttpStatusCode _health = HttpStatusCode.OK;

		public Web(StatelessServiceContext context)
			: base(context)
		{ }

		HttpStatusCode IWebService.GetHealth()
		{
			return _health;
		}

		protected override async Task RunAsync(CancellationToken cancellationToken)
		{
			await base.RunAsync(cancellationToken).ConfigureAwait(false);

			// Service is available to handle new requests.
			_health = HttpStatusCode.OK;

			var shutdownToken = new TaskCompletionSource<bool>();
			cancellationToken.Register(() =>
			{
				// Service is unavailable to handle new requests.
				_health = HttpStatusCode.ServiceUnavailable;

				// On shutdown, wait 30 seconds to give Load Balancer time to stop sending new requests.
				Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(t =>
				{
					shutdownToken.SetResult(true);
				});
			});

			// Prevent front-end from closing until requests are drained.
			await shutdownToken.Task.ConfigureAwait(false);
		}

		/// <summary>
		/// Optional override to create listeners (like tcp, http) for this service instance.
		/// </summary>
		/// <returns>The collection of listeners.</returns>
		protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
		{
			return new ServiceInstanceListener[]
			{
				new ServiceInstanceListener(serviceContext =>
					new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
					{
						return new WebHostBuilder()
							.UseKestrel()
							.ConfigureServices(
								services => services
									.AddSingleton<StatelessServiceContext>(serviceContext)
									.AddSingleton<IWebService>(this)
							)
							.UseContentRoot(Directory.GetCurrentDirectory())
							.UseStartup<Startup>()
							.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
							.UseUrls(url)
							.Build();
					}))
			};
		}
	}
}
