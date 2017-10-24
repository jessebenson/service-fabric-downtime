﻿using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListener = RemotingListener.V2Listener, RemotingClient = RemotingClient.V2Client)]
namespace ActorSvc
{
	internal static class Program
	{
		/// <summary>
		/// This is the entry point of the service host process.
		/// </summary>
		private static void Main()
		{
			// This line registers an Actor Service to host your actor class with the Service Fabric runtime.
			// The contents of your ServiceManifest.xml and ApplicationManifest.xml files
			// are automatically populated when you build this project.
			// For more information, see https://aka.ms/servicefabricactorsplatform

			ActorRuntime.RegisterActorAsync<ActorSvc>(
			   (context, actorType) => new ActorService(context, actorType)).GetAwaiter().GetResult();

			Thread.Sleep(Timeout.Infinite);
		}
	}
}
