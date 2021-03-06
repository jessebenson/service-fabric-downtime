﻿using Serilog;
using Serilog.Core.Enrichers;
using System.Fabric;

namespace Common
{
	public static class ServiceExtensions
	{
		/// <summary>
		/// Enrich the Serilog logger with context from the stateless service.
		/// </summary>
		public static ILogger WithServiceContext(this ILogger logger, StatelessServiceContext context)
		{
			return logger.ForContext(new[]
			{
				new PropertyEnricher("PartitionId", context.PartitionId),
				new PropertyEnricher("InstanceId", context.InstanceId),
				new PropertyEnricher("ServiceName", context.ServiceName),
				new PropertyEnricher("ServiceTypeName", context.ServiceTypeName),
			});
		}

		/// <summary>
		/// Enrich the Serilog logger with context from the stateful service.
		/// </summary>
		public static ILogger WithServiceContext(this ILogger logger, StatefulServiceContext context)
		{
			return logger.ForContext(new[]
			{
				new PropertyEnricher("PartitionId", context.PartitionId),
				new PropertyEnricher("ReplicaId", context.ReplicaId),
				new PropertyEnricher("ServiceName", context.ServiceName),
				new PropertyEnricher("ServiceTypeName", context.ServiceTypeName),
			});
		}
	}
}
