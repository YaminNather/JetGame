﻿#if !POWER_INSPECTOR_LITE
using Sisus.Attributes;

namespace Sisus
{
	[ToolbarFor(typeof(PreferencesInspector))]
	public sealed class PreferencesToolbar : InspectorToolbar
	{
		public const float DefaultToolbarHeight = PowerInspectorToolbar.DefaultToolbarHeight;
		
		public readonly float ToolbarHeight = DefaultToolbarHeight;
		
		/// <inheritdoc/>
		public override float Height => ToolbarHeight;

        public PreferencesToolbar() : base()
		{
			ToolbarHeight = DefaultToolbarHeight;
		}

		public PreferencesToolbar(float setHeight) : base()
		{
			ToolbarHeight = setHeight;
		}
	}
}
#endif