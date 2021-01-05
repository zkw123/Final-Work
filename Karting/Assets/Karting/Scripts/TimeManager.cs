﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{ 
    public bool IsFinite { get; private set; }
    public float TotalTime { get; private set; }
    public float TimeRemaining { get; set; }
    public bool IsOver;
    public GameObject[] ai;
    public bool raceStarted { get; set; }
    public static bool isServer=true;
    public static Action<float> OnAdjustTime;
    public static Action<int, bool, GameMode> OnSetTime;

    private void Awake()
    {
        IsFinite = false;
        TimeRemaining = 0f;
        raceStarted = false;
        ai = GameObject.FindGameObjectsWithTag("AI");
        for (int i = 0; i < ai.Length; i++)
        {
            ai[i].GetComponent<KartGame.AI.KartAgent>().enabled = false;
        }
    }


    void OnEnable()
    {
        OnAdjustTime += AdjustTime;
        OnSetTime += SetTime;
    }

    private void OnDisable()
    {
        OnAdjustTime -= AdjustTime;
        OnSetTime -= SetTime;
    }

    private void AdjustTime(float delta)
    {
        TimeRemaining += delta;
    }

    private void SetTime(int time, bool isFinite, GameMode gameMode)
    {
        TotalTime = time;
        IsFinite = isFinite;
        TimeRemaining = 0;
    }

    void Update()
    {
        
        if (!Ranking.mode)
        {
            if (!raceStarted)
            {

                return;
            }
            KartSinglePlayer.GameStart = raceStarted;

            if (IsFinite && !IsOver && raceStarted)
            {
                if (isServer)
                {
                    TimeRemaining += Time.deltaTime;
                }

            }
        }
        else
        {
            if (!IsOver&&TimeRemaining!=0||raceStarted)
            {
                TimeRemaining += Time.deltaTime;
            }

        }
    }

    public void StartRace()
    {
        raceStarted = true;
        for(int i=0;i<ai.Length;i++)
        {
            ai[i].GetComponent<KartGame.AI.KartAgent>().enabled = true;
        }    

    }

    public void StopRace() {
        raceStarted = false;
        for (int i = 0; i < ai.Length; i++)
        {
            ai[i].GetComponent<KartGame.AI.KartAgent>().enabled = false;
        }
        if(Ranking.mode&&isServer)
        {
            GameFlowManager.EndTime = TimeRemaining;
        }
    }
}

