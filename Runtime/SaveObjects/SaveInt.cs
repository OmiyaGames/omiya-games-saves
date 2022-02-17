using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <copyright file="SaveInt.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/16/2022<br/>
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
	/// Interface for loading an integer from 
	/// </summary>
	[CreateAssetMenu(menuName = "Omiya Games/Saves/Save Integer", fileName = "Integer Loader")]
	public class SaveInt : SaveObject, ITrackable<int>
	{
		/// <inheritdoc/>
		public event ITrackable<int>.ChangeEvent OnBeforeValueChanged;
		/// <inheritdoc/>
		public event ITrackable<int>.ChangeEvent OnAfterValueChanged;

		[SerializeField]
		[Tooltip("The starting default value.")]
		int defaultValue;

		[Header("Clamp Boundaries")]
		[SerializeField]
		bool hasMin = false;
		[SerializeField]
		int minValue = int.MinValue;
		[SerializeField]
		bool hasMax = false;
		[SerializeField]
		int maxValue = int.MaxValue;

		int storedValue;

		/// <inheritdoc/>
		public int Value
		{
			get => (CurrentState != SaveState.NotSetup) ? storedValue : defaultValue;
			set => SetValue(value, SaveState.Desynced);
		}

		/// <summary>
		/// The minimum value <seealso cref="Value"/> can be.
		/// </summary>
		public int? Min => hasMin ? minValue : null;
		/// <summary>
		/// The maximum value <seealso cref="Value"/> can be.
		/// </summary>
		public int? Max => hasMax ? maxValue : null;

		/// <inheritdoc/>
		public bool HasValue => true;

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
				Recorder.UnsubscribeToDeleteKey(Key, OnDeleteKey);
			}
		}

		/// <inheritdoc/>
		public override WaitLoad Load()
		{
			WaitLoadValue<int> loadInt = Recorder.GetInt(Key, defaultValue);
			loadInt.OnLoadingFinished += (source, args) =>
			{
				if (args.State == LoadState.Success)
				{
					SetValue(((LoadValueFinishedEventArgs<int>)args).Result, SaveState.Synced);
				}
			};
			return loadInt;
		}

		/// <summary>
		/// Called on deletion of key.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="args"></param>
		void OnDeleteKey(IAsyncSettingsRecorder source, KeyDeletedEventArgs args)
		{
			SetValue(defaultValue, SaveState.Synced);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="value"></param>
		/// <param name="setState"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
		protected virtual int SetValue(int value, SaveState setState)
		{
			if (CurrentState == SaveState.NotSetup)
			{
				throw new System.Exception("Object not setup yet.");
			}

			// Clamp the value
			else if (hasMin && (value < minValue))
			{
				value = minValue;
			}
			else if (hasMax && (value > maxValue))
			{
				value = maxValue;
			}

			// Check if the value is different
			if (storedValue != value)
			{
				// Fire the before-changed event
				OnBeforeValueChanged?.Invoke(storedValue, value);

				// Update the values (and cache the old one)
				int oldValue = storedValue;
				storedValue = value;

				// Update state
				CurrentState = setState;
				if (CurrentState == SaveState.Desynced)
				{
					// Record the changes
					Recorder.SetInt(Key, oldValue);
				}

				// Fire the after-changed event
				OnAfterValueChanged?.Invoke(oldValue, storedValue);
			}

			return value;
		}
	}
}
