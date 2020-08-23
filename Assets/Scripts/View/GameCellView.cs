using UnityEngine;

namespace View
{
    public class GameCellView : MonoBehaviour
    {
        [SerializeField] private Transform cachedTransform;
        [SerializeField] private Renderer cachedRenderer;

        internal void SetupCell(bool active)
        {
            cachedRenderer.enabled = active;
        }

        internal void PositionCell(Vector2 position)
        {
            cachedTransform.localPosition = new Vector3(position.x, 0, position.y);
        }
    }
}
