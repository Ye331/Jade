using System.Collections.Generic;
using System.IO;
using Jade.Player;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Jade.EditorTools
{
    public static class JadeSpiritPlayerAssetBuilder
    {
        private const string HighResFrameFolder = "Assets/Resources/Characters/JadeSpiritHighResFrames";
        private const string LegacyFrameFolder = "Assets/Resources/Characters/JadeQilinFrames";
        private const string AnimationFolder = "Assets/Animation/Characters/JadeSpirit";
        private const string ControllerPath = AnimationFolder + "/JadeSpiritPlayer.controller";
        private const string PrefabFolder = "Assets/Resources/Prefabs/Player";
        private const string PrefabPath = PrefabFolder + "/JadeSpiritPlayer.prefab";
        private const string MovementSettingsPath = "Assets/Scripts/Player/LightweightMovement.asset";
        private const string SpriteBindingPath = "VisualRoot/JadeSpiritSprite";
        private const float TargetVisualSourcePixels = 1024f;

        private static readonly string[] FrameFolders =
        {
            HighResFrameFolder,
            LegacyFrameFolder,
            "Assets/Art/PrototypeArt/Characters/JadeQilinFrames"
        };

        [MenuItem("Jade/Build Jade Spirit Player Assets")]
        public static void BuildAll()
        {
            EnsureFolders();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            ApplyImportSettingsForDoubleJumpFrames();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            EnsureDoubleJumpAnimationClip();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Updated Jade Spirit additive animation assets without rebuilding the controller or prefab.");
        }

        [MenuItem("Jade/Add Jade Spirit Double Jump State")]
        public static void AddDoubleJumpState()
        {
            EnsureFolders();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            ApplyImportSettingsForDoubleJumpFrames();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            AnimationClip doubleJumpClip = EnsureDoubleJumpAnimationClip();
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null)
            {
                throw new FileNotFoundException("Cannot add Jade Spirit double jump state because the animator controller is missing.", ControllerPath);
            }

            EnsureDoubleJumpAnimatorState(controller, doubleJumpClip);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Added Jade Spirit double jump state without rebuilding existing states or prefab.");
        }

        public static void ApplyCharacterTextureSettings(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.sRGBTexture = true;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 2048;
            importer.spritePixelsPerUnit = 512f;
            importer.spritePivot = new Vector2(0.5f, 0f);

            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = new Vector2(0.5f, 0f);
            importer.SetTextureSettings(settings);
        }

        private static void EnsureFolders()
        {
            EnsureFolder(HighResFrameFolder);
            EnsureFolder(AnimationFolder);
            EnsureFolder(PrefabFolder);
        }

        private static void EnsureFolder(string assetPath)
        {
            string[] parts = assetPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        private static void ApplyImportSettingsForExistingFrames()
        {
            for (int i = 0; i < FrameFolders.Length; i++)
            {
                string folder = FrameFolders[i];
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    continue;
                }

                string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
                for (int j = 0; j < textureGuids.Length; j++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(textureGuids[j]);
                    TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (importer == null)
                    {
                        continue;
                    }

                    ApplyCharacterTextureSettings(importer);
                    importer.SaveAndReimport();
                }
            }
        }

        private static Dictionary<string, AnimationClip> CreateAnimationClips()
        {
            ClipSpec[] specs =
            {
                new ClipSpec("Idle", Range("JadeSpirit_Idle", 1), new[] { "JadeQilin_Idle01" }, 1f, false),
                new ClipSpec("Run", Range("JadeSpirit_Run", 8), new[] { "JadeQilin_Run01", "JadeQilin_Run02", "JadeQilin_Run03", "JadeQilin_Run04" }, 10f, true),
                new ClipSpec("JumpRise", Range("JadeSpirit_JumpRise", 3), new[] { "JadeQilin_Jump", "JadeQilin_Jump", "JadeQilin_Jump" }, 10f, true),
                new ClipSpec("JumpApex", Range("JadeSpirit_JumpApex", 2), new[] { "JadeQilin_Jump", "JadeQilin_Fall" }, 8f, true),
                new ClipSpec("Fall", Range("JadeSpirit_Fall", 2), new[] { "JadeQilin_Fall", "JadeQilin_Fall" }, 8f, false),
                new ClipSpec("DoubleJump", Range("JadeSpirit_DoubleJump", 4), new string[0], 18f, false),
                new ClipSpec("Dash", Range("JadeSpirit_Dash", 4), new[] { "JadeSpirit_JumpRise_01", "JadeSpirit_Fall_01" }, 16f, false)
            };

            Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();
            for (int i = 0; i < specs.Length; i++)
            {
                ClipSpec spec = specs[i];
                Sprite[] frames = LoadFrames(spec);
                AnimationClip clip = CreateClip(spec, frames);
                clips.Add(spec.Name, clip);
            }

            return clips;
        }

        private static void ApplyImportSettingsForDoubleJumpFrames()
        {
            for (int i = 1; i <= 4; i++)
            {
                string path = HighResFrameFolder + "/JadeSpirit_DoubleJump_" + i.ToString("00") + ".png";
                string absolutePath = Path.GetFullPath(path);
                if (!File.Exists(absolutePath))
                {
                    throw new FileNotFoundException("Required Jade Spirit double jump texture is missing on disk: " + absolutePath);
                }

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    throw new FileNotFoundException("Required Jade Spirit double jump texture exists on disk but was not found by the AssetDatabase. Reimport this file in Tuanjie and rerun the builder: " + path);
                }

                ApplyCharacterTextureSettings(importer);
                importer.SaveAndReimport();
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            }
        }

        private static AnimationClip EnsureDoubleJumpAnimationClip()
        {
            ClipSpec spec = new ClipSpec("DoubleJump", Range("JadeSpirit_DoubleJump", 4), new string[0], 18f, false);
            Sprite[] frames = LoadRequiredFrames(spec);
            return CreateClip(spec, frames);
        }

        private static string[] Range(string prefix, int count)
        {
            string[] names = new string[count];
            for (int i = 0; i < count; i++)
            {
                names[i] = prefix + "_" + (i + 1).ToString("00");
            }

            return names;
        }

        private static Sprite[] LoadFrames(ClipSpec spec)
        {
            Sprite[] frames = new Sprite[spec.DesiredNames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                Sprite sprite = FindSprite(spec.DesiredNames[i]);
                if (sprite == null && spec.FallbackNames.Length > 0)
                {
                    sprite = FindSprite(spec.FallbackNames[i % spec.FallbackNames.Length]);
                }

                if (sprite == null)
                {
                    sprite = FindFirstAvailableSprite();
                }

                if (sprite == null)
                {
                    throw new FileNotFoundException("No Jade Spirit sprite frames were found.");
                }

                frames[i] = sprite;
            }

            return frames;
        }

        private static Sprite[] LoadRequiredFrames(ClipSpec spec)
        {
            Sprite[] frames = new Sprite[spec.DesiredNames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                Sprite sprite = FindSprite(spec.DesiredNames[i]);
                if (sprite == null)
                {
                    throw new FileNotFoundException("Required Jade Spirit sprite frame was not found or was not imported as a Sprite: " + spec.DesiredNames[i]);
                }

                frames[i] = sprite;
            }

            return frames;
        }

        private static Sprite FindSprite(string baseName)
        {
            for (int i = 0; i < FrameFolders.Length; i++)
            {
                string folder = FrameFolders[i];
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    continue;
                }

                string path = folder + "/" + baseName + ".png";
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    return sprite;
                }

                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                for (int j = 0; j < assets.Length; j++)
                {
                    sprite = assets[j] as Sprite;
                    if (sprite != null)
                    {
                        return sprite;
                    }
                }

                string[] guids = AssetDatabase.FindAssets(baseName + " t:Sprite", new[] { folder });
                for (int j = 0; j < guids.Length; j++)
                {
                    string foundPath = AssetDatabase.GUIDToAssetPath(guids[j]);
                    if (Path.GetFileNameWithoutExtension(foundPath) != baseName)
                    {
                        continue;
                    }

                    sprite = AssetDatabase.LoadAssetAtPath<Sprite>(foundPath);
                    if (sprite != null)
                    {
                        return sprite;
                    }
                }
            }

            return null;
        }

        private static Sprite FindFirstAvailableSprite()
        {
            for (int i = 0; i < FrameFolders.Length; i++)
            {
                string folder = FrameFolders[i];
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    continue;
                }

                string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folder });
                for (int j = 0; j < guids.Length; j++)
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guids[j]));
                    if (sprite != null)
                    {
                        return sprite;
                    }
                }
            }

            return null;
        }

        private static AnimationClip CreateClip(ClipSpec spec, Sprite[] frames)
        {
            string path = AnimationFolder + "/JadeSpirit_" + spec.Name + ".anim";
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            bool isNewClip = clip == null;
            if (isNewClip)
            {
                clip = new AnimationClip();
            }

            clip.name = "JadeSpirit_" + spec.Name;
            clip.frameRate = spec.FramesPerSecond;
            clip.wrapMode = spec.Loop ? WrapMode.Loop : WrapMode.Once;

            ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[frames.Length + 1];
            for (int i = 0; i < frames.Length; i++)
            {
                keys[i] = new ObjectReferenceKeyframe
                {
                    time = i / spec.FramesPerSecond,
                    value = frames[i]
                };
            }

            keys[keys.Length - 1] = new ObjectReferenceKeyframe
            {
                time = frames.Length / spec.FramesPerSecond,
                value = spec.Loop ? frames[0] : frames[frames.Length - 1]
            };

            EditorCurveBinding spriteBinding = new EditorCurveBinding
            {
                path = SpriteBindingPath,
                type = typeof(SpriteRenderer),
                propertyName = "m_Sprite"
            };
            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keys);

            AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = spec.Loop;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

            if (isNewClip)
            {
                AssetDatabase.CreateAsset(clip, path);
            }
            else
            {
                EditorUtility.SetDirty(clip);
            }

            return clip;
        }

        private static AnimatorController CreateAnimatorController(Dictionary<string, AnimationClip> clips)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            }
            else
            {
                ClearAnimatorController(controller);
            }

            controller.AddParameter(PlayerAnimationDriver2D.Speed01Parameter, AnimatorControllerParameterType.Float);
            controller.AddParameter(PlayerAnimationDriver2D.VerticalSpeedParameter, AnimatorControllerParameterType.Float);
            controller.AddParameter(PlayerAnimationDriver2D.GroundedParameter, AnimatorControllerParameterType.Bool);
            controller.AddParameter(PlayerAnimationDriver2D.JumpedParameter, AnimatorControllerParameterType.Trigger);
            controller.AddParameter(PlayerAnimationDriver2D.DoubleJumpedParameter, AnimatorControllerParameterType.Trigger);
            controller.AddParameter(PlayerAnimationDriver2D.LandedParameter, AnimatorControllerParameterType.Trigger);
            controller.AddParameter(PlayerAnimationDriver2D.DashingParameter, AnimatorControllerParameterType.Bool);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            AnimatorState idle = AddState(stateMachine, "Idle", clips, 250, 0);
            AnimatorState run = AddState(stateMachine, "Run", clips, 510, 0);
            AnimatorState jumpRise = AddState(stateMachine, "JumpRise", clips, 510, -180);
            AnimatorState jumpApex = AddState(stateMachine, "JumpApex", clips, 770, -180);
            AnimatorState fall = AddState(stateMachine, "Fall", clips, 1030, -180);
            AnimatorState doubleJump = AddState(stateMachine, "DoubleJump", clips, 510, -360);
            AnimatorState dash = AddState(stateMachine, "Dash", clips, 770, -360);
            stateMachine.defaultState = idle;

            AddGroundedSpeedTransition(idle, run, AnimatorConditionMode.Greater, 0.1f, false, 0f);
            AddGroundedSpeedTransition(run, idle, AnimatorConditionMode.Less, 0.08f, false, 0f);
            AddWalkOffTransition(idle, fall);
            AddWalkOffTransition(run, fall);

            AnimatorStateTransition anyJump = AddAnyTransition(stateMachine, jumpRise, 0.03f);
            anyJump.AddCondition(AnimatorConditionMode.If, 0f, PlayerAnimationDriver2D.JumpedParameter);

            AnimatorStateTransition anyDoubleJump = AddAnyTransition(stateMachine, doubleJump, 0.03f);
            anyDoubleJump.AddCondition(AnimatorConditionMode.If, 0f, PlayerAnimationDriver2D.DoubleJumpedParameter);

            AnimatorStateTransition anyDash = AddAnyTransition(stateMachine, dash, 0.04f);
            anyDash.AddCondition(AnimatorConditionMode.If, 0f, PlayerAnimationDriver2D.DashingParameter);

            AnimatorStateTransition landedToRun = AddAnyTransition(stateMachine, run, 0.03f);
            landedToRun.AddCondition(AnimatorConditionMode.If, 0f, PlayerAnimationDriver2D.LandedParameter);
            landedToRun.AddCondition(AnimatorConditionMode.Greater, 0.1f, PlayerAnimationDriver2D.Speed01Parameter);

            AnimatorStateTransition landedToIdle = AddAnyTransition(stateMachine, idle, 0.03f);
            landedToIdle.AddCondition(AnimatorConditionMode.If, 0f, PlayerAnimationDriver2D.LandedParameter);
            landedToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, PlayerAnimationDriver2D.Speed01Parameter);

            AnimatorStateTransition riseToApex = AddTransition(jumpRise, jumpApex, 0.04f);
            riseToApex.AddCondition(AnimatorConditionMode.Less, 0.65f, PlayerAnimationDriver2D.VerticalSpeedParameter);

            AnimatorStateTransition riseToFall = AddTransition(jumpRise, fall, 0.04f);
            riseToFall.AddCondition(AnimatorConditionMode.Less, -0.1f, PlayerAnimationDriver2D.VerticalSpeedParameter);

            AnimatorStateTransition apexToFall = AddTransition(jumpApex, fall, 0.04f);
            apexToFall.AddCondition(AnimatorConditionMode.Less, -0.1f, PlayerAnimationDriver2D.VerticalSpeedParameter);

            AddAirStateReturnTransitions(doubleJump, jumpRise, jumpApex, fall, 0.65f, 0.04f);

            AnimatorStateTransition dashToRise = AddTransition(dash, jumpRise, 0.05f);
            dashToRise.AddCondition(AnimatorConditionMode.IfNot, 0f, PlayerAnimationDriver2D.DashingParameter);
            dashToRise.AddCondition(AnimatorConditionMode.Greater, 0.65f, PlayerAnimationDriver2D.VerticalSpeedParameter);

            AnimatorStateTransition dashToApex = AddTransition(dash, jumpApex, 0.05f);
            dashToApex.AddCondition(AnimatorConditionMode.IfNot, 0f, PlayerAnimationDriver2D.DashingParameter);
            dashToApex.AddCondition(AnimatorConditionMode.Less, 0.65f, PlayerAnimationDriver2D.VerticalSpeedParameter);
            dashToApex.AddCondition(AnimatorConditionMode.Greater, -0.1f, PlayerAnimationDriver2D.VerticalSpeedParameter);

            AnimatorStateTransition dashToFall = AddTransition(dash, fall, 0.05f);
            dashToFall.AddCondition(AnimatorConditionMode.IfNot, 0f, PlayerAnimationDriver2D.DashingParameter);
            dashToFall.AddCondition(AnimatorConditionMode.Less, -0.1f, PlayerAnimationDriver2D.VerticalSpeedParameter);

            return controller;
        }

        private static void EnsureDoubleJumpAnimatorState(AnimatorController controller, AnimationClip doubleJumpClip)
        {
            EnsureParameter(controller, PlayerAnimationDriver2D.DoubleJumpedParameter, AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            AnimatorState jumpRise = FindState(stateMachine, "JumpRise");
            AnimatorState jumpApex = FindState(stateMachine, "JumpApex");
            AnimatorState fall = FindState(stateMachine, "Fall");
            if (jumpRise == null || jumpApex == null || fall == null)
            {
                throw new FileNotFoundException("Cannot add Jade Spirit double jump state because JumpRise, JumpApex, or Fall is missing from the controller.");
            }

            AnimatorState doubleJump = FindState(stateMachine, "DoubleJump");
            if (doubleJump == null)
            {
                doubleJump = stateMachine.AddState("DoubleJump", new Vector3(510, -360, 0f));
            }

            doubleJump.motion = doubleJumpClip;
            RemoveTransitions(doubleJump);
            RemoveAnyTransitionsTo(stateMachine, doubleJump);

            AnimatorStateTransition anyDoubleJump = AddAnyTransition(stateMachine, doubleJump, 0.03f);
            anyDoubleJump.AddCondition(AnimatorConditionMode.If, 0f, PlayerAnimationDriver2D.DoubleJumpedParameter);

            AddAirStateReturnTransitions(doubleJump, jumpRise, jumpApex, fall, 0.65f, 0.04f);
            EditorUtility.SetDirty(controller);
        }

        private static void EnsureParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            AnimatorControllerParameter[] parameters = controller.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].name == name)
                {
                    return;
                }
            }

            controller.AddParameter(name, type);
        }

        private static AnimatorState FindState(AnimatorStateMachine stateMachine, string name)
        {
            ChildAnimatorState[] states = stateMachine.states;
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].state != null && states[i].state.name == name)
                {
                    return states[i].state;
                }
            }

            return null;
        }

        private static void RemoveTransitions(AnimatorState state)
        {
            AnimatorStateTransition[] transitions = state.transitions;
            for (int i = transitions.Length - 1; i >= 0; i--)
            {
                state.RemoveTransition(transitions[i]);
            }
        }

        private static void RemoveAnyTransitionsTo(AnimatorStateMachine stateMachine, AnimatorState destination)
        {
            AnimatorStateTransition[] transitions = stateMachine.anyStateTransitions;
            for (int i = transitions.Length - 1; i >= 0; i--)
            {
                if (transitions[i].destinationState == destination)
                {
                    stateMachine.RemoveAnyStateTransition(transitions[i]);
                }
            }
        }

        private static void ClearAnimatorController(AnimatorController controller)
        {
            AnimatorControllerParameter[] parameters = controller.parameters;
            for (int i = parameters.Length - 1; i >= 0; i--)
            {
                controller.RemoveParameter(parameters[i]);
            }

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            ChildAnimatorState[] states = stateMachine.states;
            for (int i = states.Length - 1; i >= 0; i--)
            {
                stateMachine.RemoveState(states[i].state);
            }

            AnimatorStateTransition[] anyStateTransitions = stateMachine.anyStateTransitions;
            for (int i = anyStateTransitions.Length - 1; i >= 0; i--)
            {
                stateMachine.RemoveAnyStateTransition(anyStateTransitions[i]);
            }

            EditorUtility.SetDirty(controller);
        }

        private static AnimatorState AddState(AnimatorStateMachine stateMachine, string name, Dictionary<string, AnimationClip> clips, int x, int y)
        {
            AnimatorState state = stateMachine.AddState(name, new Vector3(x, y, 0f));
            state.motion = clips[name];
            return state;
        }

        private static AnimatorStateTransition AddTransition(AnimatorState from, AnimatorState to, float duration)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            ConfigureTransition(transition, false, 0f, duration);
            return transition;
        }

        private static AnimatorStateTransition AddExitTransition(AnimatorState from, AnimatorState to, float exitTime, float duration)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            ConfigureTransition(transition, true, exitTime, duration);
            return transition;
        }

        private static AnimatorStateTransition AddAnyTransition(AnimatorStateMachine stateMachine, AnimatorState to, float duration)
        {
            AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(to);
            ConfigureTransition(transition, false, 0f, duration);
            transition.canTransitionToSelf = false;
            return transition;
        }

        private static void AddGroundedSpeedTransition(AnimatorState from, AnimatorState to, AnimatorConditionMode speedMode, float threshold, bool hasExitTime, float exitTime)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            ConfigureTransition(transition, hasExitTime, exitTime, 0.05f);
            transition.AddCondition(speedMode, threshold, PlayerAnimationDriver2D.Speed01Parameter);
            transition.AddCondition(AnimatorConditionMode.If, 0f, PlayerAnimationDriver2D.GroundedParameter);
        }

        private static void AddWalkOffTransition(AnimatorState from, AnimatorState fall)
        {
            AnimatorStateTransition transition = from.AddTransition(fall);
            ConfigureTransition(transition, false, 0f, 0.04f);
            transition.AddCondition(AnimatorConditionMode.IfNot, 0f, PlayerAnimationDriver2D.GroundedParameter);
            transition.AddCondition(AnimatorConditionMode.Less, -0.1f, PlayerAnimationDriver2D.VerticalSpeedParameter);
        }

        private static void AddAirStateReturnTransitions(AnimatorState from, AnimatorState jumpRise, AnimatorState jumpApex, AnimatorState fall, float exitTime, float duration)
        {
            AnimatorStateTransition toRise = AddExitTransition(from, jumpRise, exitTime, duration);
            toRise.AddCondition(AnimatorConditionMode.Greater, 0.65f, PlayerAnimationDriver2D.VerticalSpeedParameter);

            AnimatorStateTransition toApex = AddExitTransition(from, jumpApex, exitTime, duration);
            toApex.AddCondition(AnimatorConditionMode.Less, 0.65f, PlayerAnimationDriver2D.VerticalSpeedParameter);
            toApex.AddCondition(AnimatorConditionMode.Greater, -0.1f, PlayerAnimationDriver2D.VerticalSpeedParameter);

            AnimatorStateTransition toFall = AddExitTransition(from, fall, exitTime, duration);
            toFall.AddCondition(AnimatorConditionMode.Less, -0.1f, PlayerAnimationDriver2D.VerticalSpeedParameter);
        }

        private static void ConfigureTransition(AnimatorStateTransition transition, bool hasExitTime, float exitTime, float duration)
        {
            transition.hasExitTime = hasExitTime;
            transition.exitTime = exitTime;
            transition.duration = duration;
            transition.hasFixedDuration = true;
        }

        private static void CreatePlayerPrefab(RuntimeAnimatorController controller)
        {
            Sprite initialSprite = FindSprite("JadeSpirit_Idle_01") ?? FindSprite("JadeQilin_Idle01") ?? FindFirstAvailableSprite();
            if (initialSprite == null)
            {
                throw new FileNotFoundException("Cannot build Jade Spirit prefab without at least one sprite frame.");
            }

            GameObject root = new GameObject("JadeSpiritPlayer");
            Rigidbody2D body = root.AddComponent<Rigidbody2D>();
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;

            BoxCollider2D collider = root.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.68f, 1.25f);

            root.AddComponent<PlayerInputReader>();
            root.AddComponent<PlayerAbilityInventory2D>();
            PlayerMotor2D motor = root.AddComponent<PlayerMotor2D>();
            root.AddComponent<PlayerHealth2D>();
            PlayerMovementSettings movementSettings = AssetDatabase.LoadAssetAtPath<PlayerMovementSettings>(MovementSettingsPath);
            if (movementSettings != null)
            {
                motor.Configure(movementSettings);
            }

            Animator animator = root.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            root.AddComponent<PlayerAnimationDriver2D>();

            GameObject visualRoot = new GameObject("VisualRoot");
            visualRoot.transform.SetParent(root.transform);
            visualRoot.transform.localPosition = Vector3.zero;
            visualRoot.transform.localScale = new Vector3(1.12f, 1.12f, 1f);

            GameObject spriteObject = new GameObject("JadeSpiritSprite");
            spriteObject.transform.SetParent(visualRoot.transform);
            spriteObject.transform.localPosition = new Vector3(0f, -0.68f, 0f);
            spriteObject.transform.localScale = Vector3.one * GetSourceScaleCompensation(initialSprite);
            SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = initialSprite;
            spriteRenderer.sortingOrder = 16;

            ParticleSystem landingDust = CreateDust("LandingDust", root.transform, new Vector3(0f, -0.78f, 0f), false);
            ParticleSystem runDust = CreateDust("RunDust", root.transform, new Vector3(-0.25f, -0.78f, 0f), true);

            PlayerVisualFeedback2D feedback = root.AddComponent<PlayerVisualFeedback2D>();
            feedback.Configure(visualRoot.transform, landingDust, runDust, null);

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
        }

        private static float GetSourceScaleCompensation(Sprite sprite)
        {
            if (sprite.rect.height >= TargetVisualSourcePixels)
            {
                return 1f;
            }

            return TargetVisualSourcePixels / Mathf.Max(sprite.rect.height, 1f);
        }

        private static ParticleSystem CreateDust(string name, Transform parent, Vector3 localPosition, bool loop)
        {
            GameObject dustObject = new GameObject(name);
            dustObject.transform.SetParent(parent);
            dustObject.transform.localPosition = localPosition;

            ParticleSystem particles = dustObject.AddComponent<ParticleSystem>();
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            ParticleSystem.MainModule main = particles.main;
            main.duration = loop ? 0.6f : 0.25f;
            main.loop = loop;
            main.startLifetime = loop ? 0.25f : 0.32f;
            main.startSpeed = loop ? 0.6f : 1.6f;
            main.startSize = loop ? 0.08f : 0.16f;
            main.startColor = new Color(0.65f, 0.78f, 0.72f, 0.38f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = loop;
            emission.rateOverTime = loop ? 12f : 0f;
            emission.SetBursts(loop ? new ParticleSystem.Burst[0] : new[] { new ParticleSystem.Burst(0f, 10) });

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 25f;
            shape.radius = 0.18f;

            ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.sortingOrder = 8;

            if (!loop)
            {
                particles.Stop();
            }

            return particles;
        }

        private sealed class ClipSpec
        {
            public ClipSpec(string name, string[] desiredNames, string[] fallbackNames, float framesPerSecond, bool loop)
            {
                Name = name;
                DesiredNames = desiredNames;
                FallbackNames = fallbackNames;
                FramesPerSecond = framesPerSecond;
                Loop = loop;
            }

            public string Name { get; private set; }
            public string[] DesiredNames { get; private set; }
            public string[] FallbackNames { get; private set; }
            public float FramesPerSecond { get; private set; }
            public bool Loop { get; private set; }
        }
    }
}
