using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChooseStage : MonoBehaviour
{
    // Start is called before the first frame update
    public Text t;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeStage(int i)
    {
        Ranking.stage = i;
    }
    public void AICounts()
    {
        Ranking.AICount = int.Parse(t.text);
    }
}
