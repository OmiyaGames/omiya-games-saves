using System;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
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
	/// Collection of SaveObjects, acting like a save slot.
	/// </summary>
	//[CreateAssetMenu(menuName = "Omiya Games/Saves/Save Slot", fileName = "Save Slot")]
	[System.Obsolete("Not Implemented yet")]
	public class SaveSlot : SaveObject, ITrackable<int>
	{
		const int DEFAULT_SLOT = 0;

		/// <inheritdoc/>
		public event ITrackable<int>.ChangeEvent OnBeforeValueChanged;
		/// <inheritdoc/>
		public event ITrackable<int>.ChangeEvent OnAfterValueChanged;

		[SerializeField]
		[Tooltip("Maximum number of save slots a user is allowed to have.")]
		int maxSlots = 100;

		[SerializeField]
		[Header("Stored Data")]
		SerializableHashSet<SaveObject> slotData = new SerializableHashSet<SaveObject>(new SaveObjectComparer());

		int slotNumber = DEFAULT_SLOT;
		readonly List<Guid> saveSlotIds = new List<Guid>();
		AsyncSlotSettingsRecorder slotRecorder = null;

		/// <inheritdoc/>
		public override ErrorHandling HandleLoadFailure => ErrorHandling.ProceedLogError;
		/// <inheritdoc/>
		public int Value
		{
			get => (CurrentState != SaveState.NotSetup) ? slotNumber : DEFAULT_SLOT;
			set => SetValue(value, SaveState.Desynced);
		}
		/// <summary>
		/// The maximum number of slots provided.
		/// </summary>
		public int Capacity => maxSlots;

		public int Count => slotData.Count;

		public IReadOnlyCollection<SaveObject> SlotData => slotData;

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

			if (Recorder != null)
			{
				// Listen to the new recorder
				Recorder.SubscribeToDeleteKey(Key, OnDeleteKey);

				// Create the recorder if we haven't already
				if (slotRecorder == null)
				{
					slotRecorder = new AsyncSlotSettingsRecorder(this);
				}

				// Set the child slots to use this recorder
				foreach (SaveObject save in slotData)
				{
					save.Setup(slotRecorder);
				}
			}
			else
			{
				foreach (SaveObject save in slotData)
				{
					save.Setup(null);
				}
			}
		}

		/// <inheritdoc/>
		public override WaitLoad Load()
		{
			WaitLoadValue<int> loadInt = Recorder.GetInt(Key, DEFAULT_SLOT);
			loadInt.OnLoadingFinished += (source, args) =>
			{
				if (args.State == LoadState.Success)
				{
					SetValue(((LoadValueFinishedEventArgs<int>)args).Result, SaveState.Synced);
				}
			};
			return loadInt;
		}

		/// <inheritdoc/>
		public override void RevertToDefault() => SetValue(DEFAULT_SLOT, SaveState.Desynced);

		/// <summary>
		/// Called on deletion of key.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="args"></param>
		void OnDeleteKey(IAsyncSettingsRecorder source, KeyDeletedEventArgs args)
		{
			SetValue(DEFAULT_SLOT, SaveState.Synced);
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
				throw new Exception("Object not setup yet.");
			}

			// Clamp the value
			else if (value >= Count)
			{
				value = (Count - 1);
			}
			else if (value < 0)
			{
				value = 0;
			}

			// Check if the value is different
			if (slotNumber != value)
			{
				// Fire the before-changed event
				OnBeforeValueChanged?.Invoke(slotNumber, value);

				// Update the values (and cache the old one)
				int oldValue = slotNumber;
				slotNumber = value;

				// Update state
				CurrentState = setState;
				if (CurrentState == SaveState.Desynced)
				{
					// Record the changes
					Recorder.SetInt(Key, oldValue);
				}

				// Fire the after-changed event
				OnAfterValueChanged?.Invoke(oldValue, slotNumber);
			}

			return value;
		}

		class AsyncSlotSettingsRecorder : IAsyncSettingsRecorder
		{
			SaveSlot Parent
			{
				get;
			}

			public AsyncSlotSettingsRecorder(SaveSlot parent)
			{
				// FIXME: do stuff here
			}

			public WaitLoad DeleteAll()
			{
				throw new NotImplementedException();
			}

			public WaitLoad DeleteKey(string key)
			{
				throw new NotImplementedException();
			}

			public WaitLoadValue<bool> GetBool(string key, bool defaultValue)
			{
				throw new NotImplementedException();
			}

			public WaitLoadValue<DateTime> GetDateTimeUtc(string key, DateTime defaultValue)
			{
				throw new NotImplementedException();
			}

			public WaitLoadValue<T> GetEnum<T>(string key, T defaultValue) where T : Enum
			{
				throw new NotImplementedException();
			}

			public WaitLoadValue<float> GetFloat(string key, float defaultValue)
			{
				throw new NotImplementedException();
			}

			public WaitLoadValue<int> GetInt(string key, int defaultValue)
			{
				throw new NotImplementedException();
			}

			public WaitLoadValue<string> GetString(string key, string defaultValue)
			{
				throw new NotImplementedException();
			}

			public WaitLoadValue<TimeSpan> GetTimeSpan(string key, TimeSpan defaultValue)
			{
				throw new NotImplementedException();
			}

			public WaitLoadValue<bool> HasKey(string key)
			{
				throw new NotImplementedException();
			}

			public WaitLoad Save()
			{
				throw new NotImplementedException();
			}

			public void SetBool(string key, bool value)
			{
				throw new NotImplementedException();
			}

			public void SetDateTimeUtc(string key, DateTime value)
			{
				throw new NotImplementedException();
			}

			public void SetEnum<T>(string key, T value) where T : Enum
			{
				throw new NotImplementedException();
			}

			public void SetFloat(string key, float value)
			{
				throw new NotImplementedException();
			}

			public void SetInt(string key, int value)
			{
				throw new NotImplementedException();
			}

			public void SetString(string key, string value)
			{
				throw new NotImplementedException();
			}

			public void SetTimeSpan(string key, TimeSpan value)
			{
				throw new NotImplementedException();
			}

			public void SubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action)
			{
				throw new NotImplementedException();
			}

			public void UnsubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action)
			{
				throw new NotImplementedException();
			}
		}
	}
}
