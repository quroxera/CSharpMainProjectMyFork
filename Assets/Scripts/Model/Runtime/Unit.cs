using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.StatusEffects;
using Assets.Scripts.UnitBrains.Player;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnitBrains.Pathfinding;
using UnitBrains.Player;
using UnityEngine;
using Utilities;

namespace Model.Runtime
{
    public class Unit : IReadOnlyUnit
    {
        public UnitConfig Config { get; }
        public Vector2Int Pos { get; private set; }
        public int Health { get; private set; }
        public float movementSpeedModifier = 1f;
        public float attackSpeedModifier = 1f;

        public float attackRangeModifier = 1f;
        public bool IsDead => Health <= 0;
        public BaseUnitPath ActivePath => _brain?.ActivePath;
        public IReadOnlyList<BaseProjectile> PendingProjectiles => _pendingProjectiles;
        private readonly List<BaseProjectile> _pendingProjectiles = new();
        private IReadOnlyRuntimeModel _runtimeModel;
        private BaseUnitBrain _brain;
        private float _nextBrainUpdateTime = 0f;
        private float _nextMoveTime = 0f;
        private float _nextAttackTime = 0f;
        private List<IStatusEffects<Unit>> activeStatusEffects = new List<IStatusEffects<Unit>>();

        public Unit(UnitConfig config, Vector2Int startPos, UnitsCoordinator unitsCoordinator)
        {
            Config = config;
            Pos = startPos;
            Health = config.MaxHealth;
            _brain = UnitBrainProvider.GetBrain(config);
            _brain.SetUnit(this);
            _brain.SetCoordinator(unitsCoordinator);
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
        }


        public void Update(float deltaTime, float time)
        {
            if (IsDead)
                return;

            if (_nextBrainUpdateTime < time)
            {
                _nextBrainUpdateTime = time + Config.BrainUpdateInterval;
                _brain.Update(deltaTime, time);
            }

            if (_nextMoveTime < time)
            {
                _nextMoveTime = time + Config.MoveDelay * movementSpeedModifier;
                Move();
            }

            if (_nextAttackTime < time && Attack())
            {
                _nextAttackTime = time + Config.AttackDelay * attackSpeedModifier;
            }

            if (activeStatusEffects != null)
            {
                List<IStatusEffects<Unit>> effectsToRemove = new List<IStatusEffects<Unit>>();
                foreach (IStatusEffects<Unit> statusEffect in activeStatusEffects)
                {
                    statusEffect.UpdateDuration(this, deltaTime);
                    if (statusEffect.Duration <= 0)
                    {
                        effectsToRemove.Add(statusEffect);
                    }
                }
                foreach (var statusEffect in effectsToRemove)
                {
                    statusEffect.Remove(this);
                    activeStatusEffects.Remove(statusEffect);
                    Debug.Log($"{statusEffect.GetType()} removed for {Pos}");
                }
            }
        }

        private bool Attack()
        {
            var projectiles = _brain.GetProjectiles();
            if (projectiles == null || projectiles.Count == 0)
                return false;

            _pendingProjectiles.AddRange(projectiles);
            return true;
        }

        private void Move()
        {
            var targetPos = _brain.GetNextStep();
            var delta = targetPos - Pos;
            if (delta.sqrMagnitude > 2)
            {
                Debug.LogError($"Brain for unit {Config.Name} returned invalid move: {delta}");
                return;
            }
            if (_runtimeModel.RoMap[targetPos] ||
                _runtimeModel.RoUnits.Any(u => u.Pos == targetPos))
            {
                return;
            }
            Pos = targetPos;
        }

        public void ClearPendingProjectiles()
        {
            _pendingProjectiles.Clear();
        }

        public void TakeDamage(int projectileDamage)
        {
            Health -= projectileDamage;
        }

        public void AddStatusEffect(IStatusEffects<Unit> statusEffects)
        {
            if (statusEffects != null)
            {
                if (statusEffects.CanBeAppliedTo(this))
                {
                    statusEffects.Apply(this);
                    activeStatusEffects.Add(statusEffects);
                }
            }
        }

        public void SetMovementSpeedModifier(float modifier)
        {
            movementSpeedModifier = modifier;
        }

        public void SetAttackSpeedModifier(float modifier)
        {
            attackSpeedModifier = modifier;
        }

        public void ModifyAttackRange(float modifier)
        {
            attackRangeModifier = modifier;
            ThirdUnitBrain.AttackRangeModifier = attackRangeModifier;
        }

        public void SetDoubleAttackActive(bool isActive)
        {
            SecondUnitBrain.IsDoubleAttackActive = isActive;
        }
    }
}