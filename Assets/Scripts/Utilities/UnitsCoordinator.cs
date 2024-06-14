using Model;
using Model.Runtime.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utilities;

namespace Assets.Scripts.UnitBrains.Player
{
    public class UnitsCoordinator
    {
        private IReadOnlyRuntimeModel _runtimeModel;
        private TimeUtil _timeUtil;
        private float _attackRange;
        private int _unitID;
        private bool _isEnemyOnUnitSide = false;
        public Vector2Int recommendedTarget;
        public Vector2Int recommendedPoint;

        public UnitsCoordinator(int unitID)
        {
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
            _timeUtil = ServiceLocator.Get<TimeUtil>();
            _unitID = unitID;
            if (_runtimeModel.RoPlayerUnits.Any())
            {
                _attackRange = _unitID == RuntimeModel.PlayerId
                ? _runtimeModel.RoPlayerUnits.First().Config.AttackRange
                : _runtimeModel.RoBotUnits.First().Config.AttackRange;
            }
            _timeUtil.AddUpdateAction(UpdateRecommendations);
        }

        private void UpdateRecommendations(float deltaTime)
        {
            List<IReadOnlyUnit> targetList = _unitID == RuntimeModel.PlayerId 
                ? _runtimeModel.RoBotUnits.ToList() 
                : _runtimeModel.RoPlayerUnits.ToList();
            //Debug.Log($"TargetList Count: {targetList.Count}");

            if (targetList.Count == 0)
            {
                Vector2Int targetBasePos = _runtimeModel.RoMap.Bases[Math.Abs(_unitID - 1)];
                recommendedTarget = targetBasePos;
                recommendedPoint = targetBasePos;
            }
            else
            {
                IsEnemyOnUnitSide(targetList);
                SelectRecommendedPoint(targetList);
                SelectRecommendedTarget(targetList);
            }
        }

        public void SelectRecommendedTarget(List<IReadOnlyUnit> targetList)
        {
            //Debug.Log("IsEnemyOnUnitSide: " + _isEnemyOnUnitSide + " " + _unitID);
            if (_isEnemyOnUnitSide)
            {
                SortByDistanceToBase(targetList);
            }
            else
            {
                SortByHealth(targetList);
            }
            if (targetList.Count > 0)
            {
                recommendedTarget = targetList.First().Pos;
                //Debug.Log($"Recommended target: {recommendedTarget}");
            }
            else
            {
                if (_unitID == RuntimeModel.PlayerId)
                {
                    recommendedTarget = _runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];
                }
                else if (_unitID == RuntimeModel.BotPlayerId)
                {
                    recommendedTarget = _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId];
                }
            }
        }

        private void SelectRecommendedPoint(List<IReadOnlyUnit> targetList)
        {
            if (_isEnemyOnUnitSide)
            {
                recommendedPoint = _runtimeModel.RoMap.Bases[_unitID] + Vector2Int.one;
                //Debug.Log("RecommendedPoint: " + recommendedPoint + " " + _unitID);
            }
            else
            {
                SortByDistanceToBase(targetList);

                int x = targetList.First().Pos.x;
                int y = targetList.First().Pos.y - (int)Math.Round(_attackRange);

                recommendedPoint = new Vector2Int(x, y);
                //Debug.Log("RecommendedPoint: " + recommendedPoint + " " + _unitID);
            }
        }

        public void IsEnemyOnUnitSide(List<IReadOnlyUnit> targetList)
        {
            _isEnemyOnUnitSide = false;
            int unitSideY = _runtimeModel.RoMap.Height / 2;
            //Debug.Log("Unit side Y: " + unitSideY + " " + _unitID);
            foreach (var target in targetList)
            {
                if (_unitID == RuntimeModel.PlayerId 
                    ? target.Pos.y < unitSideY 
                    : target.Pos.y > unitSideY)
                {
                    _isEnemyOnUnitSide = true;
                    return;
                }
            }
        }

        private void SortByDistanceToBase(List<IReadOnlyUnit> targetList)
        {
            targetList.Sort(CompareByDistanceToBase);
        }

        private void SortByHealth(List<IReadOnlyUnit> targetList)
        {
            targetList.Sort(CompareByHealth);
        }

        private int CompareByDistanceToBase(IReadOnlyUnit targetA, IReadOnlyUnit targetB)
        {
            var basePos = _runtimeModel.RoMap.Bases[_unitID];
            var distanceA = Vector2Int.Distance(targetA.Pos, basePos);
            var distanceB = Vector2Int.Distance(targetB.Pos, basePos);
            return distanceA.CompareTo(distanceB);
        }

        private int CompareByHealth(IReadOnlyUnit targetA, IReadOnlyUnit targetB)
        {
            var healthA = targetA.Health;
            var healthB = targetB.Health;
            return healthA.CompareTo(healthB);
        }
    }
}
