using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections.Generic;
using Lean.Common;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lean.Touch
{
	/// <summary>This component fires events if a finger has been held for a certain amount of time without moving.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanFingerHeld")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Finger Held")]
	public class LeanFingerHeld : MonoBehaviour
	{
		[System.Serializable]
		public class FingerData : LeanFingerData
		{
			public bool    LastSet; // Was this finger held?
			public Vector2 TotalScaledDelta; // The total movement so we can ignore it if it gets too high
		}

		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}

		/// <summary>Ignore fingers with StartedOverGui?</summary>
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui = true;

		/// <summary>Ignore fingers with IsOverGui?</summary>
		[Tooltip("Ignore fingers with IsOverGui?")]
		public bool IgnoreIsOverGui;

		/// <summary>Do nothing if this LeanSelectable isn't selected?</summary>
		[Tooltip("Do nothing if this LeanSelectable isn't selected?")]
		public LeanSelectable RequiredSelectable;

		/// <summary>The finger must be held for this many seconds.</summary>
		[Tooltip("The finger must be held for this many seconds.")]
		public float MinimumAge = 1.0f;

		/// <summary>The finger cannot move more than this many pixels relative to the reference DPI.</summary>
		[Tooltip("The finger cannot move more than this many pixels relative to the reference DPI.")]
		public float MaximumMovement = 5.0f;

		/// <summary>Called on the first frame the conditions are met.</summary>
		public LeanFingerEvent OnFingerDown { get { if (onFingerDown == null) onFingerDown = new LeanFingerEvent(); return onFingerDown; } } [FormerlySerializedAs("onHeldDown")] [FormerlySerializedAs("OnHeldDown")] [SerializeField] private LeanFingerEvent onFingerDown;

		/// <summary>Called on every frame the conditions are met.</summary>
		public LeanFingerEvent OnFingerSet { get { if (onFingerSet == null) onFingerSet = new LeanFingerEvent(); return onFingerSet; } } [FormerlySerializedAs("onHeldSet")] [FormerlySerializedAs("OnHeldSet")] [SerializeField] private LeanFingerEvent onFingerSet;

		/// <summary>Called on the last frame the conditions are met.</summary>
		public LeanFingerEvent OnFingerUp { get { if (onFingerUp == null) onFingerUp = new LeanFingerEvent(); return onFingerUp; } } [FormerlySerializedAs("onHeldUp")] [FormerlySerializedAs("OnHeldUp")] [SerializeField] private LeanFingerEvent onFingerUp;

		/// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = Start point based on the ScreenDepth settings.</summary>
		public Vector3Event OnPositionDown { get { if (onPositionDown == null) onPositionDown = new Vector3Event(); return onPositionDown; } } [SerializeField] private Vector3Event onPositionDown;

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = Current point based on the ScreenDepth settings.</summary>
		public Vector3Event OnPositionSet { get { if (onPositionSet == null) onPositionSet = new Vector3Event(); return onPositionSet; } } [SerializeField] private Vector3Event onPositionSet;

		/// <summary>Called on the first frame the conditions are met.
		/// Vector3 = End point based on the ScreenDepth settings.</summary>
		public Vector3Event OnPositionUp { get { if (onPositionUp == null) onPositionUp = new Vector3Event(); return onPositionUp; } } [SerializeField] private Vector3Event onPositionUp;

		// Additional finger data
		[HideInInspector]
		[SerializeField]
		private List<FingerData> fingerDatas = new List<FingerData>();
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
			LeanTouch.OnFingerDown += HandleFingerDown;
			LeanTouch.OnFingerSet  += HandleFingerSet;
			LeanTouch.OnFingerUp   += HandleFingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown -= HandleFingerDown;
			LeanTouch.OnFingerSet  -= HandleFingerSet;
			LeanTouch.OnFingerUp   -= HandleFingerUp;
		}

		private void HandleFingerDown(LeanFinger finger)
		{
			if (IgnoreStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}
			if (IgnoreIsOverGui == true && finger.IsOverGui == true)
			{
				return;
			}

			if (RequiredSelectable != null && RequiredSelectable.IsSelected == false)
			{
				return;
			}

			// Get link for this finger and reset
			var fingerData = LeanFingerData.FindOrCreate(ref fingerDatas, finger);

			fingerData.LastSet          = false;
			fingerData.TotalScaledDelta = Vector2.zero;
		}

		private void HandleFingerSet(LeanFinger finger)
		{
			// Try and find the link for this finger
			var fingerData = LeanFingerData.Find(fingerDatas, finger);

			if (fingerData != null)
			{
				// Has this finger been held for more than MinimumAge without moving more than MaximumMovement?
				var set = finger.Age >= MinimumAge && fingerData.TotalScaledDelta.magnitude < MaximumMovement;

				fingerData.TotalScaledDelta += finger.ScaledDelta;

				if (set == true && fingerData.LastSet == false)
				{
					if (onFingerDown != null)
					{
						onFingerDown.Invoke(finger);
					}

					if (onPositionDown != null)
					{
						var position = ScreenDepth.Convert(finger.ScreenPosition, gameObject);

						onPositionDown.Invoke(position);
					}
				}

				if (set == true)
				{
					if (onFingerSet != null)
					{
						onFingerSet.Invoke(finger);
					}

					if (onPositionSet != null)
					{
						var position = ScreenDepth.Convert(finger.ScreenPosition, gameObject);

						onPositionSet.Invoke(position);
					}
				}

				if (set == false && fingerData.LastSet == true)
				{
					if (onFingerUp != null)
					{
						onFingerUp.Invoke(finger);
					}

					if (onPositionUp != null)
					{
						var position = ScreenDepth.Convert(finger.ScreenPosition, gameObject);

						onPositionUp.Invoke(position);
					}
				}

				// Store last value
				fingerData.LastSet = set;
			}
		}

		private void HandleFingerUp(LeanFinger finger)
		{
			// Find link for this finger, and clear it
			var fingerData = LeanFingerData.Find(fingerDatas, finger);

			if (fingerData != null)
			{
				fingerDatas.Remove(fingerData);

				if (fingerData.LastSet == true)
				{
					if (onFingerUp != null)
					{
						onFingerUp.Invoke(finger);
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanFingerHeld))]
	public class LeanFingerHeld_Inspector : LeanInspector<LeanFingerHeld>
	{
		private bool showUnusedEvents;

		protected override void DrawInspector()
		{
			Draw("IgnoreStartedOverGui");
			Draw("IgnoreIsOverGui");
			Draw("RequiredSelectable");
			Draw("MinimumAge");
			Draw("MaximumMovement");

			EditorGUILayout.Separator();

			var usedA = Any(t => t.OnFingerDown.GetPersistentEventCount() > 0);
			var usedB = Any(t => t.OnFingerSet.GetPersistentEventCount() > 0);
			var usedC = Any(t => t.OnFingerUp.GetPersistentEventCount() > 0);
			var usedD = Any(t => t.OnPositionDown.GetPersistentEventCount() > 0);
			var usedE = Any(t => t.OnPositionSet.GetPersistentEventCount() > 0);
			var usedF = Any(t => t.OnPositionUp.GetPersistentEventCount() > 0);

			EditorGUI.BeginDisabledGroup(usedA && usedB && usedC && usedD && usedE && usedF);
				showUnusedEvents = EditorGUILayout.Foldout(showUnusedEvents, "Show Unused Events");
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Separator();

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onFingerDown");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onFingerSet");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onFingerUp");
			}

			if (usedD == true || usedE == true || usedF == true || showUnusedEvents == true)
			{
				Draw("ScreenDepth");
			}

			if (usedD == true || showUnusedEvents == true)
			{
				Draw("onPositionDown");
			}

			if (usedE == true || showUnusedEvents == true)
			{
				Draw("onPositionSet");
			}

			if (usedF == true || showUnusedEvents == true)
			{
				Draw("onPositionUp");
			}
		}
	}
}
#endif