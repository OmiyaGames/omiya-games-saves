using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="WaitLoadComposite.cs" company="Omiya Games">
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
	public class WaitLoadComposite : WaitLoad, IDictionary<IAsyncSettingsRecorder, WaitLoad>
	{
		/// <inheritdoc/>
		public override event LoadingFinished OnLoadingFinished;

		readonly Dictionary<IAsyncSettingsRecorder, WaitLoad> allWaits;

		/// <summary>
		/// TODO
		/// </summary>
		public WaitLoadComposite() => allWaits = new();
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="capacity"></param>
		public WaitLoadComposite(int capacity) => allWaits = new(capacity);

		/// <inheritdoc/>
		public override bool keepWaiting
		{
			get
			{
				var finalResult = LoadState.Success;
				foreach (var pair in allWaits)
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

				OnLoadingFinished?.Invoke(this, new LoadFinishedCompositeEventArgs<WaitLoad>(allWaits, finalResult));
				return false;
			}
		}

		/// <inheritdoc/>
		public ICollection<IAsyncSettingsRecorder> Keys => allWaits.Keys;

		/// <inheritdoc/>
		public ICollection<WaitLoad> Values => allWaits.Values;

		/// <inheritdoc/>
		public int Count => allWaits.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => ((ICollection<KeyValuePair<IAsyncSettingsRecorder, WaitLoad>>)allWaits).IsReadOnly;

		/// <inheritdoc/>
		public WaitLoad this[IAsyncSettingsRecorder key]
		{
			get => allWaits[key];
			set => allWaits[key] = value;
		}

		/// <inheritdoc/>
		public void Add(IAsyncSettingsRecorder key, WaitLoad value) => allWaits.Add(key, value);

		/// <inheritdoc/>
		public bool ContainsKey(IAsyncSettingsRecorder key) => allWaits.ContainsKey(key);

		/// <inheritdoc/>
		public bool Remove(IAsyncSettingsRecorder key) => allWaits.Remove(key);

		/// <inheritdoc/>
		public bool TryGetValue(IAsyncSettingsRecorder key, out WaitLoad value) => allWaits.TryGetValue(key, out value);

		/// <inheritdoc/>
		public void Add(KeyValuePair<IAsyncSettingsRecorder, WaitLoad> item) =>
			((ICollection<KeyValuePair<IAsyncSettingsRecorder, WaitLoad>>)allWaits).Add(item);

		/// <inheritdoc/>
		public void Clear() => allWaits.Clear();

		/// <inheritdoc/>
		public bool Contains(KeyValuePair<IAsyncSettingsRecorder, WaitLoad> item) =>
			((ICollection<KeyValuePair<IAsyncSettingsRecorder, WaitLoad>>)allWaits).Contains(item);

		/// <inheritdoc/>
		public void CopyTo(KeyValuePair<IAsyncSettingsRecorder, WaitLoad>[] array, int arrayIndex) =>
			((ICollection<KeyValuePair<IAsyncSettingsRecorder, WaitLoad>>)allWaits).CopyTo(array, arrayIndex);

		/// <inheritdoc/>
		public bool Remove(KeyValuePair<IAsyncSettingsRecorder, WaitLoad> item) =>
			((ICollection<KeyValuePair<IAsyncSettingsRecorder, WaitLoad>>)allWaits).Remove(item);

		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<IAsyncSettingsRecorder, WaitLoad>> GetEnumerator() => allWaits.GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => allWaits.GetEnumerator();
	}
}
