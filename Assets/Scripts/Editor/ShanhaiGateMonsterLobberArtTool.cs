using Jade.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jade.EditorTools
{
    public static class ShanhaiGateMonsterLobberArtTool
    {
        private const string BasePath = "Assets/Art/Enemies/MonsterLobber/";

        [MenuItem("Jade/Apply Monster Lobber Art To Selected")]
        private static void ApplyMonsterLobberArtToSelected()
        {
            Sprite monsterSprite = LoadSprite("MonsterLobber_Idle.png");
            Sprite projectileSprite = LoadSprite("MonsterProjectile_Orb.png");
            if (monsterSprite == null || projectileSprite == null)
            {
                Debug.LogWarning("Missing MonsterLobber_Idle.png or MonsterProjectile_Orb.png in " + BasePath);
                return;
            }

            foreach (GameObject selected in Selection.gameObjects)
            {
                ApplyToMonster(selected, monsterSprite, projectileSprite);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("Jade/Apply Monster Lobber Art To Selected", true)]
        private static bool CanApplyMonsterLobberArtToSelected()
        {
            foreach (GameObject selected in Selection.gameObjects)
            {
                if (selected.GetComponent<MonsterLobber2D>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static void ApplyToMonster(GameObject monster, Sprite monsterSprite, Sprite projectileSprite)
        {
            MonsterLobber2D lobber = monster.GetComponent<MonsterLobber2D>();
            if (lobber == null)
            {
                return;
            }

            SpriteRenderer renderer = monster.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = monster.AddComponent<SpriteRenderer>();
            }

            Undo.RecordObject(renderer, "Apply monster lobber art");
            renderer.sprite = monsterSprite;
            renderer.color = Color.white;
            renderer.sortingOrder = 10;

            Transform artTransform = monster.transform;
            Bounds spriteBounds = monsterSprite.bounds;
            float targetHeight = 1.35f;
            float scale = targetHeight / Mathf.Max(spriteBounds.size.y, 0.01f);
            Undo.RecordObject(artTransform, "Scale monster lobber art");
            artTransform.localScale = new Vector3(scale, scale, artTransform.localScale.z);

            Undo.RecordObject(lobber, "Bind monster projectile art");
            SerializedObject serialized = new SerializedObject(lobber);
            serialized.FindProperty("projectileSprite").objectReferenceValue = projectileSprite;
            serialized.FindProperty("projectileColor").colorValue = Color.white;
            serialized.ApplyModifiedProperties();

            EditorUtility.SetDirty(monster);
            EditorUtility.SetDirty(lobber);
            EditorUtility.SetDirty(renderer);
        }

        private static Sprite LoadSprite(string fileName)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(BasePath + fileName);
        }
    }
}
