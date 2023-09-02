using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControls : MonoBehaviour
{
    

    private void LateUpdate()
    {
        foreach (Touch touch in Input.touches)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(touch.position);

            int player = TouchPosToPlayer(pos);

            if (touch.phase == TouchPhase.Began)
            {
                GameManager.instance.playerScripts[player - 1].buttonTap = true;
                GameManager.instance.playerScripts[player - 1].buttonHold = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                GameManager.instance.playerScripts[player - 1].buttonHold = false;
            }
        }
    }

    private int TouchPosToPlayer(Vector2 pos)
    {
        if (pos.x > 0f)
        {
            if (pos.y > 0f)
                return 2;
            else
                return 4;
        }
        else
        {
            if (pos.y > 0f)
                return 1;
            else
                return 3;
        }
    }
}
