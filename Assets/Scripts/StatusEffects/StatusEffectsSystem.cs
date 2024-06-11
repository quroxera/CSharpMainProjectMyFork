using System;
using System.Collections.Generic;
using System.Linq;
using Model.Runtime;
using UnityEngine;
using Utilities;

public class StatusEffectsSystem
{
    private Dictionary<Unit, HashSet<StatusEffects>> _unitsStatusEffects = new Dictionary<Unit, HashSet<StatusEffects>>();

    public StatusEffectsSystem()
    {
        ServiceLocator.Get<TimeUtil>().AddFixedUpdateAction(UpdateStatusEffects);
    }

    public void AddStatusEffect(Unit unit, StatusEffects statusEffect)
    {
        if (!_unitsStatusEffects.ContainsKey(unit))
        {
            _unitsStatusEffects[unit] = new HashSet<StatusEffects>(new StatusEffectComparer());
        }

        if (_unitsStatusEffects[unit].Add(statusEffect))
        {
            Debug.Log($"Added {statusEffect.GetType().Name} to unit {unit}");
        }
        else
        {
            Debug.Log($"{statusEffect.GetType().Name} is already applied to unit at position {unit.Pos}");
        }
    }

    public void RemoveStatusEffect(Unit unit, StatusEffects statusEffect)
    {
        if (_unitsStatusEffects.ContainsKey(unit))
        {
            _unitsStatusEffects[unit].Remove(statusEffect);
            Debug.Log($"Removed {statusEffect.GetType().Name} from unit {unit}");
        }
    }

    public void UpdateStatusEffects(float time)
    {
        foreach (var unitEffects in _unitsStatusEffects)
        {
            foreach (StatusEffects statusEffect in unitEffects.Value.ToArray())
            {
                statusEffect.Duration -= time;
                if (statusEffect.Duration <= 0)
                {
                    RemoveStatusEffect(unitEffects.Key, statusEffect);
                }
            }
        }
    }

    public bool HasStatusEffect(Unit unit, Type statusEffectType)
    {
        if (_unitsStatusEffects.ContainsKey(unit))
        {
            foreach (StatusEffects statusEffect in _unitsStatusEffects[unit])
            {
                if (statusEffect.GetType() == statusEffectType)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public float GetMovementSpeedModifier(Unit unit)
    {
        float movementSpeedModifier = 1f;

        if (_unitsStatusEffects.ContainsKey(unit))
        {
            foreach (StatusEffects statusEffect in _unitsStatusEffects[unit])
            {
                movementSpeedModifier *= statusEffect.MovementSpeedModifier;
            }
        }
        Debug.Log($"Movement speed modifier for unit {unit}: {movementSpeedModifier}");
        return movementSpeedModifier;
    }

    public float GetAttackSpeedModifier(Unit unit)
    {
        float attackSpeedModifier = 1f;

        if (_unitsStatusEffects.ContainsKey(unit))
        {
            foreach (StatusEffects statusEffect in _unitsStatusEffects[unit])
            {
                attackSpeedModifier *= statusEffect.AttackSpeedModifier;
            }
        }
        Debug.Log($"Attack speed modifier for unit {unit}: {attackSpeedModifier}");
        return attackSpeedModifier;
    }
}