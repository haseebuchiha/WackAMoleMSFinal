﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameLogic : MonoBehaviour
{
    /// <summary>
    /// The moles in the scene.
    /// </summary>
    public Mole[] moles;

    /// <summary>
    /// All the mole types, MoleData is a scriptable object.
    /// </summary>
    public MoleData[] moleData;


    /// <summary>
    /// The time interval to spawn a new mole.
    /// </summary>
    public float spawnTimer;

    /// <summary>
    /// Score handles the score and display it.
    /// </summary>
    public Score score;

    /// <summary>
    /// Timer handels the game round time and riase an event when time is up to indicate the end of the round.
    /// </summary>
    public Timer timer;

    /// <summary>
    /// Changes the game ui state from "gameplay" to "menu" and viseversa.
    /// </summary>
    public GameUI ui;

    /// <summary>
    /// Location is responable for choosing a random location to spawn the mole at.
    /// </summary>
    public RandomLocation location;


    /// <summary>
    /// The number of moles currently on screen.
    /// </summary>
    private int currentMolesOnScreen;

    /// <summary>
    /// The current score of an active game round.
    /// </summary>
    private int points;

    /// <summary>
    /// All the disabled mole. we use one of them when we need to spawn a new mole.
    /// </summary>
    private List<Mole> disabledMoles = new List<Mole>();

    /// <summary>
    /// Wait time used to spawning coroutine.
    /// </summary>
    private WaitForSeconds wait;




    public GameObject[] cutscenes;

    [Header("Start Panel")]
    public GameObject startPanel;

    public GameObject cutscenespanel;
    public GameObject scorecanvas;

    [SerializeField]
    private int currentcutscene = 0;
    private void Awake()
    {
        points = 0;
        cutscenespanel.SetActive(false);
        scorecanvas.SetActive(true);
        for (int i = 0; i < cutscenes.Length; i++)
        {
            cutscenes[i].SetActive(false);
        }

        // Listen to all the moles' click event.
        foreach (Mole m in moles)
        {
            m.OnMoleDied += MoleDied;
        }

        // liseten to the timer's timeout event.
        timer.OnTimeOut += GameOver;

        wait = new WaitForSeconds(spawnTimer);
    }

    private void Start()
    {
        startPanel.SetActive(true);
    }


    /// <summary>
    /// Call back when a mole was clicked.
    /// if clicked is true. the mole was actually click and we need to add to the score.
    /// </summary>
    /// <param name="mole"></param>
    /// <param name="clicked"></param>
    private void MoleDied(Mole mole, bool clicked)
    {
        location.FreeLocation(mole);
        disabledMoles.Add(mole);
        currentMolesOnScreen--;

        if (clicked)
        {
            points += mole.data.points;
            score.UpdateScore(points);
        }

        SpawnImmediate();
    }

    //public void startcutscenes()
    //{

    //    cutscenespanel.SetActive(true);
    //    scorecanvas.SetActive(false);
    //    currentcutscene = 0;
    //    cutscenes[0].SetActive(true);
    //}

    //public void changeCutScene()
    //{

    //    if (currentcutscene <= 9)
    //    {//cutscenes[currentcutscene].SetActive(true);
    //        for (int i = 0; i < cutscenes.Length; i++)
    //        {
    //            if (i == currentcutscene)
    //            {
    //                cutscenes[i].SetActive(true);
    //            }
    //            else
    //            {
    //                cutscenes[i].SetActive(false);
    //            }


    //        }
    //        currentcutscene++;
    //    }
    //    else
    //    {
    //        NewGame();
    //    }
    //}
    public void NewGame()
    {
        startPanel.SetActive(false);
 
       
        ui.NewGame();
        score.NewGame();
        location.NewGame();
        disabledMoles.Clear();

        foreach (Mole m in moles)
        {
            m.Despawn();
            disabledMoles.Add(m);
        }

        points = 0;
        currentMolesOnScreen = 0;

        StartCoroutine("SpawnMoles");
        SpawnImmediate();
        timer.NewGame();
    }

    private void GameOver()
    {
        StopCoroutine("SpawnMoles");
        ui.GameOver();
        score.GameOver(points);
    }

    /// <summary>
    /// Coroutine to spawn a new mole every time interval.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnMoles()
    {
        while (true)
        {
            if (currentMolesOnScreen < 7 && disabledMoles.Count > 0)
            {
                disabledMoles[0].Respawn(location.FindLocation(disabledMoles[0]), RandomMole());
                disabledMoles.RemoveAt(0);
                currentMolesOnScreen++;
            }

            yield return wait;
        }
    }

    /// <summary>
    /// Spawn moles immediatly to make sure at least 5 moles are on screen.
    /// </summary>
    private void SpawnImmediate()
    {
        while (currentMolesOnScreen < 5 && disabledMoles.Count > 0)
        {
            disabledMoles[0].Respawn(location.FindLocation(disabledMoles[0]), RandomMole());
            disabledMoles.RemoveAt(0);
            currentMolesOnScreen++;
        }
    }


    /// <summary>
    /// Picks a random scriptable object for the mole's data.
    /// </summary>
    /// <returns></returns>
    private MoleData RandomMole()
    {
        return moleData[UnityEngine.Random.Range(0, 3)];
    }

    public void Getspeed()
    {
        if (points % 10 == 0)
        {
            if (MoleVisuals.x <= 3.0f)
            {
                MoleVisuals.x += 1.0f;
            }

        }
    }

    public void ToScoreCard()
    {
        startPanel.SetActive(false);
        scorecanvas.SetActive(true);

    }

    public void ToMainMenu()
    {
        startPanel.SetActive(true);
        scorecanvas.SetActive(false);

    }

    IEnumerator cutSceneChanger()
    { 
        for (int i = 0; i < 10; i++)
        {
            cutscenes[i].SetActive(true);
            yield return new WaitForSeconds(2);

        }
       cutscenespanel.SetActive(false);
       NewGame();

    }

    public void tocutscene()
    {
        startPanel.SetActive(false);
        cutscenespanel.SetActive(true);
        StartCoroutine(cutSceneChanger());
       
    }
}
