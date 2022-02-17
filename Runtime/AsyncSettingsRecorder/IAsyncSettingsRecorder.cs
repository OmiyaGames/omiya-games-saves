using System;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="IAsyncSettingsRecorder.cs" company="Omiya Games">
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
	/// An interface for storing settings. Useful for indicating where a game's settings should be saved.
	/// </summary>
	public interface IAsyncSettingsRecorder
	{
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="source">
		/// The source that caused this method to be called.
		/// </param>
		/// <param name="args">
		/// Any info about the key being deleted, etc.
		/// </param>
		public delegate void OnKeyDeleted(IAsyncSettingsRecorder source, KeyDeletedEventArgs args);

		/// <summary>
		/// Asynchronously gets a boolean value.
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
		/// A coroutine that indicates when it's
		/// finished loading, and provides the
		/// retrieved results.
		/// </returns>
		WaitLoadValue<bool> GetBool(string key, bool defaultValue);
		/// <summary>
		/// Store a key-value pair with boolean as value type.
		/// </summary>
		/// <param name="key">
		/// A unique key to pair
		/// <paramref name="value"/> with.
		/// </param>
		/// <param name="value">
		/// The value to store
		/// </param>
		/// <remarks>
		/// This method does not necessarily
		/// <em>save</em> a <paramref name="value"/>.
		/// Make sure to call <seealso cref="Save"/> after
		/// this method so <paramref name="value"/> presists.
		/// </remarks>
		void SetBool(string key, bool value);

		/// <summary>
		/// Asynchronously gets an integer value.
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
		/// A coroutine that indicates when it's
		/// finished loading, and provides the
		/// retrieved results.
		/// </returns>
		WaitLoadValue<int> GetInt(string key, int defaultValue);
		/// <summary>
		/// Store a key-value pair with integer as value type.
		/// </summary>
		/// <param name="key">
		/// A unique key to pair
		/// <paramref name="value"/> with.
		/// </param>
		/// <param name="value">
		/// The value to store
		/// </param>
		/// <remarks>
		/// This method does not necessarily
		/// <em>save</em> a <paramref name="value"/>.
		/// Make sure to call <seealso cref="Save"/> after
		/// this method so <paramref name="value"/> presists.
		/// </remarks>
		void SetInt(string key, int value);

		/// <summary>
		/// Asynchronously gets a float value.
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
		/// A coroutine that indicates when it's
		/// finished loading, and provides the
		/// retrieved results.
		/// </returns>
		WaitLoadValue<float> GetFloat(string key, float defaultValue);
		/// <summary>
		/// Store a key-value pair with float as value type.
		/// </summary>
		/// <param name="key">
		/// A unique key to pair
		/// <paramref name="value"/> with.
		/// </param>
		/// <param name="value">
		/// The value to store
		/// </param>
		/// <remarks>
		/// This method does not necessarily
		/// <em>save</em> a <paramref name="value"/>.
		/// Make sure to call <seealso cref="Save"/> after
		/// this method so <paramref name="value"/> presists.
		/// </remarks>
		void SetFloat(string key, float value);

		/// <summary>
		/// Asynchronously gets a string value.
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
		/// A coroutine that indicates when it's
		/// finished loading, and provides the
		/// retrieved results.
		/// </returns>
		WaitLoadValue<string> GetString(string key, string defaultValue);
		/// <summary>
		/// Store a key-value pair with string as value type.
		/// </summary>
		/// <param name="key">
		/// A unique key to pair
		/// <paramref name="value"/> with.
		/// </param>
		/// <param name="value">
		/// The value to store
		/// </param>
		/// <remarks>
		/// This method does not necessarily
		/// <em>save</em> a <paramref name="value"/>.
		/// Make sure to call <seealso cref="Save"/> after
		/// this method so <paramref name="value"/> presists.
		/// </remarks>
		void SetString(string key, string value);

		/// <summary>
		/// Asynchronously gets an enum value.
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
		/// A coroutine that indicates when it's
		/// finished loading, and provides the
		/// retrieved results.
		/// </returns>
		WaitLoadValue<TEnum> GetEnum<TEnum>(string key, TEnum defaultValue) where TEnum : struct, IConvertible;
		/// <summary>
		/// Store a key-value pair with enum as value type.
		/// </summary>
		/// <typeparam name="TEnum">
		/// An enumerator type.
		/// </typeparam>
		/// <param name="key">
		/// A unique key to pair
		/// <paramref name="value"/> with.
		/// </param>
		/// <param name="value">
		/// The value to store
		/// </param>
		/// <remarks>
		/// This method does not necessarily
		/// <em>save</em> a <paramref name="value"/>.
		/// Make sure to call <seealso cref="Save"/> after
		/// this method so <paramref name="value"/> presists.
		/// </remarks>
		void SetEnum<TEnum>(string key, TEnum value) where TEnum : struct, IConvertible;

		/// <summary>
		/// Asynchronously gets a <see cref="DateTime"/> value.
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
		/// A coroutine that indicates when it's
		/// finished loading, and provides the
		/// retrieved results.
		/// </returns>
		WaitLoadValue<DateTime> GetDateTimeUtc(string key, DateTime defaultValue);
		/// <summary>
		/// Store a key-value pair with <see cref="DateTime"/> as value type.
		/// </summary>
		/// <param name="key">
		/// A unique key to pair
		/// <paramref name="value"/> with.
		/// </param>
		/// <param name="value">
		/// The value to store
		/// </param>
		/// <remarks>
		/// This method does not necessarily
		/// <em>save</em> a <paramref name="value"/>.
		/// Make sure to call <seealso cref="Save"/> after
		/// this method so <paramref name="value"/> presists.
		/// </remarks>
		void SetDateTimeUtc(string key, DateTime value);

		/// <summary>
		/// Asynchronously gets a <see cref="TimeSpan"/> value.
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
		/// A coroutine that indicates when it's
		/// finished loading, and provides the
		/// retrieved results.
		/// </returns>
		WaitLoadValue<TimeSpan> GetTimeSpan(string key, TimeSpan defaultValue);
		/// <summary>
		/// Store a key-value pair with <see cref="TimeSpan"/> as value type.
		/// </summary>
		/// <param name="key">
		/// A unique key to pair
		/// <paramref name="value"/> with.
		/// </param>
		/// <param name="value">
		/// The value to store
		/// </param>
		/// <remarks>
		/// This method does not necessarily
		/// <em>save</em> a <paramref name="value"/>.
		/// Make sure to call <seealso cref="Save"/> after
		/// this method so <paramref name="value"/> presists.
		/// </remarks>
		void SetTimeSpan(string key, TimeSpan value);

		/// <summary>
		/// Saves the entire settings.
		/// </summary>
		/// <returns>
		/// Coroutine indicating whether the operation
		/// succeeded or not.
		/// </returns>
		WaitLoad Save();
		/// <summary>
		/// Checks if this setting has a pairing
		/// with key.
		/// </summary>
		/// <param name="key">A unique key.</param>
		/// <returns>
		/// Coroutine indicating if there is a key or not
		/// at the end of operation.
		/// </returns>
		WaitLoadValue<bool> HasKey(string key);
		/// <summary>
		/// Deletes a key and its associating value.
		/// </summary>
		/// <returns>
		/// Coroutine indicating whether the operation
		/// succeeded or not.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// If <paramref name="key"/> is <c>null</c>
		/// or empty.
		/// </exception>
		WaitLoad DeleteKey(string key);
		/// <summary>
		/// Deletes all the keys.
		/// </summary>
		/// <returns>
		/// Coroutine indicating whether the operation
		/// succeeded or not.
		/// </returns>
		/// <remarks>
		/// This method will call all events listening
		/// to <see cref="DeleteKey(string)"/>.
		/// </remarks>
		WaitLoad DeleteAll();
		/// <summary>
		/// Adds an event listening to <seealso cref="DeleteKey(string)"/>
		/// and/or <seealso cref="DeleteAll"/>.
		/// </summary>
		/// <param name="key">
		/// Listen to a specific key.  Set to empty string or <c>null</c>
		/// to listen to <see cref="DeleteAll"/> event only.
		/// </param>
		/// <param name="action">
		/// The function listening to key deletion.
		/// </param>
		void SubscribeToDeleteKey(string key, OnKeyDeleted action);
		/// <summary>
		/// Removes an event listener from <seealso cref="DeleteKey(string)"/>
		/// and/or <seealso cref="DeleteAll"/>.
		/// </summary>
		/// <param name="key">
		/// Unsubscribe to a specific key.  Set to empty string or <c>null</c>
		/// to unsubscribe from <see cref="DeleteAll"/>.
		/// </param>
		/// <param name="action">
		/// The function listening to key deletion.
		/// </param>
		void UnsubscribeToDeleteKey(string key, OnKeyDeleted action);
	}

	/// <summary>
	/// Event arguments for <seealso cref="IAsyncSettingsRecorder.SubscribeToDeleteKey(string, ISettingsRecorder.OnKeyDeleted)"/>.
	/// </summary>
	public class KeyDeletedEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor to set <see cref="Key"/>.
		/// </summary>
		/// <param name="key">
		/// Sets <see cref="Key"/>.
		/// </param>
		public KeyDeletedEventArgs(string key)
		{
			Key = key;
		}

		/// <summary>
		/// The key that got deleted.
		/// </summary>
		/// <remarks>
		/// Can be <c>null</c>, which in that case,
		/// the delete operation was called from
		/// <seealso cref="ISettingsRecorder.DeleteAll"/>.
		/// </remarks>
		public string Key
		{
			get;
		}
	}
}
