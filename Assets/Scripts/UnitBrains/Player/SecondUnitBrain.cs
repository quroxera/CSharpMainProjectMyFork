using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnitBrains.Pathfinding;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private List<Vector2Int> UnreachableTargets = new List<Vector2Int>();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            float currentTemperature = GetTemperature();
            if (currentTemperature >= overheatTemperature)
            {
                return;
            }
            IncreaseTemperature();
            for (int i = 0; i <= currentTemperature; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
        }

        public override Vector2Int GetNextStep()
        {
            if (HasTargetsInRange())
            {
                return unit.Pos;
            }

            var target = runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];
            var path = new DummyUnitPath(runtimeModel, unit.Pos, target);

            return path.GetNextStepFrom(unit.Pos);
        }

        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> result = new List<Vector2Int>();
            Vector2Int target = GetClosestTarget(GetAllTargets().ToList());

            UnreachableTargets.Clear();

            if (target.magnitude != 0)
            {
                UnreachableTargets.Add(target);

                if (IsTargetInRange(target))
                {
                    result.Add(target);
                }
            }
            else
            {
                if (IsPlayerUnitBrain)
                {
                    result.Add(runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId]);
                }
                else
                {
                    result.Add(runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);
                }
            }
            return result;
        }

        private Vector2Int GetClosestTarget(List<Vector2Int> closestEnemies)
        {
            Vector2Int closestEnemy = Vector2Int.zero;
            float minDistance = float.MaxValue;
            foreach (Vector2Int target in closestEnemies)
            {
                float distanceToTarget = DistanceToOwnBase(target);
                if (distanceToTarget < minDistance)
                {
                    minDistance = distanceToTarget;
                    closestEnemy = target;
                }
            }
            if (minDistance < float.MaxValue)
            {
                return closestEnemy;
            }
            else
            {
                return Vector2Int.zero;
            }
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown / 10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if (_overheated) return (int)OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}