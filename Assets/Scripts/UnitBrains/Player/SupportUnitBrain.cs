using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using Model;
using System.Collections.Generic;
using UnitBrains.Player;
using UnityEngine;
using Utilities;
using View;
using Model.Runtime;
using System.Linq;
using UnitBrains.Pathfinding;

public class SupportUnitBrain : DefaultPlayerUnitBrain
{
    public override string TargetUnitName => "Support";
    //public override bool IsPlayerSupportUnitBrain => true;
    private float castModeTimer = 0;
    private float castCooldown = 0.2f;
    private float castDelay = 0.5f;
    private float delayTimer = 0;
    private VFXView _vfxView = ServiceLocator.Get<VFXView>();
    private StatusEffectsSystem _statusEffectsSystem = ServiceLocator.Get<StatusEffectsSystem>();
    private bool _isCastMode = false;
    private bool _isMovingMode = true;
    private List<IReadOnlyUnit> _allies;

    protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
    {
    }

    protected void CastBuffs(StatusEffectsSystem statusEffectsSystem)
    {
        _allies = runtimeModel.RoUnits.Where(u => u.Config.IsPlayerUnit == IsPlayerUnitBrain).ToList();
        Debug.Log($"Number of allies: {_allies.Count}");

        foreach (Unit ally in _allies)
        {
            Debug.Log($"Checking ally at position: {ally.Pos}");

            if (IsTargetInRange(ally.Pos) &&
                !statusEffectsSystem.HasStatusEffect(ally, typeof(SpeedBuff)) &&
                !statusEffectsSystem.HasStatusEffect(ally, typeof(AttackSpeedBuff)))
            {
                statusEffectsSystem.AddStatusEffect(ally, new AttackSpeedBuff(ally));
                statusEffectsSystem.AddStatusEffect(ally, new SpeedBuff(ally));
                _vfxView.PlayVFX(ally.Pos, VFXView.VFXType.BuffApplied);
                Debug.Log($"Buffs applied to ally at position: {ally.Pos}");
            }
            else
            {
                Debug.Log($"Ally at position: {ally.Pos} is not in range or already has buffs.");
            }
        }
    }

    public override void Update(float deltaTime, float time)
    {
        base.Update(deltaTime, time);
        castModeTimer += deltaTime;

        Debug.Log($"Update called. castModeTimer: {castModeTimer}, _isCastMode: {_isCastMode}, _isMovingMode: {_isMovingMode}");

        if (_isCastMode)
        {
            delayTimer += deltaTime;
            Debug.Log($"In cast mode. delayTimer: {delayTimer}");

            if (delayTimer >= castDelay)
            {
                CastBuffs(_statusEffectsSystem);
                delayTimer = 0;
                _isCastMode = false;
                _isMovingMode = true;
                castModeTimer = 0f;
                Debug.Log("Cast mode ended.");
            }
        }
        else
        {
            if (castModeTimer >= castCooldown)
            {
                _isCastMode = true;
                _isMovingMode = false;
                castModeTimer = 0f;
                delayTimer = 0;
                Debug.Log("Cast mode started.");
            }
        }
    }

    public override Vector2Int GetNextStep()
    {
        return _isMovingMode ? base.GetNextStep() : unit.Pos;
    }

}