using Jade.CameraTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jade.EditorTools
{
    public static class ShanhaiGateBackgroundSetupTool
    {
        private const string BasePath = "Assets/Art/Environments/ShanhaiGate/Backgrounds/";

        [MenuItem("Jade/Create ShanhaiGate Dynamic Background")]
        private static void CreateDynamicBackground()
        {
            Camera sceneCamera = Camera.main;
            if (sceneCamera == null)
            {
                Debug.LogWarning("No Main Camera found. Add or tag a camera as MainCamera first.");
                return;
            }

            GameObject root = GameObject.Find("Art_Background");
            if (root == null)
            {
                root = new GameObject("Art_Background");
                Undo.RegisterCreatedObjectUndo(root, "Create ShanhaiGate background");
            }
            else
            {
                Undo.RecordObject(root, "Update ShanhaiGate background");
            }

            root.transform.position = new Vector3(sceneCamera.transform.position.x, sceneCamera.transform.position.y, 20f);

            RemoveLayer(root.transform, "BG_FarRuins");
            RemoveLayer(root.transform, "BG_NearFoliageFrame");
            RemoveLayer(root.transform, "BG_ForegroundVines");

            CreateLayer(root.transform, sceneCamera, "BG_Sky", "BG_Sky_Mist.png", -50, 0.00f, 0.00f, 0.001f, 0f, 0.015f, 0.12f, 1.35f, Color.white);
            CreateLayer(root.transform, sceneCamera, "BG_MidForestRuins", "BG_Mid_ForestRuins.png", -30, 0.03f, 0.015f, 0.003f, 0f, 0.025f, 0.18f, 1.25f, new Color(1f, 1f, 1f, 0.42f));

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void RemoveLayer(Transform root, string objectName)
        {
            Transform layer = root.Find(objectName);
            if (layer == null)
            {
                return;
            }

            Undo.DestroyObjectImmediate(layer.gameObject);
        }

        private static void CreateLayer(
            Transform root,
            Camera sceneCamera,
            string objectName,
            string spriteName,
            int sortingOrder,
            float parallaxX,
            float parallaxY,
            float driftX,
            float driftY,
            float floatAmplitude,
            float floatSpeed,
            float overscan,
            Color color)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BasePath + spriteName);
            if (sprite == null)
            {
                Debug.LogWarning("Missing background sprite: " + BasePath + spriteName);
                return;
            }

            Transform layerTransform = root.Find(objectName);
            GameObject layer;
            if (layerTransform == null)
            {
                layer = new GameObject(objectName);
                Undo.RegisterCreatedObjectUndo(layer, "Create background layer");
                layerTransform = layer.transform;
                layerTransform.SetParent(root);
            }
            else
            {
                layer = layerTransform.gameObject;
                Undo.RecordObject(layer, "Update background layer");
            }

            layerTransform.position = new Vector3(sceneCamera.transform.position.x, sceneCamera.transform.position.y, 20f);
            layerTransform.localRotation = Quaternion.identity;

            SpriteRenderer renderer = layer.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = layer.AddComponent<SpriteRenderer>();
            }

            Undo.RecordObject(renderer, "Update background renderer");
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;

            ScaleToCamera(layerTransform, sprite, sceneCamera, overscan);

            ParallaxBackgroundLayer2D parallax = layer.GetComponent<ParallaxBackgroundLayer2D>();
            if (parallax == null)
            {
                parallax = layer.AddComponent<ParallaxBackgroundLayer2D>();
            }

            Undo.RecordObject(parallax, "Configure parallax background");
            parallax.Configure(sceneCamera.transform, new Vector2(parallaxX, parallaxY), new Vector2(driftX, driftY), floatAmplitude, floatSpeed);

            EditorUtility.SetDirty(layer);
            EditorUtility.SetDirty(renderer);
            EditorUtility.SetDirty(parallax);
        }

        private static void ScaleToCamera(Transform layer, Sprite sprite, Camera sceneCamera, float overscan)
        {
            float height = sceneCamera.orthographicSize * 2f * overscan;
            float width = height * sceneCamera.aspect;
            Vector2 spriteSize = sprite.bounds.size;
            float scale = Mathf.Max(width / Mathf.Max(spriteSize.x, 0.01f), height / Mathf.Max(spriteSize.y, 0.01f));
            layer.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
