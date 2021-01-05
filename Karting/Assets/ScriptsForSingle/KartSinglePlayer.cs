using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class KartSinglePlayer : MonoBehaviour
{
    public Rigidbody kartRigidbody;
    public BoxCollider[] b;
    [Header("输入相关")]
    float v_Input;
    float h_Input;

    [Header("力的大小")]
    public float currentForce;
    public float normalForce = 80;  
    public float boostForce = 130;  
    public float jumpForce = 10;   
    public float gravity = 40;   

    //力的方向
    Vector3 forceDir_Horizontal;
    float verticalModified;        

    [Header("转弯相关")]
    public bool isDrifting;
    public DriftDirection driftDirection = DriftDirection.None;
    [Tooltip("由h_Input以及漂移影响")]
    public Quaternion rotationStream;  
    public float turnSpeed = 60;
    public static bool GameStart = false;
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
    public LapObject p;
    public GameFlowManager g;
    public bool success=true;
    void Start()
    {
        if (!Ranking.CollisionEnable)
        {
            for(int i=0;i<b.Length;i++)
            b[i].enabled = false;
        }
        g = GameObject.Find("GameManager").GetComponent<GameFlowManager>();
        p = GameObject.Find("StartFinishLine").GetComponent<LapObject>();
        forceDir_Horizontal = transform.forward;
        rotationStream = kartRigidbody.rotation;
        GameFlowManager.playerin = true;
       
        wheelsParticeles = wheelsParticeleTrans.GetComponentsInChildren<ParticleSystem>();
        StopDriftParticle();
    }

    void Update()
    {
       
        if (GameStart)
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
            if (p.t != null)
            {

                //Debug.Log(p.t);
                if (p.t == this.gameObject.transform.parent.Find("KartCollidersWithBounciness") || p.t == this.gameObject.transform.parent)
                {
                    Debug.Log("success");
                    
                    g.EndGame(success);
                }
                else
                {
                    Debug.Log("fail");
                    success = false;
                    
                }
            }
        }
    }

    private void FixedUpdate()
    {
        
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
        currentForce = Mathf.MoveTowards(currentForce, targetForce, 30 * Time.fixedDeltaTime);
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
