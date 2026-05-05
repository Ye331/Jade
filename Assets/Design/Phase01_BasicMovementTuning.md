# Phase 01 Basic Movement Tuning

目标手感：轻盈、响应快、有容错。当前阶段只做基础操作，不做冲刺、墙跳、二段跳或技能解锁。

## 默认参数

- `maxRunSpeed = 8`：最大水平速度。
- `groundAcceleration = 70`：地面按方向键时的加速速度。
- `groundDeceleration = 90`：松开方向键后的减速速度。
- `turnDeceleration = 125`：反向输入时的转身减速，越高越干脆。
- `airAcceleration = 45`：空中方向控制力度。
- `airDeceleration = 30`：空中松开方向键后的减速力度。
- `jumpHeight = 4.2`：长按跳跃时的大致跳跃高度。
- `timeToJumpApex = 0.42`：到达跳跃最高点所需时间，越小越利落。
- `coyoteTime = 0.10`：离开平台后仍允许跳跃的时间。
- `jumpBufferTime = 0.12`：落地前提前按跳的缓存时间。
- `jumpCutVelocityMultiplier = 0.45`：松开跳跃键时保留的上升速度比例。
- `fallGravityMultiplier = 1.75`：下落时重力倍率。
- `lowJumpGravityMultiplier = 2.25`：松开跳跃键后的上升重力倍率。
- `maxFallSpeed = 18`：最大下落速度。

## 推荐调参顺序

1. 先调 `maxRunSpeed`，确定玩家水平移动的快慢。
2. 再调 `groundAcceleration`、`groundDeceleration`、`turnDeceleration`，确定起步、停下、转身是否顺手。
3. 调 `jumpHeight` 和 `timeToJumpApex`，确定跳跃高度和轻盈感。
4. 调 `fallGravityMultiplier`，让下落更明确，不要飘太久。
5. 最后调 `coyoteTime` 和 `jumpBufferTime`，让边缘跳和提前按跳更舒服。

## 当前暂不包含

- 墙滑、墙跳。
- 冲刺、空中冲刺。
- 二段跳。
- 攀爬、抓墙。
- 技能解锁。
- 战斗、敌人、存档、地图 UI。
