using System;
using Ladon;
using Yort.Trashy.Extensions;
using Yort.Trashy;
using System.Xml.Linq;

namespace Yort.Dms
{
	/// <summary>
	/// Implements a simple, thread-safe, 'dead man switch', where the switch is 'thrown' (activated) after a specified interval if it has not be reset. Each reset of the switch restarts the count down to activation.
	/// </summary>
	/// <remarks>
	/// <para>To prevent the switch from activating, call the <see cref="Reset()"/> method. If <see cref="Reset()"/> is not called within the duration set (via the constructor) for the switch then it will activate and call the activation callback.</para>
	/// <para>Activation of the switch and calls to the <see cref="Reset()"/> method are partially synchronized. If <see cref="Reset()"/> is called while the activation callback is in progress the call to <see cref="Reset()"/> will block until the activation callback completes. 
	/// If the switch activates while a reset is in progress, the switch will wait for the reset to finish but it will still call the activation callback once the in-progress reset completes, as the switch did fire.</para>
	/// <para>Multiple calls to <see cref="Reset()"/> are also synchronised, but only for the actual reset operation, the synchronisation ends before the reset callback is called ensuring logging or other potentially 'long running' operations in the callback do not block other resets and cause false activations. This means the reset callback may be called from multiple threads simultaneoously and must be thread-safe.</para>
	/// <para>Dispose the switch to stop/disable it. Once disposed the switch cannot be recovered/reused, a new instance must be created.</para>
	/// <para>If auto-reset is enabled (via the constructor) then after activation callback completes the switch will call <see cref="Reset(DeadManSwitchResetReason)"/> itself (providing <see cref="DeadManSwitchResetReason.AutoReset"/> as the reason). If auto reset is not enabled the switch will not be reset (and will not activate again), unless <see cref="Reset()"/> is subsequently called.</para>
	/// <para>Calls to the activated callback will likely be made on a background thread. Any callback code that requires thread affinity will need to perform it's own dispatch/invoke.</para>
	/// </remarks>
	public sealed class DeadManSwitch : Yort.Trashy.DisposableManagedOnlyBase
	{

		#region Fields

		private readonly Action _ActivatedCallback;
		private readonly Action<DeadManSwitchResetReason> _ResetCallback;
		private readonly int _DelayMilliseconds;
		private readonly bool _AutoReset;

		private System.Threading.Timer _SwitchTimer;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new dead man switch.
		/// </summary>
		/// <param name="delayMilliseconds">The number of milliseconds to wait without a reset before activating. Must be greater than zero and less than <see cref="Int32.MaxValue"/>.</param>
		/// <param name="activatedCallback">A <see cref="Action"/> to invoke when the switch activates. Must not be null.</param>
		/// <param name="autoReset">A boolean indicating whether the switch automatically resets itself after firing (true) or remains inactive until a subsequent call to <see cref="Reset()"/> by user code (false).</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="delayMilliseconds"/> is less than zero or greater than <see cref="Int32.MaxValue"/>.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="activatedCallback"/> is null.</exception>
		public DeadManSwitch(int delayMilliseconds, Action activatedCallback, bool autoReset) : this(delayMilliseconds, activatedCallback, null, autoReset)
		{
		}

		/// <summary>
		/// Constructs a new dead man switch.
		/// </summary>
		/// <param name="delay">A <see cref="System.TimeSpan"/> to wait without a reset before activating. Must represent an interval greater than zero and less than <see cref="Int32.MaxValue"/> millseconds.</param>
		/// <param name="activatedCallback">A <see cref="Action"/> to invoke when the switch activates. Must not be null.</param>
		/// <param name="autoReset">A boolean indicating whether the switch automatically resets itself after firing (true) or remains inactive until a subsequent call to <see cref="Reset()"/> by user code (false). An auto reset will occur even if the activation callback throws an exception.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="delay"/> is zero, represents a negative interval, or represets a value larger than <see cref="Int32.MaxValue"/> milliseconds.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="activatedCallback"/> is null.</exception>
		public DeadManSwitch(TimeSpan delay, Action activatedCallback, bool autoReset) : 
			this(Convert.ToInt32(delay.TotalMilliseconds.GuardRange(nameof(delay), 1, Int32.MaxValue)), activatedCallback, null, autoReset)
		{
		}

		/// <summary>
		/// Constructs a new dead man switch.
		/// </summary>
		/// <param name="delay">A <see cref="System.TimeSpan"/> to wait without a reset before activating. Must represent an interval greater than zero and less than <see cref="Int32.MaxValue"/> millseconds.</param>
		/// <param name="activatedCallback">A <see cref="Action"/> to invoke when the switch activates. Must not be null.</param>
		/// <param name="resetCallback">An action accepting a <see cref="DeadManSwitchResetReason"/> argument that is called each time the switch is reset. Maybe null, in which case no callback will be performed. The callback provided may be called from multiple thread simultaneously and needs to be thread-safe.</param>
		/// <param name="autoReset">A boolean indicating whether the switch automatically resets itself after firing (true) or remains inactive until a subsequent call to <see cref="Reset()"/> by user code (false).</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="delay"/> is zero, represents a negative interval, or represets a value larger than <see cref="Int32.MaxValue"/> milliseconds.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="activatedCallback"/> is null.</exception>
		public DeadManSwitch(TimeSpan delay, Action activatedCallback, Action<DeadManSwitchResetReason> resetCallback, bool autoReset) :
			this(Convert.ToInt32(delay.TotalMilliseconds.GuardRange(nameof(delay), 1, Int32.MaxValue)), activatedCallback, resetCallback, autoReset)
		{
		}

		/// <summary>
		/// Constructs a new dead man switch.
		/// </summary>
		/// <param name="delayMilliseconds">The number of milliseconds to wait without a reset before activating. Must be greater than zero and less than <see cref="Int32.MaxValue"/>.</param>
		/// <param name="activatedCallback">A <see cref="Action"/> to invoke when the switch activates. Must not be null.</param>
		/// <param name="resetCallback">An action accepting a <see cref="DeadManSwitchResetReason"/> argument that is called each time the switch is reset. Maybe null, in which case no callback will be performed. The callback provided may be called from multiple thread simultaneously and needs to be thread-safe.</param>
		/// <param name="autoReset">A boolean indicating whether the switch automatically resets itself after firing (true) or remains inactive until a subsequent call to <see cref="Reset()"/> by user code (false).</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="delayMilliseconds"/> is less than zero or greater than <see cref="Int32.MaxValue"/>.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="activatedCallback"/> is null.</exception>
		public DeadManSwitch(int delayMilliseconds, Action activatedCallback, Action<DeadManSwitchResetReason> resetCallback, bool autoReset) 
		{
			_ActivatedCallback = activatedCallback.GuardNull(nameof(activatedCallback));
			_DelayMilliseconds = delayMilliseconds.GuardZeroOrNegative(nameof(delayMilliseconds));
			
			_ResetCallback = resetCallback;
			_AutoReset = autoReset;

			_SwitchTimer = new System.Threading.Timer(this.Activated, null, _DelayMilliseconds, System.Threading.Timeout.Infinite);
			Reset(DeadManSwitchResetReason.Initialize);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Resets the switch, preventing it from activating and restarting the countdown to activation.
		/// </summary>
		public void Reset()
		{
			Reset(DeadManSwitchResetReason.ManualReset);
		}

		/// <summary>
		/// Disables the switch so it will not activate again, without disposing it. The switch can be rearmed by calling <see cref="Reset()"/>.
		/// </summary>
		public void Disarm()
		{
			Reset(DeadManSwitchResetReason.Disarm);
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Resets the switch and calls the reset callback with the specified <paramref name="reason"/>.
		/// </summary>
		/// <param name="reason">The reason the reset occurred, provided to the reset callback, if any.</param>
		private void Reset(DeadManSwitchResetReason reason)
		{
			using (var busyToken = base.ObtainBusyToken())
			{
				_SwitchTimer.Change(reason == DeadManSwitchResetReason.Disarm ? System.Threading.Timeout.Infinite : _DelayMilliseconds, System.Threading.Timeout.Infinite);
			}

			//Actions taken on reset may be slow, should not stop timer from being reset if the event occurs again.
			_ResetCallback?.Invoke(reason);
		}

		/// <summary>
		/// Called internally when the swithc has not been <see cref="Reset()"/> within the required interval. Calls the activated callback, then does a reset if auto reset is enabled.
		/// </summary>
		/// <param name="state">Reserved for future use. Will always be null for current implementation.</param>
		private void Activated(object state)
		{
			try
			{
				using (var busyToken = base.ObtainBusyToken())
				{
					_ActivatedCallback.Invoke();
				}
			}
			catch (ObjectDisposedException)
			{
				if (this.IsDisposed) return; //Ignore race conditon if disposed during activated callback.
			}

			finally
			{
				if (_AutoReset && !IsDisposed)
					Reset(DeadManSwitchResetReason.AutoReset);
			}
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Disposes all internal managed resources and stops the countdown to activation.
		/// </summary>
		protected override void DisposeManagedResources()
		{
			if (_SwitchTimer != null)
			{
				_SwitchTimer.TryDispose(DisposeOptions.SuppressExceptions);
				_SwitchTimer = null;
			}

			base.DisposeManagedResources();
		}

		#endregion

	}
}