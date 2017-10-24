using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatefulSvc
{
	interface IStatefulSvc
	{
		Task<long> GetValueAsync(string key, CancellationToken token);
		Task SetValueAsync(string key, long value, CancellationToken token);
	}
}
