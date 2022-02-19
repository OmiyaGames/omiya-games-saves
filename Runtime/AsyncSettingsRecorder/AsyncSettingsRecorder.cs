using System;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="AsyncSettingsRecorder.cs" company="Omiya Games">
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
	/// <strong>Version:</strong> 0.2.0-exp.1<br/>
	/// <strong>Date:</strong> 2/18/2022<br/>
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
	/// A <see cref="ScriptableObject"/> implementation of <code>IAsyncSettingsRecorder</code>.
	/// </summary>
	/// <seealso cref="IAsyncSettingsRecorder"/>
	public abstract class AsyncSettingsRecorder : ScriptableObject, IAsyncSettingsRecorder, IDisposable
	{
		/// <summary>
		/// Maps a key to an event.
		/// </summary>
		/// <remarks>
		/// Any events held under an empty string key
		/// are intended to be called by <see cref="DeleteAll"/>.
		/// </remarks>
		readonly Dictionary<string, IAsyncSettingsRecorder.OnKeyDeleted> keyToDeleteEventMap = new();

		/// <inheritdoc/>
		public abstract WaitLoadValue<int> GetInt(string key, int defaultValue);
		/// <inheritdoc/>
		public abstract void SetInt(string key, int value);

		/// <inheritdoc/>
		public abstract WaitLoadValue<float> GetFloat(string key, float defaultValue);
		/// <inheritdoc/>
		public abstract void SetFloat(string key, float value);

		/// <inheritdoc/>
		public abstract WaitLoadValue<string> GetString(string key, string defaultValue);
		/// <inheritdoc/>
		public abstract void SetString(string key, string value);

		/// <inheritdoc/>
		public abstract WaitLoadValue<bool> GetBool(string key, bool defaultValue);
		/// <inheritdoc/>
		public abstract void SetBool(string key, bool value);

		/// <inheritdoc/>
		public abstract WaitLoadValue<TEnum> GetEnum<TEnum>(string key, TEnum defaultValue) where TEnum : struct, IConvertible;
		/// <inheritdoc/>
		public abstract void SetEnum<TEnum>(string key, TEnum value) where TEnum : struct, IConvertible;
		
		/// <inheritdoc/>
		public abstract WaitLoadValue<DateTime> GetDateTimeUtc(string key, DateTime defaultValue);
		/// <inheritdoc/>
		public abstract void SetDateTimeUtc(string key, DateTime value);
		
		/// <inheritdoc/>
		public abstract WaitLoadValue<TimeSpan> GetTimeSpan(string key, TimeSpan defaultValue);
		/// <inheritdoc/>
		public abstract void SetTimeSpan(string key, TimeSpan value);

		/// <inheritdoc/>
		public abstract WaitLoad DeleteKey(string key);
		/// <inheritdoc/>
		public abstract WaitLoad DeleteAll();

		/// <inheritdoc/>
		public abstract WaitLoad Save();
		/// <inheritdoc/>
		public abstract WaitLoadValue<bool> HasKey(string key);

		/// <inheritdoc/>
		public virtual void Dispose()
		{
			keyToDeleteEventMap.Clear();
		}

		/// <inheritdoc/>
		public override string ToString() => $"{name} ({GetType()})";

		/// <inheritdoc/>
		public void SubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action)
		{
			// Fix key to always be valid
			if (key == null)
			{
				key = string.Empty;
			}

			// Add event
			if (keyToDeleteEventMap.TryGetValue(key, out var events))
			{
				keyToDeleteEventMap[key] = (events + action);
			}
			else
			{
				keyToDeleteEventMap.Add(key, action);
			}
		}

		/// <inheritdoc/>
		public void UnsubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action)
		{
			// Remove event
			if (TryGetEvents(key, out var events))
			{
				keyToDeleteEventMap[key] = (events - action);
			}
		}

		/// <summary>
		/// Grabs a corresponding event to a key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="events"></param>
		/// <returns>True if one is found.</returns>
		/// <remarks>
		/// Any events held under an empty string key
		/// are intended to be called by <see cref="DeleteAll"/>.
		/// </remarks>
		protected bool TryGetEvents(string key, out IAsyncSettingsRecorder.OnKeyDeleted events)
		{
			// Fix key to always be valid
			if (key == null)
			{
				key = string.Empty;
			}
			return keyToDeleteEventMap.TryGetValue(key, out events);
		}

		/// <summary>
		/// Grabs events listening to <see cref="DeleteAll"/>.
		/// </summary>
		/// <param name="events"></param>
		/// <returns>True if one is found.</returns>
		protected IEnumerable<IAsyncSettingsRecorder.OnKeyDeleted> GetAllEvents() => keyToDeleteEventMap.Values;
	}
}
