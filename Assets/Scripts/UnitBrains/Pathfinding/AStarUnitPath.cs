using Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UnitBrains.Pathfinding
{
    public class AStarUnitPath : BaseUnitPath
    {
        private Node _nearestBlockedNode;
        private bool _isTargetReached;
        private bool _isBlockedByEnemy;
        private int _maxIterations = 100;

        private int[] dx = { -1, 0, 1, 0 };
        private int[] dy = { 0, -1, 0, 1 };
        public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
        {

        }

        protected override void Calculate()
        {
            var startNode = new Node(startPoint);
            var targetNode = new Node(endPoint);

            var openList = new List<Node> { startNode };
            var closedList = new List<Node>();
            var iterationsCounter = 0;

            while (openList.Count > 0)
            {
                var currentNode = openList[0];

                foreach (var node in openList)
                {
                    if (node.Value < currentNode.Value)
                        currentNode = node;
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (_isTargetReached)
                {
                    path = FindPath(currentNode);
                    return;
                }

                CheckNeighbourNodes(currentNode, targetNode, openList, closedList);
                iterationsCounter++;

                if (iterationsCounter >= _maxIterations)
                {
                    break;
                }
            }

            if (_isBlockedByEnemy)
            {
                path = FindPath(_nearestBlockedNode);
                return;
            }

            path = new[] { startNode.Position };
        }

        private Vector2Int[] FindPath(Node currentNode)
        {
            List<Vector2Int> path = new();

            while (currentNode != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path.ToArray();
        }

        private void CheckNeighbourNodes(Node currentNode, Node targetNode, List<Node> openList, List<Node> closedList)
        {
            for (int i = 0; i < dx.Length; i++)
            {
                Vector2Int neighbourPos = new Vector2Int(currentNode.Position.x + dx[i], currentNode.Position.y + dy[i]);

                if (neighbourPos == targetNode.Position)
                    _isTargetReached = true;

                if (IsValid(neighbourPos) || _isTargetReached)
                {
                    Node neighbour = new Node(neighbourPos);

                    if (closedList.Contains(neighbour))
                        continue;

                    neighbour.Parent = currentNode;
                    neighbour.CalculateEstimate(targetNode.Position.x, targetNode.Position.y);
                    neighbour.CalculateValue();
                    openList.Add(neighbour);
                }

                if (IsBlockedByEnemy(neighbourPos) && !_isBlockedByEnemy || IsBlockedByAlly(neighbourPos))
                {
                    _isBlockedByEnemy = true;
                    _nearestBlockedNode = currentNode;
                }
            }
        }

        private bool IsValid(Vector2Int tempPos)
        {
            bool containsX = tempPos.x >= 0 && tempPos.x < runtimeModel.RoMap.Width;
            bool containsY = tempPos.y >= 0 && tempPos.y < runtimeModel.RoMap.Height;
            return containsX && containsY && runtimeModel.IsTileWalkable(tempPos);
        }

        private bool IsBlockedByEnemy(Vector2Int tempPos)
        {
            var botPos = runtimeModel.RoBotUnits.Select(u => u.Pos).Where(u => u == tempPos);
            return botPos.Any();
        }

        private bool IsBlockedByAlly(Vector2Int tempPos)
        {
            var allyPos = runtimeModel.RoPlayerUnits.Select(u => u.Pos).Where(u => u == tempPos);
            return allyPos.Any();
        }
    }
}