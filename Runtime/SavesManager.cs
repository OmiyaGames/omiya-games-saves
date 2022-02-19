using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames.Global;
using OmiyaGames.Global.Settings;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <copyright file="SavesManager.cs" company="Omiya Games">
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
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// A manager file that allows adjusting an <see cref="SavesMixer"/>
	/// from settings.
	/// </summary>
	public class SavesManager : BaseSettingsManager<SavesManager, SavesSettings>
	{
		/// <summary>
		/// The configuration name stored in Editor Settings.
		/// </summary>
		public const string CONFIG_NAME = "com.omiyagames.saves";
		/// <summary>
		/// The name this settings will appear in the
		/// Project Setting's left-sidebar.
		/// </summary>
		public const string SIDEBAR_PATH = "Project/Omiya Games/Saves";
		/// <summary>
		/// Name of the addressable.
		/// </summary>
		public const string ADDRESSABLE_NAME = "SavesSettings";
		/// <summary>
		/// Path to UXML file.
		/// </summary>
		public const string UXML_PATH = "Packages/com.omiyagames.saves/Editor/Saves.uxml";

		/// <inheritdoc/>
		protected override string AddressableName => ADDRESSABLE_NAME;
		//public static float MuteVolumeDb => GetData().MuteVolumeDb;
		// void Start()
		// {
		// 	// Retrieve settings
		// 	SavesSettings SavesData = GetData();
		// 	GameSettings saveData = Singleton.Get<GameSettings>();
		// 	if (saveData != null)
		// 	{
		// 		UpdateVolumeDb(SavesData, SavesData.MusicVolume, saveData.MusicVolume, saveData.IsMusicMuted);
		// 		UpdateVolumeDb(SavesData, SavesData.SoundEffectsVolume, saveData.SoundVolume, saveData.IsSoundMuted);
		// 	}

		// 	// Check the TimeManager event
		// 	TimeManager.OnAfterManualPauseChanged += OnPauseChanged;
		// }

		// protected override void OnDestroy()
		// {
		// 	base.OnDestroy();
		// 	TimeManager.OnAfterManualPauseChanged -= OnPauseChanged;
		// }
	}
}
