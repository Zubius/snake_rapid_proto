using UnityEngine;

namespace View
{
    public class GameCellView : MonoBehaviour
    {
        [SerializeField] private Transform cachedTransform;
        [SerializeField] private Renderer cachedRenderer;

        internal void SetupCell(GameCellType type)
        {
            var active = type != GameCellType.Empty;
            cachedRenderer.enabled = active;

            if (active)
            {
                cachedRenderer.material.color = type == GameCellType.Target ? Color.red : Color.green;
            }

        }

        internal void PositionCell(Vector2 position)
        {
            cachedTransform.localPosition = new Vector3(position.x, 0, position.y);
        }
    }
}
