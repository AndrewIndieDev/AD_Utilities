using UnityEngine;

namespace AndrewDowsett.CustomGrid
{
    public interface IGridCellOccupier
    {
        public Vector2 GridPosition { get; }
    }
}