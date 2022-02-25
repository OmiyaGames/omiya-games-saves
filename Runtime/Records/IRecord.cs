using System.Text;
using System;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="IRecord.cs" company="Omiya Games">
	/// The MIT License (MIT)
	/// 
	/// Copyright (c) 2014-2022 Omiya Games
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
	/// <strong>Date:</strong> 5/28/2017<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial version.</description>
	/// </item><item>
	/// <term>
	/// <strong>Version:</strong> 0.2.0-exp.1<br/>
	/// <strong>Date:</strong> 2/19/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Converting to package.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Stores a record that can be stored in <see cref="GameSettings"/>.
	/// </summary>
	public abstract class IRecord<T> where T : IComparable<T>
	{
		public const char Divider = '|';
		public delegate bool TryConvertOldRecord(string record, int recordedVersion, out T newRecord);

		private readonly DateTime dateAchievedUtc;
		private string name;

		public IRecord(string pastRecord, int appVersion, TryConvertOldRecord converter)
		{
			if (string.IsNullOrEmpty(pastRecord) == true)
			{
				throw new ArgumentNullException(nameof(pastRecord), "pastRecord must be provided");
			}

			// Split the information
			string[] info = pastRecord.Split(Divider);
			if ((info == null) || (info.Length <= 0))
			{
				throw new ArgumentException("Could not parse pastRecord: " + pastRecord);
			}

			// Go through each split item
			byte index = 0;
			if (appVersion < AddLanguageSettings.AppVersion)
			{
				// Disregard the app version from the older records
				++index;
			}

			// Grab score
			ParseRecord(info[index], appVersion, converter);

			// Grab name
			++index;
			Name = info[index];

			// Grab time
			long ticks;
			++index;
			if (long.TryParse(info[index], out ticks) == true)
			{
				dateAchievedUtc = new DateTime(ticks);
			}
			else
			{
				throw new ArgumentException("Could not parse the date from pastRecord: " + pastRecord);
			}
		}

		public IRecord(T newRecord, string newName)
		{
			// Setup all the member variables
			dateAchievedUtc = DateTime.UtcNow;

			// Record name
			Name = newName;

			// Record record
			Record = newRecord;
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				// Prevent null
				if (value != null)
				{
					name = value;
				}
				else
				{
					name = string.Empty;
				}
			}
		}

		public virtual T Record
		{
			get;
			set;
		}

		public DateTime DateAchievedUtc
		{
			get
			{
				return dateAchievedUtc;
			}
		}

		protected abstract void ParseRecord(string record, int appVersion, TryConvertOldRecord converter);
		protected abstract string ConvertRecordToString();

		public override string ToString()
		{
			// Create a record out of this information
			StringBuilder builder = new StringBuilder();

			// Add score
			builder.Append(ConvertRecordToString());
			builder.Append(Divider);

			// Add name
			builder.Append(Name);
			builder.Append(Divider);

			// Add date of record in UTC
			builder.Append(DateAchievedUtc.Ticks);

			// Store this achievement as a string record
			return builder.ToString();
		}
	}
}
