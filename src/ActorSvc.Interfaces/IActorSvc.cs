using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ActorSvc.Interfaces
{
	/// <summary>
	/// This interface defines the methods exposed by an actor.
	/// Clients use this interface to interact with the actor that implements it.
	/// </summary>
	public interface IActorSvc : IActor
	{
		Task<int> GetCountAsync(CancellationToken cancellationToken);
		
		Task SetCountAsync(int count, CancellationToken cancellationToken);
	}
}
