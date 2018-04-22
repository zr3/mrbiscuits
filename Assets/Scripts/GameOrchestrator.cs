﻿using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOrchestrator : MonoBehaviour {

    public enum GamePhase { LearnActions, FeedWater, FeedVeggies, HelloBats, FeedMeats, Run, Win }

    public GameObject batSpawner;
    public GameObject mrBiscuits1;
    public GameObject mrBiscuits2;
    private GameObject cachedMrBiscuits2;
    public GameObject mrBiscuits3;
    private GameObject cachedMrBiscuits3;
    public GameObject mrBiscuits4;
    private GameObject cachedMrBiscuits4;

    private GamePhase phase;
    private static GameOrchestrator _instance;

    public static GamePhase Phase { get { return _instance.phase; } }

    public static void NextPhase(bool onlyLog = false)
    {
        _instance.EnterPhase((GamePhase)((int)_instance.phase + 1), onlyLog: onlyLog);
    }

    public static void PlayerKilled()
    {
        MessageController.AddMessage("You have fed Mr. Biscuits the ultimate meal", true);
        MessageController.AddMessage("Game over");
        MessageController.AddMessage("Time portal opening..");
        _instance.StartCoroutine(_instance.RestartGame());
    }

    public static void MrBiscuitsKilled()
    {
        MessageController.AddMessage("Mr. Biscuits has fed them", true);
        MessageController.AddMessage("Game over");
        MessageController.AddMessage("Time portal opening..");
        _instance.StartCoroutine(_instance.RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(10);
        ScreenFader.FadeOutThen(() => SceneManager.LoadScene("game"));
    }

    void Start () {
        _instance = this;
        if (PlayerPrefs.HasKey("level") && PlayerPrefs.GetInt("level") > 0)
        {
            int level = PlayerPrefs.GetInt("level");
            for (int i = 0; i < level; ++i)
            {
                NextPhase(onlyLog: i < level - 1);
            }
        } else
        {
            EnterPhase(GamePhase.LearnActions);
        }
        ScreenFader.FadeIn();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                ScreenFader.FadeOutThen(() => {
                    NextPhase();
                    MessageController.AddMessage("CHEAT: Next Phase");
                    ScreenFader.FadeIn();
                });
            }
            if (Input.GetKeyDown(KeyCode.Home))
            {
                MessageController.AddMessage("CHEAT: Clear Save");
                PlayerPrefs.DeleteAll();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.End))
            {
                SceneManager.LoadScene("game");
            }
        }
    }
    
    private void EnterPhase(GamePhase phase, bool onlyLog = false)
    {
        this.phase = phase;
        switch (phase)
        {
            case GamePhase.LearnActions:
                PlayerPrefs.SetInt("level", 0);
                MessageController.AddMessage("Your farm is doing well", true, onlyLog: onlyLog);
                MessageController.AddMessage("Water the plants and feed them to your chickens", onlyLog: onlyLog);
                MessageController.AddMessage("Eating will keep your energy up", onlyLog: onlyLog);
                break;
            case GamePhase.FeedWater:
                PlayerPrefs.SetInt("level", 1);
                mrBiscuits1.SetActive(true);
                MessageController.AddMessage("Look at it", true, onlyLog: onlyLog);
                MessageController.AddMessage("It's so thirsty", onlyLog: onlyLog);
                MessageController.AddMessage("Maybe you can help", onlyLog: onlyLog);
                MessageController.AddMessage("find it some water", onlyLog: onlyLog);
                break;
            case GamePhase.FeedVeggies:
                PlayerPrefs.SetInt("level", 2);
                cachedMrBiscuits2 = Instantiate(mrBiscuits2, mrBiscuits1.transform.position, Quaternion.identity);
                Utility.PhysicalDestroy(mrBiscuits1);
                MessageController.AddMessage("It's getting bigger and stronger", true, onlyLog: onlyLog);
                MessageController.AddMessage("It wants some veggies too", onlyLog: onlyLog);
                break;
            case GamePhase.HelloBats:
                PlayerPrefs.SetInt("level", 3);
                MessageController.AddMessage("You must protect Mr. Biscuits!", true, onlyLog: onlyLog);
                MessageController.AddMessage("They are hungry, bring meat to their lair", onlyLog: onlyLog);
                batSpawner.SetActive(true);
                break;
            case GamePhase.FeedMeats:
                PlayerPrefs.SetInt("level", 4);
                cachedMrBiscuits3 = Instantiate(mrBiscuits3, cachedMrBiscuits2.transform.position, Quaternion.identity);
                Utility.PhysicalDestroy(cachedMrBiscuits2);
                MessageController.AddMessage("Mr. Biscuits needs protein", true, onlyLog: onlyLog);
                MessageController.AddMessage("Eggs will do", onlyLog: onlyLog);
                MessageController.AddMessage("Or chickens...", onlyLog: onlyLog);
                break;
            case GamePhase.Run:
                PlayerPrefs.SetInt("level", 5);
                cachedMrBiscuits4 = Instantiate(mrBiscuits4, cachedMrBiscuits3.transform.position, Quaternion.identity);
                Utility.PhysicalDestroy(cachedMrBiscuits3);
                MessageController.AddMessage("run run run run run run run run", true);
                MessageController.AddMessage("run run run run run run run run");
                MessageController.AddMessage("run run run run run run run run");
                break;
            case GamePhase.Win:
                PlayerPrefs.SetInt("level", 0);
                MessageController.AddMessage("You escaped!", true);
                MessageController.AddMessage("but Mr. Biscuits has been unleashed upon this world");
                MessageController.AddMessage("-.. --- --- -- ... --- ...");
                MessageController.AddMessage("This was an entry in the Ludum Dare 41 compo");
                MessageController.AddMessage("By Zak Reynolds");
                MessageController.AddMessage("Thanks for playing!");
                break;
        }
    }
}