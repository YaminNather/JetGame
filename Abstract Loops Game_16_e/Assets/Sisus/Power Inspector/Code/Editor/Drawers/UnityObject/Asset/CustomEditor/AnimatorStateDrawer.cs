#if UNITY_EDITOR
using System;
using UnityEditor.Animations;
using Sisus.Attributes;
using UnityEngine;

namespace Sisus
{
	[Serializable, DrawerForAsset(typeof(AnimatorState), false, true)]
	public class AnimatorStateDrawer : CustomEditorAssetDrawer
	{
		/// <inheritdoc />
		protected override float GetOptimalPrefixLabelWidthForEditor(int indentLevel)
		{
			return 116f;
		}

		/// <inheritdoc />
		public override bool RequiresConstantRepaint => false;

        /// <inheritdoc />
		protected override bool HasDebugModeIcon =>
            // hide the icon because it overlaps with the name field
            false;

        /// <inheritdoc />
		protected override bool HasExecuteMethodIcon =>
            // hide the icon because it overlaps with the name field
            false;

#if UNITY_2018_1_OR_NEWER // Presets were added in Unity 2018.1
		/// <inheritdoc />
		protected override bool HasPresetIcon => true;
#endif

		/// <inheritdoc />
		protected override void GetHeaderSubtitle(ref GUIContent subtitle)
		{
			// hide the subtitle because it overlaps with the tag field
			subtitle.text = "";
			subtitle.tooltip = "";
		}

		/// <inheritdoc />
		public override float MaxPrefixLabelWidth =>
            // UnityEvent drawer can clip off-screen if prefix column width is too large compared to inspector width
            Mathf.Max(99f, DrawGUI.InspectorWidth - 178f);
    }
}
#endif