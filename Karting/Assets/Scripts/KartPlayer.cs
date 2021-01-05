using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
public enum DriftDirection
{
    None,
    Left,
    Right,
}
public enum DriftLevel
{
    One,
    Two,
    Three
}

[System.Obsolete]
public class KartPlayer : NetworkBehaviour
{
    
    public Rigidbody kartRigidbody;

    [Header("输入相关")]
    float v_Input;
    float h_Input;

    [Header("力的大小")]
    public float currentForce;
    public float normalForce = 80;  
    public float boostForce = 130;  
    public float jumpForce = 10;    
    public float gravity = 40;     

    
    Vector3 forceDir_Horizontal;
    float verticalModified;         

    [Header("转弯相关")]
    public bool isDrifting;
    public DriftDirection driftDirection = DriftDirection.None;
    [Tooltip("由h_Input以及漂移影响")]
    public Quaternion rotationStream;   
    public float turnSpeed = 60;
    
    //Drift()
    Quaternion m_DriftOffset = Quaternion.identity;
    public DriftLevel driftLevel;

    [Header("地面检测")]
    public Transform frontHitTrans;
    public Transform rearHitTrans;
    public Transform transform;
    public bool isGround;
    public bool isGroundLastFrame;
    public float groundDistance = 0.7f;

    [Header("特效")]
    public Transform wheelsParticeleTrans;
    public ParticleSystem[] wheelsParticeles;
    public TrailRenderer leftTrail;
    public TrailRenderer rightTrail;
    [Header("漂移颜色有关")]
    public Color[] driftColors;
    public float driftPower = 0;

    [Header("联机有关")]
    [SyncVar] public bool PlayerIn = false;
    //[SyncVar] public float CurrentTime=0;
    [SyncVar]public float AnotherTime = 0;
    [SyncVar] public float time = 0;
    public int OverTimeLimit = 10;
    public GameFlowManager g;
    TimeManager m_TimeManager;
    GameObject c;
    public LapObject p;
    float TimeInterval = 0;
    public Transform hats;
    /*
    public class Send_data : NetworkManager
    {
        public override void OnServerConnect(NetworkConnection Conn)
        {
            if (Conn.hostId >= 0)
            {
                this.PlayerIn = true;
                GameFlowManager.playerin = true;
                Debug.Log("New Player has joined");
            }
        }
    }
    */
    void Start()
    {
        m_TimeManager = FindObjectOfType<TimeManager>();
        hats = this.transform.FindChild("KartSuspension/PlayerIdle/Root1/Hips/Spine1/Spine2/Neck/Head/HeadEND/WizardHat");
        c = GameObject.FindGameObjectWithTag("Temporary Camera");
        g = GameObject.Find("GameManager").GetComponent<GameFlowManager>();
        p = GameObject.Find("StartFinishLine").GetComponent<LapObject>();
        forceDir_Horizontal = transform.forward;
        rotationStream = kartRigidbody.rotation;

        //漂移时车轮下粒子特效
        wheelsParticeles = wheelsParticeleTrans.GetComponentsInChildren<ParticleSystem>();
        StopDriftParticle();
       
    }
    /*
     private void OnPlayerConnected(NetworkPlayer player)
     {
         PlayerIn = true;
         GameFlowManager.playerin = true;
     }
    */
    void Update()
    {
        
            if (isLocalPlayer)
            {

                hats.gameObject.SetActive(false);
            }
            else
            {
                hats.gameObject.SetActive(true);
            }
            if (PlayerIn)
            {

                GameFlowManager.playerin = true;
                c.SetActive(false);
            }
            if (!isServer && !isLocalPlayer)
            {
                TimeManager.isServer = false;
                /*
                if (CurrentTime != 0)
                {
                    m_TimeManager.TimeRemaining = CurrentTime;
                }
                */
                Sort.AnotherTime = AnotherTime;


                GameFlowManager.EndTime = time;

            }
            if (isServer)
            {
                TimeManager.isServer = true;

                //CurrentTime = m_TimeManager.TimeRemaining;

            }
            if (!isLocalPlayer)
            {

                return;
            }

            if (p.t != null && !p.HasCollision)
            {

                if (p.t == this.gameObject.transform.FindChild("KartCollidersWithBounciness") || p.t == this.gameObject.transform)
                {

                    if (AnotherTime != 0)
                        m_TimeManager.IsOver = true;

                    if (m_TimeManager.raceStarted)
                    {
                        if (isServer)
                        {
                            time = m_TimeManager.TimeRemaining;
                        }
                        m_TimeManager.StopRace();
                    }
                    //g.EndGame(success);
                }
                else
                {
                    if (AnotherTime == 0)
                    {

                        AnotherTime = m_TimeManager.TimeRemaining;
                    }
                    if (m_TimeManager.raceStarted == false)
                    {
                        m_TimeManager.IsOver = true;

                    }

                }
            }
            if ((AnotherTime != 0 || m_TimeManager.raceStarted == false) && m_TimeManager.TimeRemaining > 10)
            {
                TimeInterval += Time.deltaTime;
                if (TimeInterval > OverTimeLimit)
                {
                    m_TimeManager.IsOver = true;
                }
            }
            
            if (m_TimeManager.IsOver)
            {
                if (isServer)
                {
                    Sort.AnotherTime = AnotherTime;
                }
               
                if ((GameFlowManager.EndTime > Sort.AnotherTime && Sort.AnotherTime != 0) || GameFlowManager.EndTime == 0)
                {


                    if (isServer)
                    {


                        g.EndGame(false);


                    }
                    else
                        g.EndGame(true);

                }
                else
                {

                    if (isServer)
                    {

                        g.EndGame(true);

                        //NetworkClient.ShutdownAll();
                    }
                    else
                        g.EndGame(false);

                }
               
               
            }
            if (NetworkServer.active)
            {
                List<NetworkIdentity> valueList = NetworkServer.objects.Values.ToList();
                int playerCount = valueList.Count(item => item.localPlayerAuthority);
                if (playerCount >= 2)
                {
                    PlayerIn = true;

                    /*
                    if(OtherPlayer==null)
                    {
                        GameObject[] g = GameObject.FindGameObjectsWithTag("Player");
                        for(int i=0;i<g.Length;i++)
                        {
                            if (g[i] != this.gameObject)
                                OtherPlayer = g[i].GetComponent<KartPlayer>();
                        }
                    }
                    */
                }
            }

           
            if (m_TimeManager.raceStarted)
            {
                v_Input = Input.GetAxisRaw("Vertical");     
                h_Input = Input.GetAxisRaw("Horizontal");   
                                                           
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (isGround)  
                    {

                        Jump();
                    }
                }

               
                if (Input.GetKey(KeyCode.LeftShift) && h_Input != 0)
                {


                    if (isGround && !isDrifting && kartRigidbody.velocity.sqrMagnitude > 5)
                    {
                        StartDrift();  
                    }
                }

               
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    if (isDrifting)
                    {
                        Boost(boostForce);
                        StopDrift();
                    }
                }
            }
        }
    

    private void FixedUpdate()
    {
        
        
        if (!isLocalPlayer)
            return;
        
        CheckGroundNormal();        
        Turn();                    

        
        IncreaseForce();
        
        ReduceForce();


        
        if (isDrifting)
        {
            CalculateDriftingLevel();   
            ChangeDriftColor();         
        }

        
        kartRigidbody.MoveRotation(rotationStream);
        
        CalculateForceDir();
        
        AddForceToMove();
    }
 

    public void CalculateForceDir()
    {
     
        if (v_Input > 0)
        {
            verticalModified = 1;
        }
        else if (v_Input < 0)
        {
            verticalModified = -1;
        }

        forceDir_Horizontal = m_DriftOffset * transform.forward;
    }
   
 
    public void AddForceToMove()
    {
        
        Vector3 tempForce = verticalModified * currentForce * forceDir_Horizontal;

        if (!isGround)  
        {
            tempForce = tempForce + gravity * Vector3.down;
        }
        kartRigidbody.AddForce(tempForce, ForceMode.Force);
    }  

    public void CheckGroundNormal()
    {
        
        RaycastHit frontHit;
        bool hasFrontHit = Physics.Raycast(frontHitTrans.position, -transform.up, out frontHit, groundDistance, LayerMask.GetMask("Track"));
        if (hasFrontHit)
        {
            Debug.DrawLine(frontHitTrans.position, frontHitTrans.position - transform.up * groundDistance, Color.red);
        }
        
        RaycastHit rearHit;
        bool hasRearHit = Physics.Raycast(rearHitTrans.position, -transform.up, out rearHit, groundDistance, LayerMask.GetMask("Track"));
        if (hasRearHit)
        {
            Debug.DrawLine(rearHitTrans.position, rearHitTrans.position - transform.up * groundDistance, Color.red);
        }
        isGroundLastFrame = isGround;
        if (hasFrontHit || hasRearHit)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
        
        
        Vector3 tempNormal = (frontHit.normal + rearHit.normal).normalized;
        Quaternion quaternion = Quaternion.FromToRotation(transform.up, tempNormal);
        rotationStream = quaternion * rotationStream;
    }

    public void ReduceForce()
    {
        float targetForce = currentForce;
        if (isGround && v_Input == 0)
        {
            targetForce = 0;
        }
        else if (currentForce > normalForce)   
        {
            targetForce = normalForce;
        }

        if (currentForce <= normalForce)
        {
            DisableTrail();
        }
        currentForce = Mathf.MoveTowards(currentForce, targetForce,30 * Time.fixedDeltaTime);
    }

   
    public void IncreaseForce()
    {
        float targetForce = currentForce;
        if (v_Input != 0 && currentForce < normalForce)
        {
            currentForce = Mathf.MoveTowards(currentForce, normalForce, 60 * Time.fixedDeltaTime);
        }
    }

    public void Turn()
    {
        
        if (kartRigidbody.velocity.sqrMagnitude <= 0.1)
        {
            return;
        }

        
        if (driftDirection == DriftDirection.Left)
        {
            rotationStream = rotationStream * Quaternion.Euler(0, -60 * Time.fixedDeltaTime, 0);
        }
        else if (driftDirection == DriftDirection.Right)
        {
            rotationStream = rotationStream * Quaternion.Euler(0, 60 * Time.fixedDeltaTime, 0);
        }

        
        float modifiedSteering = Vector3.Dot(kartRigidbody.velocity, transform.forward) >= 0 ? h_Input : -h_Input;

        
        turnSpeed = driftDirection != DriftDirection.None ? 30 : 60;
        float turnAngle = modifiedSteering * turnSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, turnAngle, 0);

        rotationStream = rotationStream * deltaRotation;
    }

    public void Jump()
    {
        kartRigidbody.AddForce(jumpForce * transform.up, ForceMode.Impulse);
    }



    public void StartDrift()
    {
        Debug.Log("Start Drift");
        isDrifting = true;
        
        
        if (h_Input < 0)
        {
            driftDirection = DriftDirection.Left;
          
            m_DriftOffset = Quaternion.Euler(0f, 30, 0f);
        }
        else if (h_Input > 0)
        {
            driftDirection = DriftDirection.Right;
            m_DriftOffset = Quaternion.Euler(0f, -30, 0f);
        }

 
        PlayDriftParticle();
    }

    public void CalculateDriftingLevel()
    {
        driftPower += Time.fixedDeltaTime;

        if (driftPower < 0.7)   
        {
            driftLevel = DriftLevel.One;
        }
        else if (driftPower < 1.4)
        {
            driftLevel = DriftLevel.Two;
        }
        else
        {
            driftLevel = DriftLevel.Three;
        }
    }



    public void StopDrift()
    {
        isDrifting = false;
        driftDirection = DriftDirection.None;
        driftPower = 0;
        m_DriftOffset = Quaternion.identity;
        StopDriftParticle();
    }

    public void Boost(float boostForce)
    {
        currentForce = (1 + (int)driftLevel / 5) * boostForce;
        EnableTrail();
    }


    public void PlayDriftParticle()
    {
        foreach (var tempParticle in wheelsParticeles)
        {
            tempParticle.Play();
        }
    }

    public void ChangeDriftColor()
    {
        foreach (var tempParticle in wheelsParticeles)
        {
            var t = tempParticle.main;
            t.startColor = driftColors[(int)driftLevel];
        }
    }


    public void StopDriftParticle()
    {
        foreach (var tempParticle in wheelsParticeles)
        {
            tempParticle.Stop();
        }
    }

    public void EnableTrail()
    {
        leftTrail.enabled = true;
        rightTrail.enabled = true;
    }

    public void DisableTrail()
    {
        leftTrail.enabled = false;
        rightTrail.enabled = false;
    }
}
