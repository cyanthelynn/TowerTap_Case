using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerTap
{
    public class InputManager : MonoBehaviour
    {
        public bool TryClickActionInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverUI(-1, Input.mousePosition))
                    return true;
            }
        
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (!IsPointerOverUI(touch.fingerId, touch.position))
                        return true;
                }
            }

            return false;
        }
    
        private bool IsPointerOverUI(int pointerId, Vector2 screenPosition)
        {
            if (EventSystem.current == null)
                return false;
        
            var pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = pointerId,
                position  = screenPosition
            };
        
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            return results.Count > 0;
        }
    }
}