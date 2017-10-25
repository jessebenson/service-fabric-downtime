using ActorSvc.Interfaces;
using Common;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ActorSvc
{
	/// <remarks>
	/// This class represents an actor.
	/// Every ActorID maps to an instance of this class.
	/// The StatePersistence attribute determines persistence and replication of actor state:
	///  - Persisted: State is written to disk and replicated.
	///  - Volatile: State is kept in memory only and replicated.
	///  - None: State is kept in memory only and not replicated.
	/// </remarks>
	[StatePersistence(StatePersistence.Persisted)]
	internal class ActorSvc : Actor, IActorSvc
	{
		private readonly ILogger _logger;

		/// <summary>
		/// Initializes a new instance of ActorSvc
		/// </summary>
		/// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
		/// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
		public ActorSvc(ActorService actorService, ActorId actorId, ILogger logger)
			: base(actorService, actorId)
		{
			_logger = logger.WithServiceContext(actorService.Context);
		}

		protected override Task OnActivateAsync()
		{
			_logger.Information("Service Fabric API {ServiceFabricApi}.  Actor id: {ActorId}.", "OnActivateAsync", this.Id);
			return base.OnActivateAsync();
		}

		protected override Task OnDeactivateAsync()
		{
			_logger.Information("Service Fabric API {ServiceFabricApi}.  Actor id: {ActorId}.", "OnDeactivateAsync", this.Id);
			return base.OnDeactivateAsync();
		}

		async Task<long> IActorSvc.CountAsync(Guid correlationId, CancellationToken cancellationToken)
		{
			var timer = Stopwatch.StartNew();
			try
			{
				long value = await this.StateManager.AddOrUpdateStateAsync<long>("count", 0, (k, v) => v + 1, cancellationToken).ConfigureAwait(false);

				_logger.Information("{MethodName} completed in {ElapsedTime} ms. {CorrelationId}", "ActorSvc.Count", timer.ElapsedMilliseconds, correlationId);

				return value;
			}
			catch (Exception e)
			{
				_logger.Error(e, "{MethodName} failed in {ElapsedTime} ms. {CorrelationId}", "ActorSvc.Count", timer.ElapsedMilliseconds, correlationId);
				throw;
			}
		}
	}
}
