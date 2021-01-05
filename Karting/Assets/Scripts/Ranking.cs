using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class Ranking : MonoBehaviour
{
    // Start is called before the first frame update
    public Text t;
    public static int stage=1;
    public static bool mode= false;//false-SinglePlayer,true-MultiPlayer
    public static int AICount = 3;
    public static bool CollisionEnable = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetText(int index)
    {
        string FileName =Application.dataPath + "/StreamingAssets/"  + "Records" + index.ToString()+".txt";
        t.text = "";
        string[] strs = File.ReadAllLines(FileName);
        for(int i=0;i<strs.Length;i++)
        {
            t.text += strs[i] + '\n';
        }
        
    }
}
