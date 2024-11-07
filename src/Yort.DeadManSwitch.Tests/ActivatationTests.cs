using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yort.Dms.Tests
{
	[TestClass]
	public class ActivationTests
	{

		[TestMethod]
		public async Task DeadManSwitch_FiresWhenNotReset()
		{
			var delay = 250;
			bool activated = false;
			using (var dms = new DeadManSwitch(TimeSpan.FromMilliseconds(delay), () => activated = true, false))
			{
				for (int cnt = 0; cnt < 10; cnt++)
				{
					await Task.Delay(delay / 10);
					dms.Reset();
				}
				Assert.AreEqual(false, activated, "Switch was incorrectly activated prior to interval elapsing.");

				await Task.Delay(delay * 2);

				Assert.AreEqual(true, activated, "Switch not activated after specified interval.");
			}
		}

		[TestMethod]
		public async Task DeadManSwitch_FiresIfResetNeverCalled()
		{
			bool activated = false;
			using (var dms = new DeadManSwitch(TimeSpan.FromMilliseconds(250), () => activated = true, false))
			{
				await Task.Delay(270);

				Assert.AreEqual(true, activated, "Switch not activated after specified interval.");
			}
		}

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

			await Task.Delay(270);

			Assert.AreEqual(false, activated, "Switch activated after it was disposed.");
		}

		[TestMethod]
		public async Task DeadManSwitch_DoesNotFireWhenDisarmed()
		{
			var delay = 250;
			bool activated = false;
			using (var dms = new DeadManSwitch(TimeSpan.FromMilliseconds(delay), () => activated = true, false))
			{
				dms.Disarm();

				await Task.Delay(delay * 2);
				Assert.AreEqual(false, activated, "Switch was incorrectly activated prior to interval elapsing.");
			}

			await Task.Delay(270);

			Assert.AreEqual(false, activated, "Switch activated after it was disposed.");
		}

	}
}