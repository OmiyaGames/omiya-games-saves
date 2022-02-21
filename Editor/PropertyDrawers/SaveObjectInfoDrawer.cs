using UnityEngine;
using UnityEditor;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.Saves.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="DefaultSaveObjectDrawer.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/19/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial verison.</description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// TODO
	/// </summary>
	[CustomPropertyDrawer(typeof(SaveInt))]
	[CustomPropertyDrawer(typeof(SaveFloat))]
	[CustomPropertyDrawer(typeof(SaveBool))]
	[CustomPropertyDrawer(typeof(SaveString))]
	[CustomPropertyDrawer(typeof(SaveDateTime))]
	[CustomPropertyDrawer(typeof(SaveTimeSpan))]
	[CustomPropertyDrawer(typeof(SaveEnum<>))]
	[CustomPropertyDrawer(typeof(SaveObjectInfoAttribute))]
	public class SaveObjectInfoDrawer : PropertyDrawer
	{
		const int BUTTON_WIDTH = 160;

		/// <inheritdoc/>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Move this up
			using (var scope = new EditorGUI.PropertyScope(position, GUIContent.none, property))
			{
				// Calculate where to draw the fold-out
				Rect drawPosition = position;
				drawPosition.width = EditorHelpers.IndentSpace;
				drawPosition.height = EditorGUIUtility.singleLineHeight;

				// Draw the fold-out
				property.isExpanded = EditorGUI.Foldout(drawPosition, property.isExpanded, GUIContent.none);

				// Indent everything else
				using (var indentScope = new EditorGUI.IndentLevelScope())
				{
					// Calculate where to draw the object
					drawPosition = position;
					drawPosition.height = EditorGUIUtility.singleLineHeight;

					// Draw the object field
					EditorGUI.ObjectField(drawPosition, property, label);
					if (property.isExpanded)
					{
						DrawStatus(position, property);
					}
				}
			}
		}

		static void DrawStatus(Rect position, SerializedProperty property)
		{
			const string NO_KEY = "Key:";
			const string LABEL_PREPEND = NO_KEY + " \"";

			// Calculate where to draw status information
			Rect labelPosition = position;
			labelPosition.y += (EditorGUIUtility.singleLineHeight + EditorHelpers.VerticalMargin);
			labelPosition.height = EditorGUIUtility.singleLineHeight;
			labelPosition.width -= (BUTTON_WIDTH + EditorHelpers.VerticalMargin);

			Rect buttonPosition = position;
			buttonPosition.x += (labelPosition.width + EditorHelpers.VerticalMargin);
			buttonPosition.y = labelPosition.y;
			buttonPosition.height = labelPosition.height;
			buttonPosition.width = BUTTON_WIDTH;

			// Setup other stuff
			GUIContent labelContent = new(NO_KEY, "The key name stored in this save object.");
			GUIContent buttonContent = new("Add Into Settings", "Add this save object into save settings.");

			// Retrieve property object
			SaveObject saveObject = property.objectReferenceValue as SaveObject;
			if (saveObject == null)
			{
				using (var disableScope = new EditorGUI.DisabledGroupScope(true))
				{
					// Draw label and button
					EditorGUI.LabelField(labelPosition, labelContent);
					GUI.Button(buttonPosition, buttonContent);
				}
				return;
			}

			// Generate a string displaying key name
			System.Text.StringBuilder builder = new(saveObject.Key.Length + LABEL_PREPEND.Length + 1);
			builder.Append(LABEL_PREPEND);
			builder.Append(saveObject.Key);
			builder.Append('"');

			// Draw label
			labelContent.text = builder.ToString();
			EditorGUI.LabelField(labelPosition, labelContent);

			// Check if object is in save settings
			SavesSettingsProvider.ContainsData result = SavesSettingsProvider.ContainsSaveData(saveObject);
			switch (result)
			{
				case SavesSettingsProvider.ContainsData.No:
					// Draw the add button
					if (GUI.Button(buttonPosition, buttonContent))
					{
						if (SavesSettingsProvider.RemoveSaveData(saveObject) == 1)
						{
							EditorUtility.DisplayDialog("Success!", $"Successfully added \"{saveObject.name}\" into settings.", "OK");
						}
						else
						{
							EditorUtility.DisplayDialog("Fail!", $"Could not added \"{saveObject.name}\" into settings.", "OK");
						}
					}
					break;

				case SavesSettingsProvider.ContainsData.Yes:
					// Draw the remove button
					buttonContent.text = "Remove From Settings";
					buttonContent.tooltip = "Remove this save object from save settings.";
					if (GUI.Button(buttonPosition, buttonContent))
					{
						if (SavesSettingsProvider.RemoveSaveData(saveObject) == 1)
						{
							EditorUtility.DisplayDialog("Success!", $"Successfully removed \"{saveObject.name}\" from settings", "OK");
						}
						else
						{
							EditorUtility.DisplayDialog("Fail!", $"Could not remove \"{saveObject.name}\" from settings", "OK");
						}
					}
					break;

				default:
					// Draw the setup button
					buttonContent.text = "Open Save Settings";
					buttonContent.tooltip = "Opens the save settings window so one can set it up.";
					if (GUI.Button(buttonPosition, buttonContent))
					{
						SettingsService.OpenProjectSettings(SavesManager.SIDEBAR_PATH);
					}
					break;
			}
		}

		/// <inheritdoc/>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (property.isExpanded)
			{
				return EditorGUIUtility.singleLineHeight * 2 + EditorHelpers.VerticalMargin;
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}
	}
}
