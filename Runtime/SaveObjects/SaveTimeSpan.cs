using System;
using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SaveTimeSpan.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/20/2022<br/>
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
	/// Interface for loading a <see cref="TimeSpan"/> from <see cref="IAsyncSettingsRecorder"/>
	/// </summary>
	[CreateAssetMenu(menuName = "Omiya Games/Save Time Duration", fileName = "Save TimeSpan", order = (MENU_ORDER + 5))]
	public class SaveTimeSpan : SaveSingleValue<TimeSpan, string>
	{
		/// <summary>
		/// Converts a string to <see cref="TimeSpan"/>.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Equivalent <see cref="TimeSpan"/>.
		/// </returns>
		public static TimeSpan Convert(string value)
		{
			long.TryParse(value, out long ticks);
			return new(ticks);
		}
		/// <summary>
		/// Converts a <see cref="TimeSpan"/> to string.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Equivalent string.
		/// </returns>
		public static string Convert(in TimeSpan value) => value.Ticks.ToString();

		/// <summary>
		/// 5-Minutes.
		/// </summary>
		static readonly TimeSpan DEFAULT_TIME_SPAN = TimeSpan.FromMinutes(5);

		TimeSpan? cacheDefaultValue = null;

		/// <inheritdoc/>
		public override TimeSpan ConvertedDefaultValue
		{
			get
			{
				if (cacheDefaultValue == null)
				{
					// Convert the default value into DateTime
					cacheDefaultValue = Convert(defaultValue);
				}
				return cacheDefaultValue.Value;
			}
		}

		/// <inheritdoc/>
		public override bool HasValue => true;

		/// <inheritdoc/>
		protected override void RecordValue(TimeSpan newValue) => Recorder.SetTimeSpan(Key, newValue);

		/// <inheritdoc/>
		protected override WaitLoadValue<TimeSpan> RetrieveValue() => Recorder.GetTimeSpan(Key, ConvertedDefaultValue);

		/// <inheritdoc/>
		public override void Reset()
		{
			base.Reset();
			defaultValue = DEFAULT_TIME_SPAN.Ticks.ToString();
		}
	}
}
