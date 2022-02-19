using System;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="AsyncCompositeSettingsRecorder.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/19/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial verison.</description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// TODO.
	/// </summary>
	public class AsyncCompositeSettingsRecorder : IAsyncSettingsRecorder, IReadOnlyList<IAsyncSettingsRecorder>
	{
		readonly ListSet<IAsyncSettingsRecorder> allRecorders;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="settings"></param>
		public AsyncCompositeSettingsRecorder(IEnumerable<IAsyncSettingsRecorder> settings)
		{
			// Attempt to discern the size of settings
			if (settings is ICollection<IAsyncSettingsRecorder>)
			{
				// Apply it as capacity
				allRecorders = new(((ICollection<IAsyncSettingsRecorder>)settings).Count);
			}
			else
			{
				// Otherwise, run the default constructor
				allRecorders = new();
			}

			// Add all the elements into the list
			if (settings != null)
			{
				foreach (IAsyncSettingsRecorder recorder in settings)
				{
					allRecorders.Add(recorder);
				}
			}
		}

		/// <inheritdoc/>
		public int Count => allRecorders.Count;

		/// <inheritdoc/>
		public IAsyncSettingsRecorder this[int index] => allRecorders[index];

		/// <inheritdoc/>
		public WaitLoad Save()
		{
			// Call Save() on all recorders
			WaitLoadComposite waitList = new();
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				waitList.Add(recorder, recorder.Save());
			}
			return waitList;
		}

		/// <inheritdoc/>
		public WaitLoad DeleteAll()
		{
			// Call DeleteAll() on all recorders
			WaitLoadComposite waitList = new();
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				waitList.Add(recorder, recorder.DeleteAll());
			}
			return waitList;
		}

		/// <inheritdoc/>
		public WaitLoad DeleteKey(string key)
		{
			// Argument check
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			// Call DeleteKey() on all recorders
			WaitLoadComposite waitList = new();
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				waitList.Add(recorder, recorder.DeleteKey(key));
			}
			return waitList;
		}

		/// <summary>
		/// Asynchronously gets a boolean value from the
		/// <em>first</em> <see cref="IAsyncSettingsRecorder"/>.
		/// </summary>
		/// <param name="key">
		/// The key associated with this value.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if a value
		/// associated with <paramref name="key"/>
		/// is not found.
		/// </param>
		/// <returns>
		/// A coroutine that indicates when it's finished
		/// loading, and provides the retrieved results.
		/// If list is empty, returns <c>null</c> instead.
		/// </returns>
		public WaitLoadValue<bool> GetBool(string key, bool defaultValue)
		{
			if (allRecorders.Count > 0)
			{
				return allRecorders[0].GetBool(key, defaultValue);
			}
			return null;
		}

		/// <summary>
		/// Asynchronously gets a <see cref="DateTime"/> value from the
		/// <em>first</em> <see cref="IAsyncSettingsRecorder"/>.
		/// </summary>
		/// <param name="key">
		/// The key associated with this value.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if a value
		/// associated with <paramref name="key"/>
		/// is not found.
		/// </param>
		/// <returns>
		/// A coroutine that indicates when it's finished
		/// loading, and provides the retrieved results.
		/// If list is empty, returns <c>null</c> instead.
		/// </returns>
		public WaitLoadValue<DateTime> GetDateTimeUtc(string key, DateTime defaultValue)
		{
			if (allRecorders.Count > 0)
			{
				return allRecorders[0].GetDateTimeUtc(key, defaultValue);
			}
			return null;
		}

		/// <summary>
		/// Asynchronously gets an enum value from the
		/// <em>first</em> <see cref="IAsyncSettingsRecorder"/>.
		/// </summary>
		/// <typeparam name="TEnum">
		/// An enumerator type.
		/// </typeparam>
		/// <param name="key">
		/// The key associated with this value.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if a value
		/// associated with <paramref name="key"/>
		/// is not found.
		/// </param>
		/// <returns>
		/// A coroutine that indicates when it's finished
		/// loading, and provides the retrieved results.
		/// If list is empty, returns <c>null</c> instead.
		/// </returns>
		public WaitLoadValue<TEnum> GetEnum<TEnum>(string key, TEnum defaultValue) where TEnum : struct, IConvertible
		{
			if (allRecorders.Count > 0)
			{
				return allRecorders[0].GetEnum(key, defaultValue);
			}
			return null;
		}

		/// <summary>
		/// Asynchronously gets a float value from the
		/// <em>first</em> <see cref="IAsyncSettingsRecorder"/>.
		/// </summary>
		/// <param name="key">
		/// The key associated with this value.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if a value
		/// associated with <paramref name="key"/>
		/// is not found.
		/// </param>
		/// <returns>
		/// A coroutine that indicates when it's finished
		/// loading, and provides the retrieved results.
		/// If list is empty, returns <c>null</c> instead.
		/// </returns>
		public WaitLoadValue<float> GetFloat(string key, float defaultValue)
		{
			if (allRecorders.Count > 0)
			{
				return allRecorders[0].GetFloat(key, defaultValue);
			}
			return null;
		}

		/// <summary>
		/// Asynchronously gets an integer value from the
		/// <em>first</em> <see cref="IAsyncSettingsRecorder"/>.
		/// </summary>
		/// <param name="key">
		/// The key associated with this value.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if a value
		/// associated with <paramref name="key"/>
		/// is not found.
		/// </param>
		/// <returns>
		/// A coroutine that indicates when it's finished
		/// loading, and provides the retrieved results.
		/// If list is empty, returns <c>null</c> instead.
		/// </returns>
		public WaitLoadValue<int> GetInt(string key, int defaultValue)
		{
			if (allRecorders.Count > 0)
			{
				return allRecorders[0].GetInt(key, defaultValue);
			}
			return null;
		}

		/// <summary>
		/// Asynchronously gets a string value from the
		/// <em>first</em> <see cref="IAsyncSettingsRecorder"/>.
		/// </summary>
		/// <param name="key">
		/// The key associated with this value.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if a value
		/// associated with <paramref name="key"/>
		/// is not found.
		/// </param>
		/// <returns>
		/// A coroutine that indicates when it's finished
		/// loading, and provides the retrieved results.
		/// If list is empty, returns <c>null</c> instead.
		/// </returns>
		public WaitLoadValue<string> GetString(string key, string defaultValue)
		{
			if (allRecorders.Count > 0)
			{
				return allRecorders[0].GetString(key, defaultValue);
			}
			return null;
		}

		/// <summary>
		/// Asynchronously gets a <see cref="TimeSpan"/> value from the
		/// <em>first</em> <see cref="IAsyncSettingsRecorder"/>.
		/// </summary>
		/// <param name="key">
		/// The key associated with this value.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if a value
		/// associated with <paramref name="key"/>
		/// is not found.
		/// </param>
		/// <returns>
		/// A coroutine that indicates when it's finished
		/// loading, and provides the retrieved results.
		/// If list is empty, returns <c>null</c> instead.
		/// </returns>
		public WaitLoadValue<TimeSpan> GetTimeSpan(string key, TimeSpan defaultValue)
		{
			if (allRecorders.Count > 0)
			{
				return allRecorders[0].GetTimeSpan(key, defaultValue);
			}
			return null;
		}

		/// <inheritdoc/>
		public WaitLoadValue<bool> HasKey(string key)
		{
			// Argument check
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			// Call DeleteKey() on all recorders
			WaitLoadFlags waitList = new();
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				waitList.Add(recorder, recorder.HasKey(key));
			}
			return waitList;
		}

		/// <inheritdoc/>
		public void SetBool(string key, bool value)
		{
			// Argument check
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			// Call SetBool() on all recorders
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				recorder.SetBool(key, value);
			}
		}

		/// <inheritdoc/>
		public void SetDateTimeUtc(string key, DateTime value)
		{
			// Argument check
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			// Call SetDateTimeUtc() on all recorders
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				recorder.SetDateTimeUtc(key, value);
			}
		}

		/// <inheritdoc/>
		public void SetEnum<TEnum>(string key, TEnum value) where TEnum : struct, IConvertible
		{
			// Argument check
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			// Call SetEnum<TEnum>() on all recorders
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				recorder.SetEnum<TEnum>(key, value);
			}
		}

		/// <inheritdoc/>
		public void SetFloat(string key, float value)
		{
			// Argument check
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			// Call SetFloat() on all recorders
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				recorder.SetFloat(key, value);
			}
		}

		/// <inheritdoc/>
		public void SetInt(string key, int value)
		{
			// Argument check
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			// Call SetInt() on all recorders
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				recorder.SetInt(key, value);
			}
		}

		/// <inheritdoc/>
		public void SetString(string key, string value)
		{
			// Argument check
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			// Call SetString() on all recorders
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				recorder.SetString(key, value);
			}
		}

		/// <inheritdoc/>
		public void SetTimeSpan(string key, TimeSpan value)
		{
			// Argument check
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			// Call SetTimeSpan() on all recorders
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				recorder.SetTimeSpan(key, value);
			}
		}

		/// <inheritdoc/>
		public void SubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action)
		{
			// Argument check
			if (key == null)
			{
				key = string.Empty;
			}

			// Call SubscribeToDeleteKey() on all recorders
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				recorder.SubscribeToDeleteKey(key, action);
			}
		}

		/// <inheritdoc/>
		public void UnsubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action)
		{
			// Argument check
			if (key == null)
			{
				key = string.Empty;
			}

			// Call UnsubscribeToDeleteKey() on all recorders
			foreach (IAsyncSettingsRecorder recorder in allRecorders)
			{
				recorder.UnsubscribeToDeleteKey(key, action);
			}
		}

		/// <inheritdoc/>
		public IEnumerator<IAsyncSettingsRecorder> GetEnumerator() => allRecorders.GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)allRecorders).GetEnumerator();
	}
}
