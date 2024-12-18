using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using System.Collections.Generic;
using System.Linq;
using AndrewDowsett.CustomGrid;
using AndrewDowsett.Utility;
using static AndrewDowsett.Pathfinding.AStar.Pathfinding;

namespace AndrewDowsett.Pathfinding.AStar
{
    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding Instance;
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            UnitTest.Execute(()=> FindPath(new Vector2Int(0, 0), new Vector2Int(99, 99), GridComponent.Instance.Grid), 1f, 10);
        }

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int endPosition, MyGrid grid)
        {
            int testCount = 100;

            FindPathJob[] pathJobs = new FindPathJob[testCount];
            NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(testCount, Allocator.TempJob);
            for (int i = 0; i < testCount; i++)
            {
                FindPathJob findPathJob = new FindPathJob
                {
                    startPosition = new int2(startPosition.x, startPosition.y),
                    endPosition = new int2(UnityEngine.Random.Range(0, grid.GridWidth), UnityEngine.Random.Range(0, grid.GridHeight)),
                    gridSize = new int2(grid.GridWidth, grid.GridHeight),
                    result = new NativeList<int2>(Allocator.Persistent)
                };
                pathJobs[i] = findPathJob;
                jobHandles[i] = pathJobs[i].ScheduleByRef();
            }
            JobHandle.CompleteAll(jobHandles);

            for (int i = 0; i < testCount; i++)
            {
                pathJobs[i].result.Dispose();
            }
            
            jobHandles.Dispose();

            return null;
        }

        [BurstCompile]
        public struct FindPathJob : IJob
        {
            public int2 startPosition;
            public int2 endPosition;
            public int2 gridSize;
            public NativeList<int2> result;
            public void Execute()
            {
                NativeArray<PathNode> pathNodeArray = new(gridSize.x * gridSize.y, Allocator.Temp);

                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        PathNode pathNode = new PathNode();
                        pathNode.x = x;
                        pathNode.y = y;
                        pathNode.index = CalculateIndex(x, y, gridSize.x);

                        pathNode.gCost = int.MaxValue;
                        pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                        pathNode.CalculateFCost();

                        pathNode.isWalkable = true;
                        pathNode.cameFromNodeIndex = -1;

                        pathNodeArray[pathNode.index] = pathNode;
                    }
                }

                NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
                neighbourOffsetArray[0] = new int2(-1, +0); // left
                neighbourOffsetArray[1] = new int2(+1, +0); // right
                neighbourOffsetArray[2] = new int2(+0, +1); // up
                neighbourOffsetArray[3] = new int2(+0, -1); // down
                neighbourOffsetArray[4] = new int2(-1, -1); // left down
                neighbourOffsetArray[5] = new int2(-1, +1); // left up
                neighbourOffsetArray[6] = new int2(+1, -1); // right down
                neighbourOffsetArray[7] = new int2(+1, +1); // right up

                int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

                PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
                startNode.gCost = 0;
                startNode.CalculateFCost();
                pathNodeArray[startNode.index] = startNode;

                NativeList<int> openList = new(Allocator.Temp);
                NativeList<int> closedList = new(Allocator.Temp);

                openList.Add(startNode.index);

                while (openList.Length > 0)
                {
                    int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                    PathNode currentNode = pathNodeArray[currentNodeIndex];

                    if (currentNodeIndex == endNodeIndex)
                    {
                        // Reached destination
                        break;
                    }

                    for (int i = 0; i < openList.Length; i++)
                    {
                        if (openList[i] == currentNodeIndex)
                        {
                            openList.RemoveAtSwapBack(i);
                            break;
                        }
                    }

                    closedList.Add(currentNodeIndex);

                    for (int i = 0; i < neighbourOffsetArray.Length; i++)
                    {
                        int2 neighbouroffset = neighbourOffsetArray[i];
                        int2 neighbourPosition = new int2(currentNode.x + neighbouroffset.x, currentNode.y + neighbouroffset.y);

                        if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                            continue;

                        int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);
                        if (closedList.Contains(neighbourNodeIndex))
                            continue;

                        PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                        if (!neighbourNode.isWalkable)
                            continue;

                        int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                        int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                        if (tentativeGCost < neighbourNode.gCost)
                        {
                            neighbourNode.cameFromNodeIndex = currentNodeIndex;
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.CalculateFCost();
                            pathNodeArray[neighbourNodeIndex] = neighbourNode;

                            if (!openList.Contains(neighbourNode.index))
                                openList.Add(neighbourNode.index);
                        }
                    }
                }

                PathNode endNode = pathNodeArray[endNodeIndex];
                if (endNode.cameFromNodeIndex == -1)
                {
                    // No path
                    Debug.Log("No path found!");
                }
                else
                {
                    // Found path
                    CalculatePath(pathNodeArray, endNode);
                }

                pathNodeArray.Dispose();
                openList.Dispose();
                closedList.Dispose();
                neighbourOffsetArray.Dispose();
            }

            private void CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
            {
                result.Add(new int2(endNode.x, endNode.y));

                PathNode currentNode = endNode;
                while (currentNode.cameFromNodeIndex != -1)
                {
                    PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                    result.Add(new int2(cameFromNode.x, cameFromNode.y));
                    currentNode = cameFromNode;
                }
            }

            private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
            {
                return
                    gridPosition.x >= 0 &&
                    gridPosition.x < gridSize.x &&
                    gridPosition.y >= 0 &&
                    gridPosition.y < gridSize.y;
            }

            private int CalculateIndex(int x, int y, int gridWidth)
            {
                return x + y * gridWidth;
            }

            private int CalculateDistanceCost(int2 a, int2 b)
            {
                int xDistance = math.abs(a.x - b.x);
                int yDistance = math.abs(a.y - b.y);
                int remaining = math.abs(xDistance - yDistance);
                return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
            }

            private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
            {
                PathNode lowestCodePathNode = pathNodeArray[openList[0]];
                for (int i = 1; i < openList.Length; i++)
                {
                    PathNode testPathNode = pathNodeArray[openList[i]];
                    if (testPathNode.fCost < lowestCodePathNode.fCost)
                    {
                        lowestCodePathNode = testPathNode;
                    }
                }
                return lowestCodePathNode.index;
            }
        }

        private struct PathNode
        {
            public int index;
            public int x;
            public int y;
            public int gCost;
            public int hCost;
            public int fCost;
            public bool isWalkable;
            public int cameFromNodeIndex;

            public void CalculateFCost() => fCost = gCost + hCost;
            public void SetIsWalkable(bool isWalkable) => this.isWalkable = isWalkable;
        }
    }
}