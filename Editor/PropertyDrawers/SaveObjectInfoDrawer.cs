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
	[CustomPropertyDrawer(typeof(SaveObjectInfoAttribute))]
	[CustomPropertyDrawer(typeof(SaveBool))]
	[CustomPropertyDrawer(typeof(SaveDateTime))]
	[CustomPropertyDrawer(typeof(SaveEnum<>))]
	[CustomPropertyDrawer(typeof(SaveFloat))]
	[CustomPropertyDrawer(typeof(SaveInt))]
	[CustomPropertyDrawer(typeof(SaveString))]
	[CustomPropertyDrawer(typeof(SaveTimeSpan))]
	public class SaveObjectInfoDrawer : PropertyDrawer
	{
		const int SETTINGS_BUTTON_WIDTH = 110;
		const int ACTION_BUTTON_WIDTH = 150;

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
				//using (var indentScope = new EditorGUI.IndentLevelScope())
				{
					// Calculate where to draw the object
					drawPosition = position;
					drawPosition.height = EditorGUIUtility.singleLineHeight;

					// Draw the object field
					EditorGUI.ObjectField(drawPosition, property, label);
					if (property.isExpanded)
					{
						DrawExpandedInfo(position, property);
					}
				}
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

		static void DrawExpandedInfo(Rect position, SerializedProperty property)
		{
			/// Calculate where to add the settings button
			float xPos = position.x + Mathf.Max(0, (position.width - SETTINGS_BUTTON_WIDTH));
			float yPos = position.y + EditorGUIUtility.singleLineHeight + EditorHelpers.VerticalMargin;
			Rect settingsButtonPosition = new Rect(xPos, yPos, SETTINGS_BUTTON_WIDTH, EditorGUIUtility.singleLineHeight);

			Rect objectButtonPosition = settingsButtonPosition;
			objectButtonPosition.width = ACTION_BUTTON_WIDTH;
			objectButtonPosition.x -= (objectButtonPosition.width + EditorHelpers.VerticalMargin);
			if (objectButtonPosition.x < position.x)
			{
				objectButtonPosition.x = position.x;
			}

			// Calculate where to draw status information
			Rect labelPosition = objectButtonPosition;
			labelPosition.x = position.x;
			labelPosition.width = Mathf.Max(0, (objectButtonPosition.x - position.x - EditorHelpers.VerticalMargin));

			// Retrieve property object
			SaveObject saveObject = property.objectReferenceValue as SaveObject;
			DrawKeyLabel(saveObject, in labelPosition);

			// Check if object is in save settings
			DrawActionButton(saveObject, in objectButtonPosition);

			// Draw open settings
			GUIContent buttonContent = new("Open Settings", "Open the saves settings window, where it can be setup and configured.");
			if (GUI.Button(settingsButtonPosition, buttonContent))
			{
				SettingsService.OpenProjectSettings(SavesManager.SIDEBAR_PATH);
			}
		}

		static void DrawKeyLabel(SaveObject saveObject, in Rect labelPosition)
		{
			const string NO_KEY = "Key:";
			const string LABEL_PREPEND = NO_KEY + " \"";
			GUIContent labelContent = new(NO_KEY, "The key name stored in this save object.");
			if (saveObject != null)
			{
				// Generate a string displaying key name
				System.Text.StringBuilder builder = new(saveObject.Key.Length + LABEL_PREPEND.Length + 1);
				builder.Append(LABEL_PREPEND);
				builder.Append(saveObject.Key);
				builder.Append('"');

				// Draw label
				labelContent.text = builder.ToString();
				EditorGUI.LabelField(labelPosition, labelContent);
			}
			else
			{
				// Draw label
				using (var disableScope = new EditorGUI.DisabledGroupScope(true))
				{
					EditorGUI.LabelField(labelPosition, labelContent);
				}
			}
		}

		static void DrawActionButton(SaveObject saveObject, in Rect objectButtonPosition)
		{
			SavesSettingsProvider.ContainsData result = SavesSettingsProvider.ContainsSaveData(saveObject);
			GUIContent buttonContent = new("Add Into Settings", "Add this save object into save settings.");
			switch (result)
			{
				case SavesSettingsProvider.ContainsData.No:
					// Draw the add button
					if (GUI.Button(objectButtonPosition, buttonContent))
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
					if (GUI.Button(objectButtonPosition, buttonContent))
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

				case SavesSettingsProvider.ContainsData.SettingsNotSetup:
					// Draw the setup button
					buttonContent.text = "(Settings Not Setup)";
					buttonContent.tooltip = "Settings needs to be setup before this object can be added to it.";
					using (var disableScope = new EditorGUI.DisabledGroupScope(true))
					{
						GUI.Button(objectButtonPosition, buttonContent);
					}
					break;

				case SavesSettingsProvider.ContainsData.NullArg:
					// Draw the null button
					buttonContent.text = "(Can't Add Null)";
					buttonContent.tooltip = "Cannot add null into settings.";
					using (var disableScope = new EditorGUI.DisabledGroupScope(true))
					{
						GUI.Button(objectButtonPosition, buttonContent);
					}
					break;
			}
		}
	}
}
