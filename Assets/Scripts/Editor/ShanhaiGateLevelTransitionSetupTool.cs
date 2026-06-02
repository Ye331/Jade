using Jade.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jade.EditorTools
{
    public static class ShanhaiGateLevelTransitionSetupTool
    {
        [MenuItem("Jade/Create Fall Level Transition Trigger")]
        private static void CreateFallLevelTransitionTrigger()
        {
            GameObject trigger = new GameObject("LevelTransition_FallToNextScene");
            Undo.RegisterCreatedObjectUndo(trigger, "Create fall level transition trigger");
            trigger.transform.position = new Vector3(150f, -10f, 0f);

            BoxCollider2D collider = trigger.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(12f, 4f);

            trigger.AddComponent<LevelTransition2D>();
            Selection.activeGameObject = trigger;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("Jade/Create Top Entry Spawn Point")]
        private static void CreateTopEntrySpawnPoint()
        {
            GameObject spawn = new GameObject("SceneEntry_TopDropSpawn");
            Undo.RegisterCreatedObjectUndo(spawn, "Create top entry spawn point");
            spawn.transform.position = new Vector3(0f, 12f, 0f);
            spawn.AddComponent<SceneEntrySpawnPoint2D>();
            Selection.activeGameObject = spawn;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("Jade/Create Basic Loading Scene Controller")]
        private static void CreateBasicLoadingSceneController()
        {
            GameObject controller = new GameObject("LoadingSceneController");
            Undo.RegisterCreatedObjectUndo(controller, "Create loading scene controller");
            controller.AddComponent<LoadingSceneController2D>();
            Selection.activeGameObject = controller;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
