using System.Collections;
using UnityEngine;
using OmiyaGames.Global;
using OmiyaGames.Global.Settings;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
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
	/// </item><item>
	/// <term>
	/// <strong>Version:</strong> 0.2.1-exp.1<br/>
	/// <strong>Date:</strong> 2/26/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Throwing exceptions if manager is not setup when calling
	/// <see cref="TryGet(string, out SaveObject)"/> and
	/// <see cref="Contains(SaveObject)"/>. Renamed
	/// <c>TryGetSave(string, out SaveObject)</c> to
	/// <see cref="TryGet(string, out SaveObject)"/>.
	/// Added <see cref="Contains(string)"/>.
	/// </description>
	/// </item><item>
	/// <term>
	/// <strong>Version:</strong> 0.2.2-exp.1<br/>
	/// <strong>Date:</strong> 3/2/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Updated class to be <c>static</c>, preventing it from being attached
	/// to any <see cref="GameObject"/>.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// A manager file that configures and loads <seealso cref="SaveObject"/>s
	/// on <seealso cref="Setup"/>. Allows retrieving <seealso cref="SaveObject"/>
	/// by their keys. Handles global <seealso cref="Save"/>.
	/// </summary>
	public static class SavesManager
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

		/// <summary>
		/// Indicates whether the manager is either still
		/// in the middle of setting up, or is already setup.
		/// </summary>
		public static LoadState SetupState => SavesSettingsManager.GetInstance().SetupState;

		/// <summary>
		/// A coroutine to setup this manager.
		/// </summary>
		/// <returns></returns>
		public static IEnumerator Setup(bool forceSetup = false)
		{
			// Check if setup yet
			SavesSettingsManager manager = SavesSettingsManager.GetInstance();
			forceSetup |= (manager.SetupState == LoadState.Loading);
			if (forceSetup)
			{
				// Check if manager previously was setup before
				if (manager.SetupState != LoadState.Loading)
				{
					// Force setup
					manager.ForceSetup();
				}

				// Wait until the manager is ready
				yield return new WaitUntil(IsReady);
				bool IsReady() => manager.SetupState != LoadState.Loading;
			}
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static SaveObject Get(string key) =>
			CheckInstance().Settings.SaveData[key];

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <param name="saveObject"></param>
		/// <returns></returns>
		public static bool TryGet(string key, out SaveObject saveObject) =>
			CheckInstance().Settings.SaveData.TryGetValue(key, out saveObject);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="saveObject"></param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static bool Contains(SaveObject saveObject) =>
			CheckInstance().Settings.SaveData.Contains(saveObject);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException"></exception>
		public static bool Contains(string key) => CheckInstance().Settings.SaveData.ContainsKey(key);

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public static WaitLoad Save() => CheckInstance().CurrentRecorder.Save();

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public static WaitLoad DeleteAllKeys() => CheckInstance().CurrentRecorder.DeleteAll();

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static WaitLoad DeleteKey(string key) => CheckInstance().CurrentRecorder.DeleteKey(key);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static WaitLoadValue<bool> HasKey(string key) => CheckInstance().CurrentRecorder.HasKey(key);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <param name="action"></param>
		public static void SubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action) =>
			CheckInstance().CurrentRecorder.SubscribeToDeleteKey(key, action);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <param name="action"></param>
		public static void UnsubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action) =>
			CheckInstance().CurrentRecorder.UnsubscribeToDeleteKey(key, action);

		static SavesSettingsManager CheckInstance()
		{
			SavesSettingsManager manager = SavesSettingsManager.GetInstance();
			if (manager.SetupState != LoadState.Success)
			{
				throw new System.Exception("SavesManager is not setup yet.");
			}
			return manager;
		}

		/// <summary>
		/// Retrieves the recorder that is supported for this build.
		/// If there are multiple, a composite recorder is generated.
		/// </summary>
		static IAsyncSettingsRecorder GetSupportedRecorder(SavesSettings.SupportedRecorder[] recorders, out bool isNewRecorderCreated)
		{
			// Setup variables
			isNewRecorderCreated = false;
			ListSet<AsyncSettingsRecorder> capturedRecorders = new(recorders.Length);

			// Go through all the recorders
			foreach (SavesSettings.SupportedRecorder pair in recorders)
			{
				// Add the recorder to list if the platform is supported, and recorder is not null
				if (pair.Platforms.IsSupported() && (pair.Recorder != null))
				{
					capturedRecorders.Add(pair.Recorder);
				}
			}

			// Check recorder size
			if (capturedRecorders.Count == 0)
			{
				// If none is provided, set the recorder to default (playerprefs)
				isNewRecorderCreated = true;
				return ScriptableObject.CreateInstance<AsyncPlayerPrefsSettingsRecorder>();
			}
			else if (capturedRecorders.Count == 1)
			{
				// Set the recorder to the only one retrieved
				return capturedRecorders[0];
			}
			else
			{
				// Composite the recorders into a single list
				return new AsyncCompositeSettingsRecorder(capturedRecorders);
			}
		}

		class SavesSettingsManager : BaseSettingsManager<SavesSettingsManager, SavesSettings>, System.IDisposable
		{
			bool cleanUpRecorder = false;

			/// <inheritdoc/>
			protected override string AddressableName => ADDRESSABLE_NAME;

			internal LoadState SetupState
			{
				get;
				private set;
			} = LoadState.Fail;
			internal IAsyncSettingsRecorder CurrentRecorder
			{
				get;
				private set;
			} = null;
			internal SavesSettings Settings => Data;

			internal IEnumerator ForceSetup()
			{
				if (SetupState != LoadState.Loading)
				{
					// Load the data
					yield return Manager.StartCoroutine(OnSetup());
				}
			}

			/// <inheritdoc/>
			protected override IEnumerator OnSetup()
			{
				// Reset flags
				cleanUpRecorder = false;
				SetupState = LoadState.Loading;
				WaitLoad loadStatus;

				// Load the data
				yield return Manager.StartCoroutine(base.OnSetup());

				// Setup data
				SavesSettings data = GetData();
				data.SaveData.Setup();

				// Setup recorders
				CurrentRecorder = GetSupportedRecorder(data.Recorders, out cleanUpRecorder);

				#region Load the version number
				// Setup and load version saver
				data.Version.Setup(CurrentRecorder);
				loadStatus = data.Version.Load();
				yield return loadStatus;

				// Confirm load succeeded
				if (loadStatus.CurrentState == LoadState.Fail)
				{
					// Indicate failure
					Debug.LogErrorFormat(this, "Unable to retrieve the stored version number from Recorder \"{0}\"", CurrentRecorder);
					SetupState = LoadState.Fail;

					// Clean-up, and don't proceed any further
					Dispose();
					yield break;
				}
				#endregion

				#region Upgrade Data
				// Check if this version number is smaller than expected
				int lastVersion = data.Version.Value,
					currentVersion = data.Upgraders.Length;
				if (lastVersion < currentVersion)
				{
					// Go through all the upgraders
					for (int i = lastVersion; i < currentVersion; ++i)
					{
						// Run the upgrade
						yield return StartCoroutine(data.Upgraders[i].Upgrade(data, CurrentRecorder));

						// Check if upgrade failed
						if (data.Upgraders[i].CurrentState != LoadState.Success)
						{
							// Print an error, and flag that this coroutine failed
							Debug.LogErrorFormat(this, "Unable to run upgrader \"{0}\", to version {1}", data.Upgraders[i], i);
							SetupState = LoadState.Fail;

							// Clean-up, and don't proceed any further
							Dispose();
							yield break;
						}
					}

					// Set the version
					data.Version.Value = currentVersion;
					loadStatus = CurrentRecorder.Save();
					yield return loadStatus;

					// If saving failed for some reason, indicate as such
					if (loadStatus.CurrentState == LoadState.Fail)
					{
						// Print an error, and flag that this coroutine failed
						Debug.LogErrorFormat(this, "Unable to save the version number, \"{0}\".  Proceeding, anyway.", data.Version.Value);
						SetupState = LoadState.Fail;

						// Clean-up, and don't proceed any further
						Dispose();
						yield break;
					}
				}
				#endregion

				#region Load All Data
				// Go through all data
				foreach (SaveObject save in data.SaveData.Values)
				{
					// Of course, disregard nulls
					if (save == null)
					{
						continue;
					}

					// Setup the save object
					save.Setup(CurrentRecorder);
					loadStatus = save.Load();
					yield return loadStatus;

					// Check if load failed
					if (loadStatus.CurrentState == LoadState.Fail)
					{
						// Check how to handle this load failure
						switch (save.HandleLoadFailure)
						{
							case ErrorHandling.HaltLogError:
								// Indicate failure
								Debug.LogErrorFormat(this, "Unable to load \"{0}\"", save);
								SetupState = LoadState.Fail;

								// Clean-up, and don't proceed any further
								Dispose();
								yield break;
							case ErrorHandling.ProceedLogError:
								Debug.LogErrorFormat(this, "Unable to load \"{0}\"", save);
								break;
							case ErrorHandling.ProceedLogWarning:
								Debug.LogWarningFormat(this, "Unable to load \"{0}\"", save);
								break;
							case ErrorHandling.ProceedLogInfo:
								Debug.LogFormat(this, "Unable to load \"{0}\"", save);
								break;
						}
					}
				}
				#endregion

				// Made it to the end, everything passed!
				SetupState = LoadState.Success;
			}

			/// <inheritdoc/>
			protected override void OnDestroy()
			{
				Dispose();
				base.OnDestroy();
			}

			/// <inheritdoc/>
			public void Dispose()
			{
				// Check if recorder needs to be cleaned
				if (cleanUpRecorder)
				{
					// Destroy the recorder
					Destroy((ScriptableObject)CurrentRecorder);

					// Flag as destroyed
					CurrentRecorder = null;
					cleanUpRecorder = false;
				}
			}
		}
	}
}
