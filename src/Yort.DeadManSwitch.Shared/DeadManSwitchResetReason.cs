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
		AutoReset
	}
}
