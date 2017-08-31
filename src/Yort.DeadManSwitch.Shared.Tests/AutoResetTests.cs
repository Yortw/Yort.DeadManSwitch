using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yort.Dms.Shared.Tests
{
	[TestClass]
	public class AutoResetTests
	{
		[TestMethod]
		public void DeadManSwitch_ResetCallback_CalledOnConstruction()
		{
			DeadManSwitchResetReason resetReason = DeadManSwitchResetReason.ManualReset;
			using (var dms = new DeadManSwitch(TimeSpan.FromMilliseconds(100), () => { }, (r) => resetReason = r, false))
			{
				Assert.AreEqual(DeadManSwitchResetReason.Initialize, resetReason, "Reset callback not called or not called with initialize as the reason when switch constructed.");
			}
		}

		[TestMethod]
		public void DeadManSwitch_ResetCallback_CalledOnManualReset()
		{
			DeadManSwitchResetReason resetReason = DeadManSwitchResetReason.Initialize;
			using (var dms = new DeadManSwitch(TimeSpan.FromMilliseconds(100), () => { }, (r) => resetReason = r, false))
			{
				dms.Reset();

				Assert.AreEqual(DeadManSwitchResetReason.ManualReset, resetReason, "Reset callback not called or not called with  manual as reason after calling Reset() method.");
			}
		}

		[TestMethod]
		public async Task DeadManSwitch_ResetCallback_CalledOnAutoReset()
		{
			DeadManSwitchResetReason resetReason = DeadManSwitchResetReason.Initialize;
			using (var dms = new DeadManSwitch(TimeSpan.FromMilliseconds(100), () => { }, (r) => resetReason = r, true))
			{
				await Task.Delay(150);

				Assert.AreEqual(DeadManSwitchResetReason.AutoReset, resetReason, "Reset callback not called or not called with auto as reason after switch activated and auto reset enabled.");
			}
		}

		[TestMethod]
		public async Task DeadManSwitch_ResetCallback_NoExceptionWhenResetCallbackNull()
		{
			using (var dms = new DeadManSwitch(TimeSpan.FromMilliseconds(100), () => { }, null, true))
			{
				dms.Reset();
				await Task.Delay(260);
			}
		}

	}
}