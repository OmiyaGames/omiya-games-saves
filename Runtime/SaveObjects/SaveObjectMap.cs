using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SaveObjectMap.cs" company="Omiya Games">
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
	public class SaveObjectMap<T> : IDictionary<string, T>, ISerializationCallbackReceiver where T : SaveObject
	{
		[SerializeField]
		List<T> serializedList;
		[SerializeField, HideInInspector]
		bool isSerializing = false;

		readonly Dictionary<string, T> actualMap;

		/// <summary>
		/// Default constructor that sets up an empty set.
		/// </summary>
		public SaveObjectMap()
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
		public SaveObjectMap(IEqualityComparer<string> comparer)
		{
			serializedList = new List<T>();
			actualMap = new Dictionary<string, T>(comparer);
		}

		/// <summary>
		/// Constructor an empty set with initial capacity defined.
		/// </summary>
		/// <param name="capacity">Initial capacity of this list.</param>
		public SaveObjectMap(int capacity)
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
		public SaveObjectMap(int capacity, IEqualityComparer<string> comparer)
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
		public bool Add(T saveObject)
		{
			if (saveObject == null)
			{
				return false;
			}
			else if(string.IsNullOrEmpty(saveObject.Key))
			{
				return false;
			}
			else if (actualMap.ContainsKey(saveObject.Key))
			{
				return false;
			}

			actualMap.Add(saveObject.Key, saveObject);
			return true;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T item)
		{
			if ((item == null) || string.IsNullOrEmpty(item.Key))
			{
				return false;
			}
			else if (actualMap.TryGetValue(item.Key, out var result))
			{
				return result == item;
			}
			return false;
		}
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool ContainsKey(T item) => (item != null) && (string.IsNullOrEmpty(item.Key) == false) && actualMap.ContainsKey(item.Key);

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
			if (value == null)
			{
				throw new System.ArgumentNullException(nameof(value));
			}
			else if (string.IsNullOrEmpty(key))
			{
				throw new System.ArgumentException("Key is invalid.", nameof(key));
			}
			else if (value.Key != key)
			{
				throw new System.ArgumentException($"{nameof(value)}'s key does not match.", nameof(key));
			}
			else
			{
				actualMap.Add(key, value);
			}
		}
		/// <inheritdoc/>
		bool ICollection<KeyValuePair<string, T>>.Contains(KeyValuePair<string, T> item)
		{
			return ((ICollection<KeyValuePair<string, T>>)actualMap).Contains(item);
		}
		/// <inheritdoc/>
		void ICollection<KeyValuePair<string, T>>.Add(KeyValuePair<string, T> item) => actualMap.Add(item.Key, item.Value);
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

			// Remove entries from the serialized list that are not in the set
			for (int i = 0; i < serializedList.Count; ++i)
			{
				if (ContainsKey(serializedList[i]) == false)
				{
					serializedList.RemoveAt(i);
					--i;
				}
			}

			// Populate the list with new entries
			HashSet<T> serializedSet = new(serializedList);
			foreach (T item in actualMap.Values)
			{
				if (serializedSet.Contains(item) == false)
				{
					serializedList.Add(item);
				}
			}
		}

		/// <inheritdoc/>
		[System.Obsolete("Manual call not supported.", true)]
		public void OnAfterDeserialize()
		{
			// Clear this Dictionary's contents
			actualMap.Clear();

			// Populate this HashSet
			foreach (T item in serializedList)
			{
				if (ContainsKey(item) == false)
				{
					actualMap.Add(item.Key, item);
				}
			}

			// Indicate we're done serializing
			isSerializing = false;
		}
	}
}
