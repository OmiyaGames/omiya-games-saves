using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="WaitLoadFlags.cs" company="Omiya Games">
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
	/// <description>
	/// Making the interface asynchronous.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// A coroutine that waits for all listed flags to finish loading.
	/// </summary>
	public class WaitLoadFlags : WaitLoadValue<bool>, IDictionary<IAsyncSettingsRecorder, WaitLoadValue<bool>>
	{
		/// <inheritdoc/>
		public override event LoadingFinished OnLoadingFinished;

		readonly Dictionary<IAsyncSettingsRecorder, WaitLoadValue<bool>> allFlags;

		/// <summary>
		/// TODO
		/// </summary>
		public WaitLoadFlags() => allFlags = new();
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="capacity"></param>
		public WaitLoadFlags(int capacity) => allFlags = new(capacity);

		/// <inheritdoc/>
		public override bool Result
		{
			get
			{
				foreach (WaitLoadValue<bool> result in allFlags.Values)
				{
					if (result.Result == false)
					{
						return false;
					}
				}
				return true;
			}
		}

		/// <inheritdoc/>
		public override bool keepWaiting
		{
			get
			{
				var finalResult = LoadState.Success;
				foreach (var pair in allFlags)
				{
					if (pair.Value.keepWaiting)
					{
						return true;
					}

					if (pair.Value.CurrentState == LoadState.Fail)
					{
						finalResult = LoadState.Fail;
					}
				}

				OnLoadingFinished?.Invoke(this, new LoadFinishedCompositeEventArgs<WaitLoadValue<bool>>(allFlags, finalResult));
				return false;
			}
		}

		/// <inheritdoc/>
		public ICollection<IAsyncSettingsRecorder> Keys => allFlags.Keys;

		/// <inheritdoc/>
		public ICollection<WaitLoadValue<bool>> Values => allFlags.Values;

		/// <inheritdoc/>
		public int Count => allFlags.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => ((ICollection<KeyValuePair<IAsyncSettingsRecorder, WaitLoadValue<bool>>>)allFlags).IsReadOnly;

		/// <inheritdoc/>
		public WaitLoadValue<bool> this[IAsyncSettingsRecorder key]
		{
			get => allFlags[key];
			set => allFlags[key] = value;
		}

		/// <inheritdoc/>
		public void Add(IAsyncSettingsRecorder key, WaitLoadValue<bool> value) => allFlags.Add(key, value);

		/// <inheritdoc/>
		public bool ContainsKey(IAsyncSettingsRecorder key) => allFlags.ContainsKey(key);

		/// <inheritdoc/>
		public bool Remove(IAsyncSettingsRecorder key) => allFlags.Remove(key);

		/// <inheritdoc/>
		public bool TryGetValue(IAsyncSettingsRecorder key, out WaitLoadValue<bool> value) => allFlags.TryGetValue(key, out value);

		/// <inheritdoc/>
		public void Add(KeyValuePair<IAsyncSettingsRecorder, WaitLoadValue<bool>> item) =>
			((ICollection<KeyValuePair<IAsyncSettingsRecorder, WaitLoadValue<bool>>>)allFlags).Add(item);

		/// <inheritdoc/>
		public void Clear() => allFlags.Clear();

		/// <inheritdoc/>
		public bool Contains(KeyValuePair<IAsyncSettingsRecorder, WaitLoadValue<bool>> item) =>
			((ICollection<KeyValuePair<IAsyncSettingsRecorder, WaitLoadValue<bool>>>)allFlags).Contains(item);

		/// <inheritdoc/>
		public void CopyTo(KeyValuePair<IAsyncSettingsRecorder, WaitLoadValue<bool>>[] array, int arrayIndex) =>
			((ICollection<KeyValuePair<IAsyncSettingsRecorder, WaitLoadValue<bool>>>)allFlags).CopyTo(array, arrayIndex);

		/// <inheritdoc/>
		public bool Remove(KeyValuePair<IAsyncSettingsRecorder, WaitLoadValue<bool>> item) =>
			((ICollection<KeyValuePair<IAsyncSettingsRecorder, WaitLoadValue<bool>>>)allFlags).Remove(item);

		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<IAsyncSettingsRecorder, WaitLoadValue<bool>>> GetEnumerator() => allFlags.GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() =>
			((IEnumerable)allFlags).GetEnumerator();
	}
}
