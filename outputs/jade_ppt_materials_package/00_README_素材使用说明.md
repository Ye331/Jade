# Jade 汇报素材包使用说明

这个包是给 PPT 制作者使用的，不包含 PPT 成品。

## 推荐 PPT 结构

1. 封面：使用 `BG_Composite_NoForegroundVines_Preview.png` 或 `JadeSpirit_FinalConcept.png`。
2. 项目定位：讲 Unity 2D 平台跳跃、中式神话、玉色/山海/水墨意象。
3. 开发路线：Phase01 手感 -> Phase02 灰盒 -> Phase04 可玩关卡 -> 山海门/洞穴。
4. 核心玩法：移动、跳跃容错、检查点、掉落复位。
5. 玩家移动系统：贴 `PlayerMotor2D.cs`，重点讲跳跃缓存、土狼时间、冲刺。
6. 参数配置：贴 `PlayerMovementSettings.cs`，重点讲集中调参。
7. 关卡生成：贴 `PrototypePhase04FirstPlayableLevelSceneBuilder.cs`。
8. 能力交互：贴 `PlayerAbilityInventory2D.cs`、`DashAbilityPickup2D.cs`、`ShanhaiShardGate2D.cs`。
9. HUD 反馈：放 `TopLeftStatusHud.png`，贴 `GameplayHud2D.cs`。
10. 敌人与机关：放 `MonsterLobber_Preview.png`，贴 `MonsterLobber2D.cs`、`WideFallingStoneTrap2D.cs`。
11. 美术氛围：放背景、角色、洞穴草图、UI 图标。
12. 总结与下一步：可玩场景、移动手感、能力交互、HUD/美术/音频；下一步是地图 UI、存档、更多敌人与能力回访。

## 目录说明

- `01_key_images`：PPT 直接可用的图片素材。
- `02_key_scripts`：适合技术页贴代码的核心 C# 脚本。
- `03_design_docs`：项目设计说明、阶段验收和素材使用说明。
- `04_media`：开场视频、BGM 和音效，可用于演示或答辩补充。

## 最推荐使用的图片

- `BG_Composite_NoForegroundVines_Preview.png`：封面/美术氛围页主视觉。
- `JadeSpirit_FinalConcept.png`：角色设定页。
- `MonsterLobber_Preview.png`：敌人与机关页。
- `TopLeftStatusHud.png`：HUD 页。
- `AbilityOrb_Dash.png`、`AbilityOrb_DoubleJump.png`：能力系统页。
- `CaveBlockout_FINAL_REFERENCE.png`、`CaveDashSection_ArtDraft_15_RemoveRedNoVines.png`：关卡设计过程页。

## 最推荐讲的脚本

- `PlayerMotor2D.cs`：玩家移动、跳跃容错、冲刺、复活点。
- `PlayerMovementSettings.cs`：把移动参数集中成可调配置。
- `PrototypePhase04FirstPlayableLevelSceneBuilder.cs`：运行时生成第一张可玩关卡。
- `PlayerAbilityInventory2D.cs`：Dash 和 Double Jump 解锁状态。
- `GameplayHud2D.cs`：生命、能力、暂停和提示 UI。
- `MonsterLobber2D.cs`：敌人抛物线投射攻击。
- `WideFallingStoneTrap2D.cs`：机关状态机和触发检测。

## 风格建议

文字少，图片大。技术页用深色代码框，旁边只放 2-3 个标签说明脚本职责。
