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

## 视觉流畅度验收

1. 左右移动时，临时角色代理会自然翻转。
2. 长跑时能看到身体前倾、飘带拖尾和速度感。
3. 起跳时身体略微拉长，下落时姿态收紧。
4. 落地时有短暂压缩、少量尘土和极轻微镜头反馈。
5. 视觉表现不改变碰撞，玩家仍能稳定站上平台边缘。
6. 粒子和拖尾不能遮挡平台边缘，也不能影响判断跳跃落点。

## 汇报要点

- `PlayerInputReader` 只负责读输入。
- `PlayerMotor2D` 负责移动、跳跃、容错和重力。
- `PlayerMovementSettings` 集中保存手感参数。
- `PlayerVisualFeedback2D` 负责临时角色代理的朝向、拉伸、落地反馈和拖尾。
- `SimpleCameraFollow2D` 负责平滑相机。
- `RespawnZone2D` 负责掉落重置。
