using Assets.Scripts.StatusEffects;
using Model.Runtime;

public class SpeedBuff : IStatusEffects<Unit>
{
    public float Duration { get; set; } = 3f;
    private float _modifier = 0.5f;

    public void Apply(Unit unit)
    {
        unit.SetMovementSpeedModifier(_modifier);
    }

    public void Remove(Unit unit)
    {
        unit.SetMovementSpeedModifier(1f);

    }
    public bool CanBeAppliedTo(Unit unit)
    {
        return unit.movementSpeedModifier <= 1f;
    }

}

public class SlowDebuff : IStatusEffects<Unit>
{
    public float Duration { get; set; } = 1f;
    private float _modifier = 2f;
    public void Apply(Unit unit)
    {
        unit.SetMovementSpeedModifier(_modifier);
    }
    public bool CanBeAppliedTo(Unit unit)
    {
        return unit.Health > 0;
    }
    public void Remove(Unit unit)
    {
        unit.SetMovementSpeedModifier(1f);
    }
}

public class AttackSpeedBuff : IStatusEffects<Unit>
{
    public float Duration { get; set; } = 2f;
    private float _modifier = 0.1f;
    public void Apply(Unit unit)
    {
        unit.SetAttackSpeedModifier(_modifier);
    }
    public bool CanBeAppliedTo(Unit unit)
    {
        return unit.attackSpeedModifier <= 1f;
    }
    public void Remove(Unit unit)
    {
        unit.SetAttackSpeedModifier(1f);
    }
}


public class AttackSpeedDebuff : IStatusEffects<Unit>
{
    public float Duration { get; set; } = 1f;
    private float _modifier = 2f;
    public void Apply(Unit unit)
    {
        unit.SetAttackSpeedModifier(_modifier);
    }
    public bool CanBeAppliedTo(Unit unit)
    {
        return unit.Health > 0;
    }
    public void Remove(Unit unit)
    {
        unit.SetAttackSpeedModifier(1 / _modifier);
    }
}

public class DoubleAttackBuff : IStatusEffects<Unit>
{
    public float Duration { get; set; } = 4f;
    public void Apply(Unit unit)
    {
        unit.SetDoubleAttackActive(true);
    }
    public bool CanBeAppliedTo(Unit unit)
    {
        return unit.Health > 0;
    }
    public void Remove(Unit unit)
    {
        unit.SetDoubleAttackActive(false);
    }
}

public class AttackRangeBuff : IStatusEffects<Unit>
{
    public float Duration { get; set; } = 2f;
    public float modifier = 10f;
    public void Apply(Unit unit)
    {
        unit.ModifyAttackRange(modifier);
    }
    public bool CanBeAppliedTo(Unit unit)
    {
        return unit.attackRangeModifier < modifier;
    }
    public void Remove(Unit unit)
    {
        unit.ModifyAttackRange(1f);
    }
}
