using Model.Runtime;
using Model.Runtime.ReadOnly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuff : StatusEffects
{
    public SpeedBuff(Unit unit) : base(unit, 2f, 0.5f, 1f) { }
}

public class SlowDebuff : StatusEffects
{
    public SlowDebuff(Unit unit) : base(unit, 3f, 2f, 1f) { }
}

public class AttackSpeedBuff : StatusEffects
{
    public AttackSpeedBuff(Unit unit) : base(unit, 2f, 1f, 0.5f) { }
}

public class AttackSpeedDebuff : StatusEffects
{
    public AttackSpeedDebuff(Unit unit) : base(unit, 3f, 1f, 2f) { }
}