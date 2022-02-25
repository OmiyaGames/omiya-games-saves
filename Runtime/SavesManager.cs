using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// A manager file that allows adjusting an <see cref="SavesMixer"/>
	/// from settings.
	/// </summary>
	public class SavesManager : BaseSettingsManager<SavesManager, SavesSettings>, System.IDisposable
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

		LoadState setupState = LoadState.Fail;
		bool cleanUpRecorder = false;
		IAsyncSettingsRecorder currentRecorder = null;
		LoadState lastCoroutineState = LoadState.Loading;

		/// <summary>
		/// Indicates whether the manager is either still
		/// in the middle of setting up, or is already setup.
		/// </summary>
		public static LoadState SetupState => GetInstance().setupState;

		/// <summary>
		/// A coroutine to setup this manager.
		/// </summary>
		/// <returns></returns>
		public static IEnumerator Setup()
		{
			// Check if setup yet
			SavesManager manager = GetInstance();

			if (manager.setupState == LoadState.Fail)
			{
				// Reset flags
				manager.cleanUpRecorder = false;
				manager.setupState = LoadState.Loading;

				// First wait to make sure the manager is ready.
				yield return manager.StartCoroutine(WaitUntilReady());

				// Setup a recorder
				SavesSettings data = GetData();
				manager.currentRecorder = GetSupportedRecorder(data.Recorders, out manager.cleanUpRecorder);

				// Load the version number
				yield return manager.StartCoroutine(SetupSaveObject(manager, data.Version));

				// Confirm load succeeded
				if (manager.lastCoroutineState == LoadState.Fail)
				{
					// Indicate failure
					Debug.LogErrorFormat(manager, "Unable to retrieve the stored version number from Recorder \"{0}\"", manager.currentRecorder);
					manager.setupState = LoadState.Fail;

					// Clean-up, and don't proceed any further
					manager.Dispose();
					yield break;
				}

				// Check if this version number is smaller than expected
				if (data.Version.Value < data.Upgraders.Length)
				{
					// Run the upgraders
					yield return manager.StartCoroutine(RunUpgraders(manager, data));

					// If upgrade failed, break immediately
					if (manager.lastCoroutineState == LoadState.Fail)
					{
						manager.setupState = LoadState.Fail;
						yield break;
					}
				}

				// Setup and load all the save data
				yield return manager.StartCoroutine(LoadAllSaveObjects(manager, data.SaveData));

				// "return" the state of the loading all save objects
				manager.setupState = manager.lastCoroutineState;
			}
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <param name="saveObject"></param>
		/// <returns></returns>
		public static bool TryGetSave(string key, out SaveObject saveObject)
		{
			// Check if setup yet
			SavesSettings settings = GetData();

			// Setup return stuff
			return settings.SaveData.TryGetValue(key, out saveObject);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="saveObject"></param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static bool Contains(SaveObject saveObject)
		{
			// Null-check argument
			if (saveObject == null)
			{
				throw new System.ArgumentNullException(nameof(saveObject));
			}

			// Check if setup yet
			SavesSettings settings = GetData();

			// Attempt to grab a save object with the same key.
			bool returnFlag = false;
			if (settings.SaveData.TryGetValue(saveObject.Key, out var compare))
			{
				// Verify these are the same objects
				returnFlag = (saveObject == compare);
			}
			return returnFlag;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public static WaitLoad Save() => CheckInstance().currentRecorder.Save();

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public static WaitLoad DeleteAllKeys() => CheckInstance().currentRecorder.DeleteAll();

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static WaitLoad DeleteKey(string key) => CheckInstance().currentRecorder.DeleteKey(key);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static WaitLoadValue<bool> HasKey(string key) => CheckInstance().currentRecorder.HasKey(key);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <param name="action"></param>
		public static void SubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action) =>
			CheckInstance().currentRecorder.SubscribeToDeleteKey(key, action);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="key"></param>
		/// <param name="action"></param>
		public static void UnsubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action) =>
			CheckInstance().currentRecorder.UnsubscribeToDeleteKey(key, action);

		/// <inheritdoc/>
		public void Dispose()
		{
			// Check if recorder needs to be cleaned
			SavesManager manager = GetInstance();
			if (manager.cleanUpRecorder)
			{
				// Destroy the recorder
				Destroy((ScriptableObject)manager.currentRecorder);

				// Flag as destroyed
				manager.currentRecorder = null;
				manager.cleanUpRecorder = false;
			}
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			Dispose();
			base.OnDestroy();
		}

		#region Helper Functions
		/// <summary>
		/// Retrieves the recorder that is supported for this build.
		/// If there are multiple, a composite recorder is generated.
		/// </summary>
		/// <param name="recorders"></param>
		/// <param name="isNewRecorderCreated"></param>
		/// <returns></returns>
		static IAsyncSettingsRecorder GetSupportedRecorder(IReadOnlyList<SavesSettings.SupportedRecorder> recorders, out bool isNewRecorderCreated)
		{
			// Setup variables
			isNewRecorderCreated = false;
			ListSet<AsyncSettingsRecorder> capturedRecorders = new(recorders.Count);

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

		static IEnumerator SetupSaveObject(SavesManager manager, SaveObject save)
		{
			// Set load state to default
			manager.lastCoroutineState = LoadState.Loading;

			// Setup and load
			save.Setup(manager.currentRecorder);
			WaitLoad loadStatus = save.Load();
			yield return loadStatus;

			// Confirm load succeeded
			manager.lastCoroutineState = loadStatus.CurrentState;
		}

		static IEnumerator RunUpgraders(SavesManager manager, SavesSettings data)
		{
			// Set load state to default
			manager.lastCoroutineState = LoadState.Loading;

			// Go through all the upgraders
			for (int i = data.Version.Value; i < data.Upgraders.Length; ++i)
			{
				// Run the upgrade
				yield return manager.StartCoroutine(data.Upgraders[i].Upgrade(data, manager.currentRecorder));

				// Check if upgrade failed
				if (data.Upgraders[i].CurrentState != LoadState.Success)
				{
					// Print an error, and flag that this coroutine failed
					Debug.LogErrorFormat(manager, "Unable to run upgrader \"{0}\", to version {1}", data.Upgraders[i], i);
					manager.lastCoroutineState = LoadState.Fail;

					// Clean-up, and don't proceed any further
					manager.Dispose();
					yield break;
				}
			}

			// Set the version
			data.Version.Value = data.Upgraders.Length;
			WaitLoad loadStatus = manager.currentRecorder.Save();
			yield return loadStatus;

			// If saving failed for some reason, indicate as such
			if (loadStatus.CurrentState == LoadState.Fail)
			{
				Debug.LogWarningFormat(manager, "Unable to save the version number, \"{0}\".  Proceeding, anyway.", data.Version.Value);
			}

			// Indicate this coroutine succeeded
			manager.lastCoroutineState = LoadState.Success;
		}

		static IEnumerator LoadAllSaveObjects(SavesManager manager, SaveObjectMap<SaveObject> allData)
		{
			// Set load state to default
			manager.lastCoroutineState = LoadState.Loading;

			foreach (SaveObject data in allData.Values)
			{
				if (data == null)
				{
					continue;
				}

				// Setup the save object
				yield return manager.StartCoroutine(SetupSaveObject(manager, data));

				// Check if load failed
				if (manager.lastCoroutineState == LoadState.Fail)
				{
					// Briefly revert coroutine state (in case anything is listening to this coroutine)
					manager.lastCoroutineState = LoadState.Loading;

					// Check how to handle this load failure
					switch (data.HandleLoadFailure)
					{
						case ErrorHandling.HaltLogError:
							// Clean-up, and don't proceed any further
							Debug.LogErrorFormat(manager, "Unable to load \"{0}\"", data);
							manager.lastCoroutineState = LoadState.Fail;
							manager.Dispose();
							yield break;
						case ErrorHandling.ProceedLogError:
							Debug.LogErrorFormat(manager, "Unable to load \"{0}\"", data);
							break;
						case ErrorHandling.ProceedLogWarning:
							Debug.LogWarningFormat(manager, "Unable to load \"{0}\"", data);
							break;
						case ErrorHandling.ProceedLogInfo:
							Debug.LogFormat(manager, "Unable to load \"{0}\"", data);
							break;
					}
				}
			}

			// Indicate this coroutine succeeded
			manager.lastCoroutineState = LoadState.Success;
		}

		static SavesManager CheckInstance()
		{
			SavesManager manager = GetInstance();
			if (manager.setupState != LoadState.Success)
			{
				throw new System.Exception("SavesManager is not setup yet.");
			}
			return manager;
		}
		#endregion
	}
}
