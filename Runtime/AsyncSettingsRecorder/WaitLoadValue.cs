namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="WaitLoadValue.cs" company="Omiya Games">
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
	/// Contains a property indicating if the operation succeeded or not,
	/// as well as the retrieved value if successful.
	/// </summary>
	/// <typeparam name="T">Sets the <seealso cref="WaitLoadImmediate{T}.Result"/> type.</typeparam>
	public abstract class WaitLoadValue<T> : WaitLoad
	{
		/// <summary>
		/// The results of loading an object.
		/// </summary>
		public abstract T Result
		{
			get;
		}
	}

	public class WaitLoadValueImmediate<T> : WaitLoadValue<T>
	{
		/// <inheritdoc/>
		public override event LoadingFinished OnLoadingFinished;

		/// <summary>
		/// Constructs a non-waiting coroutine, with result already set.
		/// </summary>
		/// <param name="initialResultValue">
		/// The value of <seealso cref="Result"/>
		/// </param>
		public WaitLoadValueImmediate(T initialResultValue) : base()
		{
			Result = initialResultValue;
		}

		/// <inheritdoc/>
		public override bool keepWaiting
		{
			get
			{
				if (CurrentState == LoadState.Loading)
				{
					CurrentState = LoadState.Success;
					OnLoadingFinished?.Invoke(this, new LoadValueFinishedEventArgs<T>(Result));
				}
				return false;
			}
		}

		/// <summary>
		/// The results of loading an object.
		/// </summary>
		public override T Result
		{
			get;
		}
	}

	/// <summary>
	/// Event arguments for <seealso cref="OnLoadingFinished"/>.
	/// </summary>
	public class LoadValueFinishedEventArgs<T> : LoadFinishedEventArgs
	{
		/// <summary>
		/// Constructor for success status.
		/// </summary>
		/// <param name="state">
		/// Sets <see cref="Result"/>.
		/// </param>
		public LoadValueFinishedEventArgs(T value) : base(LoadState.Success)
		{
			Result = value;
		}
		/// <summary>
		/// Constructor for failure status.
		/// </summary>
		public LoadValueFinishedEventArgs() : base(LoadState.Fail)
		{
			Result = default(T);
		}

		/// <summary>
		/// The finished state.
		/// </summary>
		public T Result
		{
			get;
		}
	}
}
