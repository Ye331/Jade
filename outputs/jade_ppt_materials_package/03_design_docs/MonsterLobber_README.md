# Monster Lobber Art

These sprites are for the existing `MonsterLobber2D` and `MonsterProjectile2D` gameplay objects.
They match the ShanhaiGate forest ruin style: mossy stone shell, jade core, and readable projectile effects.

## Sprite Set

- `MonsterLobber_Idle.png`: idle pose.
- `MonsterLobber_Windup.png`: pre-attack charge pose.
- `MonsterLobber_Attack.png`: projectile launch pose.
- `MonsterLobber_Stunned.png`: hit or disabled pose.
- `MonsterProjectile_Orb.png`: clean projectile body.
- `MonsterProjectile_ArcTrail.png`: projectile with motion trail.
- `MonsterProjectile_ImpactBurst.png`: impact effect.
- `MonsterProjectile_TargetGlyph.png`: warning or targeting glyph.

## Placement

- Use the monster pose sprites on the monster's visual child, not on the collider object if you want easier tuning.
- Use a small circle collider for `MonsterProjectile_Orb.png`; do not use the full trail sprite as the hitbox.
- Suggested sorting order: monster `10`, projectile `12`, impact `13`.

