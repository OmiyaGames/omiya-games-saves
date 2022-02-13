﻿using System;

namespace OmiyaGames.Saves
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IntRecord.cs" company="Omiya Games">
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
    /// <date>5/28/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Stores int information
    /// </summary>
    /// <seealso cref="GameSettings"/>
    public class IntRecord : IRecord<int>
    {
        public IntRecord(string pastRecord, int appVersion, TryConvertOldRecord converter = null) : base(pastRecord, appVersion, converter)
        { }

        public IntRecord(int newRecord, string newName) : base(newRecord, newName)
        { }

        protected override void ParseRecord(string record, int appVersion, TryConvertOldRecord converter)
        {
            int parsedRecord;
            if ((converter == null) || (converter(record, appVersion, out parsedRecord) == false))
            {
                if (int.TryParse(record, out parsedRecord) == true)
                {
                    Record = parsedRecord;
                }
                else
                {
                    throw new ArgumentException("Could not parse the record from: " + record);
                }
            }
        }

        protected override string ConvertRecordToString()
        {
            return Record.ToString();
        }
    }
}
