using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SaveSingleValue.cs" company="Omiya Games">
	/// The MIT License (MIT)
	/// 
	/// Copyright (c) 2022 Omiya Games
	/// 
	/// Permission is hereby granted, free of charge, to any person obtaining a copy
	/// of this software and associated documentation files (the "Software"), to deal
	/// in the Software without restriction, including without limitation the rights
	/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	/// copies of the Software, and to permit persons to whom the Software is
	/// furnished to do so, subject to the following conditions:
	/// 
	/// The above copyright notice and this permission notice shall be included in
	/// all copies or substantial portions of the Software.
	/// 
	/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	/// THE SOFTWARE.
	/// </copyright>
	/// <list type="table">
	/// <listheader>
	/// <term>Revision</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>
	/// <strong>Version:</strong> 0.2.0-exp<br/>
	/// <strong>Date:</strong> 2/17/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Initial draft.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Helper abstract class with common methods already defined for most situations.
	/// </summary>
	/// <typeparam name="TValue">
	/// The type of value being tracked.
	/// </typeparam>
	/// <typeparam name="TSerialized">
	/// The type that is actually serialized;
	/// specifically, <seealso cref="defaultValue"/>.
	/// </typeparam>
	public abstract class SaveSingleValue<TValue, TSerialized> : SaveObject, ITrackable<TValue>
	{
		/// <inheritdoc/>
		public event ITrackable<TValue>.ChangeEvent OnBeforeValueChanged;
		/// <inheritdoc/>
		public event ITrackable<TValue>.ChangeEvent OnAfterValueChanged;

		[SerializeField, UnityEngine.Serialization.FormerlySerializedAs("defaultValue")]
		[Tooltip("The starting default value.")]
		protected TSerialized defaultValue;
		[SerializeField]
		ErrorHandling onLoadFailed = ErrorHandling.ProceedLogWarning;

		protected TValue storedValue;

		/// <inheritdoc/>
		public abstract bool HasValue { get; }

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="newValue"></param>
		protected abstract void RecordValue(TValue newValue);
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		protected abstract WaitLoadValue<TValue> RetrieveValue();

		/// <inheritdoc/>
		public override ErrorHandling HandleLoadFailure => onLoadFailed;
		/// <inheritdoc/>
		public TValue Value
		{
			get => (CurrentState != SaveState.NotSetup) ? storedValue : ConvertedDefaultValue;
			set => SetValue(value, SaveState.Desynced);
		}
		/// <summary>
		/// Default value set in the inspector.
		/// </summary>
		public abstract TValue ConvertedDefaultValue
		{
			get;
		}

		/// <inheritdoc/>
		public override void Setup(IAsyncSettingsRecorder recorder)
		{
			// Clean-up this object
			if (Recorder != null)
			{
				Recorder.UnsubscribeToDeleteKey(Key, OnDeleteKey);
			}

			// Call base method
			base.Setup(recorder);

			// Listen to the new recorder
			if (Recorder != null)
			{
				Recorder.SubscribeToDeleteKey(Key, OnDeleteKey);
			}
		}

		/// <inheritdoc/>
		public override WaitLoad Load()
		{
			if (CurrentState == SaveState.NotSetup)
			{
				throw new System.Exception("Object not setup yet.");
			}

			WaitLoadValue<TValue> loadInt = RetrieveValue();
			loadInt.OnLoadingFinished += (source, args) =>
			{
				if (args.State == LoadState.Success)
				{
					SetValue(((LoadValueFinishedEventArgs<TValue>)args).Result, SaveState.Synced);
				}
			};
			return loadInt;
		}

		/// <inheritdoc/>
		public override void RevertToDefault() => SetValue(ConvertedDefaultValue, SaveState.Desynced);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="value"></param>
		/// <param name="setState"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
		protected virtual TValue SetValue(TValue value, SaveState setState)
		{
			if (CurrentState == SaveState.NotSetup)
			{
				throw new System.Exception("Object not setup yet.");
			}

			// Check if the value is different
			if (Equals(storedValue, value) == false)
			{
				// Fire the before-changed event
				OnBeforeValueChanged?.Invoke(storedValue, value);

				// Update the values (and cache the old one)
				TValue oldValue = storedValue;
				storedValue = value;

				// Update state
				CurrentState = setState;
				if (CurrentState == SaveState.Desynced)
				{
					// Record the changes
					RecordValue(storedValue);
				}

				// Fire the after-changed event
				OnAfterValueChanged?.Invoke(oldValue, storedValue);
			}

			return value;
		}

		/// <summary>
		/// Called on deletion of key.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="args"></param>
		void OnDeleteKey(IAsyncSettingsRecorder source, KeyDeletedEventArgs args)
		{
			SetValue(ConvertedDefaultValue, SaveState.Synced);
		}
	}
}
