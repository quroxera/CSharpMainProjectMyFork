using Codice.Client.Common.GameUI;
using Model.Config;
using Model.Runtime.Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnitBrains.Player;
using UnityEngine;

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{
    public override string TargetUnitName => "Ironclad Behemoth";
    private bool isMovingMode = true;
    private bool isAttackMode = false;
    private float changingModeTimer;
    public static float AttackRangeModifier { get; set; }


    protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
    {
        if (isMovingMode)
        {
            isAttackMode = true;
            isMovingMode = false;
            changingModeTimer = 1f;
            return;
        }
        else
        {
            if (changingModeTimer > 0)
            {
                changingModeTimer -= Time.deltaTime;
            }
            else
            {
                base.GenerateProjectiles(forTarget, intoList);
            }
        }
    }

    public override Vector2Int GetNextStep()
    {
        if (HasTargetsInRange())
        {
            return base.GetNextStep();
        }
        else
        {
            if (isAttackMode)
            {
                isMovingMode = true;
                isAttackMode = false;
                changingModeTimer -= Time.deltaTime;
                return unit.Pos;
            }
            else
            {
                if (changingModeTimer > 0)
                {
                    changingModeTimer -= Time.deltaTime;
                    return unit.Pos;
                }
                else
                {
                    return base.GetNextStep();
                }
            }
        }
    }

    protected override bool HasTargetsInRange()
    {
        if (AttackRangeModifier == 0)
            AttackRangeModifier = 1;
        var attackRangeSqr = unit.Config.AttackRange * unit.Config.AttackRange * AttackRangeModifier;
        foreach (var possibleTarget in GetAllTargets())
        {
            var diff = possibleTarget - unit.Pos;
            if (diff.sqrMagnitude < attackRangeSqr)
                return true;
        }
        return false;
    }

    protected override bool IsTargetInRange(Vector2Int targetPos)
    {
        var attackRangeSqr = unit.Config.AttackRange * unit.Config.AttackRange * AttackRangeModifier;
        var diff = targetPos - unit.Pos;
        return diff.sqrMagnitude <= attackRangeSqr;
    }
}