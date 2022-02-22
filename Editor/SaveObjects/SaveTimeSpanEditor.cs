using System;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace OmiyaGames.Saves.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SaveTimeSpanEditor.cs" company="Omiya Games">
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
	/// Editor for <seealso cref="SaveTimeSpan"/>.
	/// </summary>
	[CustomEditor(typeof(SaveTimeSpan))]
	public class SaveTimeSpanEditor : BaseSaveObjectEditor
	{
		const string UXML_PATH = UXML_DIRECTORY + "SaveTimeSpanContent.uxml";

		IntegerField days, hours, minutes, seconds, milliseconds;

		/// <inheritdoc/>
		protected override void FillContent(VisualElement content)
		{
			// Load from UXML file
			AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH).CloneTree(content);
			SetupErrorHandlingDropDown(content, "onLoadFailed", "onLoadFailed");

			// Setup member variables
			days = content.Q<IntegerField>("days");
			hours = content.Q<IntegerField>("hours");
			minutes = content.Q<IntegerField>("minutes");
			seconds = content.Q<IntegerField>("seconds");
			milliseconds = content.Q<IntegerField>("milliseconds");

			// Convert serialized value to TimeSpan
			SerializedProperty defaultString = serializedObject.FindProperty("defaultValue");
			TimeSpan span = SaveTimeSpan.Convert(defaultString.stringValue);
			UpdateFields(in span);

			// Update button behaviors
			days.RegisterCallback<ChangeEvent<int>>(e =>
				ApplyChanged(e.newValue, hours.value, minutes.value, seconds.value, milliseconds.value));
			hours.RegisterCallback<ChangeEvent<int>>(e =>
				ApplyChanged(days.value, e.newValue, minutes.value, seconds.value, milliseconds.value));
			minutes.RegisterCallback<ChangeEvent<int>>(e =>
				ApplyChanged(days.value, hours.value, e.newValue, seconds.value, milliseconds.value));
			seconds.RegisterCallback<ChangeEvent<int>>(e =>
				ApplyChanged(days.value, hours.value, minutes.value, e.newValue, milliseconds.value));
			milliseconds.RegisterCallback<ChangeEvent<int>>(e =>
				ApplyChanged(days.value, hours.value, minutes.value, seconds.value, e.newValue));

			// Update button behavior
			Button simplify = content.Q<Button>("simplifyButton");
			simplify.RegisterCallback<ClickEvent>(e =>
			{
				// Only change the UI
				TimeSpan span = new(days.value, hours.value, minutes.value, seconds.value, milliseconds.value);
				UpdateFields(in span);
			});
		}

		void UpdateFields(in TimeSpan span)
		{
			// Apply the changes to all fiels
			days.SetValueWithoutNotify(span.Days);
			hours.SetValueWithoutNotify(span.Hours);
			minutes.SetValueWithoutNotify(span.Minutes);
			seconds.SetValueWithoutNotify(span.Seconds);
			milliseconds.SetValueWithoutNotify(span.Milliseconds);
		}

		void ApplyChanged(int days, int hours, int minutes, int seconds, int milliseconds)
		{
			// Convert the params into TimeSpan
			TimeSpan span = new(days, hours, minutes, seconds, milliseconds);
			SerializedProperty defaultString = serializedObject.FindProperty("defaultValue");

			// Convert timespan into a string
			serializedObject.Update();
			defaultString.stringValue = SaveTimeSpan.Convert(span);
			serializedObject.ApplyModifiedProperties();
		}
	}
}
