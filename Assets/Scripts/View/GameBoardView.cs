using UnityEngine;

namespace View
{
    public class GameBoardView : MonoBehaviour
    {
        private GameCellView[,] _gameCellRenderers;

        private bool _inited = false;

        internal void InitView(GameObject protoCell, int x, int y, float dist)
        {
            _gameCellRenderers = new GameCellView[x, y];
            var folder = this.transform;

            //center elements infront of camera
            float offset = -1 * x / 2f + 0.5f;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    _gameCellRenderers[i, j] = Instantiate(protoCell, folder).GetComponent<GameCellView>();
                    _gameCellRenderers[i, j].PositionCell(new Vector2(GetPos(i),  GetPos(j)));
                }
            }

            _inited = true;

            float GetPos(int i)
            {
                return (i + offset) * dist;
            }
        }

        internal void SetBoardState(GameCell[,] Field)
        {
            if (!_inited)
            {
                Debug.LogError($"View has not been inited!");
                return;
            }

            if (Field.GetLength(0) != _gameCellRenderers.GetLength(0) ||
                Field.GetLength(1) != _gameCellRenderers.GetLength(1))
            {
                Debug.LogError($"Different dimensions!");
                return;
            }

            for (int i = 0; i < Field.GetLength(0); i++)
            {
                for (int j = 0; j < Field.GetLength(1); j++)
                {
                    _gameCellRenderers[i, j].SetupCell(Field[i, j].Type != GameCellType.Empty);
                }
            }
        }
    }
}
