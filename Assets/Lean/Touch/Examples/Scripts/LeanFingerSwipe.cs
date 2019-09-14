using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lean.Touch
{
	/// <summary>This component fires events if a finger has been held for a certain amount of time without moving.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanFingerSwipe")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Finger Swipe")]
	public class LeanFingerSwipe : LeanSwipeBase
	{
		/// <summary>Ignore fingers with StartedOverGui?</summary>
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		/// <summary>Ignore fingers with IsOverGui?</summary>
		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		/// <summary>Do nothing if this LeanSelectable isn't selected?</summary>
		[Tooltip("Do nothing if this LeanSelectable isn't selected?")]
		public LeanSelectable RequiredSelectable;
#if UNITY_EDITOR
		protected virtual void Reset()
		{
			RequiredSelectable = GetComponentInParent<LeanSelectable>();
		}
#endif
		protected virtual void Awake()
		{
			if (RequiredSelectable == null)
			{
				RequiredSelectable = GetComponentInParent<LeanSelectable>();
			}
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerSwipe += HandleFingerSwipe;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerSwipe -= HandleFingerSwipe;
		}

		private void HandleFingerSwipe(LeanFinger finger)
		{
			if (IgnoreStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			if (IgnoreIsOverGui == true && finger.IsOverGui == true)
			{
				return;
			}

			if (RequiredSelectable != null && RequiredSelectable.IsSelectedBy(finger) == false)
			{
				return;
			}

			HandleFingerSwipe(finger, finger.StartScreenPosition, finger.ScreenPosition);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanFingerSwipe))]
	public class LeanFingerSwipe_Inspector : LeanSwipeBase_Inspector<LeanFingerSwipe>
	{
		protected override void DrawInspector()
		{
			Draw("IgnoreStartedOverGui");
			Draw("IgnoreIsOverGui");
			Draw("RequiredSelectable");

			base.DrawInspector();
		}
	}
}
#endif