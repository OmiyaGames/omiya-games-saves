using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="WaitLoad.cs" company="Omiya Games">
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
	/// <summary>
	/// An abstract coroutine that waits for a loading operation to finish.
	/// Contains a property indicating if the operation succeeded or not.
	/// </summary>
	public abstract class WaitLoad : CustomYieldInstruction
	{
		/// <summary>
		/// Args for successful operations.
		/// </summary>
		public static readonly LoadFinishedEventArgs SUCCESS_ARGS = new LoadFinishedEventArgs(LoadState.Success);
		/// <summary>
		/// Args for failed operations.
		/// </summary>
		public static readonly LoadFinishedEventArgs FAIL_ARGS = new LoadFinishedEventArgs(LoadState.Fail);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="source">
		/// The caller of this event.
		/// </param>
		/// <param name="args">
		/// The argument for this event.
		/// </param>
		public delegate void LoadingFinished(WaitLoad source, LoadFinishedEventArgs args);
		/// <summary>
		/// Event triggered when loading is finished.
		/// </summary>
		public abstract event LoadingFinished OnLoadingFinished;

		/// <summary>
		/// The current load state.
		/// </summary>
		public LoadState CurrentState
		{
			get;
			protected set;
		} = LoadState.Loading;

		/// <inheritdoc/>
		public override void Reset()
		{
			base.Reset();
			CurrentState = LoadState.Loading;
		}
	}

	/// <summary>
	/// A coroutine that immediately finishes with success status.
	/// </summary>
	public sealed class WaitLoadImmediate : WaitLoad
	{
		/// <inheritdoc/>
		public override event LoadingFinished OnLoadingFinished;

		/// <inheritdoc/>
		public override bool keepWaiting
		{
			get
			{
				if (CurrentState == LoadState.Loading)
				{
					CurrentState = LoadState.Success;
					OnLoadingFinished?.Invoke(this, SUCCESS_ARGS);
				}
				return false;
			}
		}
	}

	/// <summary>
	/// Event arguments for <seealso cref="OnLoadingFinished"/>.
	/// </summary>
	public class LoadFinishedEventArgs : System.EventArgs
	{
		/// <summary>
		/// Constructor to set <see cref="State"/>.
		/// </summary>
		/// <param name="state">
		/// Sets <see cref="State"/>.
		/// </param>
		public LoadFinishedEventArgs(LoadState state)
		{
			State = state;
		}

		/// <summary>
		/// The finished state.
		/// </summary>
		public LoadState State
		{
			get;
		}
	}
}
