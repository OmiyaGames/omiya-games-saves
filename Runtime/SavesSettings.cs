using System.Collections.Generic;
using UnityEngine;
using OmiyaGames.Global.Settings;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SavesSettings.cs" company="Omiya Games">
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
	/// <description>Initial draft.</description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Scriptable object with settings info.
	/// </summary>
	public class SavesSettings : BaseSettingsData
	{
		const string DATA_DIRECTORY = "Packages/com.omiyagames.saves/Runtime/Data/";
		const string PLAYERPREFS_RECORDER_PATH = DATA_DIRECTORY + "PlayerPrefsRecorder.asset";
		const string VERSION_PATH = DATA_DIRECTORY + "Version.asset";

		// Don't forget to update this property each time there's an upgrade to make!
		/// <inheritdoc/>
		public override int CurrentVersion => 0;

		[System.Serializable]
		public struct SupportedRecorder
		{
			[SerializeField]
			SupportedPlatforms platforms;
			[SerializeField]
			AsyncSettingsRecorder recorder;

			public SupportedRecorder(SupportedPlatforms platforms, AsyncSettingsRecorder recorder)
			{
				this.platforms = platforms;
				this.recorder = recorder;
			}

			public SupportedPlatforms Platforms => platforms;
			public AsyncSettingsRecorder Recorder => recorder;
		}

		[SerializeField]
		SupportedRecorder[] recorders;

		[Header("Version Handling")]
		[SerializeField]
		SaveInt versionSaver;
		[SerializeField]
		SavesUpgrader[] upgraders;
		[SerializeField]
		SerializableHashSet<SaveObject> saveData = new();

		/// <summary>
		/// TODO
		/// </summary>
		public IReadOnlyList<SupportedRecorder> Recorders => recorders;
		/// <summary>
		/// TODO
		/// </summary>
		public SaveInt Version => versionSaver;
		/// <summary>
		/// TODO
		/// </summary>
		public IReadOnlyList<SavesUpgrader> Upgraders => upgraders;
		/// <summary>
		/// TODO
		/// </summary>
		public ISet<SaveObject> SaveData => saveData;

#if UNITY_EDITOR
		void Reset()
		{
			var defaultRecorder = UnityEditor.AssetDatabase.LoadAssetAtPath<AsyncSettingsRecorder>(PLAYERPREFS_RECORDER_PATH);
			if (defaultRecorder)
			{
				recorders = new SupportedRecorder[]
				{
					new SupportedRecorder(SupportedPlatforms.AllPlatforms, defaultRecorder)
				};
			}
			
			versionSaver = UnityEditor.AssetDatabase.LoadAssetAtPath<SaveInt>(VERSION_PATH);

			// TODO: consider adding these settings into the save settings as well.
		}
#endif
	}
}
