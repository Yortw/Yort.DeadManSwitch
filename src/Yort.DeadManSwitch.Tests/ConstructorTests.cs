using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yort.Dms;

namespace Yort.Dms.Tests
{
	[TestClass]
	public class ConstructorTests
	{
		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		[TestMethod]
		public void Constructor_ThrowsOnZeroDelayInMs()
		{
			var ds = new DeadManSwitch(0, () => { }, false);
		}

		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		[TestMethod]
		public void Constructor_ThrowsOnNegativeDelayInMs()
		{
			var ds = new DeadManSwitch(-1, () => { }, false);
		}

		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		[TestMethod]
		public void Constructor_ThrowsOnNegativeDelayAsTimespan()
		{
			var ds = new DeadManSwitch(TimeSpan.FromMilliseconds(-1), () => { }, false);
		}

		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		[TestMethod]
		public void Constructor_ThrowsOnZeroDelayAsTimespan()
		{
			var ds = new DeadManSwitch(TimeSpan.Zero, () => { }, false);
		}

		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		[TestMethod]
		public void Constructor_ThrowsOnTooLargeDelayAsTimespan()
		{
			var ds = new DeadManSwitch(TimeSpan.MaxValue, () => { }, false);
		}

		[ExpectedException(typeof(System.ArgumentNullException))]
		[TestMethod]
		public void Constructor_ThrowsOnNullActivationCallback()
		{
			var ds = new DeadManSwitch(1000, null, false);
		}

		[TestMethod]
		public void Constructor_ConstructsOkWithAcceptableValuesTimeInMs()
		{
			using (var ds = new DeadManSwitch(1000, () => { }, (r) => { }, false))
			{
			}
		}

		[TestMethod]
		public void Constructor_ConstructsOkWithAcceptableValuesTimeAsTimespan()
		{
			using (var ds = new DeadManSwitch(TimeSpan.FromSeconds(1), () => { }, (r) => { }, false))
			{
			}
		}

		[TestMethod]
		public void Constructor_ConstructsOkWithNullResetCallbackTimeInMs()
		{
			using (var ds = new DeadManSwitch(1000, () => { }, null, true))
			{
			}
		}

		[TestMethod]
		public void Constructor_ConstructsOkWithNullResetCallbackTimeAsTimespan()
		{
			using (var ds = new DeadManSwitch(TimeSpan.FromSeconds(1), () => { }, null, true))
			{
			}
		}

	}
}