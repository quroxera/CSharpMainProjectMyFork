//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Assets.Scripts.StatusEffects;
//using Model.Runtime;
//using UnityEngine;
//using Utilities;

//public class StatusEffectsSystem
//{
//    private Dictionary<Unit, HashSet<IStatusEffects<Unit>>> _unitsStatusEffects = new Dictionary<Unit, HashSet<IStatusEffects<Unit>>>();

//    public StatusEffectsSystem()
//    {
//        ServiceLocator.Get<TimeUtil>().AddFixedUpdateAction(UpdateStatusEffects);
//    }

//    public void AddStatusEffect<T>(Unit unit, IStatusEffects<Unit> statusEffect)
//    {
//        if (!_unitsStatusEffects.ContainsKey(unit))
//        {
//            _unitsStatusEffects[unit] = new HashSet<IStatusEffects<Unit>>(new StatusEffectComparer());
//        }
//        if (_unitsStatusEffects[unit].Add(statusEffect))
//        {
//            Debug.Log($"Added {statusEffect.GetType().Name} to unit {unit}");
//        }
//        else
//        {
//            Debug.Log($"{statusEffect.GetType().Name} is already applied to unit at position {unit.Pos}");
//        }
//    }

//    public void RemoveStatusEffect(Unit unit, IStatusEffects<Unit> statusEffect)
//    {
//        if (_unitsStatusEffects.ContainsKey(unit))
//        {
//            statusEffect.Remove(unit); 
//            _unitsStatusEffects[unit].Remove(statusEffect);
//            Debug.Log($"Removed {statusEffect.GetType().Name} from unit {unit}");
//        }
//    }

//    public void UpdateStatusEffects(float time)
//    {
//        foreach (var unitEffects in _unitsStatusEffects)
//        {
//            foreach (IStatusEffects<Unit> statusEffect in unitEffects.Value.ToArray())
//            {
//                statusEffect.Duration -= time;
//                if (statusEffect.Duration <= 0)
//                {
//                    RemoveStatusEffect(unitEffects.Key, statusEffect);
//                }
//            }
//        }
//    }

//    public bool HasStatusEffect(Unit unit, Type statusEffectType)
//    {
//        if (_unitsStatusEffects.ContainsKey(unit))
//        {
//            foreach (IStatusEffects<Unit> statusEffect in _unitsStatusEffects[unit])
//            {
//                if (statusEffect.GetType() == statusEffectType)
//                {
//                    return true;
//                }
//            }
//        }
//        return false;
//    }
//}
//    public float GetMovementSpeedModifier(Unit unit)
//    {
//        float movementSpeedModifier = 1f;

//        if (_unitsStatusEffects.ContainsKey(unit))
//        {
//            foreach (IStatusEffects<Unit> statusEffect in _unitsStatusEffects[unit])
//            {
//                movementSpeedModifier *= statusEffect.MovementSpeedModifier;
//            }
//        }
//        Debug.Log($"Movement speed modifier for unit {unit}: {movementSpeedModifier}");
//        return movementSpeedModifier;
//    }

//    public float GetAttackSpeedModifier(Unit unit)
//    {
//        float attackSpeedModifier = 1f;

//        if (_unitsStatusEffects.ContainsKey(unit))
//        {
//            foreach (IStatusEffects<Unit> statusEffect in _unitsStatusEffects[unit])
//            {
//                attackSpeedModifier *= statusEffect.AttackSpeedModifier;
//            }
//        }
//        Debug.Log($"Attack speed modifier for unit {unit}: {attackSpeedModifier}");
//        return attackSpeedModifier;
//    }
//}