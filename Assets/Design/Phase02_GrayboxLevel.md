# Phase 02 Graybox Level

目标：把 Phase 01 的基础跑跳手感放进一段完整灰盒小关卡，验证关卡节奏，而不是继续只在单点测试场景里跑跳。

## 当前实现落档

- 测试场景：`Assets/Scenes/Prototype/Phase02_GrayboxLevel.scene`
- 场景生成器：`PrototypeGrayboxLevelSceneBuilder`
- 玩家优先使用：`Assets/Resources/Prefabs/Player/JadeSpiritPlayer.prefab`
- 共用移动参数：`Assets/Scripts/Player/LightweightMovement.asset`
- 远景背景：`Assets/Resources/Backgrounds/JadeMythForest_Background.png`
- 交互对象：`Checkpoint2D`、`Collectible2D`、`LevelGoal2D`、`RespawnZone2D`
- 当前目标是验证一段“起跑 -> 短跳 -> 分支收集 -> 检查点 -> 上升平台 -> 终点”的完整节奏。

## 场景

打开 `Assets/Scenes/Prototype/Phase02_GrayboxLevel.scene`，进入 Play Mode。

Phase02 使用和 Phase01 相同的移动参数。角色缩小后，跳跃高度也在共享的 `LightweightMovement.asset` 中统一调低，避免不同场景手感不一致。

## 路线结构

1. `Start_LongRun`：长跑起步，测试加速、减速和镜头跟随。
2. `GapJump_Short_01/02`：短平台跳跃，测试边缘跳和提前按跳。
3. `MainRoute_BranchEntry`：主路线分叉入口。
4. `LowerCollectible_DropIn/Run`：下层可选收集路线，平台更短并和主路横向错开，唯一收集物放在检查点下方右侧的恢复平台上。
5. `Checkpoint_MidRoute`：中段检查点，后续掉落会回到这里。
6. `ClimbStep_01/02/03`：上升平台，测试跳跃高度和空中控制。
7. `Final_Runup`：终点前的稳定平台。
8. `LevelGoal_EndMarker`：终点，触发完成日志。

## 验收步骤

1. 从起点跑到第一个短平台，确认基础手感仍然稳定。
2. 下探到检查点下方右侧的恢复平台，收集唯一的玉色收集物。
3. 故意掉到下层路线，确认能继续通过恢复平台回到主路。
4. 在下层路线跳跃和转身，确认头顶没有卡住或顶到主路线平台，同时下层路线在相机里可提前看见。
5. 触碰中段检查点，观察 Console 日志。
6. 从检查点平台下方跳起，确认不会从下方顶到触发区并激活检查点。
7. 在下层恢复平台连续跳跃，确认缩小后的角色不会卡进上下层平台之间。
8. 确认缩小后的角色不会跳得过高，短平台和上升平台仍需要正常操作。
9. 通过上升平台到达终点。
10. 终点触发后，Console 应显示完成信息。
11. 在检查点之后故意掉落，确认玩家回到最近检查点，而不是最初出生点。

## 汇报要点

- 这是灰盒关卡，不是正式美术。
- `Checkpoint2D` 负责更新玩家复活点。
- `Collectible2D` 负责简单收集反馈。
- `LevelGoal2D` 负责关卡完成反馈。
- `PrototypeGrayboxLevelSceneBuilder` 在运行时生成路线，便于快速迭代关卡结构。
- `JadeSpiritPlayer.prefab` 让 Phase01 和 Phase02 使用同一套角色、动画和碰撞设置；旧 `JadeQilinPlayer.prefab` 只作为回退。
- `JadeMythForest_Background` 只是远景氛围层，不参与碰撞与路线判断。

## 当前不做

- 技能、冲刺、墙跳。
- 敌人和战斗。
- 正式美术和动画。
- 地图 UI 和存档系统。

## 下一轮关卡关注点

- 继续保留灰盒优先，不要在路线稳定前投入正式场景美术。
- 检查下层收集路线是否有明确动机：玩家应该能提前看到玉色收集物或路线提示。
- 检查检查点位置是否足够靠近后半段失败点，避免重复跑前半段造成疲劳。
- 如果 Phase02 后续加入能力门，建议只先加入一个能力和一条回访捷径，保持课程作业规模可控。
