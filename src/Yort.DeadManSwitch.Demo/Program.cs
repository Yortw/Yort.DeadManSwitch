using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yort.Dms;

namespace Yort.Dms.Demo
{
	class Program
	{
		private static DeadManSwitch _DSwitch;

		static void Main(string[] args)
		{
			_DSwitch = new DeadManSwitch(5000, Fired, Reset, true);

			while (true)
			{
				var k = Console.ReadKey();
				if (k.Key == ConsoleKey.X) break;
				_DSwitch.Reset();
			}
		}

		private static void Fired()
		{
			Console.WriteLine ("Boom!");
		}

		private static void Reset(DeadManSwitchResetReason reason)
		{
			if (reason == DeadManSwitchResetReason.Initialize)
				Console.WriteLine("Dead man switch active... Press key within 5 seconds...");
			else
				Console.WriteLine("Reset...Press key within 5 seconds...");
		}
	}
}
