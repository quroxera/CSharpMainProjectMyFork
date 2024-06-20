using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using System.Collections.Generic;
using UnitBrains.Player;
using UnityEngine;
using Utilities;
using View;
using System.Linq;
using Unit = Model.Runtime.Unit;
using Assets.Scripts.StatusEffects;

public class SupportUnitBrain : DefaultPlayerUnitBrain
{
    public override string TargetUnitName => "Support";
    private float castModeTimer = 0;
    private float castCooldown = 0.5f;
    private float castDelay = 0.1f;
    private float delayTimer = 0;
    private VFXView _vfxView = ServiceLocator.Get<VFXView>();
    private StatusEffectsFactory _statusEffectsFactory = new StatusEffectsFactory();
    private bool _isCastMode = false;
    private bool _isMovingMode = true;
    private List<IReadOnlyUnit> _allies;
    IStatusEffects<Unit> statusEffect = null;

    protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList) { }

    protected void CastBuffs(StatusEffectsFactory _statusEffectsFactory)
    {
        _allies = runtimeModel.RoUnits.Where(u => u.Config.IsPlayerUnit == IsPlayerUnitBrain).ToList();
        Debug.Log($"Number of allies: {_allies.Count}");

        foreach (Unit ally in _allies)
        {
            statusEffect = _statusEffectsFactory.CreateStatusEffect(ally.Config);
            var unitType = ally.Config.Type;
            if (IsTargetInRange(ally.Pos))
            {
                ally.AddStatusEffect(statusEffect);
                _vfxView.PlayVFX(ally.Pos, VFXView.VFXType.BuffApplied);
                Debug.Log($"{statusEffect.GetType()} applied to ally at position: {ally.Pos}");
            }
            else
            {
                Debug.Log($"Ally at position: {ally.Pos} is not in range or already has {statusEffect.GetType()} buff.");
            }
        }
    }

    public override void Update(float deltaTime, float time)
    {
        base.Update(deltaTime, time);
        castModeTimer += deltaTime;

        if (_isCastMode)
        {
            delayTimer += deltaTime;
            if (delayTimer >= castDelay)
            {
                if (_statusEffectsFactory != null)
                {
                    CastBuffs(_statusEffectsFactory);
                    delayTimer = 0;
                    _isCastMode = false;
                    _isMovingMode = true;
                    castModeTimer = 0f;
                }
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
            }
        }
    }

    public override Vector2Int GetNextStep()
    {
        return _isMovingMode ? base.GetNextStep() : unit.Pos;
    }
}