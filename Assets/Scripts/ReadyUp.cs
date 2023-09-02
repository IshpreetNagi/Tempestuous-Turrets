using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadyUp : MonoBehaviour
{
    public RectTransform readyCanvas;
    public RectTransform arrow;
    public RectTransform text;

    public Image arrowImg;
    public TMP_Text txt;


    public void Setup(int num)
    {
        txt.text = "PRESS\n" + GameManager.instance.controls[num - 1].ToString().ToUpper();

        arrowImg.material = GameManager.instance.mats[num-1];
        txt.color = GameManager.instance.colours[num-1];

        //if (num == 1)
        //{
        //    readyCanvas.localPosition = new Vector2(-5f, 3f);
        //}
        //else if (num == 2)
        //{
        //    readyCanvas.localPosition = new Vector2(5f, 3f);

        //}
        //else if (num == 3)
        //{
        //    readyCanvas.localPosition = new Vector2(-5f, -3f);
        //    arrow.localPosition = new Vector2(0f, -60f);
        //    arrow.rotation = Quaternion.identity;
        //    text.localPosition = new Vector2(0f, 60f);
        //}
        //else if (num == 4)
        //{
        //    readyCanvas.localPosition = new Vector2(5f, -3f);
        //    arrow.localPosition = new Vector2(0f, -60f);
        //    arrow.rotation = Quaternion.identity;
        //    text.localPosition = new Vector2(0f, 60f);
        //}

        

    }
}
