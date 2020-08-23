using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace View
{
    public class DragController : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [NonSerialized]
        public Vector2 Direction = Vector2.left;

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var directionV2 = (eventData.position - eventData.pressPosition).normalized;

            float positiveX = Mathf.Abs(directionV2.x);
            float positiveY = Mathf.Abs(directionV2.y);

            if (positiveX > positiveY)
            {
                Direction = (directionV2.x > 0) ? Vector2.right : Vector2.left;
            }
            else
            {
                Direction = (directionV2.y > 0) ? Vector2.up : Vector2.down;
            }
        }
    }
}
