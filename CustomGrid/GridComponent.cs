using AndrewDowsett.Utilities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AndrewDowsett.CustomGrid
{
    public enum EGridType
    {
        TwoDimensional,
        ThreeDimensional
    }

    public enum EGridAxis
    {
        XZ, // horizontal
        XY // vertical
    }

    public class GridComponent : MonoBehaviour
    {
        public static GridComponent Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        [Header("Grid")]
        [SerializeField] private Vector2 gridSize;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private bool updateGrid;
        [Range(0.1f, 1f)][SerializeField] float updateGridTime;

        [Header("Cell LayerMasks")]
        [SerializeField] private LayerMask walkableLayerMask;
        [SerializeField] private LayerMask spawnBlockedLayerMask;
        [SerializeField] private LayerMask enemySpawnableLayerMask;
        [SerializeField] private LayerMask gridEdgeLayerMask;

        [Space(20)]
        [Header("Gizmos")]
        [SerializeField] bool showGrid;
        [SerializeField] private Color walkableColor = Color.white;
        [SerializeField] private Color walkableEdgeOrEnemySpawnerColor = Color.red;
        [SerializeField] private Color gridEdgeOrSpawnBlockedColor = Color.blue;
        [SerializeField] private float gizmosAlpha = 0.5f;

        private float updateGridTimer;
        private MyGrid grid;

        public MyGrid Grid => grid;
        public LayerMask WalkableLayerMask => walkableLayerMask;
        public LayerMask SpawnBlockedLayerMask => spawnBlockedLayerMask;
        public LayerMask EnemySpawnableLayerMask => enemySpawnableLayerMask;
        public LayerMask GridEdgeLayerMask => gridEdgeLayerMask;
        public LayerMask AllMasks => walkableLayerMask | spawnBlockedLayerMask | enemySpawnableLayerMask | gridEdgeLayerMask;

        private void Start()
        {
            grid = new MyGrid((int)gridSize.x, (int)gridSize.y, cellSize.x, cellSize.y, this);
        }

        private void Update()
        {
            if (updateGrid)
            {
                updateGridTimer -= Time.deltaTime;
                if (updateGridTimer <= 0)
                {
                    grid.UpdateCells();
                    updateGridTimer = updateGridTime;
                }
            }
        }

        public Vector2 GetGridCellSize()
        {
            return new Vector2(grid.CellWidth, grid.CellHeight);
        }

        private void OnDrawGizmos()
        {
            if (grid == null || !showGrid)
                return;

            GridCell cell;
            for (int i = 0; i < grid.Cells.Length; i++)
            {
                int x = i % grid.Cells.GetLength(0);
                int y = i / grid.Cells.GetLength(1);
                cell = grid.Cells[x, y];
                if (!cell.IsWalkable)
                    continue;
                Color alphaChange = new Color(1, 1, 1, gizmosAlpha);
                if (cell.IsOccupied)
                    Gizmos.color = Color.yellow * alphaChange;
                else
                    Gizmos.color = ((cell.IsGridEdge || cell.IsSpawnBlocked) ? gridEdgeOrSpawnBlockedColor : (cell.IsWalkableEdge || cell.IsEnemySpawnable) ? walkableEdgeOrEnemySpawnerColor : walkableColor) * alphaChange;
                Gizmos.DrawWireCube(cell.CenterWorldPosition, new Vector3(grid.CellWidth * 0.7f, 0, grid.CellHeight * 0.7f));
            }
        }
    }
}