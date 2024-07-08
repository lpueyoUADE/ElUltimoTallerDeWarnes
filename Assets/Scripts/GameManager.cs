using EstructurasDeDatos.ColaPrioridad;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using EstructurasDeDatos.Pila;

public class GameManager : MonoBehaviour
{
    [Header("SpawnPoints Parents")]
    [SerializeField] private GameObject spawnPointsParent;
    [SerializeField] private GameObject boostsSpawnPointsParent;

    [Header("SpawnPoints Prefabs")]
    [SerializeField] private GameObject Pickable;
    [SerializeField] private GameObject TimeBoostPickable;
    [SerializeField] private GameObject SpeedBoostPickable;

    [SerializeField] private int respawnAfter;

    [Header("Sounds")]
    [SerializeField] private AudioClip respawnedSound;
    [SerializeField] private AudioClip activatePowerup;

    [Header("Car Controller")]
    [SerializeField] private CarController carController;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerDisplay;
    [SerializeField] private float maxTime;

    [Header("Points")]
    [SerializeField] private TextMeshProUGUI pointsDisplay;

    [Header("End Game")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI finalScoreDisplay;

    [Header("Boosts")]
    [SerializeField] private TextMeshProUGUI boostsDisplay;
    
    private ColaPrioridadDinamica<Transform> spawnPointsPriorityQueue;
    private Transform[] spawnPoints;

    private ColaPrioridadDinamica<Transform> boostsSpawnPointsPriorityQueue;
    private Transform[] boostsSpawnPoints;

    private enum Boost {
        TimeStop,
        Speed
    }

    private PilaDinamica<Boost> boostsStack;

    private int pickedCount;
    private int points;
    private float totalTime;

    private bool useBoost;

    //Boosts
    private float stopTimer;
    private float speedBoostTimer;

    private void Awake()
    {
        CarController.ItemPickedAction += ItemPickedActionHandler;
        CarController.TimeStopBoostAction += TimeStopBoostActionHandler;
        CarController.SpeedBoostAction += SpeedBoostActionHandler;
    }
    private void OnDestroy()
    {
        CarController.ItemPickedAction -= ItemPickedActionHandler;
        CarController.TimeStopBoostAction -= TimeStopBoostActionHandler;
        CarController.SpeedBoostAction -= SpeedBoostActionHandler;
    }

    private void Start()
    {
        pickedCount = 0;
        points = 0;
        totalTime = maxTime;

        spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();
        spawnPointsPriorityQueue = new();

        boostsSpawnPoints = boostsSpawnPointsParent.GetComponentsInChildren<Transform>();
        boostsSpawnPointsPriorityQueue = new();

        boostsStack = new();

        boostsDisplay.text = "No Boosts";

        stopTimer = 0;
        speedBoostTimer = 0;

        endGamePanel.SetActive(false);

        GeneratePickables();
        GenerateBoosts();
    }

    private void Update()
    {
        useBoost = Input.GetKeyDown(KeyCode.LeftControl);

        updateBoosts();
        updateTimer();
        
    }

    private void updateBoosts()
    {
        if(useBoost)
        {
            if (!boostsStack.IsPilaVacia())
            {
                Boost currentBoost = boostsStack.Desapilar();

                boostsDisplay.text = boostsStack.IsPilaVacia() ? "No Boosts" : boostsStack.Tope().ToString();

                switch (currentBoost)
                {
                    case Boost.TimeStop:
                        stopTimer += 10;
                        break;

                    case Boost.Speed:
                        speedBoostTimer += 10;
                        carController.SpeedBoosted = true;
                        break;
                }

                AudioSource.PlayClipAtPoint(activatePowerup, carController.transform.position);
            }
        }
    }
    private void updateTimer()
    {
        if (totalTime > 0)
        {
            if(stopTimer > 0)
            {
                stopTimer -= Time.deltaTime;
            }else
            {
                stopTimer = 0;
                totalTime -= Time.deltaTime;
            }
        }
        else
        {
            totalTime = 0;
            endGamePanel.SetActive(true);
            finalScoreDisplay.text = points.ToString();
        }

        if(speedBoostTimer > 0)
        {
            speedBoostTimer -= Time.deltaTime;
        } else
        {
            speedBoostTimer = 0;
            carController.SpeedBoosted = false;
        }

        int minutes = (int)Mathf.Floor(totalTime / 60);

        // Returns the remainder
        int seconds = (int)Mathf.Floor(totalTime % 60);
        timerDisplay.text = minutes + ":" + (seconds < 10 ? "0" + seconds : seconds);
    }

    private void GeneratePickables()
    {
        AudioSource.PlayClipAtPoint(respawnedSound, carController.transform.position);
        spawnPointsPriorityQueue = new();

        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.CompareTag("SpawnPoint"))

                spawnPointsPriorityQueue.Encolar(spawnPoint, UnityEngine.Random.Range(0, spawnPoints.Length));
        }

        for (int i = 0; i < respawnAfter; i++)
        {
            var randomSpawnPoint = spawnPointsPriorityQueue.DesEncolar();
            Instantiate(Pickable, randomSpawnPoint.position, randomSpawnPoint.rotation);
        }
    }

    private void GenerateBoosts() {
        boostsSpawnPointsPriorityQueue = new();
        foreach (var boostSpawnPoint in boostsSpawnPoints)
        {
            if (boostSpawnPoint.CompareTag("TimeBoostSpawnPoint") || boostSpawnPoint.CompareTag("SpeedBoostSpawnPoint"))

                boostsSpawnPointsPriorityQueue.Encolar(boostSpawnPoint, UnityEngine.Random.Range(0, boostsSpawnPoints.Length));
        }

        for (int i = 0; i < 5; i++)
        {
            var randomSpawnPoint = boostsSpawnPointsPriorityQueue.DesEncolar();

            if (randomSpawnPoint.CompareTag("TimeBoostSpawnPoint"))
            {
                Instantiate(TimeBoostPickable, randomSpawnPoint.position, randomSpawnPoint.rotation);
            }

            if (randomSpawnPoint.CompareTag("SpeedBoostSpawnPoint"))
            {
                Instantiate(SpeedBoostPickable, randomSpawnPoint.position, randomSpawnPoint.rotation);
            }
        }
    }

    private void ItemPickedActionHandler()
    {
        pickedCount++;

        points += 10;
        if (pickedCount == respawnAfter) {
            GeneratePickables();
            pickedCount = 0;
            points += 100;
            totalTime += 30;
        }

        pointsDisplay.text = points.ToString();
    }

    private void TimeStopBoostActionHandler()
    {
        boostsStack.Apilar(Boost.TimeStop);
        boostsDisplay.text = boostsStack.Tope().ToString();
    }

    private void SpeedBoostActionHandler()
    {
        boostsStack.Apilar(Boost.Speed);
        boostsDisplay.text = boostsStack.Tope().ToString();
    }
}
