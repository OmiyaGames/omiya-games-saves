using UnityEngine;
using UnityEditor;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.Saves.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SerializableHashSetDrawer.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/6/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Initial version.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// <see cref="PropertyDrawer"/> for <seealso cref="SerializableHashSet{T}"/>.
	/// </summary>
	[CustomPropertyDrawer(typeof(SavesSettings.SupportedRecorder))]
	public class SupportedRecordersDrawer : PropertyDrawer
	{
		/// <inheritdoc/>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using PropertyScope on the parent property means that
			// prefab override logic works on the entire property.
			using (var scope = new EditorGUI.PropertyScope(position, label, property))
			{
				// Grab the child properties
				SerializedProperty platforms = property.FindPropertyRelative("platforms");
				SerializedProperty recorder = property.FindPropertyRelative("recorder");

				// Split the position between the two properties
				position.width -= EditorHelpers.VerticalMargin;
				position.width /= 2f;

				// Draw the first one
				EditorGUI.PropertyField(position, platforms, GUIContent.none);

				// Draw the second
				position.x += (position.width + EditorHelpers.VerticalMargin);
				EditorGUI.PropertyField(position, recorder, GUIContent.none);
			}
		}

		/// <inheritdoc/>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}
	}
}
