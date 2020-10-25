#if UNITY_EDITOR
using System;
using Sisus.Attributes;

namespace Sisus
{
	[Serializable, DrawerForAsset("UnityEditor.Graphs.AnimationStateMachine.AnyStateNode", false, true)]
	public class AnyStateNodeDrawer : CustomEditorAssetDrawer
	{
		/// <inheritdoc />
		public override PrefixResizer PrefixResizer => PrefixResizer.Disabled;
    }
}
#endif