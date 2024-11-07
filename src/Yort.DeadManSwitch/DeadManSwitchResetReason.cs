using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Dms
{
	/// <summary>
	/// Provides information to the callback executed when a <see cref="DeadManSwitch"/> is reset, about why the reset happened.
	/// </summary>
	public enum DeadManSwitchResetReason
	{
		/// <summary>
		/// Occurs once when the switch is first initialised. Not technically a 'reset', but allows a single handler to provide logging/handling of both switch initialisation and reset.
		/// </summary>
		Initialize = 0,
		/// <summary>
		/// The <see cref="DeadManSwitch.Reset()"/> method was explicitly called.
		/// </summary>
		ManualReset,
		/// <summary>
		/// The switch automatically reset itself after activation.
		/// </summary>
		AutoReset,
		/// <summary>
		/// The switch was disarmed using the <see cref="DeadManSwitch.Disarm"/> method,
		/// </summary>
		/// <remarks>
		/// When a switch is disarmed it is inactive but not disposed. It can be re-activated by calling the <see cref="DeadManSwitch.Reset()"/> method, but until/unless that happens it will not fire the callback event again.
		/// </remarks>
		Disarm
	}
}
