using System;
using System.Threading.Tasks;

namespace Client
{
	class Program
	{
		static async Task MainAsync(string[] args)
		{


			Console.WriteLine("Press enter to exit ...");
			Console.ReadLine();
		}

		static void Main(string[] args)
		{
			MainAsync(args).GetAwaiter().GetResult();
		}
	}
}
