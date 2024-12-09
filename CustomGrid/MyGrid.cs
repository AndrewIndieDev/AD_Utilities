using AndrewDowsett.Utilities;
using AndrewDowsett.Utility;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace AndrewDowsett.CustomGrid
{
    public class MyGrid
    {
        public GridCell[,] Cells { get { return cells; } }

        public int GridWidth;
        public int GridHeight;
        public float CellWidth;
        public float CellHeight;

        private Vector3 gridStartingPosition => gridComponent.transform.position;

        private GridComponent gridComponent;
        private GridCell[,] cells;

        public MyGrid(int gridWidth, int gridHeight, float cellWidth, float cellHeight, GridComponent gridComponent)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            CellWidth = cellWidth;
            CellHeight = cellHeight;
            this.gridComponent = gridComponent;

            cells = new GridCell[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    GridCell cell = new GridCell(this, new Vector2(x, y));
                    cells[x, y] = cell;
                }
            }

            UpdateCells();
        }

        public GridCell GetCell(Vector3 worldPosition)
        {
            Vector2 gridPosition = WorldToGridPosition(worldPosition);
            return GetCell(gridPosition);
        }

        public GridCell GetCell(Vector2 gridPosition)
        {
            if (WithinGrid(gridPosition))
                return cells[(int)gridPosition.x, (int)gridPosition.y];
            return new GridCell(null);
        }

        public Vector2 WorldToGridPosition(Vector3 worldPosition)
        {
            Vector3 offsetWorldPosition = worldPosition - gridStartingPosition;
            float x = offsetWorldPosition.x / CellWidth;
            float y = offsetWorldPosition.z / CellHeight;
            return new Vector2(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        public Vector3 GridToWorldPosition(Vector2 gridPosition)
        {
            float x = gridPosition.x * CellWidth;
            float y = gridPosition.y * CellHeight;
            return new Vector3(x, 0, y) + gridStartingPosition;
        }

        public bool WithinGrid(Vector2 gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.x < GridWidth && gridPosition.y >= 0 && gridPosition.y < GridHeight;
        }

        public void UpdateCells()
        {
            int jobsPerHandler = 100;
            int jobCount = Mathf.CeilToInt(cells.Length / (float)jobsPerHandler);

            jobHandleArray = new NativeArray<JobHandle>(jobCount, Allocator.TempJob);
            NativeArray<RaycastHit>[] raycastHitsBatches = new NativeArray<RaycastHit>[jobCount];

            for (int iteration = 0; iteration < jobCount; iteration++)
            {
                int start = iteration * jobsPerHandler;
                int end = Mathf.Min(start + jobsPerHandler, cells.Length);

                // Create commands and hits arrays for this batch.
                NativeArray<RaycastCommand> raycastCommands = new NativeArray<RaycastCommand>(end - start, Allocator.TempJob);
                raycastHitsBatches[iteration] = new NativeArray<RaycastHit>(end - start, Allocator.TempJob);

                // Populate raycast commands for this batch.
                for (int i = start; i < end; i++)
                {
                    int x = i % cells.GetLength(0);
                    int y = i / cells.GetLength(1);

                    raycastCommands[i - start] = new RaycastCommand(
                        cells[x, y].CenterWorldPosition + Vector3.up,
                        Vector3.down,
                        new QueryParameters(GridComponent.Instance.AllMasks),
                        10f
                    );
                }

                // Schedule the raycast batch and store the handle.
                JobHandle handle = RaycastCommand.ScheduleBatch(raycastCommands, raycastHitsBatches[iteration], 1);
                jobHandleArray[iteration] = handle;

                // Schedule disposal of raycastCommands once the batch completes.
                raycastCommands.Dispose(handle);
            }

            // Complete all raycast batches.
            JobHandle.CompleteAll(jobHandleArray);
            jobHandleArray.Dispose();

            // Combine results from all batches.
            combinedHits = new NativeArray<RaycastHit>(cells.Length, Allocator.Persistent);
            int offset = 0;
            for (int i = 0; i < jobCount; i++)
            {
                NativeArray<RaycastHit>.Copy(raycastHitsBatches[i], 0, combinedHits, offset, raycastHitsBatches[i].Length);
                offset += raycastHitsBatches[i].Length;

                // Dispose each batch's hits array after copying.
                raycastHitsBatches[i].Dispose();
            }

            // Update all cells with results.
            for (int i = 0; i < combinedHits.Length; i++)
            {
                int x = i % cells.GetLength(0);
                int y = i / cells.GetLength(1);
                cells[x, y].UpdateIsWalkable(combinedHits[i].point != Vector3.zero ? true : false);
                cells[x, y].UpdateIsWalkableEdge();
            }

            combinedHits.Dispose();
        }
        private NativeArray<JobHandle> jobHandleArray;
        private NativeArray<RaycastHit> combinedHits;
    }
}