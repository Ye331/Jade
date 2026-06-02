using Jade.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jade.EditorTools
{
    public static class ShanhaiGateBreakablePlatformArtTool
    {
        private const string BasePath = "Assets/Art/Environments/ShanhaiGate/FinalTerrain/BreakablePlatform/";

        [MenuItem("Jade/Apply Breakable Platform Art To Selected")]
        private static void ApplyBreakablePlatformArtToSelected()
        {
            Sprite intact = LoadSprite("BreakablePlatform_ObviousIntact.png");
            Sprite cracked = LoadSprite("BreakablePlatform_Cracked.png");
            Sprite fracturing = LoadSprite("BreakablePlatform_Fracturing.png");
            Sprite rubble = LoadSprite("BreakablePlatform_Rubble.png");

            if (intact == null || cracked == null || fracturing == null || rubble == null)
            {
                Debug.LogWarning("Missing one or more breakable platform sprites in " + BasePath);
                return;
            }

            foreach (GameObject selected in Selection.gameObjects)
            {
                ApplyToPlatform(selected, intact, cracked, fracturing, rubble);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("Jade/Apply Breakable Platform Art To Selected", true)]
        private static bool CanApplyBreakablePlatformArtToSelected()
        {
            foreach (GameObject selected in Selection.gameObjects)
            {
                if (selected.GetComponent<BreakablePlatform2D>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static void ApplyToPlatform(GameObject platform, Sprite intact, Sprite cracked, Sprite fracturing, Sprite rubble)
        {
            BreakablePlatform2D breakable = platform.GetComponent<BreakablePlatform2D>();
            BoxCollider2D collider = platform.GetComponent<BoxCollider2D>();
            if (breakable == null || collider == null)
            {
                return;
            }

            SpriteRenderer sourceRenderer = platform.GetComponent<SpriteRenderer>();
            Transform artTransform = platform.transform.Find("Art_BreakablePlatform");
            GameObject artObject;
            if (artTransform == null)
            {
                artObject = new GameObject("Art_BreakablePlatform");
                Undo.RegisterCreatedObjectUndo(artObject, "Create breakable platform art");
                artTransform = artObject.transform;
                artTransform.SetParent(platform.transform);
            }
            else
            {
                artObject = artTransform.gameObject;
                Undo.RecordObject(artObject, "Update breakable platform art");
            }

            SpriteRenderer artRenderer = artObject.GetComponent<SpriteRenderer>();
            if (artRenderer == null)
            {
                artRenderer = artObject.AddComponent<SpriteRenderer>();
            }

            Undo.RecordObject(artRenderer, "Update breakable platform renderer");
            artRenderer.sprite = intact;
            artRenderer.color = Color.white;
            artRenderer.sortingLayerID = sourceRenderer != null ? sourceRenderer.sortingLayerID : 0;
            artRenderer.sortingOrder = sourceRenderer != null ? sourceRenderer.sortingOrder + 8 : 8;

            Vector2 worldSize = Vector2.Scale(collider.size, Abs(platform.transform.lossyScale));
            Bounds spriteBounds = intact.bounds;
            float targetWidth = Mathf.Max(worldSize.x, 0.01f);
            float targetHeight = targetWidth * spriteBounds.size.y / Mathf.Max(spriteBounds.size.x, 0.01f);
            Vector3 parentScale = platform.transform.lossyScale;
            artTransform.localScale = new Vector3(
                targetWidth / Mathf.Max(spriteBounds.size.x, 0.01f) / Mathf.Max(Mathf.Abs(parentScale.x), 0.0001f),
                targetHeight / Mathf.Max(spriteBounds.size.y, 0.01f) / Mathf.Max(Mathf.Abs(parentScale.y), 0.0001f),
                1f);

            Vector3 worldCenter = platform.transform.TransformPoint(collider.offset);
            float colliderTop = worldCenter.y + worldSize.y * 0.5f;
            artTransform.position = new Vector3(worldCenter.x, colliderTop - targetHeight * 0.5f, platform.transform.position.z);
            artTransform.localRotation = Quaternion.identity;

            if (sourceRenderer != null)
            {
                Undo.RecordObject(sourceRenderer, "Hide breakable graybox renderer");
                sourceRenderer.enabled = false;
            }

            Undo.RecordObject(breakable, "Bind breakable platform art");
            breakable.Configure(platform.name, artRenderer, collider);

            SerializedObject serialized = new SerializedObject(breakable);
            SerializedProperty frames = serialized.FindProperty("breakAnimationFrames");
            frames.arraySize = 3;
            frames.GetArrayElementAtIndex(0).objectReferenceValue = intact;
            frames.GetArrayElementAtIndex(1).objectReferenceValue = cracked;
            frames.GetArrayElementAtIndex(2).objectReferenceValue = fracturing;
            serialized.FindProperty("brokenSprite").objectReferenceValue = rubble;
            serialized.FindProperty("breakFrameDuration").floatValue = 0.12f;
            serialized.FindProperty("hideRendererAfterBreak").boolValue = true;
            serialized.ApplyModifiedProperties();

            EditorUtility.SetDirty(platform);
            EditorUtility.SetDirty(artObject);
            EditorUtility.SetDirty(breakable);
        }

        private static Sprite LoadSprite(string fileName)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(BasePath + fileName);
        }

        private static Vector2 Abs(Vector3 value)
        {
            return new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
        }
    }
}
