using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yort.Dms.Shared.Tests
{
	[TestClass]
	public class BehaviourTests
	{

		[TestMethod]
		public async Task DeadManSwitch_DoesNotFireWhenReset()
		{
			bool activated = false;
			using (var dms = new DeadManSwitch(TimeSpan.FromMilliseconds(250), () => activated = true, false))
			{
				for (int cnt = 0; cnt < 10; cnt++)
				{
					await Task.Delay(100);
					dms.Reset();
				}
				Assert.AreEqual(false, activated);
			}
		}

		[TestMethod]
		public async Task DeadManSwitch_FiresWhenNotReset()
		{
			bool activated = false;
			using (var dms = new DeadManSwitch(TimeSpan.FromMilliseconds(250), () => activated = true, false))
			{
				for (int cnt = 0; cnt < 10; cnt++)
				{
					await Task.Delay(100);
					dms.Reset();
				}
				Assert.AreEqual(false, activated, "Switch was incorrectly activated prior to interval elapsing.");

				await Task.Delay(260);

				Assert.AreEqual(true, activated, "Switch not activated after specified interval.");
			}
		}

		[TestMethod]
		public async Task DeadManSwitch_DoesNotFireAfterDispose()
		{
			bool activated = false;
			using (var dms = new DeadManSwitch(TimeSpan.FromMilliseconds(250), () => activated = true, false))
			{
				for (int cnt = 0; cnt < 10; cnt++)
				{
					await Task.Delay(100);
					dms.Reset();
				}
				Assert.AreEqual(false, activated, "Switch was incorrectly activated prior to interval elapsing.");
			}

			await Task.Delay(260);

			Assert.AreEqual(false, activated, "Switch activated after it was disposed.");
		}


	}
}