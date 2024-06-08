using Model.Runtime;
using System.Diagnostics;

public abstract class StatusEffects
{
    protected Unit _unit;
    public virtual float Duration { get; set; }
    public virtual float MovementSpeedModifier { get; set; }
    public virtual float AttackSpeedModifier { get; set; }
    public StatusEffects(Unit unit, float duration, float movementSpeedModifier, float attackSpeedModifier)
    {
        _unit = unit;
        Duration = duration;
        MovementSpeedModifier = movementSpeedModifier;
        AttackSpeedModifier = attackSpeedModifier;
    }
}
