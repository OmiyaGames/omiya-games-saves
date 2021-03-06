# Change Log:

## 0.2.3-exp.1

- **New Enhancement**: updating [`SavesManager.cs`](/Runtime/SavesManager.cs) to match latest `com.omiyagames.global.settings` standards.

## 0.2.2-exp

- **Bug Fix**: fixed [`AsyncSettingsRecorderDecorator.cs`](/Runtime/AsyncSettingsRecorder/AsyncSettingsRecorderDecorator.cs) so returns of `SetBool(string, bool)`, `SetEnum<TEnum>(string, TEnum)`, `GetDateTimeUtc(string, DateTime)`, `GetTimeSpan(string, TimeSpan)` has the event format expected by [`SaveBool.cs`](/Runtime/SaveObjects/SaveBool.cs), [`SaveEnum.cs`](/Runtime/SaveObjects/SaveEnum.cs), [`SaveDateTime.cs`](/Runtime/SaveObjects/SaveDateTime.cs), and [`SaveTimeSpan.cs`](/Runtime/SaveObjects/SaveTimeSpan.cs).
    - This should also fix [`AsyncPlayerPrefsSettingsRecorder.cs`](/Runtime/AsyncSettingsRecorder/AsyncPlayerPrefsSettingsRecorder.cs).
- **New Enhancement**: changed [`SavesManager.cs`](/Runtime/SavesManager.cs) to a `static class`.  This prevents `SavesManager` from being attached to a `GameObject`.

## 0.2.1-exp

- **Bug Fix**: fixed [`SaveObjectMap.cs`](/Runtime/SaveObjects/SaveObjectMap.cs) not deserializing correctly on the inspector and Project Settings window, thus losing all data.  The fix requires future code to call the new method `Setup()`.
- **Breaking Change**: renamed [`SavesManager.cs`](/Runtime/SavesManager.cs) method `TryGetSave()` to `TryGet()` to remove the implication that any save operation is being performed.

## 0.2.0-exp

- **New Features**: starting on a new, `ScriptableObject` driven framework to save and recall `PlayerPrefs` data.  It's currently a work-in-progress, but we believe is the more intuitive and flexible method for saving and recalling persistent data.
    - Created [`SavesSettingsProvider.cs`](/Editor/SavesSettingsProvider.cs), which can be edited in Project Settings window.  It contains the method save data will be stored, scripts that can upgrade previous data (for adding backwards compatibility,) and what `SaveObjects` to pre-load.
    - Created [`SavesManager.cs`](/Runtime/SavesManager.cs), which handles loading and saving `SaveObjects`.  The singleton is accessible anyhere.
    - Created [`AsyncSettingsRecorder.cs`](/Runtime/AsyncSettingsRecorder/AsyncSettingsRecorder.cs): an abstract `ScriptableObject` provides an interface for writing and recording data asynchronously.
        - Created [`AsyncPlayerPrefsSettingsRecorder.cs`](/Runtime/AsyncSettingsRecorder/AsyncPlayerPrefsSettingsRecorder.cs), an implementation that uses `PlayerPrefs`.
    - Created [`SaveObject.cs`](/Runtime/SaveObjects/SaveObject.cs) `ScriptableObject`.  It's an abstract class for saving and recalling a specific save data.  `SavesManager` configures a `SaveObject` by setting up which `AsyncSettingsRecorder` all objects should use based on platform.
        - Created [`SaveInt.cs`](/Runtime/SaveObjects/SaveInt.cs), an implementation that stores and recalls an integer.
        - Created [`SaveFloat.cs`](/Runtime/SaveObjects/SaveFloat.cs), an implementation that stores and recalls a float.
        - Created [`SaveString.cs`](/Runtime/SaveObjects/SaveString.cs), an implementation that stores and recalls a string.
        - Created [`SaveBool.cs`](/Runtime/SaveObjects/SaveBool.cs), an implementation that stores and recalls a bool.
        - Created [`SaveEnum.cs`](/Runtime/SaveObjects/SaveEnum.cs), an abstract implementation that stores and recalls an enum.
        - Created [`SaveDateTime.cs`](/Runtime/SaveObjects/SaveDateTime.cs), an abstract implementation that stores and recalls a `DateTime` in UTC timezone.
        - Created [`SaveTimeSpan.cs`](/Runtime/SaveObjects/SaveTimeSpan.cs), an abstract implementation that stores and recalls a `TimeSpan`.
    - Created [`SavesUpgrader.cs`](/Runtime/SavesUpgrader/SavesUpgrader.cs) `ScriptableObject`.  It's an abstract class for upgrading previous saves into the latest expectation.  `SavesManager.cs` calls these scripts first before retrieving any `SaveObject.cs` data.

## 0.1.1-exp

- Polishing DocFX setup.

## 0.1.0-exp

- Initial release:
    - Copied source from [Template Unity Project](https://github.com/OmiyaGames/template-unity-project).
