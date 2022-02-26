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
	/// <description>Initial draft.</description>
	/// </item><item>
	/// <term>
	/// <strong>Version:</strong> 0.2.1-exp<br/>
	/// <strong>Date:</strong> 2/26/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Fixing deserialization logic.  Since <see cref="SaveObject"/>s are
	/// not guaranteed to be setup by the time <see cref="OnAfterDeserialize"/>
	/// is called, moved its logic into <see cref="Setup"/> instead.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// A collection of <seealso cref="SaveObject"/>s. If used as an
	/// inspector-exposed variable, <seealso cref="Setup"/> <strong>must</strong>
	/// be called before using this map.
	/// </summary>
	[System.Serializable]
	public class SaveObjectMap<T> : IDictionary<string, T>, ISerializationCallbackReceiver where T : SaveObject
	{
		[SerializeField]
		List<T> serializedList;
		[SerializeField, HideInInspector]
		bool isSerializing = false;

		bool isDeserializedNeedSetup = false;
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
		/// Indicates if the map is setup or not.
		/// </summary>
		public bool IsSetup => (isDeserializedNeedSetup == false);
		/// <summary>
		/// Indicates if this collection is in the middle of serializing.
		/// </summary>
		public bool IsSerializing => isSerializing;

		/// <summary>
		/// Sets up this map. Necessary if this map
		/// is edited in the Unity inspector.
		/// </summary>
		public void Setup()
		{
			// Check if this map needs setup.
			if (isDeserializedNeedSetup)
			{
				// Clear this Dictionary's contents
				actualMap.Clear();

				// Populate this HashSet with temporary keys,
				// as there's no guarantee these SaveObjects has
				// been deserialized yet.
				foreach (T item in serializedList)
				{
					if (CanAddKey(item))
					{
						actualMap.Add(item.Key, item);
					}
				}

				// Indicate setup is done
				isDeserializedNeedSetup = false;
			}
		}

		/// <summary>
		/// Adds an object ont this set.
		/// </summary>
		/// <param name="saveObject"></param>
		/// <returns></returns>
		public bool Add(T saveObject)
		{
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			else if (saveObject == null)
			{
				return false;
			}
			else if (string.IsNullOrEmpty(saveObject.Key))
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
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			else if ((item == null) || string.IsNullOrEmpty(item.Key))
			{
				return false;
			}
			else if (actualMap.TryGetValue(item.Key, out var result))
			{
				return result == item;
			}
			return false;
		}

		#region IDictionary<string, T> Implementations
		/// <inheritdoc/>
		public T this[string key]
		{
			get
			{
				if (isDeserializedNeedSetup)
				{
					throw new System.Exception("SaveObjectMap is not setup!");
				}

				return actualMap[key];
			}
			set
			{
				if (isDeserializedNeedSetup)
				{
					throw new System.Exception("SaveObjectMap is not setup!");
				}

				actualMap[key] = value;
			}
		}
		/// <inheritdoc/>
		public ICollection<string> Keys
		{
			get
			{
				if (isDeserializedNeedSetup)
				{
					throw new System.Exception("SaveObjectMap is not setup!");
				}

				return actualMap.Keys;
			}
		}
		/// <inheritdoc/>
		public ICollection<T> Values
		{
			get
			{
				if (isDeserializedNeedSetup)
				{
					throw new System.Exception("SaveObjectMap is not setup!");
				}

				return actualMap.Values;
			}
		}
		/// <inheritdoc/>
		public int Count
		{
			get
			{
				if (isDeserializedNeedSetup)
				{
					throw new System.Exception("SaveObjectMap is not setup!");
				}

				return actualMap.Count;
			}
		}
		/// <inheritdoc/>
		public bool IsReadOnly => ((ICollection<KeyValuePair<string, T>>)actualMap).IsReadOnly;

		/// <inheritdoc/>
		public bool ContainsKey(string key)
		{
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			return actualMap.ContainsKey(key);
		}
		
		/// <inheritdoc/>
		public bool Remove(string key)
		{
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			return actualMap.Remove(key);
		}
		
		/// <inheritdoc/>
		public bool TryGetValue(string key, out T value)
		{
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			return actualMap.TryGetValue(key, out value);
		}

		/// <inheritdoc/>
		public void Clear()
		{
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			actualMap.Clear();
		}

		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
		{
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			return actualMap.GetEnumerator();
		}

		/// <inheritdoc/>
		void IDictionary<string, T>.Add(string key, T value)
		{
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			else if (value == null)
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
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			return ((ICollection<KeyValuePair<string, T>>)actualMap).Contains(item);
		}

		/// <inheritdoc/>
		void ICollection<KeyValuePair<string, T>>.Add(KeyValuePair<string, T> pair) => ((IDictionary<string, T>)this).Add(pair.Key, pair.Value);

		/// <inheritdoc/>
		void ICollection<KeyValuePair<string, T>>.CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
		{
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			((ICollection<KeyValuePair<string, T>>)actualMap).CopyTo(array, arrayIndex);
		}

		/// <inheritdoc/>
		bool ICollection<KeyValuePair<string, T>>.Remove(KeyValuePair<string, T> item)
		{
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			return ((ICollection<KeyValuePair<string, T>>)actualMap).Remove(item);
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			if (isDeserializedNeedSetup)
			{
				throw new System.Exception("SaveObjectMap is not setup!");
			}
			return ((IEnumerable)actualMap).GetEnumerator();
		}
		#endregion

		/// <inheritdoc/>
		[System.Obsolete("Manual call not supported.", true)]
		public void OnBeforeSerialize()
		{
			// Indicate we started serializing
			isSerializing = true;

			// Setup the map, just in case
			Setup();

			// Remove entries from the serialized list that are not in the set
			for (int i = 0; i < serializedList.Count; ++i)
			{
				if (CanAddKey(serializedList[i]))
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
			// Indicate actualMap contains placeholder data.
			// Setup needs to happen *after* each SaveObject
			// has been properly deserialized.
			isDeserializedNeedSetup = true;

			// Indicate we're done serializing
			isSerializing = false;
		}

		bool CanAddKey(T item) => (item != null) && (string.IsNullOrEmpty(item.Key) == false) && (actualMap.ContainsKey(item.Key) == false);
	}
}
