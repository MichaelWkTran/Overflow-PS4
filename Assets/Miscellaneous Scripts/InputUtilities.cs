using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class InputUtilities
{
    public static bool IsMouseOverUI(Func<RaycastResult, bool> _ignoreCondition = null)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = (Vector2)Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach(RaycastResult result in results)
        {
            if (_ignoreCondition != null && _ignoreCondition.Invoke(result)) continue;
            return true;
        }

        return false;
    }

    public static bool CanUseJumpInput()
    {
        return Input.GetButton("Jump") && !IsMouseOverUI((RaycastResult _raycastResult) =>
            { return _raycastResult.gameObject.layer != 2; });
    }
}
