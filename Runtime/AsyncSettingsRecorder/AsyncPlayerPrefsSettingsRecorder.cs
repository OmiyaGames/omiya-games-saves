using System;
using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="AsyncPlayerPrefsSettingsRecorder.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/16/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Making the interface asynchronous.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// An implementation of <seealso cref="IAsyncSettingsRecorder"/>
	/// using <seealso cref="PlayerPrefs"/>.
	/// </summary>
	public class AsyncPlayerPrefsSettingsRecorder : AsyncSettingsRecorderDecorator
	{
		/// <inheritdoc/>
		public override WaitLoadValue<int> GetInt(string key, int defaultValue)
		{
			int value = PlayerPrefs.GetInt(key, defaultValue);
			return new WaitLoadValueImmediate<int>(value);
		}

		/// <inheritdoc/>
		public override void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
		}

		/// <inheritdoc/>
		public override WaitLoadValue<float> GetFloat(string key, float defaultValue)
		{
			float value = PlayerPrefs.GetFloat(key, defaultValue);
			return new WaitLoadValueImmediate<float>(value);
		}

		/// <inheritdoc/>
		public override void SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(key, value);
		}

		/// <inheritdoc/>
		public override WaitLoadValue<string> GetString(string key, string defaultValue)
		{
			string value = PlayerPrefs.GetString(key, defaultValue);
			return new WaitLoadValueImmediate<string>(value);
		}

		/// <inheritdoc/>
		public override void SetString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		/// <inheritdoc/>
		public override WaitLoad DeleteKey(string key)
		{
			// Check the arguments
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Key cannot be null or empty string", "key");
			}

			// Delete the key
			PlayerPrefs.DeleteKey(key);

			// Invoke the corresponding event
			if (TryGetEvents(key, out var events))
			{
				events?.Invoke(this, new KeyDeletedEventArgs(key));
			}
			return new WaitLoadImmediate();
		}

		/// <inheritdoc/>
		public override WaitLoad DeleteAll()
		{
			PlayerPrefs.DeleteAll();

			// Invoke all the stored events
			foreach(var events in GetAllEvents())
			{
				events?.Invoke(this, new KeyDeletedEventArgs(null));
			}

			return new WaitLoadImmediate();
		}

		/// <inheritdoc/>
		public override WaitLoad Save()
		{
			PlayerPrefs.Save();
			return new WaitLoadImmediate();
		}

		/// <inheritdoc/>
		public override WaitLoadValue<bool> HasKey(string key)
		{
			bool results = PlayerPrefs.HasKey(key);
			return new WaitLoadValueImmediate<bool>(results);
		}
	}
}
