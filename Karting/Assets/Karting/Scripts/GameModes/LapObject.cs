using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// This class inherits from TargetObject and represents a LapObject.
/// </summary>
public class LapObject : TargetObject
{
    [Header("LapObject")]
    [Tooltip("Is this the first/last lap object?")]
    public bool finishLap;
    public Transform t;
    public  bool HasCollision = false;
     float time = 0;
    public List<Transform> player;
    [HideInInspector]
    public bool lapOverNextPass;
    TimeManager m_TimeManager;
    void Start() {
        t = null;
        Register();
        m_TimeManager = FindObjectOfType<TimeManager>();
        if(Ranking.mode)
        {
            player = new List<Transform>();
        }
    }
    private void Update()
    {
        if(HasCollision)
        {
            time += Time.deltaTime;
            if(time>2)
            {
                time = 0;
                HasCollision = false;
                t = null;
            }
        }
    }
    void OnEnable()
    {
        lapOverNextPass = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!((layerMask.value & 1 << other.gameObject.layer) > 0 && other.CompareTag("Player")))
        {
            if (!Ranking.mode)
            {
                if (GameObject.Find("Checkpoint2") == null)
                {
                    
                    t = other.gameObject.transform;
                    return;
                }
            }
            else
            {
                
                    if (player.Contains(other.gameObject.transform)&& GameObject.Find("Checkpoint2") == null)
                        t = other.gameObject.transform;
                    else
                    {
                        player.Add(other.gameObject.transform);
                        HasCollision = true;
                    }
                
            }
        }
        if(other.gameObject.transform.parent!=null&&other.gameObject.transform.parent.tag=="AI"&&m_TimeManager.TimeRemaining>5)
        {
            t = other.gameObject.transform;
            other.gameObject.transform.parent.GetComponent<KartGame.AI.KartAgent>().enabled = false;
            
            other.gameObject.transform.parent.GetComponent<KartGame.KartSystems.ArcadeKart>().baseStats.Steer = 0;
            other.gameObject.transform.parent.GetComponent<KartGame.KartSystems.ArcadeKart>().baseStats.TopSpeed = 0;
            //other.gameObject.transform.parent.GetComponent<KartGame.KartSystems.ArcadeKart>().enabled = false;
            //other.gameObject.transform.parent.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            
        }
       
        Objective.OnUnregisterPickup?.Invoke(this);
    }
}
