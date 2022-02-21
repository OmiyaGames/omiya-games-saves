using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace OmiyaGames.Saves.Editor
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="SaveComparableEditor.cs" company="Omiya Games">
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
	/// Abstract editor for <seealso cref="SaveComparableValue{TValue, TValue}"/>.
	/// </summary>
	public abstract class SaveComparableEditor<TValue, TSave, TTextField, TSliderField> : BaseSaveObjectEditor
		where TValue : System.IComparable<TValue>
		where TSave : SaveComparableValue<TValue, TValue>
		where TTextField : TextValueField<TValue>
		where TSliderField : BaseSlider<TValue>
	{
		bool hasMin, hasMax;
		TValue min, max;
		
		TTextField valueInput;

		VisualElement sliderGroup;
		TSliderField valueSlider;
		TTextField valueSliderInput;

		TTextField minInput;
		TTextField maxInput;

		/// <summary>
		/// The path to the UXML file with all the content setup
		/// </summary>
		protected abstract string UxmlPath
		{
			get;
		}

		/// <inheritdoc/>
		protected override void FillContent(VisualElement content)
		{
			AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath).CloneTree(content);
			SetupErrorHandlingDropDown(content, "onLoadFailed", "onLoadFailed");

			// Update member variables
			TSave saveInt = (TSave)target;
			hasMin = saveInt.HasMin;
			hasMax = saveInt.HasMax;
			max = saveInt.MaxValue;
			min = saveInt.MinValue;

			valueInput = content.Q<TTextField>("defaultValueInput");
			sliderGroup = content.Q<VisualElement>("sliderGroup");
			valueSlider = content.Q<TSliderField>("defaultValueSlider");
			valueSliderInput = content.Q<TTextField>("defaultValueSliderInput");
			minInput = content.Q<TTextField>("minValue");
			maxInput = content.Q<TTextField>("maxValue");

			// Update the UI
			UpdateUi();

			// Register to min input
			minInput.RegisterCallback<ChangeEvent<TValue>>(e =>
			{
				min = e.newValue;
				UpdateUi();
			});

			// Register to max input
			maxInput.RegisterCallback<ChangeEvent<TValue>>(e =>
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

			// Register to input
			valueInput.RegisterCallback<ChangeEvent<TValue>>(e =>
			{
				// Perform clamping (doing comparison to avoid recursion)
				TValue compare = Clamp(e.newValue);
				if (compare.CompareTo(e.newValue) != 0)
				{
					valueInput.value = compare;
				}
			});

			valueSliderInput.RegisterCallback<ChangeEvent<TValue>>(e =>
			{
				// Perform clamping (doing comparison to avoid recursion)
				TValue compare = Clamp(e.newValue);
				if (compare.CompareTo(e.newValue) != 0)
				{
					valueSliderInput.value = compare;
				}
			});
		}

		void UpdateUi()
		{
			// Update min/max enable state
			minInput.SetEnabled(hasMin);
			maxInput.SetEnabled(hasMax);

			// Check if clamp variables are properly setup
			IValueField<TValue> clampField = valueSliderInput;
			if ((hasMin && hasMax) && (min.CompareTo(max) < 0))
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

				// Change the control to clamp
				clampField = valueInput;
			}

			// Perform clamping (doing comparison to avoid event triggering)
			TValue compare = Clamp(clampField.value);
			if (compare.CompareTo(clampField.value) != 0)
			{
				clampField.value = compare;
			}
		}

		TValue Clamp(TValue compare)
		{
			if (hasMin != hasMax)
			{
				if (hasMin && (min.CompareTo(valueInput.value) > 0))
				{
					return min;
				}
				else if (hasMax && (max.CompareTo(valueInput.value) < 0))
				{
					return max;
				}
			}
			return compare;
		}
	}
}
