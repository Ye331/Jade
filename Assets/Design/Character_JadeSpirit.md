# Character: Jade Spirit

## 角色定位

当前主角是中式童话风格的小型玉灵精灵。它保留玉、云气、麒麟角和发光灵核等神话意象，但不再作为旧四足灵兽处理，而是一个适合平台跳跃的轻盈角色。

## 视觉关键词

- 小巧灵动，便于在平台之间穿梭。
- 白玉面、发光大眼、短麒麟角、短云尾和胸口玉芯是主要识别点。
- 服饰细节保持简化，避免小尺寸下糊成一团。
- 当前游戏帧使用简化跑跳测试版，优先保证动作读法和落点判断。

## 当前实现

- 最终概念参考图：`Assets/Art/Concept/Characters/JadeSpirit_FinalConcept.png`
- 简化跑跳测试参考图：`Assets/Art/Concept/Characters/JadeSpirit_SimplifiedRunJumpSheetReference.png`
- 当前游戏动画帧：`Assets/Resources/Characters/JadeQilinFrames/`
- 美术预览图：`Assets/Art/PrototypeArt/Characters/JadeQilinFrames_Preview.png`
- 玩家 prefab：`Assets/Resources/Prefabs/Player/JadeSpiritPlayer.prefab`
- Animator Controller：`Assets/Animation/Characters/JadeSpirit/JadeSpiritPlayer.controller`
- 动画片段目录：`Assets/Animation/Characters/JadeSpirit/`
- 规格文档：`Assets/Design/JadeSpirit_AnimationImportSpec.md`
- 构建入口：Unity/Tuanjie 菜单 `Jade > Build Jade Spirit Player Assets`

## 状态机

- 状态：`Idle`、`Run`、`JumpStart`、`JumpRise`、`JumpApex`、`Fall`、`Land`。
- 参数：`Speed01`、`VerticalSpeed`、`Grounded`、`Jumped`、`Landed`。
- `PlayerAnimationDriver2D` 继续负责把移动状态送入 Animator。
- `PlayerVisualFeedback2D` 继续负责视觉朝向、拉伸压缩和尘土反馈。

## 历史资源

- 旧四足角色参考图：`Assets/Resources/Characters/JadeQilin.png`
- 旧四足角色剪影：`Assets/Resources/Characters/JadeQilin_Silhouette.png`
- 旧 `JadeQilin` 动画链路暂时保留为历史归档和回退，不再作为当前主角标准。
