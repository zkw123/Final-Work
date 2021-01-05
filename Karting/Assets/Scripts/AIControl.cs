using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ai1;
    public GameObject ai2;
    public GameObject ai3;
    void Start()
    {
        if(Ranking.AICount==1)
        {
            ai2.SetActive(false);
            ai3.SetActive(false);
        }
        if (Ranking.AICount == 2)
        {
            
            ai3.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
