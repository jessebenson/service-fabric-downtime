using Common;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using Serilog;
using System;
using System.Fabric;
using System.Threading;

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
			try
			{
				var logger = LogConfig.CreateLogger(FabricRuntime.GetNodeContext(), FabricRuntime.GetActivationContext());

				ActorRuntime.RegisterActorAsync<ActorSvc>(
				   (context, actorType) => new ActorService(context, actorType, (svc, id) => new ActorSvc(svc, id, logger))
				).GetAwaiter().GetResult();

				Log.Information("Service host process registered service type {ServiceTypeName}.", "ActorSvcActorServiceType");

				Thread.Sleep(Timeout.Infinite);
			}
			catch (Exception e)
			{
				Log.Error(e, "Service host process initialization failed for service type {ServiceTypeName}.", "ActorSvcActorServiceType");
				throw;
			}
		}
	}
}
