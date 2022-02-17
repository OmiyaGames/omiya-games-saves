using System;
using System.Collections;
using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <copyright file="SaveObject.cs" company="Omiya Games">
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
	/// Interface for implementing saving and loading a value.
	/// </summary>
	public abstract partial class SaveObject : ScriptableObject, IEquatable<SaveObject>, IDisposable
	{

		[SerializeField]
		[Tooltip("A unique per save object.")]
		string key;

		/// <summary>
		/// Indicates if this object has been setup.
		/// </summary>
		public SaveState CurrentState
		{
			get;
			protected set;
		} = SaveState.NotSetup;
		/// <summary>
		/// The recorder used for this object.
		/// </summary>
		public IAsyncSettingsRecorder Recorder
		{
			get;
			private set;
		}
		/// <summary>
		/// The unique key for this object.
		/// </summary>
		public string Key => key;

		/// <summary>
		/// Attempts to load the data from <see cref="Recorder"/>
		/// asynchronously.
		/// </summary>
		/// <returns>
		/// Coroutine indicating whether the operation
		/// succeeded or not.
		/// </returns>
		public abstract WaitLoad Load();

		/// <summary>
		/// Configures this save object.
		/// </summary>
		/// <param name="recorder">
		/// The interface to saving persistent data.
		/// </param>
		public virtual void Setup(IAsyncSettingsRecorder recorder)
		{
			// Set the recorder property
			Recorder = recorder;

			// Indicate if the object is properly setup or not
			if (Recorder == null)
			{
				CurrentState = SaveState.NotSetup;
			}
			else
			{
				CurrentState = SaveState.Desynced;
			}
		}

		/// <inheritdoc/>
		public bool Equals(SaveObject other)
		{
			return string.Equals(key, other.key);
		}

		/// <inheritdoc/>
		public void Dispose() => Setup(null);
	}
}
