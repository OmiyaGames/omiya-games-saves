using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using OmiyaGames.Global.Settings.Editor;

namespace OmiyaGames.Saves.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SavesSettingsProvider.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/18/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial verison.</description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Editor for <see cref="SavesSettings"/>.
	/// Appears under the Project Settings window.
	/// </summary>
	public class SavesSettingsProvider : BaseSettingsEditor<SavesSettings>
	{
		public enum ContainsData
		{
			Yes,
			No,
			IsVersion,
			SettingsNotSetup,
			InvalidKey,
		}

		static SavesSettingsProvider lastInstance = null;

		/// <inheritdoc/>
		public override string DefaultSettingsFileName => "SavesSettings";
		/// <inheritdoc/>
		public override string UxmlPath => SavesManager.UXML_PATH;
		/// <inheritdoc/>
		public override string AddressableGroupName => SettingsEditorHelpers.OMIYA_GAMES_GROUP_NAME;
		/// <inheritdoc/>
		public override string AddressableName => SavesManager.ADDRESSABLE_NAME;
		/// <inheritdoc/>
		public override string ConfigName => SavesManager.CONFIG_NAME;
		/// <inheritdoc/>
		public override string HeaderText => "Saves Settings";
		/// <inheritdoc/>
		public override string HelpUrl => "https://omiyagames.github.io/omiya-games-saves";

		/// <summary>
		/// Constructs a project-scoped <see cref="SettingsProvider"/>.
		/// </summary>
		public SavesSettingsProvider(string path, IEnumerable<string> keywords) : base(path, keywords) { }

		/// <summary>
		/// Registers this <see cref="SettingsProvider"/>.
		/// </summary>
		/// <returns></returns>
		[SettingsProvider]
		public static SettingsProvider CreateSettingsProvider()
		{
			// Create the settings provider
			lastInstance = new SavesSettingsProvider(SavesManager.SIDEBAR_PATH, GetSearchKeywordsFromGUIContentProperties<Styles>());
			return lastInstance;
		}

		/// <summary>
		/// Helper function for editor in adding new save data slots in save settings.
		/// </summary>
		/// <param name="newData">
		/// All the data to be added into settings.
		/// </param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException">
		/// If this function is *not* called from the editor.
		/// </exception>
		public static ContainsData ContainsSaveData(SaveObject checkData)
		{
			SavesSettings settings;
			if (EditorBuildSettings.TryGetConfigObject(SavesManager.CONFIG_NAME, out settings) == false)
			{
				return ContainsData.SettingsNotSetup;
			}
			else if ((checkData == null) || string.IsNullOrEmpty(checkData.Key))
			{
				return ContainsData.InvalidKey;
			}
			else if ((settings.Version != null) && (string.Equals(settings.Version.Key, checkData.Key)))
			{
				return ContainsData.IsVersion;
			}
			else if (settings.SaveData.ContainsKey(checkData.Key))
			{
				return ContainsData.Yes;
			}
			else
			{
				return ContainsData.No;
			}
		}

		/// <summary>
		/// Adds new save data slots in save settings.
		/// </summary>
		/// <param name="newData">
		/// All the data to be added into settings.
		/// </param>
		/// <returns>
		/// Number of save data successfully added.
		/// </returns>
		public static int AddSaveData(params SaveObject[] newData)
		{
			// Confirm there are data to add,
			// and that there's an instance of SavesSettings.
			int returnNumSavesAdded = 0;
			if ((newData != null) && (newData.Length > 0) && (EditorBuildSettings.TryGetConfigObject(SavesManager.CONFIG_NAME, out SavesSettings settings)))
			{
				// Add all the new data into settings
				foreach (var data in newData)
				{
					if ((data != null) && settings.SaveData.Add(data))
					{
						++returnNumSavesAdded;
					}
				}

				// Save these changes
				EditorUtility.SetDirty(settings);
				AssetDatabase.SaveAssetIfDirty(settings);
			}
			return returnNumSavesAdded;
		}


		/// <summary>
		/// Removes existing save data slots from save settings.
		/// </summary>
		/// <param name="removeData">
		/// All the data to be removed from settings.
		/// </param>
		/// <returns></returns>
		public static int RemoveSaveData(params SaveObject[] removeData)
		{
			// Confirm there are data to add,
			// and that there's an instance of SavesSettings.
			int returnNumSavesRemoved = 0;
			if ((removeData != null) && (removeData.Length > 0) && (EditorBuildSettings.TryGetConfigObject(SavesManager.CONFIG_NAME, out SavesSettings settings)))
			{
				// Remove the new data from settings
				foreach (var data in removeData)
				{
					if ((data != null) && settings.SaveData.Remove(data.Key))
					{
						++returnNumSavesRemoved;
					}
				}

				// Save these changes
				EditorUtility.SetDirty(settings);
				AssetDatabase.SaveAssetIfDirty(settings);
			}
			return returnNumSavesRemoved;
		}

		/// <inheritdoc/>
		protected override VisualElement CustomizeEditSettingsTree(VisualElement returnTree, SerializedObject settingsProperty)
		{
			SerializedProperty property = settingsProperty.FindProperty("upgraders");

			// Grab the serialized property
			ListView listView = returnTree.Q<ListView>("upgraders");
			listView.fixedItemHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			listView.makeItem = () =>
			{
				ObjectField returnField = new();
				returnField.objectType = typeof(SavesUpgrader);
				returnField.allowSceneObjects = false;
				return returnField;
			};
			listView.bindItem = (e, index) =>
			{
				ObjectField field = (ObjectField)e;
				field.label = $"Version {index + 1}";
				field.value = property.objectReferenceValue;
			};
			return base.CustomizeEditSettingsTree(returnTree, settingsProperty);
		}

		class Styles
		{
			// FIXME: fill this group
			//public static readonly GUIContent defaultHitPauseDuration = new GUIContent("Default Hit Pause Duration Seconds");
		}
	}
}
