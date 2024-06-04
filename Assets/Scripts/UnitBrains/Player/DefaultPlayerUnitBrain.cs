using System.Collections.Generic;
using Assets.Scripts.UnitBrains.Player;
using Model;
using Model.Runtime.ReadOnly;
using UnitBrains.Pathfinding;
using UnityEngine;

namespace UnitBrains.Player
{
    public class DefaultPlayerUnitBrain : BaseUnitBrain
    {
        protected UnitsCoordinator unitsCoordinator = new UnitsCoordinator(RuntimeModel.PlayerId);
        protected float DistanceToOwnBase(Vector2Int fromPos) =>
            Vector2Int.Distance(fromPos, runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);

        protected void SortByDistanceToOwnBase(List<Vector2Int> list)
        {
            list.Sort(CompareByDistanceToOwnBase);
        }

        private int CompareByDistanceToOwnBase(Vector2Int a, Vector2Int b)
        {
            var distanceA = DistanceToOwnBase(a);
            var distanceB = DistanceToOwnBase(b);
            return distanceA.CompareTo(distanceB);
        }

        public override Vector2Int GetNextStep()
        {

            Vector2Int recommendedTarget = unitsCoordinator.recommendedTarget;
            Vector2Int recommendedPoint = unitsCoordinator.recommendedPoint;

            AStarUnitPath path = new AStarUnitPath(runtimeModel, unit.Pos, recommendedPoint);
            if (IsTargetInVisionRange(recommendedTarget))
            {
                if (IsTargetInRange(recommendedTarget))
                {
                    return unit.Pos;
                }
                path = new AStarUnitPath(runtimeModel, unit.Pos, recommendedTarget);
            }
            return path.GetNextStepFrom(unit.Pos);
        }

        protected override List<Vector2Int> SelectTargets()
        {
            var result = new List<Vector2Int>();

            var targets = GetReachableTargets();
            var recommendedTarget = unitsCoordinator.recommendedTarget;

            if (targets.Contains(recommendedTarget))
                result.Add(recommendedTarget);

            return result;

        }

        private bool IsTargetInVisionRange(Vector2Int targetPos)
        {
            float visionRangeSqr = unit.Config.AttackRange* unit.Config.AttackRange * 2;
            var diff = targetPos - unit.Pos;
            return diff.sqrMagnitude <= visionRangeSqr;
        }
    }
}
