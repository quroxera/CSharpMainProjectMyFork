using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UnitBrains.Player;
using Codice.Client.Commands;
using Codice.Client.Common;
using Codice.CM.Client.Differences;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;

namespace Model.Runtime
{
    public class Unit : IReadOnlyUnit
    {
        public UnitConfig Config { get; }
        public Vector2Int Pos { get; private set; }
        public int Health { get; private set; }
        public bool IsDead => Health <= 0;
        public BaseUnitPath ActivePath => _brain?.ActivePath;
        public IReadOnlyList<BaseProjectile> PendingProjectiles => _pendingProjectiles;

        private readonly List<BaseProjectile> _pendingProjectiles = new();
        private IReadOnlyRuntimeModel _runtimeModel;
        private BaseUnitBrain _brain;

        private StatusEffectsSystem _statusEffectsSystem;

        private float _nextBrainUpdateTime = 0f;
        private float _nextMoveTime = 0f;
        private float _nextAttackTime = 0f;

        public Unit(UnitConfig config, Vector2Int startPos, UnitsCoordinator unitsCoordinator)
        {
            Config = config;
            Pos = startPos;
            Health = config.MaxHealth;
            _brain = UnitBrainProvider.GetBrain(config);
            _brain.SetUnit(this);
            _brain.SetCoordinator(unitsCoordinator);
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();

            _statusEffectsSystem = ServiceLocator.Get<StatusEffectsSystem>();

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
                float moveModifier = 1f;

                if (Random.Range(1, 100) <= 10)
                {
                    ServiceLocator.Get<StatusEffectsSystem>().AddStatusEffect(this, new SpeedBuff(this));
                    moveModifier = _statusEffectsSystem.GetMovementSpeedModifier(this);
                }
                _nextMoveTime = time + Config.MoveDelay * moveModifier;
                Debug.Log("Move modifier: " + moveModifier);
                //Debug.Log("Next move time: " + _nextMoveTime);
                Move();
            }

            if (_nextAttackTime < time && Attack())
            {
                float attackModifier = 1f;
                if (Random.Range(1, 100) <= 10)
                {
                    ServiceLocator.Get<StatusEffectsSystem>().AddStatusEffect(this, new AttackSpeedBuff(this));
                    attackModifier = _statusEffectsSystem.GetAttackSpeedModifier(this);
                }
                _nextAttackTime = time + Config.AttackDelay * attackModifier;
                Debug.Log("AttackModifier: " + attackModifier);
                //Debug.Log("Next attack time: " + _nextAttackTime);
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
            if (Random.Range(1, 100) <= 5)
            {
                ServiceLocator.Get<StatusEffectsSystem>().AddStatusEffect(this, new SlowDebuff(this));
                ServiceLocator.Get<StatusEffectsSystem>().AddStatusEffect(this, new AttackSpeedDebuff(this));
            }
        }
    }
}