using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class SavedViewContextMenu : ContextMenu
    {
        SidebarSavedViewItem m_Item;

        public SavedViewContextMenu(SidebarSavedViewItem item)
        {
            m_Item = item;
        }

        public override void SetupContextMenuEntries(ContextualMenuPopulateEvent evt)
        {
            if (evt.target == evt.currentTarget)
            {
                // Delete
                AddMenuEntry(evt, Constants.SidebarDeleteSavedView, true,
                    (_) =>
                    {
                        DeleteSavedView();
                    });
                // Rename
                AddMenuEntry(evt, Constants.SidebarRenameSavedView, true,
                    (_) =>
                    {
                        RenameSavedView();
                    });
            }
        }

        void RenameSavedView()
        {
            m_Item.StartRenaming();
        }

        void DeleteSavedView()
        {
            m_Item.Delete();
        }
    }
}
