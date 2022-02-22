using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SaveObjectSet.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/21/2022<br/>
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
	/// A collection of <seealso cref="SaveObject"/>s.
	/// </summary>
	[System.Serializable]
	public class SaveObjectSet<T> : IDictionary<string, T>, ISerializationCallbackReceiver where T : SaveObject
	{
		[SerializeField]
		List<T> serializedList;
		[SerializeField, HideInInspector]
		bool isSerializing = false;

		readonly Dictionary<string, T> actualMap;

		/// <summary>
		/// Default constructor that sets up an empty set.
		/// </summary>
		public SaveObjectSet()
		{
			serializedList = new();
			actualMap = new();
		}

		/// <summary>
		/// Constructor to set the <see cref="IEqualityComparer{T}"/>,
		/// used to check if two elements matches.
		/// </summary>
		/// <param name="comparer">
		/// Comparer to check if two elements matches.
		/// </param>
		public SaveObjectSet(IEqualityComparer<string> comparer)
		{
			serializedList = new List<T>();
			actualMap = new Dictionary<string, T>(comparer);
		}

		/// <summary>
		/// Constructor an empty set with initial capacity defined.
		/// </summary>
		/// <param name="capacity">Initial capacity of this list.</param>
		public SaveObjectSet(int capacity)
		{
			serializedList = new(capacity);
			actualMap = new(capacity);
		}

		/// <summary>
		/// Constructor to set the <see cref="IEqualityComparer{T}"/>,
		/// used to check if two elements matches.
		/// </summary>
		/// <param name="capacity">Initial capacity of this set.</param>
		/// <param name="comparer">
		/// Comparer to check if two elements matches.
		/// </param>
		public SaveObjectSet(int capacity, IEqualityComparer<string> comparer)
		{
			serializedList = new List<T>(capacity);
			actualMap = new(capacity, comparer);
		}

		/// <summary>
		/// Indicates if this collection is in the middle of serializing.
		/// </summary>
		public bool IsSerializing => isSerializing;

		/// <summary>
		/// Adds an object ont this set.
		/// </summary>
		/// <param name="saveObject"></param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		public bool Add(T saveObject)
		{
			if (saveObject == null)
			{
				throw new System.ArgumentNullException(nameof(saveObject));
			}

			if (actualMap.ContainsKey(saveObject.Key) == false)
			{
				actualMap.Add(saveObject.Key, saveObject);
				return true;
			}

			return false;
		}

		#region IDictionary<string, T> Implementations
		/// <inheritdoc/>
		public T this[string key]
		{
			get => actualMap[key];
			set => actualMap[key] = value;
		}
		/// <inheritdoc/>
		public ICollection<string> Keys => actualMap.Keys;
		/// <inheritdoc/>
		public ICollection<T> Values => actualMap.Values;
		/// <inheritdoc/>
		public int Count => actualMap.Count;
		/// <inheritdoc/>
		public bool IsReadOnly => ((ICollection<KeyValuePair<string, T>>)actualMap).IsReadOnly;
		/// <inheritdoc/>
		public bool ContainsKey(string key) => actualMap.ContainsKey(key);
		/// <inheritdoc/>
		public bool Remove(string key) => actualMap.Remove(key);
		/// <inheritdoc/>
		public bool TryGetValue(string key, out T value) => actualMap.TryGetValue(key, out value);
		/// <inheritdoc/>
		public void Clear() => actualMap.Clear();
		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<string, T>> GetEnumerator() => actualMap.GetEnumerator();

		/// <inheritdoc/>
		void IDictionary<string, T>.Add(string key, T value)
		{
			((IDictionary<string, T>)actualMap).Add(key, value);
		}
		/// <inheritdoc/>
		bool ICollection<KeyValuePair<string, T>>.Contains(KeyValuePair<string, T> item)
		{
			return ((ICollection<KeyValuePair<string, T>>)actualMap).Contains(item);
		}
		/// <inheritdoc/>
		void ICollection<KeyValuePair<string, T>>.Add(KeyValuePair<string, T> item)
		{
			((ICollection<KeyValuePair<string, T>>)actualMap).Add(item);
		}
		/// <inheritdoc/>
		void ICollection<KeyValuePair<string, T>>.CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, T>>)actualMap).CopyTo(array, arrayIndex);
		}
		/// <inheritdoc/>
		bool ICollection<KeyValuePair<string, T>>.Remove(KeyValuePair<string, T> item)
		{
			return ((ICollection<KeyValuePair<string, T>>)actualMap).Remove(item);
		}
		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)actualMap).GetEnumerator();
		}
		#endregion

		/// <inheritdoc/>
		[System.Obsolete("Manual call not supported.", true)]
		public void OnBeforeSerialize()
		{
			// Indicate we started serializing
			isSerializing = true;

			// FIXME: Sync this set's data into the list
			throw new System.NotImplementedException();
			//SerializableHelpers.PushSetIntoSerializedList(this, serializedList, false);
		}

		/// <inheritdoc/>
		[System.Obsolete("Manual call not supported.", true)]
		public void OnAfterDeserialize()
		{
			// FIXME: Sync the list's data into the set.
			//SerializableHelpers.PushSerializedListIntoSet(serializedList, this, false);
			throw new System.NotImplementedException();

			// Indicate we're done serializing
			isSerializing = false;
		}
	}
}
