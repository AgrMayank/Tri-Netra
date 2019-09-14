using UnityEngine;
using UnityEngine.UI;

namespace Lean.Touch
{
	/// <summary>This component overrides the attached Text component with the specified text when you call the Display method with a value.</summary>
	[RequireComponent(typeof(Text))]
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanInfoText")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Info Text")]
	public class LeanInfoText : MonoBehaviour
	{
		/// <summary>The final text will use this string formatting.</summary>
		[Tooltip("The final text will use this string formatting.")]
		public string Format = "Current Value = {0}";

		[System.NonSerialized]
		private Text cachedText;

		public void Display(string value)
		{
			if (cachedText == null) cachedText = GetComponent<Text>();

			cachedText.text = string.Format(Format, value);
		}

		public void Display(int value)
		{
			Display(value.ToString());
		}

		public void Display(float value)
		{
			Display(value.ToString());
		}

		public void Display(Vector2 value)
		{
			Display(value.ToString());
		}

		public void Display(Vector3 value)
		{
			Display(value.ToString());
		}

		public void Display(Vector4 value)
		{
			Display(value.ToString());
		}
	}
}