using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames.Managers;
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
		/// <summary>
		/// TODO
		/// </summary>
		public static float MuteVolumeDb => GetData().MuteVolumeDb;
		/// <summary>
		/// TODO
		/// </summary>
		public static float MainVolumeDb
		{
			get
			{
				SavesSettings setting = GetData();
				return GetVolumeDb(setting, setting.MainVolume);
			}
		}
		/// <summary>
		/// TODO
		/// </summary>
		public static float MusicVolumeDb
		{
			get
			{
				SavesSettings setting = GetData();
				return GetVolumeDb(setting, setting.MusicVolume);
			}
		}
		/// <summary>
		/// TODO
		/// </summary>
		public static float SoundEffectsVolumeDb
		{
			get
			{
				SavesSettings setting = GetData();
				return GetVolumeDb(setting, setting.SoundEffectsVolume);
			}
		}
		/// <summary>
		/// TODO
		/// </summary>
		public static float VoiceVolumeDb
		{
			get
			{
				SavesSettings setting = GetData();
				return GetVolumeDb(setting, setting.VoiceVolume);
			}
		}
		/// <summary>
		/// TODO
		/// </summary>
		public static float AmbienceVolumeDb
		{
			get
			{
				SavesSettings setting = GetData();
				return GetVolumeDb(setting, setting.AmbienceVolume);
			}
		}

		// FIXME: convert this to an ITrackable
		/// <summary>
		/// TODO
		/// </summary>
		public static float MainVolumePercent
		{
			get
			{
				// FIXME: this is incorrect
				SavesSettings setting = GetData();
				return GetVolumeDb(setting, setting.MainVolume);
			}
			set
			{
				SavesSettings setting = GetData();
				float volumeDecibels = Mathf.Clamp01(value);
				volumeDecibels = ConvertPercentToVolumeDb(setting, volumeDecibels);
				SetMixerFloat(setting, setting.MainVolume, volumeDecibels);
			}
		}

		/// <summary>
		/// TODO
		/// </summary>
		public static float MainPitch
		{
			get
			{
				SavesSettings setting = GetData();
				return GetPitch(setting, setting.MainPitch);
			}
			set
			{
				SavesSettings setting = GetData();
				SetMixerFloat(setting, setting.MainPitch, value);
			}
		}
		/// <summary>
		/// TODO
		/// </summary>
		public static float MusicPitch
		{
			get
			{
				SavesSettings setting = GetData();
				return GetPitch(setting, setting.MusicPitch);
			}
			set
			{
				SavesSettings setting = GetData();
				SetMixerFloat(setting, setting.MusicPitch, value);
			}
		}
		/// <summary>
		/// TODO
		/// </summary>
		public static float SoundEffectsPitch
		{
			get
			{
				SavesSettings setting = GetData();
				return GetPitch(setting, setting.SoundEffectsPitch);
			}
			set
			{
				SavesSettings setting = GetData();
				SetMixerFloat(setting, setting.SoundEffectsPitch, value);
			}
		}
		/// <summary>
		/// TODO
		/// </summary>
		public static float VoicePitch
		{
			get
			{
				SavesSettings setting = GetData();
				return GetPitch(setting, setting.VoicePitch);
			}
			set
			{
				SavesSettings setting = GetData();
				SetMixerFloat(setting, setting.VoicePitch, value);
			}
		}
		/// <summary>
		/// TODO
		/// </summary>
		public static float AmbiencePitch
		{
			get
			{
				SavesSettings setting = GetData();
				return GetPitch(setting, setting.AmbiencePitch);
			}
			set
			{
				SavesSettings setting = GetData();
				SetMixerFloat(setting, setting.AmbiencePitch, value);
			}
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="percent"></param>
		/// <returns></returns>
		public static float ConvertPercentToVolumeDb(float percent) => ConvertPercentToVolumeDb(GetData(), percent);

		// Start is called before the first frame update
		void Start()
		{
			// Retrieve settings
			SavesSettings SavesData = GetData();
			GameSettings saveData = Singleton.Get<GameSettings>();
			if (saveData != null)
			{
				UpdateVolumeDb(SavesData, SavesData.MusicVolume, saveData.MusicVolume, saveData.IsMusicMuted);
				UpdateVolumeDb(SavesData, SavesData.SoundEffectsVolume, saveData.SoundVolume, saveData.IsSoundMuted);
			}

			// Check the TimeManager event
			TimeManager.OnAfterManualPauseChanged += OnPauseChanged;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			TimeManager.OnAfterManualPauseChanged -= OnPauseChanged;
		}
	}
}
