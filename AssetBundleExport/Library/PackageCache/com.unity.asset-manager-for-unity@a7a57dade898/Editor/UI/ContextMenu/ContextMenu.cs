using System;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    abstract class ContextMenu
    {
        public abstract void SetupContextMenuEntries(ContextualMenuPopulateEvent evt);

        protected static void AddMenuEntry(ContextualMenuPopulateEvent evt, string actionName, bool enabled,
            Action<DropdownMenuAction> action)

        {
            if (evt == null || evt.menu == null)
                return;

            evt.menu.InsertAction(0, actionName, action,
                enabled ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
        }

        protected static void AddMenuEntry(ContextualMenuPopulateEvent evt, string actionName, bool enabled,
            bool toggled, Action<DropdownMenuAction> action)

        {
            if (evt == null || evt.menu == null)
                return;

            var status = DropdownMenuAction.Status.Normal;

            if (!enabled)
            {
                status = DropdownMenuAction.Status.Disabled;
            }
            else if (toggled)
            {
                status = DropdownMenuAction.Status.Checked;
            }

            evt.menu.InsertAction(0, actionName, action, status);
        }
    }
}
