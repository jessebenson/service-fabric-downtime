using Common;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using System;
using System.Fabric;
using System.Threading;

namespace Web
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

				ServiceRuntime.RegisterServiceAsync("WebType",
					context => new Web(context, logger)).GetAwaiter().GetResult();

				Log.Information("Service host process registered service type {ServiceTypeName}.", "WebType");

				Thread.Sleep(Timeout.Infinite);
			}
			catch (Exception e)
			{
				Log.Error(e, "Service host process initialization failed for service type {ServiceTypeName}.", "WebType");
				throw;
			}
		}
	}
}
