# Jade Qilin Animation Import Spec

> Historical note: 当前主角动画链路已迁移到 `Assets/Design/JadeSpirit_AnimationImportSpec.md`。本文件仅保留旧 `JadeQilin` 管线记录。

当前玩家已经使用 prefab 和 Animator Controller。正式美术准备好后，把当前原型帧替换为高分辨率透明 PNG，再运行构建菜单重建动画资源。

## 帧图要求

- 每帧为透明 PNG，建议 1024x1024。
- 512 pixels per unit。
- pivot 使用底部中心。
- 所有帧保持相同画布尺寸、相同脚底基线、相同默认朝向。
- 角色主体建议占画布高度约 70%-78%，避免进游戏后过小或裁切。

## 必需命名

- `JadeQilin_Idle_01` to `JadeQilin_Idle_04`
- `JadeQilin_Run_01` to `JadeQilin_Run_08`
- `JadeQilin_Turn_01` to `JadeQilin_Turn_03`
- `JadeQilin_JumpStart_01` to `JadeQilin_JumpStart_03`
- `JadeQilin_JumpRise_01` to `JadeQilin_JumpRise_03`
- `JadeQilin_JumpApex_01` to `JadeQilin_JumpApex_02`
- `JadeQilin_Fall_01` to `JadeQilin_Fall_03`
- `JadeQilin_Land_01` to `JadeQilin_Land_04`

## 导入设置

`Assets/Resources/Characters/JadeQilinHighResFrames` 下的帧图会由 editor postprocessor 自动套用这些设置：

- Sprite 2D，Single Sprite。
- Filter Mode: Bilinear。
- Compression: None。
- Mip Maps: Off。
- Max Size: 2048。
- Alpha Is Transparency: On。
- Pivot: Bottom Center。

## 构建步骤

1. 把正式帧放入 `Assets/Resources/Characters/JadeQilinHighResFrames/`。
2. 打开 Unity/Tuanjie，运行菜单 `Jade > Build Jade Qilin Player Assets`。
3. 构建器会重建 `Assets/Animation/Characters/JadeQilin/` 下的动画片段、Animator Controller 和 `Assets/Resources/Prefabs/Player/JadeQilinPlayer.prefab`。
4. 打开 `Phase01_BasicMovement.scene`，先确认 Idle、Run、Jump、Fall 能正常切换，再进入 Phase02 检查关卡尺度。

## 兜底规则

- 如果高分辨率帧缺失，构建器会尝试使用 `Assets/Resources/Characters/JadeQilinFrames/` 或 `Assets/Art/PrototypeArt/Characters/JadeQilinFrames/` 里的低清原型帧。
- 如果某个细分动作缺帧，会用已有 Idle/Run/Jump/Fall 原型帧临时补齐，保证 prefab 和 Animator 能先跑起来。
