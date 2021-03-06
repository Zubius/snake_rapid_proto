using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    internal class Snake
    {
        private LinkedList<Vector2> _cells;

        internal Snake(Vector2[] snakeCells)
        {
            _cells = new LinkedList<Vector2>(snakeCells);
        }

        internal Vector2 Head => _cells.First.Value;
        internal Vector2 Tail => _cells.Last.Value;

        private bool ReachTarget(GameCell target)
        {
            return target.Type == GameCellType.Target;
        }

        internal void MoveTo(GameCell position)
        {
            _cells.AddFirst(position.Position);
            if (!ReachTarget(position))
                _cells.RemoveLast();
        }
    }
}
