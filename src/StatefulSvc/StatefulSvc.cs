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
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace StatefulSvc
{
	/// <summary>
	/// The FabricRuntime creates an instance of this class for each service type instance. 
	/// </summary>
	internal sealed class StatefulSvc : StatefulService, IStatefulSvc
	{
		public StatefulSvc(StatefulServiceContext context)
			: base(context)
		{ }

		public async Task<long> GetValueAsync(string key, CancellationToken token)
		{
			var state = await StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("state").ConfigureAwait(false);

			using (var tx = StateManager.CreateTransaction())
			{
				var result = await state.TryGetValueAsync(tx, key, TimeSpan.FromSeconds(4), token).ConfigureAwait(false);
				await tx.CommitAsync().ConfigureAwait(false);

				return result.Value;
			}
		}

		public async Task SetValueAsync(string key, long value, CancellationToken token)
		{
			var state = await StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("state").ConfigureAwait(false);

			using (var tx = StateManager.CreateTransaction())
			{
				await state.SetAsync(tx, key, value, TimeSpan.FromSeconds(4), token).ConfigureAwait(false);
				await tx.CommitAsync().ConfigureAwait(false);
			}
		}

		protected override async Task RunAsync(CancellationToken cancellationToken)
		{
			var state = await StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("state").ConfigureAwait(false);

			await base.RunAsync(cancellationToken).ConfigureAwait(false);
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
									.AddSingleton<StatefulServiceContext>(serviceContext)
									.AddSingleton<IReliableStateManager>(this.StateManager))
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
