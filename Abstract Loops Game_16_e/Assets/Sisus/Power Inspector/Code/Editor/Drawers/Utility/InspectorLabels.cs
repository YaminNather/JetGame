﻿//#define PI_CREATE_ASSET_MENUS

using UnityEngine;
using UnityEngine.Serialization;
using JetBrains.Annotations;

namespace Sisus
{
	/// <summary>
	/// Labels used in an Inspector. Fill the values via a scriptable object asset,
	/// then add a reference to the asset in the InspectorPreferences asset used by
	/// the Inspector.
	/// </summary>
	#if PI_CREATE_ASSET_MENUS
	[CreateAssetMenu]
	#endif
	public class InspectorLabels : ScriptableObject
	{
		[Header("Inspector")]
		[SerializeField]
		private SkinnedLabel splitViewIcon = new SkinnedLabel("", "Split View");
		[SerializeField]
		private SkinnedLabel closeSplitViewIcon = new SkinnedLabel("", "Close View");

		[Header("Toolbar")]
		[SerializeField]
		private GUIContent viewMenu = new GUIContent("View", "Change multi-editing mode, display settings or read documentation on currently shown elements.");
		[SerializeField]
		private GUIContent enableLockView = new GUIContent("", "Lock the inspector view so that its contents won't change even if the selected Object changes.");
		[SerializeField]
		private GUIContent disableLockView = new GUIContent("", "Release the lock from the inspector view so that its contents will update to reflect the currently selected Object.");
		[SerializeField]
		private GUIContent mergedMultiEditing = new GUIContent("", "Multi-Editing Mode: Merged");
		[SerializeField]
		private GUIContent stackedMultiEditing = new GUIContent("", "Multi-Editing Mode: Stacked");

		[Header("Header Button")]
		[SerializeField]
		private GUIContent open = new GUIContent("Open");
		[SerializeField, FormerlySerializedAs("edit")]
		private GUIContent startEditing = new GUIContent("Start Editing");
		[SerializeField]
		private GUIContent stopEdit = new GUIContent("Stop Editing");
		[SerializeField]
		private GUIContent showInExplorer = new GUIContent("", "Show In Explorer");
		[SerializeField]
		private GUIContent formatted = new GUIContent("", "Formatted");
		[SerializeField]
		private GUIContent unformatted = new GUIContent("", "Unformatted");
		[SerializeField]
		internal GUIContent contextMenu = new GUIContent("");

		[Header("Transform Drawer")]
		[SerializeField]
		internal GUIContent position = new GUIContent("Position");
		[SerializeField]
		internal GUIContent rotation = new GUIContent("Rotation");
		[SerializeField]
		internal GUIContent scale = new GUIContent("Scale");
		[SerializeField]
		internal GUIContent x = new GUIContent("X");
		[SerializeField]
		internal GUIContent y = new GUIContent("Y");
		[SerializeField]
		internal GUIContent z = new GUIContent("Z");
		
		[Header("Method Drawer")]
		[SerializeField]
		internal GUIContent invokeMethod = new GUIContent("Invoke");
		[SerializeField]
		internal GUIContent startCoroutine = new GUIContent("Start");

		public static InspectorLabels Current => InspectorUtility.Preferences.labels;


        public SkinnedLabel SplitViewIcon => splitViewIcon;

        public SkinnedLabel CloseSplitViewIcon => closeSplitViewIcon;

        public SkinnedLabel ViewMenu => viewMenu;

        public SkinnedLabel EnableLockView => enableLockView;

        public SkinnedLabel DisableLockView => disableLockView;

        public GUIContent MergedMultiEditing => mergedMultiEditing;

        public GUIContent StackedMultiEditing => stackedMultiEditing;

        public GUIContent Open => GUIContentPool.Create(open);

        public GUIContent StartEditing => GUIContentPool.Create(startEditing);

        public GUIContent StopEditing => GUIContentPool.Create(stopEdit);

        public GUIContent ShowInExplorer => GUIContentPool.Create(showInExplorer);

        public GUIContent Formatted => GUIContentPool.Create(formatted);

        public GUIContent Unformatted => GUIContentPool.Create(unformatted);


        public GUIContent ContextMenu => GUIContentPool.Create(contextMenu);

        public GUIContent Position
		{
			get => GUIContentPool.Create(position);

            internal set => Set(ref position, value, "Position Label: {0}");
        }

		public GUIContent Rotation
		{
			get => GUIContentPool.Create(rotation);

            internal set => Set(ref rotation, value, "Rotation Label: {0}");
        }

		public GUIContent Scale
		{
			get => GUIContentPool.Create(scale);

            internal set => Set(ref scale, value, "Scale Label: {0}");
        }

		public GUIContent X
		{
			get => GUIContentPool.Create(x);

            internal set => Set(ref x, value, "X Label: {0}");
        }

		public GUIContent Y
		{
			get => GUIContentPool.Create(y);

            internal set => Set(ref y, value, "Y Label: {0}");
        }

		public GUIContent Z
		{
			get => GUIContentPool.Create(z);

            internal set => Set(ref z, value, "Z Label: {0}");
        }

		public GUIContent InvokeMethod => GUIContentPool.Create(invokeMethod);

        private void Set<T>(ref T subject, [NotNull]T value, string undoMessage)
		{
			if(!value.Equals(subject))
			{
				UndoHandler.RegisterUndoableAction(this, string.Format(undoMessage, value));

				subject = value;

				Platform.Active.SetDirty(this);
			}
		}

		#if DEV_MODE && PI_ASSERTATIONS
		private static System.Reflection.FieldInfo[] fields;
		public void AssertDoesNotContain([CanBeNull]GUIContent test)
		{
			if(test == null)
			{
				return;
			}

			if(fields == null)
			{
				fields = GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			}

			for(int n = fields.Length - 1; n >= 0; n--)
			{
				Debug.Assert(!ReferenceEquals(fields[n].GetValue(this) as GUIContent, test));
			}
		}
		#endif
	}
}