using System;
using System.Net;

namespace Web
{
	public interface IWebService
	{
		HttpStatusCode GetHealth();
	}
}
