using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class UserChip : Chip
    {
        public UserChip(UserInfo userInfo) : base(userInfo.Name)
        {
            var initialsCircle = new VisualElement();
            initialsCircle.name = "InitialsCircle";
            initialsCircle.style.backgroundColor = GetUserColor(userInfo.UserId);
            initialsCircle.pickingMode = PickingMode.Ignore;
            Add(initialsCircle);
            m_Label.PlaceInFront(initialsCircle);

            var initials = new Label();
            initials.name = "Initials";
            initials.text = Utilities.GetInitials(userInfo.Name);
            initialsCircle.Add(initials);

            tooltip = userInfo.Name;
        }

        // Base on same color palette used by AM Dashboard
        static readonly Color[] k_UserDefaultColors =
        {
            new Color32(229, 77, 46, 255), // Tomato
            new Color32(0, 162, 199, 255), // Cyan
            new Color32(247, 107, 21, 255), // Orange
            new Color32(110, 86, 207, 255), // Violet
            new Color32(18, 165, 148, 255), // Teal
            new Color32(48, 164, 108, 255), // Green
            new Color32(171, 74, 186, 255), // Plum
        };

        static Color GetUserColor(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return k_UserDefaultColors[0];
            }

            var lastCharIndex = userId.Length - 1;
            var lastCharCode = userId[lastCharIndex];
            var colorIndex = lastCharCode % k_UserDefaultColors.Length;

            return k_UserDefaultColors[colorIndex];
        }
    }
}
