using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectComparer : IEqualityComparer<StatusEffects>
{
    public bool Equals(StatusEffects x, StatusEffects y)
    {
        return x.GetType() == y.GetType();
    }

    public int GetHashCode(StatusEffects obj)
    {
        return obj.GetType().GetHashCode();
    }
}