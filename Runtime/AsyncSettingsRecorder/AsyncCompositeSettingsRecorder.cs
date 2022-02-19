using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Saves
{
	public class AsyncCompositeSettingsRecorder : IAsyncSettingsRecorder
	{
		public AsyncCompositeSettingsRecorder(IEnumerable<IAsyncSettingsRecorder> settings)
		{
			// FIXME: implement
			throw new NotImplementedException();
		}

		public WaitLoad DeleteAll()
		{
			throw new NotImplementedException();
		}

		public WaitLoad DeleteKey(string key)
		{
			throw new NotImplementedException();
		}

		public WaitLoadValue<bool> GetBool(string key, bool defaultValue)
		{
			throw new NotImplementedException();
		}

		public WaitLoadValue<DateTime> GetDateTimeUtc(string key, DateTime defaultValue)
		{
			throw new NotImplementedException();
		}

		public WaitLoadValue<TEnum> GetEnum<TEnum>(string key, TEnum defaultValue) where TEnum : struct, IConvertible
		{
			throw new NotImplementedException();
		}

		public WaitLoadValue<float> GetFloat(string key, float defaultValue)
		{
			throw new NotImplementedException();
		}

		public WaitLoadValue<int> GetInt(string key, int defaultValue)
		{
			throw new NotImplementedException();
		}

		public WaitLoadValue<string> GetString(string key, string defaultValue)
		{
			throw new NotImplementedException();
		}

		public WaitLoadValue<TimeSpan> GetTimeSpan(string key, TimeSpan defaultValue)
		{
			throw new NotImplementedException();
		}

		public WaitLoadValue<bool> HasKey(string key)
		{
			throw new NotImplementedException();
		}

		public WaitLoad Save()
		{
			throw new NotImplementedException();
		}

		public void SetBool(string key, bool value)
		{
			throw new NotImplementedException();
		}

		public void SetDateTimeUtc(string key, DateTime value)
		{
			throw new NotImplementedException();
		}

		public void SetEnum<TEnum>(string key, TEnum value) where TEnum : struct, IConvertible
		{
			throw new NotImplementedException();
		}

		public void SetFloat(string key, float value)
		{
			throw new NotImplementedException();
		}

		public void SetInt(string key, int value)
		{
			throw new NotImplementedException();
		}

		public void SetString(string key, string value)
		{
			throw new NotImplementedException();
		}

		public void SetTimeSpan(string key, TimeSpan value)
		{
			throw new NotImplementedException();
		}

		public void SubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action)
		{
			throw new NotImplementedException();
		}

		public void UnsubscribeToDeleteKey(string key, IAsyncSettingsRecorder.OnKeyDeleted action)
		{
			throw new NotImplementedException();
		}
	}
}
