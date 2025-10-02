using Unity.Cloud.AppLinkingEmbedded.Runtime;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.AppLinkingEmbedded.Editor
{
    /// <summary>
    /// Editor for <see cref="ActivateAppFromUrl"/>.
    /// </summary>
   [CustomEditor(typeof(ActivateAppFromUrl))]
   internal class ActivateAppFromUrlEditor : UnityEditor.Editor
    {
        SerializedProperty m_ActivationUrlProperty;
        SerializedProperty m_ActivateAtStartUpProperty;
        IUrlRedirectionInterceptor m_UrlRedirectionInterceptor;

        void OnEnable()
        {
            m_UrlRedirectionInterceptor = UrlRedirectionInterceptor.GetInstance();

            m_ActivationUrlProperty = serializedObject.FindProperty(nameof(ActivateAppFromUrl.m_ActivationUrl));
            m_ActivateAtStartUpProperty = serializedObject.FindProperty(nameof(ActivateAppFromUrl.m_ActivateAtStartUp));
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            DrawGUI();
        }

        /// <summary>
        /// Draw the Editor GUI.
        /// </summary>
        public void DrawGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_ActivationUrlProperty);
            EditorGUILayout.PropertyField(m_ActivateAtStartUpProperty);

            GUI.enabled = !m_ActivateAtStartUpProperty.boolValue && !string.IsNullOrEmpty(m_ActivationUrlProperty.stringValue) && Application.isPlaying;

            if (GUILayout.Button("Activate"))
            {
                m_UrlRedirectionInterceptor.InterceptAwaitedUrl(m_ActivationUrlProperty.stringValue);
            }

            serializedObject.ApplyModifiedProperties();
        }
   }
}
