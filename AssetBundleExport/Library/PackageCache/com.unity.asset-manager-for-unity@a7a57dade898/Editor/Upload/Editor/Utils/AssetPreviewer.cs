using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.AssetManager.Upload.Editor
{
    static class AssetPreviewer
    {
        static bool s_UseCustomPreview = false;
        static bool s_UseStaticCustomPreview = false;

        public static async Task<Texture2D> GenerateAdvancedPreview(Object asset, string assetPath, int size = 128)
        {
            Texture2D texture = null;

            if (s_UseCustomPreview && asset is GameObject gameObject)
            {
                texture = CreateAdvancedPreview(gameObject, size);
            }

            if (texture == null)
            {
                texture = CreatePreview()
                    .Invoke(null, new object[] { asset, null, assetPath, size, size }) as Texture2D;
            }

            if (texture == null)
            {
                return await GetDefaultPreviewTexture(asset);
            }

            return texture;
        }

        public static async Task<Texture2D> GetDefaultPreviewTexture(Object asset)
        {
            Texture2D texture;

            var attempt = 2;
            do
            {
                texture = UnityEditor.AssetPreview.GetAssetPreview(asset);

                if (texture != null)
                    break;

                --attempt;
                await Task.Delay(50);
            } while (attempt > 0);

            return texture;
        }

        // This code was taken from GameObjectInspector.DoRenderPreview
        static Bounds GetRenderableBounds(GameObject go)
        {
            var renderBounds = new Bounds();

            if (go == null)
            {
                return renderBounds;
            }

            var results = new List<Renderer>();
            go.GetComponentsInChildren(results);
            foreach (var rendererComponents in results)
            {
                if (!IsRendererUsableForPreview(rendererComponents))
                    continue;

                if (renderBounds.extents == Vector3.zero)
                {
                    renderBounds = rendererComponents.bounds;
                }
                else
                {
                    renderBounds.Encapsulate(rendererComponents.bounds);
                }
            }

            return renderBounds;
        }

        static bool IsRendererUsableForPreview(Renderer r)
        {
            switch (r)
            {
                case MeshRenderer meshRenderer:
                    meshRenderer.gameObject.TryGetComponent(out MeshFilter component);
                    if (component == null || component.sharedMesh == null)
                    {
                        return false;
                    }

                    break;
                case SkinnedMeshRenderer skinnedMeshRenderer:
                    if (skinnedMeshRenderer.sharedMesh == null)
                    {
                        return false;
                    }

                    break;
                case SpriteRenderer spriteRenderer:
                    if (spriteRenderer.sprite == null)
                    {
                        return false;
                    }

                    break;
                case BillboardRenderer billboardRenderer:
                    if (billboardRenderer.billboard == null || billboardRenderer.sharedMaterial == null)
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }

        // This method handle all types of assets but exposes the ability to set the size to something other than 128 x 128
        static MethodInfo CreatePreview()
        {
            // Get the type of the AssetPreviewUpdater class
            var method = Type.GetType("UnityEditor.AssetPreviewUpdater,UnityEditor.dll")
                ?.GetMethod("CreatePreview", BindingFlags.Static | BindingFlags.Public);

            return method;
        }

        static Texture2D CreateAdvancedPreview(GameObject gameObject, int size)
        {
            Texture2D texture = null;

            var renderUtility = new PreviewRenderUtility();

            try
            {
                var backgroundColor = new Color(0.094f, 0.094f, 0.094f, 1.0f); // Same as AM Dashboard
                var ambientColor = new Color(0.4f, 0.4f, 0.4f, 0.0f); // Brighter than the regular one
                var r = new Rect(0.0f, 0.0f, size, size);
                var previewDir = new Vector2(120f, -20f);

                var copy = Object.Instantiate(gameObject);
                renderUtility.AddSingleGO(copy);

                // This code was taken from GameObjectInspector.DoRenderPreview
                var renderableBounds = GetRenderableBounds(copy);
                var num1 = Mathf.Max(renderableBounds.extents.magnitude, 0.0001f);
                var num2 = num1 * 3.8f;
                var quaternion = Quaternion.Euler(-previewDir.y, -previewDir.x, 0.0f);
                var vector3 = renderableBounds.center - quaternion * (Vector3.forward * num2);
                renderUtility.camera.fieldOfView = 30f;
                renderUtility.camera.transform.position = vector3;
                renderUtility.camera.transform.rotation = quaternion;
                renderUtility.camera.backgroundColor = backgroundColor;
                renderUtility.camera.nearClipPlane = num2 - num1 * 1.1f;
                renderUtility.camera.farClipPlane = num2 + num1 * 1.1f;
                renderUtility.lights[0].intensity = 0.7f;
                renderUtility.lights[0].transform.rotation = quaternion * Quaternion.Euler(40f, 40f, 0.0f);
                renderUtility.lights[1].intensity = 0.7f;
                renderUtility.lights[1].transform.rotation = quaternion * Quaternion.Euler(340f, 218f, 177f);
                renderUtility.ambientColor = ambientColor;

                // TODO Cache this texture
                var backgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, true);
                backgroundTexture.SetPixel(0, 0, backgroundColor);
                backgroundTexture.Apply();

                if (s_UseStaticCustomPreview)
                {
                    renderUtility.BeginStaticPreview(r);

                    // BeginStaticPreview applies a gray background color.
                    // Apply our background texture on top of it.
                    Graphics.DrawTexture(r, backgroundTexture); // Works only on Standard.

                    renderUtility.Render(true);

                    texture = renderUtility.EndStaticPreview();
                }
                else
                {
                    var previewBackground = new GUIStyle { normal = { background = backgroundTexture } };

                    renderUtility.BeginPreview(r, previewBackground);

                    renderUtility.Render(true);

                    var rt = renderUtility.EndPreview() as RenderTexture;

                    RenderTexture.active = rt;
                    texture = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGB24, false, false);

                    texture.ReadPixels(r, 0, 0);
                    texture.Apply();

                    RenderTexture.active = null;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                renderUtility.Cleanup();
            }

            return texture;
        }
    }
}
