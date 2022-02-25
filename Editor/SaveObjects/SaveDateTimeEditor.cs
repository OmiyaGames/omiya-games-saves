using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace OmiyaGames.Saves.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SaveDateTimeEditor.cs" company="Omiya Games">
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
	/// Editor for <seealso cref="SaveDateTime"/>.
	/// </summary>
	[CustomEditor(typeof(SaveDateTime))]
	public class SaveDateTimeEditor : BaseSaveObjectEditor
	{
		const string UXML_PATH = UXML_DIRECTORY + "SaveDateTimeContent.uxml";
		static readonly int MIN_YEAR = DateTime.MinValue.Year;
		static readonly int MAX_YEAR = DateTime.MaxValue.Year;

		Foldout customDataTime;
		IntegerField year, month, day, hour, minute, second, millisecond;

		/// <inheritdoc/>
		protected override void FillContent(VisualElement content)
		{
			// Load from UXML file
			AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH).CloneTree(content);
			SetupErrorHandlingDropDown(content, "onLoadFailed", "onLoadFailed");

			// Setup member variables
			customDataTime = content.Q<Foldout>("customDataTime");
			year = content.Q<IntegerField>("year");
			month = content.Q<IntegerField>("month");
			day = content.Q<IntegerField>("day");
			hour = content.Q<IntegerField>("hour");
			minute = content.Q<IntegerField>("minute");
			second = content.Q<IntegerField>("second");
			millisecond = content.Q<IntegerField>("millisecond");

			// Convert serialized value to TimeSpan
			SerializedProperty defaultString = serializedObject.FindProperty("defaultValue");
			DateTime dateTime = SaveDateTime.Convert(defaultString.stringValue);
			UpdateFields(in dateTime);

			// Update field behaviors
			year.RegisterCallback<ChangeEvent<int>>(OnYearChanged);
			month.RegisterCallback<ChangeEvent<int>>(OnMonthChanged);
			day.RegisterCallback<ChangeEvent<int>>(OnDayChanged);
			hour.RegisterCallback<ChangeEvent<int>>(OnHourChanged);
			minute.RegisterCallback<ChangeEvent<int>>(OnMinuteChanged);
			second.RegisterCallback<ChangeEvent<int>>(OnSecondChanged);
			millisecond.RegisterCallback<ChangeEvent<int>>(OnMillisecondChanged);

			// Update toggle behavior
			Toggle defaultToNow = content.Q<Toggle>("defaultToNow");
			defaultToNow.RegisterCallback<ChangeEvent<bool>>(e =>
				customDataTime.style.display = (e.newValue ? DisplayStyle.None : DisplayStyle.Flex));

			// Update button behavior
			Button convertToUtc = content.Q<Button>("convertToUtc");
			convertToUtc.RegisterCallback<ClickEvent>(e =>
			{
				// Convert fields to local time
				DateTime localDateTime = new(year.value, month.value, day.value,
					hour.value, minute.value, second.value,
					millisecond.value, DateTimeKind.Local);

				// Then convert that to UTC
				DateTime utcDateTime = localDateTime.ToUniversalTime();
				UpdateFields(in utcDateTime);
				ApplyChanged(in utcDateTime);
			});
		}

		void UpdateFields(in DateTime dateTime)
		{
			// Apply the changes to all fiels
			year.SetValueWithoutNotify(dateTime.Year);
			month.SetValueWithoutNotify(dateTime.Month);
			day.SetValueWithoutNotify(dateTime.Day);
			hour.SetValueWithoutNotify(dateTime.Hour);
			minute.SetValueWithoutNotify(dateTime.Minute);
			second.SetValueWithoutNotify(dateTime.Second);
			millisecond.SetValueWithoutNotify(dateTime.Millisecond);
		}

		void ApplyChanged(int year, int month, int day, int hour, int minute, int second, int millisecond) =>
			ApplyChanged(new(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc));

		void ApplyChanged(in DateTime dateTime)
		{
			// Convert the params into TimeSpan
			SerializedProperty defaultString = serializedObject.FindProperty("defaultValue");

			// Convert timespan into a string
			serializedObject.Update();
			defaultString.stringValue = SaveDateTime.Convert(dateTime);
			serializedObject.ApplyModifiedProperties();
		}

		#region Integer Fields Events
		void OnYearChanged(ChangeEvent<int> e)
		{
			// First, clamp the year
			int clampYear = Mathf.Clamp(e.newValue, MIN_YEAR, MAX_YEAR);
			year.SetValueWithoutNotify(clampYear);

			// Also clamp the day
			int clampDay = Mathf.Clamp(day.value, 1, DateTime.DaysInMonth(clampYear, month.value));
			day.SetValueWithoutNotify(clampDay);

			// Apply the changes
			ApplyChanged(clampYear, month.value, clampDay,
				hour.value, minute.value, second.value,
				millisecond.value);
		}

		void OnMonthChanged(ChangeEvent<int> e)
		{
			// First, clamp the month
			int clampMonth = Mathf.Clamp(e.newValue, 1, 12);
			month.SetValueWithoutNotify(clampMonth);

			// Also clamp the day
			int clampDay = Mathf.Clamp(day.value, 1, DateTime.DaysInMonth(year.value, clampMonth));
			day.SetValueWithoutNotify(clampDay);

			// Apply the changes
			ApplyChanged(year.value, clampMonth, clampDay,
				hour.value, minute.value, second.value,
				millisecond.value);
		}

		void OnDayChanged(ChangeEvent<int> e)
		{
			// First, clamp the day
			int clampDay = Mathf.Clamp(e.newValue, 1, DateTime.DaysInMonth(year.value, month.value));
			day.SetValueWithoutNotify(clampDay);

			// Apply the changes
			ApplyChanged(year.value, month.value, clampDay,
				hour.value, minute.value, second.value,
				millisecond.value);
		}

		void OnHourChanged(ChangeEvent<int> e)
		{
			// First, clamp the hour
			int clampHour = Mathf.Clamp(e.newValue, 0, 23);
			hour.SetValueWithoutNotify(clampHour);

			// Apply the changes
			ApplyChanged(year.value, month.value, day.value,
				clampHour, minute.value, second.value,
				millisecond.value);
		}

		void OnMinuteChanged(ChangeEvent<int> e)
		{
			// First, clamp the minute
			int clampMinute = Mathf.Clamp(e.newValue, 0, 59);
			minute.SetValueWithoutNotify(clampMinute);

			// Apply the changes
			ApplyChanged(year.value, month.value, day.value,
				hour.value, clampMinute, second.value,
				millisecond.value);
		}

		void OnSecondChanged(ChangeEvent<int> e)
		{
			// First, clamp the minute
			int clampSecond = Mathf.Clamp(e.newValue, 0, 59);
			second.SetValueWithoutNotify(clampSecond);

			// Apply the changes
			ApplyChanged(year.value, month.value, day.value,
				hour.value, minute.value, clampSecond,
				millisecond.value);
		}

		void OnMillisecondChanged(ChangeEvent<int> e)
		{
			// First, clamp the minute
			int clampMilli = Mathf.Clamp(e.newValue, 0, 999);
			millisecond.SetValueWithoutNotify(clampMilli);

			// Apply the changes
			ApplyChanged(year.value, month.value, day.value,
				hour.value, minute.value, second.value,
				clampMilli);
		}
		#endregion
	}
}
