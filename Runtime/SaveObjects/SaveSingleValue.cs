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
	public abstract class SaveSingleValue<T> : SaveObject, ITrackable<T>
	{
		/// <inheritdoc/>
		public event ITrackable<T>.ChangeEvent OnBeforeValueChanged;
		/// <inheritdoc/>
		public event ITrackable<T>.ChangeEvent OnAfterValueChanged;

		[SerializeField]
		[Tooltip("The starting default value.")]
		T defaultValue;

#if UNITY_EDITOR
		[SerializeField]
		[TextArea]
		[Tooltip("Comments on this value.")]
		string comments;
#endif

		protected T storedValue;

		/// <inheritdoc/>
		public abstract bool HasValue { get; }

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="newValue"></param>
		protected abstract void RecordValue(T newValue);
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		protected abstract WaitLoadValue<T> RetrieveValue();

		/// <inheritdoc/>
		public T Value
		{
			get => (CurrentState != SaveState.NotSetup) ? storedValue : DefaultValue;
			set => SetValue(value, SaveState.Desynced);
		}

		/// <summary>
		/// Default value set in the inspector.
		/// </summary>
		public T DefaultValue => defaultValue;

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

			WaitLoadValue<T> loadInt = RetrieveValue();
			loadInt.OnLoadingFinished += (source, args) =>
			{
				if (args.State == LoadState.Success)
				{
					SetValue(((LoadValueFinishedEventArgs<T>)args).Result, SaveState.Synced);
				}
			};
			return loadInt;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="value"></param>
		/// <param name="setState"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
		protected virtual T SetValue(T value, SaveState setState)
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
				T oldValue = storedValue;
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
			SetValue(DefaultValue, SaveState.Synced);
		}
	}
}
