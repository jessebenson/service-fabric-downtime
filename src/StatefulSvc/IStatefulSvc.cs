using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatefulSvc
{
	public interface IStatefulSvc
	{
		Task<long> IncrementAsync(CancellationToken token);
	}
}
