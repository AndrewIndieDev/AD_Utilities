using UnityEngine;

namespace AndrewDowsett.CustomGrid
{
    public class OccupyGridSpaces : MonoBehaviour, IGridCellOccupier
    {
        public Vector2 GridPosition => GridComponent.Instance.Grid.WorldToGridPosition(transform.position);

        [SerializeField] private Vector2 occupySize;

        private void Start()
        {
            for (int x = 0; x < occupySize.x; x++)
            {
                for (int y = 0; y < occupySize.y; y++)
                {
                    GridComponent.Instance.Grid.GetCell(GridPosition + new Vector2(x, y)).SetOccupier(this);
                }
            }
        }
    }
}