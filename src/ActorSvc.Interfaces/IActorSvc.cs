﻿using System;
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
		Task<long> GetValueAsync(string key, CancellationToken token);
		Task SetValueAsync(string key, long value, CancellationToken token);
	}
}
