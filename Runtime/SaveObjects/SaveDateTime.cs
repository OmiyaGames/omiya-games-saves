using System;
using UnityEngine;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SaveDateTime.cs" company="Omiya Games">
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
	/// Interface for loading a <see cref="DateTime"/> from <see cref="IAsyncSettingsRecorder"/>
	/// </summary>
	[CreateAssetMenu(menuName = "Omiya Games/Saves/Save Date & Time", fileName = "Save DateTime")]
	public class SaveDateTime : SaveSingleValue<DateTime, string>
	{
		[SerializeField]
		bool defaultToNow = true;

		DateTime? cacheDefaultValue = null;

		/// <inheritdoc/>
		/// <remarks>
		/// Time will always be in UTC.
		/// </remarks>
		public override DateTime ConvertedDefaultValue
		{
			get
			{
				if (defaultToNow)
				{
					return DateTime.UtcNow;
				}
				else if (cacheDefaultValue == null)
				{
					// Convert the default value into DateTime
					long ticks = long.Parse(defaultValue);
					cacheDefaultValue = new(ticks, DateTimeKind.Utc);
				}
				return cacheDefaultValue.Value;
			}
		}

		/// <inheritdoc/>
		public override bool HasValue => true;

		/// <inheritdoc/>
		/// <remarks>
		/// Make sure <paramref name="newValue"/> is in UTC.
		/// </remarks>
		protected override void RecordValue(DateTime newValue) => Recorder.SetDateTimeUtc(Key, newValue);

		/// <inheritdoc/>
		/// <remarks>
		/// Retrieved value will be in UTC.
		/// </remarks>
		protected override WaitLoadValue<DateTime> RetrieveValue() => Recorder.GetDateTimeUtc(Key, ConvertedDefaultValue);
	}
}