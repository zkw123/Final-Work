using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!Ranking.mode)
        {
            if(Ranking.stage==1)
            this.GetComponent<KartGame.UI.LoadSceneButton>().SceneName = "SinglePlayer";
            else
            this.GetComponent<KartGame.UI.LoadSceneButton>().SceneName = "SinglePlayer2";
        }
        else
        {
            
            this.GetComponent<KartGame.UI.LoadSceneButton>().SceneName = "MultiPlayer";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
