using Assets.Scripts.StatusEffects;
using Model.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectComparer : IEqualityComparer<IStatusEffects<Unit>>
{
    public bool Equals(IStatusEffects<Unit> x, IStatusEffects<Unit> y)
    {
        return x.GetType() == y.GetType();
    }

    public int GetHashCode(IStatusEffects<Unit> obj)
    {
        return obj.GetType().GetHashCode();
    }
}