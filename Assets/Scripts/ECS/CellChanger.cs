using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace ECS
{
    [BurstCompile]
    public struct CellChanger : IJobParallelFor
    {
        public NativeArray<Color> Colors;
        public NativeArray<int> Indexes;

        [ReadOnly]
        public NativeArray<GameCell> Cells;

        public void Execute(int index)
        {
            Color color = Color.white;
            switch (Cells[Indexes[index]].Type)
            {
                case GameCellType.Snake:
                    color = Color.green;
                    break;
                case GameCellType.Target:
                    color = Color.red;
                    break;
            }

            Colors[Indexes[index]] = color;
        }
    }
}
