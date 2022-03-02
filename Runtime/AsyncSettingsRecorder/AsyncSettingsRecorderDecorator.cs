using System;
using System.Globalization;

namespace OmiyaGames.Saves
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="AsyncSettingsRecorderDecorator.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/16/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial draft.</description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// A decorator that implements a couple of <code>AsyncSettingsRecorder</code> methods by wrapping a couple of methods with other methods.
	/// Extending this class should reduce the amount of work necessary to implement an <code>AsyncSettingsRecorder</code>.
	/// </summary>
	/// <seealso cref="AsyncSettingsRecorder"/>
	public abstract class AsyncSettingsRecorderDecorator : AsyncSettingsRecorder
	{
		/// <summary>
		/// Gets a <code>bool</code> from stored settings.
		/// This method is actually a wrapper of <code>GetInt(string, int)</code>.
		/// </summary>
		/// <seealso cref="GetInt(string, int)"/>
		public override WaitLoadValue<bool> GetBool(string key, bool defaultValue)
		{
			WaitLoadValue<int> source = GetInt(key, WaitLoadBool.ToInt(defaultValue));
			return new WaitLoadBool(source);
		}

		/// <summary>
		/// Sets an <code>bool</code> in the stored settings.
		/// This method is actually a wrapper of <code>SetInt(string, int)</code>.
		/// </summary>
		/// <seealso cref="SetInt(string, int)"/>
		public override void SetBool(string key, bool value)
		{
			SetInt(key, WaitLoadBool.ToInt(value));
		}

		/// <summary>
		/// Gets an <code>enum</code> from stored settings.
		/// This method is actually a wrapper of <code>GetInt(string, int)</code>.
		/// </summary>
		/// <seealso cref="GetInt(string, int)"/>
		public override WaitLoadValue<TEnum> GetEnum<TEnum>(string key, TEnum defaultValue)
		{
			WaitLoadValue<int> source = GetInt(key, WaitLoadEnum<TEnum>.ToInt(defaultValue));
			return new WaitLoadEnum<TEnum>(source);
		}

		/// <summary>
		/// Sets an <code>enum</code> in the stored settings.
		/// This method is actually a wrapper of <code>SetInt(string, int)</code>.
		/// </summary>
		/// <seealso cref="SetInt(string, int)"/>
		public override void SetEnum<TEnum>(string key, TEnum value)
		{
			if (typeof(TEnum).IsEnum == false)
			{
				throw new NotSupportedException("Generic type must be an enum");
			}
			SetInt(key, WaitLoadEnum<TEnum>.ToInt(value));
		}

		/// <summary>
		/// Gets a <code>DateTime</code> (in UTC) from stored settings.
		/// This method is actually a wrapper of <code>GetString(string, string)</code>.
		/// </summary>
		/// <seealso cref="GetString(string, string)"/>
		public override WaitLoadValue<DateTime> GetDateTimeUtc(string key, DateTime defaultValue)
		{
			WaitLoadValue<string> source = GetString(key, WaitLoadDateTime.ToString(defaultValue));
			return new WaitLoadDateTime(source);
		}

		/// <summary>
		/// Sets a <code>DateTime</code> in the stored settings.
		/// Make sure the value is in UTC!
		/// This method is actually a wrapper of <code>SetString(string, string)</code>.
		/// </summary>
		/// <seealso cref="SetString(string, string)"/>
		public override void SetDateTimeUtc(string key, DateTime value)
		{
			SetString(key, WaitLoadDateTime.ToString(value));
		}

		/// <summary>
		/// Gets a <code>TimeSpan</code> from stored settings.
		/// This method is actually a wrapper of <code>GetString(string, string)</code>.
		/// </summary>
		/// <seealso cref="GetString(string, string)"/>
		public override WaitLoadValue<TimeSpan> GetTimeSpan(string key, TimeSpan defaultValue)
		{
			var source = GetString(key, WaitLoadTimeSpan.ToString(defaultValue));
			return new WaitLoadTimeSpan(source);
		}

		/// <summary>
		/// Sets a <code>TimeSpan</code> in the stored settings.
		/// This method is actually a wrapper of <code>SetString(string, string)</code>.
		/// </summary>
		/// <seealso cref="SetString(string, string)"/>
		public override void SetTimeSpan(string key, TimeSpan value)
		{
			SetString(key, WaitLoadTimeSpan.ToString(value));
		}
	}

	/// <summary>
	/// Wraps <c>WaitLoadValue&lt;int&gt;</c>, converting
	/// <see cref="Result"/> to a boolean.
	/// </summary>
	public class WaitLoadBool : WaitLoadConvertValue<bool, int>
	{
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static int ToInt(bool flag)
		{
			return (flag ? 1 : 0);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool ToBool(int value)
		{
			if (value != 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <inheritdoc/>
		public WaitLoadBool(WaitLoadValue<int> source) : base(source) { }

		/// <inheritdoc/>
		public override bool Convert(int oldValue) => ToBool(oldValue);
	}

	/// <summary>
	/// Wraps <c>WaitLoadValue&lt;int&gt;</c>, converting
	/// <see cref="Result"/> to an enum.
	/// </summary>
	public class WaitLoadEnum<T> : WaitLoadConvertValue<T, int> where T : Enum
	{
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int ToInt(IConvertible value)
		{
			return value.ToInt32(CultureInfo.InvariantCulture.NumberFormat);
		}

		/// <inheritdoc/>
		public WaitLoadEnum(WaitLoadValue<int> source) : base(source) { }

		/// <inheritdoc/>
		public override T Convert(int oldValue) => (T)(object)oldValue;
	}

	/// <summary>
	/// Wraps <c>WaitLoadValue&lt;string&gt;</c>, converting
	/// <see cref="Result"/> to a <see cref="DateTime"/>.
	/// </summary>
	public class WaitLoadDateTime : WaitLoadConvertValue<DateTime, string>
	{
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="timeUtc"></param>
		/// <returns></returns>
		public static string ToString(DateTime timeUtc)
		{
			return timeUtc.Ticks.ToString();
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static DateTime ToDateTimeUtc(string value)
		{
			long ticks;
			DateTime time = DateTime.MinValue;
			if (long.TryParse(value, out ticks) == true)
			{
				time = new DateTime(ticks, DateTimeKind.Utc);
			}
			return time;
		}

		/// <inheritdoc/>
		public WaitLoadDateTime(WaitLoadValue<string> source) : base(source) { }

		/// <inheritdoc/>
		public override DateTime Convert(string oldValue) => ToDateTimeUtc(oldValue);
	}

	/// <summary>
	/// Wraps <c>WaitLoadValue&lt;string&gt;</c>, converting
	/// <see cref="Result"/> to a <see cref="TimeSpan"/>.
	/// </summary>
	public class WaitLoadTimeSpan : WaitLoadConvertValue<TimeSpan, string>
	{
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="duration"></param>
		/// <returns></returns>
		public static string ToString(TimeSpan duration)
		{
			return duration.Ticks.ToString();
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TimeSpan ToTimeSpan(string value)
		{
			long ticks;
			TimeSpan span = TimeSpan.Zero;
			if (long.TryParse(value, out ticks) == true)
			{
				span = new TimeSpan(ticks);
			}
			return span;
		}

		/// <inheritdoc/>
		public WaitLoadTimeSpan(WaitLoadValue<string> source) : base(source) { }

		/// <inheritdoc/>
		public override TimeSpan Convert(string oldValue) => ToTimeSpan(oldValue);
	}

	/// <summary>
	/// Wrapper class that converts a <see cref="WaitLoadValue{T}.Result"/>
	/// from <typeparamref name="TOld"/> to <typeparamref name="TNew"/>.
	/// </summary>
	/// <typeparam name="TNew">
	/// The final type.
	/// </typeparam>
	/// <typeparam name="TOld">
	/// The original type.
	/// </typeparam>
	/// <remarks>
	/// <list type="table">
	/// <listheader>
	/// <term>Revision</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>
	/// <strong>Version:</strong> 0.2.0-exp.1<br/>
	/// <strong>Date:</strong> 2/16/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial draft.</description>
	/// </item><item>
	/// <term>
	/// <strong>Version:</strong> 0.2.2-exp.1<br/>
	/// <strong>Date:</strong> 3/2/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Changed <see cref="OnLoadingFinished"/> to convert the <c>args</c> parameter
	/// from <see cref="LoadValueFinishedEventArgs{TOld}"/> to
	/// <see cref="LoadValueFinishedEventArgs{TNew}"/>.  Added abstract method,
	/// <see cref="Convert(TOld)"/>.  Made <see cref="Result"/> <c>sealed</c>.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	public abstract class WaitLoadConvertValue<TNew, TOld> : WaitLoadValue<TNew>, IDisposable
	{
		LoadingFinished finishAction;
		protected WaitLoadValue<TOld> Parent
		{
			get;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="source"></param>
		public WaitLoadConvertValue(WaitLoadValue<TOld> source)
		{
			Parent = source;
			finishAction = new LoadingFinished((source, args) =>
			{
				// Check if listening to any events
				if (OnLoadingFinished != null)
				{
					// Convert the old args to new
					LoadValueFinishedEventArgs<TNew> newArgs;
					if (args.State == LoadState.Success)
					{
						var oldArgs = (LoadValueFinishedEventArgs<TOld>)args;
						newArgs = new(Convert(oldArgs.Result));
					}
					else
					{
						newArgs = new();
					}

					// Call new event
					OnLoadingFinished(this, newArgs);
				}
			});
			Parent.OnLoadingFinished += finishAction;
		}

		~WaitLoadConvertValue()
		{
			Dispose();
		}

		/// <inheritdoc/>
		public override event LoadingFinished OnLoadingFinished;
		/// <inheritdoc/>
		public override bool keepWaiting => Parent.keepWaiting;
		/// <inheritdoc/>
		public override sealed TNew Result => Convert(Parent.Result);

		/// <summary>
		/// Convert value from one to other.
		/// </summary>
		/// <param name="oldValue"></param>
		/// <returns></returns>
		public abstract TNew Convert(TOld oldValue);

		/// <inheritdoc/>
		public void Dispose()
		{
			if (finishAction != null)
			{
				Parent.OnLoadingFinished -= finishAction;
				finishAction = null;
			}
		}
	}
}
