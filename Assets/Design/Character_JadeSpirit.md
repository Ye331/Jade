# Character: JadeSpirit

## 当前定位

JadeSpirit 是当前玩家主角：小巧、灵动、偏中式童话的玉灵精灵，服务于平台跳跃中的快速移动、起跳和下落判断。它已经替代旧四足 JadeQilin，作为当前主角的唯一标准。

小尺寸下优先保证读图：白玉面、发光眼、短角或玉叶冠轮廓、短袍体块、清楚脚掌和短灵尾。服饰细节只保留大形状，不作为当前动画判断重点。

## 当前实现

- 最终概念图：`Assets/Art/Concept/Characters/JadeSpirit_FinalConcept.png`
- 简化跑跳参考图：`Assets/Art/Concept/Characters/JadeSpirit_SimplifiedRunJumpSheetReference.png`
- 当前高分辨率帧目录：`Assets/Resources/Characters/JadeSpiritHighResFrames/`
- 玩家 prefab：`Assets/Resources/Prefabs/Player/JadeSpiritPlayer.prefab`
- Animator Controller：`Assets/Animation/Characters/JadeSpirit/JadeSpiritPlayer.controller`
- 动画片段目录：`Assets/Animation/Characters/JadeSpirit/`
- 构建菜单：`Jade > Build Jade Spirit Player Assets`

## 当前状态机

当前实际参与播放的状态：

- `Idle`
- `Run`
- `JumpRise`
- `JumpApex`
- `Fall`

当前参数：

- `Speed01`
- `VerticalSpeed`
- `Grounded`
- `Jumped`
- `Landed`

`Jumped` 直接进入 `JumpRise`。`JumpRise`、`JumpApex`、`Fall` 由 `VerticalSpeed` 驱动切换。`Landed` 不播放落地动画，只根据 `Speed01` 立即回到 `Idle` 或 `Run`。

## 保留资源

`JumpStart`、`Land`、`Turn` 相关 PNG 或 anim 可以继续留在项目中，作为历史或备用美术资源，但它们不是当前可玩状态机的一部分，也不是当前必需状态。

旧 `JadeQilin` 资源只作为迁移历史和兜底上下文保留，不再作为当前主角概念或动画标准。

## 当前设计规则

- 角色保持小巧、清楚、适合平台穿梭。
- 空中阶段不再做运行时拉伸或压缩，避免跳跃轮廓变形。
- 碰撞和移动仍由 `PlayerMotor2D` 与 `BoxCollider2D` 控制；视觉变化只发生在 `VisualRoot` 下。
- 优先稳定 `Idle`、读得清的 `Run`、以及由物理驱动的 `JumpRise/JumpApex/Fall`，再考虑额外过渡动作。
