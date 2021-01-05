using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CollisionControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite Right;
    public Sprite Cross;
    public void ButtonClick()
    {
        Ranking.CollisionEnable = !Ranking.CollisionEnable;
        if(Ranking.CollisionEnable)
        {
            this.GetComponent<Image>().sprite = Right;
        }
        else
        {
            this.GetComponent<Image>().sprite = Cross;
        }
    }
    
}
