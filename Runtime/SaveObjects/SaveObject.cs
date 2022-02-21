using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
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
	public abstract partial class SaveObject : ScriptableObject, System.IDisposable
	{
		public const string RANDOM_CHAR_ARRAY = "0123456789qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM.,;:?-_=+!@#$%&*";
		const string PREPEND_KEY = "Key - ";
		const int RANDOM_KEY_LENGTH = 12;

		[SerializeField]
		string key;

#if UNITY_EDITOR
		[SerializeField]
		[TextArea]
		string comments;
#endif

		/// <summary>
		/// Indicate how to handle errors calling <seealso cref="Load"/>.
		/// </summary>
		public abstract ErrorHandling HandleLoadFailure
		{
			get;
		}
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

		/// <summary>
		/// Set default values here.
		/// </summary>
		public virtual void Reset()
		{
			// Setup a text builder
			System.Text.StringBuilder builder = new(PREPEND_KEY.Length + RANDOM_KEY_LENGTH);

			// Prepend common key phrase
			builder.Append(PREPEND_KEY);

			// Generate a short random phrase
			for (int i = 0; i < RANDOM_KEY_LENGTH; ++i)
			{
				int randomIndex = Random.Range(0, RANDOM_CHAR_ARRAY.Length);
				builder.Append(RANDOM_CHAR_ARRAY[randomIndex]);
			}

			// Set member variable
			key = builder.ToString();

			// Setup comments
#if UNITY_EDITOR
			comments = "(Add notes here!)";
#endif
		}

		/// <inheritdoc/>
		public override string ToString() => $"{name} ({GetType()}), Key: \"{Key}\"";

		/// <inheritdoc/>
		public void Dispose() => Setup(null);
	}

	/// <summary>
	/// TODO
	/// </summary>
	public class SaveObjectComparer : IEqualityComparer<SaveObject>
	{
		static string GetKey(SaveObject x) => (x.Key != null) ? x.Key : string.Empty;

		/// <inheritdoc/>
		public bool Equals(SaveObject x, SaveObject y)
		{
			if ((x != null) && (y != null))
			{
				return string.Equals(GetKey(x), GetKey(y));
			}
			else
			{
				return x == y;
			}
		}

		/// <inheritdoc/>
		public int GetHashCode(SaveObject obj) => (obj != null) ? GetKey(obj).GetHashCode() : 0;
	}
}
