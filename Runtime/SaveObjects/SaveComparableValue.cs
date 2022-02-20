using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SaveComparableValue.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/17/2022<br/>
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
	/// Helper abstract class with common methods already defined for most
	/// comparable variable types.
	/// </summary>
	public abstract class SaveComparableValue<T> : SaveSingleValue<T> where T : System.IComparable<T>
	{
		[Header("Clamp Boundaries")]
		[SerializeField]
		bool hasMin = false;
		[SerializeField]
		T minValue;
		[SerializeField]
		bool hasMax = false;
		[SerializeField]
		T maxValue;

		/// <summary>
		/// TODO
		/// </summary>
		public bool HasMin => hasMin;
		/// <summary>
		/// TODO
		/// </summary>
		public T MinValue
		{
			get => minValue;
			protected set => minValue = value;
		}
		/// <summary>
		/// TODO
		/// </summary>
		public bool HasMax => hasMax;
		/// <summary>
		/// TODO
		/// </summary>
		public T MaxValue
		{
			get => maxValue;
			protected set => maxValue = value;
		}

		/// <inheritdoc/>
		protected override T SetValue(T value, SaveState setState)
		{
			// Clamp the value first
			return base.SetValue(Clamp(value), setState);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual T Clamp(T value)
		{
			if (HasMin && (MinValue.CompareTo(value) > 0))
			{
				value = MinValue;
			}
			else if(HasMax && (MaxValue.CompareTo(value) < 0))
			{
				value = MaxValue;
			}
			return value;
		}
	}
}
