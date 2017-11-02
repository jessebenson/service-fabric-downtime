using Common;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using System;
using System.Diagnostics;
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
				var timer = Stopwatch.StartNew();
				var nodeContext = FabricRuntime.GetNodeContext();
				long nodeContextTime = timer.ElapsedMilliseconds;

				timer.Restart();
				var activationContext = FabricRuntime.GetActivationContext();
				long activationContextTime = timer.ElapsedMilliseconds;

				timer.Restart();
				var logger = LogConfig.CreateLogger(nodeContext, activationContext);
				long createLoggerTime = timer.ElapsedMilliseconds;

				ServiceRuntime.RegisterServiceAsync("WebType",
					context => new Web(context, logger)).GetAwaiter().GetResult();

				Log.Information("Service host process registered service type {ServiceTypeName}. GetNodeContext: {GetNodeContextTimeInMs} ms. GetActivationContext: {GetActivationContextTimeInMs} ms. CreateLogger: {CreateLoggerTimeInMs} ms.", "WebType", nodeContextTime, activationContextTime, createLoggerTime);

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
