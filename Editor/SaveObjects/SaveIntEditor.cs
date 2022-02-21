using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace OmiyaGames.Saves.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SaveIntEditor.cs" company="Omiya Games">
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
	/// <strong>Version:</strong> 0.2.0-exp<br/>
	/// <strong>Date:</strong> 2/21/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Initial draft.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Editor for <seealso cref="SaveInt"/>.
	/// </summary>
	[CustomEditor(typeof(SaveInt))]
	public class SaveIntEditor : SaveObjectEditor
	{
		const string UXML_PATH = UXML_DIRECTORY + "SaveIntContent.uxml";

		bool hasMin, hasMax;
		int min, max;
		IntegerField valueInput;
		VisualElement sliderGroup;
		SliderInt valueSlider;
		IntegerField minInput;
		IntegerField maxInput;

		/// <inheritdoc/>
		protected override void FillContent(VisualElement content)
		{
			AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH).CloneTree(content);
			SetupErrorHandlingDropDown(content, "onLoadFailed", "onLoadFailed");

			// Update member variables
			SaveInt saveInt = (SaveInt)target;
			hasMin = saveInt.HasMin;
			hasMax = saveInt.HasMax;
			max = saveInt.MaxValue;
			min = saveInt.MinValue;

			valueInput = content.Q<IntegerField>("defaultValueInput");
			sliderGroup = content.Q<VisualElement>("sliderGroup");
			valueSlider = content.Q<SliderInt>("defaultValueSlider");
			minInput = content.Q<IntegerField>("minValue");
			maxInput = content.Q<IntegerField>("maxValue");

			// Update the UI
			UpdateUi();

			// Register to min input
			minInput.RegisterCallback<ChangeEvent<int>>(e =>
			{
				min = e.newValue;
				UpdateUi();
			});

			// Register to max input
			maxInput.RegisterCallback<ChangeEvent<int>>(e =>
			{
				max = e.newValue;
				UpdateUi();
			});

			// Register to min toggle
			Toggle toggle = content.Q<Toggle>("hasMin");
			toggle.RegisterCallback<ChangeEvent<bool>>(e =>
			{
				hasMin = e.newValue;
				UpdateUi();
			});

			// Register to max toggle
			toggle = content.Q<Toggle>("hasMax");
			toggle.RegisterCallback<ChangeEvent<bool>>(e =>
			{
				hasMax = e.newValue;
				UpdateUi();
			});
		}

		void UpdateUi()
		{
			// Update min/max enable state
			minInput.SetEnabled(hasMin);
			maxInput.SetEnabled(hasMax);

			// Check if clamp variables are properly setup
			if ((hasMin && hasMax) && (min < max))
			{
				// Toggle slider as visible
				sliderGroup.style.display = DisplayStyle.Flex;
				valueInput.style.display = DisplayStyle.None;

				// Setup slider bounds
				valueSlider.lowValue = min;
				valueSlider.highValue = max;
			}
			else
			{
				// Toggle input as visible
				valueInput.style.display = DisplayStyle.Flex;
				sliderGroup.style.display = DisplayStyle.None;
			}
		}
	}
}
