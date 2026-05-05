# Phase 01 Test Checklist

打开 `Assets/Scenes/Prototype/Phase01_BasicMovement.scene`，进入 Play Mode。

## 输入

- A/D 或左右方向键：左右移动。
- Space：跳跃。

## 验收路线

1. 在长跑区左右移动，确认加速、减速、转身都平滑。
2. 在短平台边缘测试：离开平台后短时间按 Space 仍能跳起。
3. 在短平台落地前提前按 Space，确认落地后自动起跳。
4. 分别短按和长按 Space，确认跳跃高度明显不同。
5. 跳上高低平台，确认空中仍能调整方向。
6. 掉进底部区域，确认玩家回到出生点。
7. 观察相机跟随，确认不明显抖动，也不会遮挡前进方向。

## 汇报要点

- `PlayerInputReader` 只负责读输入。
- `PlayerMotor2D` 负责移动、跳跃、容错和重力。
- `PlayerMovementSettings` 集中保存手感参数。
- `SimpleCameraFollow2D` 负责平滑相机。
- `RespawnZone2D` 负责掉落重置。
