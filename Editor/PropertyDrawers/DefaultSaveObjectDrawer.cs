using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.Saves.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="DefaultSaveObjectDrawer.cs" company="Omiya Games">
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
	/// <strong>Date:</strong> 2/19/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial verison.</description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// TODO
	/// </summary>
	[CustomPropertyDrawer(typeof(SaveObject))]
	[CustomPropertyDrawer(typeof(SaveInt))]
	[CustomPropertyDrawer(typeof(SaveFloat))]
	[CustomPropertyDrawer(typeof(SaveBool))]
	[CustomPropertyDrawer(typeof(SaveString))]
	public class DefaultSaveObjectDrawer : PropertyDrawer
	{
		/// <inheritdoc/>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Move this up
			using (var scope = new EditorGUI.PropertyScope(position, GUIContent.none, property))
			{
				// Calculate where to draw the fold-out
				Rect drawPosition = position;
				drawPosition.width = EditorHelpers.IndentSpace;
				drawPosition.height = EditorGUIUtility.singleLineHeight;

				// Draw the fold-out
				property.isExpanded = EditorGUI.Foldout(drawPosition, property.isExpanded, GUIContent.none);

				// Indent everything else
				using (var indentScope = new EditorGUI.IndentLevelScope())
				{
					// Calculate where to draw the object
					drawPosition = position;
					drawPosition.height = EditorGUIUtility.singleLineHeight;

					// Draw the object field
					EditorGUI.ObjectField(drawPosition, property, label);
					if (property.isExpanded)
					{
						// Calculate where to draw status information
						drawPosition = position;
						drawPosition.y += EditorGUIUtility.singleLineHeight;
						drawPosition.y += EditorHelpers.VerticalMargin;
						drawPosition.height = EditorGUIUtility.singleLineHeight;

						// Check if object reference is null
						GUIContent labelContent = new();
						if (property.objectReferenceValue == null)
						{
							// FIXME: Indicate there's nothing to do here.
							labelContent.text = "(No actions to display)";
							labelContent.tooltip = "As the field is null, cannot display any actions";
							EditorGUI.LabelField(drawPosition, labelContent);
						}
						else
						{
							// FIXME: check if saveObject is already in the list
							labelContent.text = "(No actions to display)";
							labelContent.tooltip = "Not implemented";
							EditorGUI.LabelField(drawPosition, labelContent);

							// FIXME: draw buttons?
						}
					}
				}
			}
		}

		/// <inheritdoc/>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorHelpers.GetHeight(property.isExpanded ? 2 : 1);

		///// <summary>
		///// Draws info box of the provided scene
		///// </summary>
		//void DrawSceneInfoGUI(Rect position, int sceneControlID)
		//{
		//	// Label Prefix
		//	var iconContent = new GUIContent();
		//	var labelContent = new GUIContent();

		//	// Missing from build scenes
		//	if (buildScene.buildIndex == -1)
		//	{
		//		iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_close");
		//		labelContent.text = "NOT In Build";
		//		labelContent.tooltip = "This scene is NOT in build settings.\nIt will be NOT included in builds.";
		//	}
		//	// In build scenes and enabled
		//	else if (buildScene.scene.enabled)
		//	{
		//		iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_max");
		//		labelContent.text = "BuildIndex: " + buildScene.buildIndex;
		//		labelContent.tooltip = "This scene is in build settings and ENABLED.\nIt will be included in builds.";
		//	}
		//	// In build scenes and disabled
		//	else
		//	{
		//		iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_min");
		//		labelContent.text = "BuildIndex: " + buildScene.buildIndex;
		//		labelContent.tooltip = "This scene is in build settings and DISABLED.\nIt will be NOT included in builds.";
		//	}

		//	// Left status label
		//	using (new EditorGUI.DisabledScope(readOnly))
		//	{
		//		var labelRect = DrawUtils.GetLabelRect(position);
		//		var iconRect = labelRect;
		//		iconRect.width = iconContent.image.width + PAD_SIZE;
		//		labelRect.width -= iconRect.width;
		//		labelRect.x += iconRect.width;
		//		EditorGUI.PrefixLabel(iconRect, sceneControlID, iconContent);
		//		EditorGUI.PrefixLabel(labelRect, sceneControlID, labelContent);
		//	}

		//	// Right context buttons
		//	var buttonRect = DrawUtils.GetFieldRect(position);
		//	buttonRect.width = (buttonRect.width) / 3;

		//	var tooltipMsg = "";
		//	using (new EditorGUI.DisabledScope(readOnly))
		//	{
		//		// NOT in build settings
		//		if (buildScene.buildIndex == -1)
		//		{
		//			buttonRect.width *= 2;
		//			var addIndex = EditorBuildSettings.scenes.Length;
		//			tooltipMsg = "Add this scene to build settings. It will be appended to the end of the build scenes as buildIndex: " + addIndex + "." + readOnlyWarning;
		//			if (DrawUtils.ButtonHelper(buttonRect, "Add...", "Add (buildIndex " + addIndex + ")", EditorStyles.miniButtonLeft, tooltipMsg))
		//				BuildUtils.AddBuildScene(buildScene);
		//			buttonRect.width /= 2;
		//			buttonRect.x += buttonRect.width;
		//		}
		//		// In build settings
		//		else
		//		{
		//			var isEnabled = buildScene.scene.enabled;
		//			var stateString = isEnabled ? "Disable" : "Enable";
		//			tooltipMsg = stateString + " this scene in build settings.\n" + (isEnabled ? "It will no longer be included in builds" : "It will be included in builds") + "." + readOnlyWarning;

		//			// FIXME: show buttons
		//			//if (DrawUtils.ButtonHelper(buttonRect, stateString, stateString + " In Build", EditorStyles.miniButtonLeft, tooltipMsg))
		//			//	BuildUtils.SetBuildSceneState(buildScene, !isEnabled);
		//			buttonRect.x += buttonRect.width;

		//			tooltipMsg = "Completely remove this scene from build settings.\nYou will need to add it again for it to be included in builds!" + readOnlyWarning;
		//			if (DrawUtils.ButtonHelper(buttonRect, "Remove...", "Remove from Build", EditorStyles.miniButtonMid, tooltipMsg))
		//				BuildUtils.RemoveBuildScene(buildScene);
		//		}
		//	}

		//	buttonRect.x += buttonRect.width;

		//	tooltipMsg = "Open the 'Build Settings' Window for managing scenes." + readOnlyWarning;
		//	if (DrawUtils.ButtonHelper(buttonRect, "Settings", "Build Settings", EditorStyles.miniButtonRight, tooltipMsg))
		//	{
		//		BuildUtils.OpenBuildSettings();
		//	}
		//}

		//private static class DrawUtils
		//{
		//	/// <summary>
		//	/// Draw a GUI button, choosing between a short and a long button text based on if it fits
		//	/// </summary>
		//	public static bool ButtonHelper(Rect position, string msgShort, string msgLong, GUIStyle style, string tooltip = null)
		//	{
		//		var content = new GUIContent(msgLong) { tooltip = tooltip };

		//		var longWidth = style.CalcSize(content).x;
		//		if (longWidth > position.width) content.text = msgShort;

		//		return GUI.Button(position, content, style);
		//	}

		//	/// <summary>
		//	/// Given a position rect, get its field portion
		//	/// </summary>
		//	public static Rect GetFieldRect(Rect position)
		//	{
		//		position.width -= EditorGUIUtility.labelWidth;
		//		position.x += EditorGUIUtility.labelWidth;
		//		return position;
		//	}
		//	/// <summary>
		//	/// Given a position rect, get its label portion
		//	/// </summary>
		//	public static Rect GetLabelRect(Rect position)
		//	{
		//		position.width = EditorGUIUtility.labelWidth - PAD_SIZE;
		//		return position;
		//	}
		//}
	}
}
