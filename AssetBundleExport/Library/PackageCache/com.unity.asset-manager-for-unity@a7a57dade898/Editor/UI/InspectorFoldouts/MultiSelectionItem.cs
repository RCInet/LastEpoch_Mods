using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class MultiSelectionItem : VisualElement
    {
        const string k_DetailsPageFileItemUssStyle = "details-page-file-item";
        const string k_DetailsPageFileIconItemUssStyle = "details-page-file-item-icon";
        const string k_DetailsPageFileLabelItemUssStyle = "details-page-file-item-label";

        readonly Label m_FileName;
        readonly VisualElement m_Icon;

        public MultiSelectionItem()
        {
            m_FileName = new Label("");
            m_Icon = new VisualElement();

            AddToClassList(k_DetailsPageFileItemUssStyle);
            m_Icon.AddToClassList(k_DetailsPageFileIconItemUssStyle);
            m_FileName.AddToClassList(k_DetailsPageFileLabelItemUssStyle);

            Add(m_Icon);
            Add(m_FileName);
        }

        public async Task Refresh(BaseAssetData fileItem)
        {
            m_FileName.text = fileItem.Name;
            m_Icon.style.backgroundImage = AssetDataTypeHelper.GetIconForExtension(fileItem.PrimaryExtension);

            if (fileItem.PrimarySourceFile != null)
                return;

            var tasks = new List<Task>
            {
                fileItem.ResolveDatasetsAsync()
            };

            await TaskUtils.WaitForTasksWithHandleExceptions(tasks);
        }
    }
}
