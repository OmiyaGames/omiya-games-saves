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
	public class WaitLoadComposite : WaitLoad, ICollection<WaitLoad>
	{
		/// <inheritdoc/>
		public override event LoadingFinished OnLoadingFinished;

		readonly HashSet<WaitLoad> allWaits;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="waits"></param>
		public WaitLoadComposite(params WaitLoad[] waits) : this((IEnumerable<WaitLoad>)waits) { }
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="waits"></param>
		public WaitLoadComposite(IEnumerable<WaitLoad> waits)
		{
			if (waits != null)
			{
				allWaits = new HashSet<WaitLoad>(waits);
				allWaits.Remove(null);
			}
			else
			{
				allWaits = new HashSet<WaitLoad>();
			}
		}

		/// <inheritdoc/>
		public override bool keepWaiting
		{
			get
			{
				List<WaitLoad> failedWaits = new(allWaits.Count);
				foreach (WaitLoad wait in allWaits)
				{
					if (wait.keepWaiting)
					{
						return true;
					}
					else if (wait.CurrentState == LoadState.Fail)
					{
						failedWaits.Add(wait);
					}
				}

				OnLoadingFinished?.Invoke(this, new LoadFinishedCompositeEventArgs(failedWaits));
				return false;
			}
		}

		/// <inheritdoc/>
		public void Add(WaitLoad item)
		{
			if (item != null)
			{
				allWaits.Add(item);
			}
		}
		/// <inheritdoc/>
		public int Count => allWaits.Count;
		/// <inheritdoc/>
		public bool IsReadOnly => ((ICollection<WaitLoad>)allWaits).IsReadOnly;
		/// <inheritdoc/>
		public void Clear() => allWaits.Clear();
		/// <inheritdoc/>
		public bool Contains(WaitLoad item) => allWaits.Contains(item);
		/// <inheritdoc/>
		public void CopyTo(WaitLoad[] array, int arrayIndex) => allWaits.CopyTo(array, arrayIndex);
		/// <inheritdoc/>
		public bool Remove(WaitLoad item) => allWaits.Remove(item);
		/// <inheritdoc/>
		public IEnumerator<WaitLoad> GetEnumerator() => allWaits.GetEnumerator();
		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)allWaits).GetEnumerator();
	}

	/// <summary>
	/// TODO
	/// </summary>
	public class LoadFinishedCompositeEventArgs : LoadFinishedEventArgs
	{
		readonly WaitLoad[] failedWaits;

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="failedWaits"></param>
		public LoadFinishedCompositeEventArgs(List<WaitLoad> failedWaits) :
			this(failedWaits, (failedWaits.Count > 0 ? LoadState.Success : LoadState.Fail))
		{ }
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="failedWaits"></param>
		/// <param name="overwriteState"></param>
		public LoadFinishedCompositeEventArgs(List<WaitLoad> failedWaits, LoadState overwriteState) : base(overwriteState)
		{
			this.failedWaits = failedWaits.ToArray();
		}

		/// <summary>
		/// TODO
		/// </summary>
		public IReadOnlyList<WaitLoad> FailedWaits => failedWaits;
	}
}
