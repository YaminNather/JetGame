using JetBrains.Annotations;
using UnityEditor;

namespace Sisus
{
	/// <summary>
	/// Helper class that binds the opening of the PopupMenuWindow to when PopupMenuManager.Open
	/// is called in Editor mode. This decoupling is necessary because PopupMenuManager can't directly
	/// see PopupMenuWindow which exists in an Editor only assembly.
	/// </summary>
	[InitializeOnLoad]
	internal class PopupMenuWindowAttacher : IPopupMenuAttacher
	{
		public PopupMenuManager.OpenRequest OnRequestingOpen => PopupMenuWindow.Create;

        public PopupMenuManager.SelectItemRequest OnRequestingSelectItem => PopupMenuWindow.SelectItem;

        [UsedImplicitly]
		static PopupMenuWindowAttacher()
		{
			var attacher = new PopupMenuWindowAttacher();
			PopupMenuManager.RegisterPopupMenuDrawer(attacher);
		}
	}
}