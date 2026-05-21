# Character: JadeQilin

> 历史归档文件。当前玩家主角已经迁移到 `Assets/Design/Character_JadeSpirit.md` 和 `JadeSpirit` 动画链路。

## 归档状态

旧 JadeQilin 概念和资源命名只保留用于迁移历史、兜底素材和 Unity GUID 连续性。它不再是当前玩家概念、动画标准或生成参考。

当前玩家标准：

- 角色文档：`Assets/Design/Character_JadeSpirit.md`
- 动画规格：`Assets/Design/JadeSpirit_AnimationImportSpec.md`
- 玩家 prefab：`Assets/Resources/Prefabs/Player/JadeSpiritPlayer.prefab`
- Animator Controller：`Assets/Animation/Characters/JadeSpirit/JadeSpiritPlayer.controller`

## 历史资源

- 旧概念图：`Assets/Resources/Characters/JadeQilin.png`
- 旧剪影图：`Assets/Resources/Characters/JadeQilin_Silhouette.png`
- 旧原型帧目录：`Assets/Resources/Characters/JadeQilinFrames/`
- 旧原型美术目录：`Assets/Art/PrototypeArt/Characters/JadeQilinFrames/`

这些资源在当前 JadeSpirit 帧缺失时，仍可能被 builder 当作临时兜底使用。它们不应作为新主角美术方向。

## 当前迁移规则

不要围绕旧四足 JadeQilin 设计重建新玩家行为。新玩家动画和文档遵循 JadeSpirit 基线：

- 当前状态：`Idle`、`Run`、`JumpRise`、`JumpApex`、`Fall`。
- `JumpStart`、`Land`、`Turn` 不是当前基础状态。
- 落地直接回到 `Idle` 或 `Run`。
- 起跳直接进入 `JumpRise`。

## 后续使用

本文件只保留为简短归档说明。如果之后要删除或移动旧 JadeQilin 资源，需要作为独立清理任务处理，并先确认没有 Unity 引用依赖它们的 GUID。
