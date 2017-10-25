using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Serilog;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StatefulSvc
{
	/// <summary>
	/// The FabricRuntime creates an instance of this class for each service type instance. 
	/// </summary>
	internal sealed class StatefulSvc : LoggingStatefulService, IStatefulSvc
	{
		public StatefulSvc(StatefulServiceContext context, ILogger logger)
			: base(context, logger)
		{ }

		public async Task<long> IncrementAsync(CancellationToken token)
		{
			var state = await StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("state").ConfigureAwait(false);

			using (var tx = StateManager.CreateTransaction())
			{
				long result = await state.AddOrUpdateAsync(tx, "count", 0, (k, v) => v + 1, TimeSpan.FromSeconds(4), token).ConfigureAwait(false);
				await tx.CommitAsync().ConfigureAwait(false);

				return result;
			}
		}

		/// <summary>
		/// Optional override to create listeners (like tcp, http) for this service instance.
		/// </summary>
		/// <returns>The collection of listeners.</returns>
		protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
		{
			return new ServiceReplicaListener[]
			{
				new ServiceReplicaListener(serviceContext =>
					new KestrelCommunicationListener(serviceContext, (url, listener) =>
					{
						return new WebHostBuilder()
							.UseKestrel()
							.ConfigureServices(
								services => services
									.AddSingleton<ILogger>(_logger)
									.AddSingleton<IReliableStateManager>(this.StateManager)
									.AddSingleton<StatefulServiceContext>(serviceContext)
									.AddSingleton<IStatefulSvc>(this)
							)
							.UseContentRoot(Directory.GetCurrentDirectory())
							.UseStartup<Startup>()
							.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
							.UseUrls(url)
							.Build();
					}))
			};
		}
	}
}
