using System;

namespace Unity.AssetManager.Core.Editor
{
    class ImportTrigger
    {
        /// <summary>
        /// When the user manually imports an asset that is not already in the project from the Asset panel details tab.
        /// </summary>
        public static readonly ImportTrigger Import = new("Import, Details Tab");
        /// <summary>
        /// When the user manually imports an asset that is already in the project from the Asset panel details tab.
        /// </summary>
        public static readonly ImportTrigger Reimport = new("Reimport, Details Tab");
        /// <summary>
        /// When the user manually imports an asset that is already in the project and the local asset is outdated from the Asset panel details tab.
        /// </summary>
        public static readonly ImportTrigger UpdateToLatest = new("Update to Latest, Details Tab");
        /// <summary>
        /// When the user manually imports an asset by version (from the Asset panel version tab) that is not already in the project.
        /// </summary>
        public static readonly ImportTrigger ImportVersion = new("Import, Version Tab");
        /// <summary>
        /// When the user manually imports an asset by version (from the Asset panel version tab) that is already in the project.
        /// </summary>
        public static readonly ImportTrigger ReimportVersion = new("Reimport, Version Tab");
        /// <summary>
        /// When the user manually imports an asset that is not already in the project from right clicking the context menu.
        /// </summary>
        public static readonly ImportTrigger ImportContextMenu = new("Import, Context Menu");
        /// <summary>
        /// When the user manually imports an asset that is already in the project from right clicking the context menu.
        /// </summary>
        public static readonly ImportTrigger ReimportContextMenu = new("Reimport, Context Menu");
        /// <summary>
        /// When the user manually imports an asset that is already in the project and the local asset is outdated from the Asset panel details tab.
        /// </summary>
        public static readonly ImportTrigger UpdateToLatestContextMenu = new("Update to Latest, Context Menu");
        /// <summary>
        /// When the user multiselects assets and imports them from the multiselect panel.
        /// </summary>
        public static readonly ImportTrigger ImportMultiselect = new("Import All, Multiselect");
        /// <summary>
        /// When the user multiselects assets and reimports them from the multiselect panel.
        /// </summary>
        public static readonly ImportTrigger ReimportMultiselect = new("Reimport All, Multiselect");
        /// <summary>
        /// When the user multiselects assets and imports all from right clicking the context menu.
        /// </summary>
        public static readonly ImportTrigger ImportAllContextMenu = new("Import All, Context Menu");
        /// <summary>
        /// When the user multiselects assets and updates all from right clicking the context menu.
        /// </summary>
        public static readonly ImportTrigger UpdateAllToLatestContextMenu = new("Update All to Latest, Context Menu");
        /// <summary>
        /// When the user selects the Update All button.
        /// </summary>
        public static readonly ImportTrigger UpdateAllToLatest = new("Update All to Latest");
        /// <summary>
        /// When a user drags and drops asset(s) from the grid view to the project browser.
        /// </summary>
        public static readonly ImportTrigger DragAndDrop = new("Import All, Drag and Drop");
        /// <summary>
        /// When the user triggers an automatic import from the public API.
        /// </summary>
        public static readonly ImportTrigger AutoImport = new("Auto Import");

        readonly string m_Value;

        ImportTrigger(string value)
        {
            m_Value = value;
        }

        public override string ToString() => m_Value;
    }
}
