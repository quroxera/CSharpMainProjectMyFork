using Codice.Client.Common.GameUI;
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
}