# Phase 04 First Playable Level

目标：在 JadeSpirit 基础角色稳定后，制作第一张更像“正式第一关原型”的灰盒关卡，用来验证平台穿梭、跳跃节奏和轻探索路线。

## 当前资源

- 测试场景：`Assets/Scenes/Prototype/Phase04_FirstPlayableLevel.scene`
- 场景生成器：`PrototypePhase04FirstPlayableLevelSceneBuilder`
- 玩家：`Assets/Resources/Prefabs/Player/JadeSpiritPlayer.prefab`
- 移动参数：继续使用共享 `LightweightMovement.asset`

## 关卡结构

1. `Start_SafeTrainingPlatform`：安全起点，给玩家观察角色尺寸、Idle 和起跑。
2. `RunRead_LeadIn`：短跑引导，测试跑步进入跳跃前的节奏。
3. `AbilityPickup_AirDash`：主路早期安全位置，触碰后解锁空中冲刺。
4. `DashTutorial_Takeoff/Landing`：冲刺教学缺口，普通跳较难通过，跳跃中按 `Left Shift` 可通过。
5. `ShortJump_01/02/03`：三段基础短跳，测试 `Run -> JumpRise -> Fall` 的读图。
6. `BranchChoice_Main`：主路和下层路线的分歧点。
7. `LowerBranch_DropCatch/JadeRun`：下层可选路线，放置 3 个玉色收集物。
8. `LowerBranch_ReturnStep_01/02`：可选路线返回主路。
9. `Checkpoint_MainPlatform`：中段检查点。
10. `ClimbStep_01/02/03/04`：上升平台段，测试跳跃高度和空中控制。
11. `Final_Runup` 和 `Goal_Platform`：终点前稳定平台和终点触发器。

## 测试清单

1. 从起点到终点完整跑通，不需要高级技巧。
2. 主路线短跳能清楚观察起跳、上升、下落、落地接跑。
3. 拾取 `AbilityPickup_AirDash` 前按 `Left Shift` 无冲刺效果，拾取后可以空中冲刺。
4. Dash 教学缺口可通过“跳跃 + 空中按 Left Shift”完成。
5. 下层路线能进入、收集、返回主路，不会卡死。
6. 检查点激活后，掉落能回到中段安全点。
7. 角色不会因为碰撞体、尺寸或跳跃高度卡在平台边缘。
8. Phase01、Phase02、Phase03 不受 Phase04 新增内容影响。

## 当前不做

- 不做正式场景美术。
- 不做敌人、机关、存档或地图 UI。
- 不改 JadeSpirit 动画帧、碰撞体或移动手感。
- Dash 解锁只在本次 Play Mode 运行时保留，不做永久存档。

## 后续方向

如果 Phase04 跑通后节奏成立，下一步可以选择：

- 把灰盒平台改成更像中式神话地形的分层美术块。
- 增加一个简单能力门或回访捷径。
- 开始规划 2-3 个区域后再做地图 UI。
