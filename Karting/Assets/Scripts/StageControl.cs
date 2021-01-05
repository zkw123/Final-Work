using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StageControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        if (!Ranking.mode)
        { 
            if (Ranking.stage == 1)
            {
                this.GetComponentInChildren<TextMeshProUGUI>().text = "Second Stage";
                this.GetComponent<KartGame.UI.LoadSceneButton>().SceneName = "SinglePlayer2";
            }
            else
            {
                this.GetComponentInChildren<TextMeshProUGUI>().text = "First Stage";
                this.GetComponent<KartGame.UI.LoadSceneButton>().SceneName = "SinglePlayer";
            }
        }
        else
        {
            this.GetComponentInChildren<TextMeshProUGUI>().text = "Single Player";
            this.GetComponent<KartGame.UI.LoadSceneButton>().SceneName = "ChooseMenu";
        }
    }
    public void ChangeStage()
    {
       
            if (Ranking.stage == 1)
                Ranking.stage = 2;
            else
                Ranking.stage = 1;
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
