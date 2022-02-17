using System;

namespace OmiyaGames.Saves
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SettingsRecorderDecorator.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
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
    /// <author>Taro Omiya</author>
    /// <date>5/16/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A decorator that implements a couple of <code>ISettingsRecorder</code> methods by wrapping a couple of methods with other methods.
    /// Extending this class should reduce the amount of work necessary to implement an <code>ISettingsRecorder</code>.
    /// </summary>
    /// <seealso cref="ISettingsRecorder"/>
    /// <seealso cref="GameSettings"/>
    [Obsolete("Use AsyncSettingsRecorderDecorator instead")]
    public abstract class SettingsRecorderDecorator : ISettingsRecorder
    {
        public abstract int GetInt(string key, int defaultValue);
        public abstract void SetInt(string key, int value);

        public abstract float GetFloat(string key, float defaultValue);
        public abstract void SetFloat(string key, float value);

        public abstract string GetString(string key, string defaultValue);
        public abstract void SetString(string key, string value);

        public abstract void DeleteKey(string key);
        public abstract void DeleteAll();

        public abstract void Save();
        public abstract bool HasKey(string key);

        #region Bool Settings
        /// <summary>
        /// Gets a <code>bool</code> from stored settings.
        /// This method is actually a wrapper of <code>GetInt(string, int)</code>.
        /// </summary>
        /// <seealso cref="GetInt(string, int)"/>
        public virtual bool GetBool(string key, bool defaultValue)
        {
            return WaitLoadBool.ToBool(GetInt(key, WaitLoadBool.ToInt(defaultValue)));
        }

        /// <summary>
        /// Sets an <code>bool</code> in the stored settings.
        /// This method is actually a wrapper of <code>SetInt(string, int)</code>.
        /// </summary>
        /// <seealso cref="SetInt(string, int)"/>
        public virtual void SetBool(string key, bool value)
        {
            SetInt(key, WaitLoadBool.ToInt(value));
        }
        #endregion

        #region Enum Settings
        /// <summary>
        /// Gets an <code>enum</code> from stored settings.
        /// This method is actually a wrapper of <code>GetInt(string, int)</code>.
        /// </summary>
        /// <seealso cref="GetInt(string, int)"/>
        public virtual ENUM GetEnum<ENUM>(string key, ENUM defaultValue) where ENUM : struct, IConvertible
        {
            if (typeof(ENUM).IsEnum == false)
            {
                throw new NotSupportedException("Generic type must be an enum");
            }
            return (ENUM)(object)GetInt(key, WaitLoadEnum<ENUM>.ToInt(defaultValue));
        }

        /// <summary>
        /// Sets an <code>enum</code> in the stored settings.
        /// This method is actually a wrapper of <code>SetInt(string, int)</code>.
        /// </summary>
        /// <seealso cref="SetInt(string, int)"/>
        public virtual void SetEnum<ENUM>(string key, ENUM value) where ENUM : struct, IConvertible
        {
            if (typeof(ENUM).IsEnum == false)
            {
                throw new NotSupportedException("Generic type must be an enum");
            }
            SetInt(key, WaitLoadEnum<ENUM>.ToInt(value));
        }
        #endregion

        #region DateTime Settings
        /// <summary>
        /// Gets a <code>DateTime</code> (in UTC) from stored settings.
        /// This method is actually a wrapper of <code>GetString(string, string)</code>.
        /// </summary>
        /// <seealso cref="GetString(string, string)"/>
        public virtual DateTime GetDateTimeUtc(string key, DateTime defaultValue)
        {
            return WaitLoadDateTime.ToDateTimeUtc(GetString(key, WaitLoadDateTime.ToString(defaultValue)));
        }

        /// <summary>
        /// Sets a <code>DateTime</code> in the stored settings.
        /// Make sure the value is in UTC!
        /// This method is actually a wrapper of <code>SetString(string, string)</code>.
        /// </summary>
        /// <seealso cref="SetString(string, string)"/>
        public virtual void SetDateTimeUtc(string key, DateTime value)
        {
            SetString(key, WaitLoadDateTime.ToString(value));
        }
        #endregion

        #region TimeSpan Settings
        /// <summary>
        /// Gets a <code>TimeSpan</code> from stored settings.
        /// This method is actually a wrapper of <code>GetString(string, string)</code>.
        /// </summary>
        /// <seealso cref="GetString(string, string)"/>
        public virtual TimeSpan GetTimeSpan(string key, TimeSpan defaultValue)
        {
            return WaitLoadTimeSpan.ToTimeSpan(GetString(key, WaitLoadTimeSpan.ToString(defaultValue)));
        }

        /// <summary>
        /// Sets a <code>TimeSpan</code> in the stored settings.
        /// This method is actually a wrapper of <code>SetString(string, string)</code>.
        /// </summary>
        /// <seealso cref="SetString(string, string)"/>
        public virtual void SetTimeSpan(string key, TimeSpan value)
        {
            SetString(key, WaitLoadTimeSpan.ToString(value));
        }
        #endregion
    }
}
