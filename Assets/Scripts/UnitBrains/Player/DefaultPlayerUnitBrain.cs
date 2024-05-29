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
            Vector2Int recommendedTarget = UnitsCoordinator.GetInstance().recommendedTarget;
            Vector2Int recommendedPoint = UnitsCoordinator.GetInstance().recommendedPoint;

            AStarUnitPath path = new AStarUnitPath(runtimeModel, unit.Pos, recommendedPoint);
            if (HasTargetsInRange())
            {
                if (IsTargetInRange(recommendedTarget))
                {
                    return unit.Pos;
                }
                return path.GetNextStepFrom(unit.Pos);
            }
            return base.GetNextStep();
        }
    }
}
