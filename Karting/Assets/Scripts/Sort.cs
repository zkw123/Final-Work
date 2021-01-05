using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
class Score
{
    public double time;
    public string date;
    
    public Score(double time, string date)
    {
        this.time = time;
        this.date = date;
    }

    public string  Data()
    {
        if(time>=100)
            return String.Format("{0:000.000}", time) + "\t\t\t\t" + date.ToString();
        else
            return String.Format("{0:00.000}", time) + "\t\t\t\t\t" + date.ToString() ;
    }

    internal int CompareTo(Score y)
    {
        if (time < y.time)
            return 1;
        else if (time > y.time)
            return -1;
        else
            return 0; 
    }
}
public class Sort : MonoBehaviour
{
    

    // Start is called before the first frame update
    List<Score>PlayerData;
    public int maxRecords = 20;
    public static double AnotherTime;
    Text t;
    string date;
    string WritingData;
    double time;
    void Start()
    {
        GameObject n = GameObject.FindGameObjectWithTag("Network");
        if (n != null)
        {
            n.GetComponent<MyNetwork>().EndNetwork();
        }
        TimeManager.isServer = true;
        
        GameFlowManager.playerin = false;
        PlayerData = new List<Score>();
        t = GetComponentInChildren<Text>();
        t.text = "";
        time = Math.Round(GameFlowManager.EndTime, 3);
        date = DateTime.Now.Year.ToString()+"/"+ DateTime.Now.Month.ToString()+"/"+ DateTime.Now.Day.ToString();
        if (Ranking.mode)
        {
            
            
            AnotherTime= Math.Round(AnotherTime, 3);
            if (time != 0)
            {
                PlayerData.Add(new Score(time, date));
            }
            
            if(AnotherTime!=0)
            {
                PlayerData.Add(new Score(AnotherTime, date));

            }
            string FileName = Application.dataPath + "/StreamingAssets/" + "Records3.txt";

            if (!File.Exists(FileName))
            {
                FileStream fs1 = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }
            string[] a = File.ReadAllLines(FileName);
            string[] data;
            if (a.Length == 0)
            {

                WritingData = "\t排名\t\t\t\t\t\t\t\t成绩\t\t\t\t\t\t日期" + "\n";
                t.text += "\t排名\t\t\t\t\t\t\t\t成绩\t\t\t\t\t\t日期" + '\n';
            }
            else
            {
                WritingData = a[0] + "\n";
                t.text += a[0] + '\n';
            }
           
            for (int i = 1; i < a.Length; i++)
            {

                data = a[i].Split(new Char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length != 0)
                {
                    Score s = new Score(Convert.ToDouble(data[1]), data[2]);
                    bool l = true;
                    for(int it=0;i<PlayerData.Count;i++)
                    {
                        if(PlayerData[it].time==s.time&& PlayerData[it].date==s.date)
                        {
                            l = false;
                            break;
                        }
                    }
                    if(l)
                    {
                        PlayerData.Add(s);
                    }
                }
            }
            PlayerData.Sort((x, y) => -x.CompareTo(y));

            for (int i = 0; i < PlayerData.Count; i++)
            {
                if (PlayerData[i].time == time && PlayerData[i].date == date)
                {
                    
                        t.text += "<color=red>" + "\t" + String.Format("{0, 3}", i + 1) + "\t\t\t\t\t\t\t\t" + PlayerData[i].Data() + "\t\t1P\n" + "</color>";
                 
                }
                else if(PlayerData[i].time == AnotherTime && PlayerData[i].date == date)
                {
                    t.text += "<color=yellow>" + "\t" + String.Format("{0, 3}", i + 1) + "\t\t\t\t\t\t\t\t" + PlayerData[i].Data() + "\t\t2P\n" + "</color>";
                    
                }
                else
                {
                    t.text += "\t" + String.Format("{0, 3}", i + 1) + "\t\t\t\t\t\t\t\t" + PlayerData[i].Data() + "\n";
                }
                if (i < maxRecords)
                {
                    WritingData += "\t" + String.Format("{0, 3}", i + 1) + "\t\t\t\t\t\t\t\t" + PlayerData[i].Data() + "\n";
                }
            }
            if (time == 0)
            {
             
                    t.text += "<color=red>" + "\t" + String.Format("{0, 3}", "/") + "\t\t\t\t\t\t\t\t\t" + "未完成" + "\t\t\t\t\t" + date.ToString() + "\t\t1P\n" + "</color>";
               
                
            }
            if (AnotherTime == 0)
            {
                
                    t.text += "<color=yellow>" + "\t" + String.Format("{0, 3}", "/") + "\t\t\t\t\t\t\t\t\t" + "未完成" + "\t\t\t\t\t" + date.ToString() + "\t\t2P\n" + "</color>";
                
               
            }
            GameFlowManager.EndTime = 0;
            Sort.AnotherTime =0;
            WriteFile(Application.dataPath + "/StreamingAssets/", "Records3.txt", WritingData);
        }
        else
        {
            
            if (time != 0)
            {
                PlayerData.Add(new Score(time, date));
            }
            string FileName =  Application.dataPath + "/StreamingAssets/"  + "Records"+Ranking.stage.ToString() +  ".txt";
            
            if (!File.Exists(FileName))
            {
                FileStream fs1 = new FileStream(FileName, FileMode.Create,FileAccess.ReadWrite);
                fs1.Close();
            }
            string[] a = File.ReadAllLines(FileName);
            string[] data;
            if (a.Length == 0)
            {

                WritingData = "\t排名\t\t\t\t\t\t\t\t成绩\t\t\t\t\t\t日期" + "\n";
                t.text += "\t排名\t\t\t\t\t\t\t\t成绩\t\t\t\t\t\t日期" + '\n';
            }
            else
            {
                WritingData = a[0] + "\n";
                t.text += a[0] + '\n';
            }
            for (int i=1;i<a.Length;i++)
            {
                
                data= a[i].Split(new Char[]{ '\t'}, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length!=0)
                {
                    PlayerData.Add(new Score(Convert.ToDouble(data[1]), data[2]));
                }
            }
            PlayerData.Sort((x, y) => -x.CompareTo(y));
        
            for (int i = 0; i < PlayerData.Count; i++)
            {
                if(PlayerData[i].time==time&&PlayerData[i].date==date)
                {
                    t.text += "<color=red>" + "\t" + String.Format("{0, 3}", i+1) + "\t\t\t\t\t\t\t\t" + PlayerData[i].Data() + "\n</color>";
                }
                else
                {
                    t.text+= "\t" + String.Format("{0, 3}", i + 1) + "\t\t\t\t\t\t\t\t" + PlayerData[i].Data()+"\n";
                }
                if(i<maxRecords)
                {
                    WritingData += "\t" + String.Format("{0, 3}", i + 1) + "\t\t\t\t\t\t\t\t" + PlayerData[i].Data()+"\n";
                }
            }
            GameFlowManager.EndTime = 0;
            Sort.AnotherTime = 0;
            KartSinglePlayer.GameStart = false;
            WriteFile(Application.dataPath + "/StreamingAssets/", "Records"+ Ranking.stage.ToString()+ ".txt", WritingData);
            
        }
    }
    void WriteFile(string path, string name, string info)
    {          //路径、文件名、写入内容
        StreamWriter sw;
        FileInfo fi = new FileInfo(path + "//" + name);
        sw = fi.CreateText();        
        sw.Write(info);
        sw.Close();
        sw.Dispose();
    }
    // Update is called once per frame
    
}
