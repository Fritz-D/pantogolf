using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

public class LevelManager : MonoBehaviour
{
    public GameObject[] levels;
    public GameObject ballPrefab;
    public GameObject club;
    public GameObject panto;
    private GameObject ball;
    private int levelNumber = 0;
    // Start is called before the first frame update
    void Start()
    {
        club.SetActive(true);
        panto.SetActive(true);

        foreach(GameObject level in levels)
        {
            level.SetActive(false);
        }

        LoadLevel(0);
        StartGame();
    }
    async void StartGame()
    {   
        await club.GetComponent<Club>().Activate();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NextLevel()
    {
        levels[levelNumber].SetActive(false);
        levelNumber++;
        LoadLevel(levelNumber);
    }

    async void ActivateBall()
    {
        await ball.GetComponent<GolfBall>().Activate();
    }
    async void LoadLevel(int n)
    {
        if (n < 0 || n >= levels.Length)
        {
            return;
        }
        LevelData ld = levels[n].GetComponent<LevelData>();
        ball = ld.ball;
        levels[n].SetActive(true);
        
        //Level level = GameObject.Find("Panto").GetComponent<Level>();
        //await level.PlayIntroduction();
        club.SetActive(false);
        await panto.GetComponent<UpperHandle>().MoveToPosition(ld.cspawn.transform.position);
        club.transform.position = panto.GetComponent<UpperHandle>().GetPosition();
        club.SetActive(true);
        //club.transform.position = ld.cspawn.transform.position;
        //club.transform.rotation = ld.cspawn.transform.rotation;
        ActivateBall();
        
        ball.GetComponent<GolfBall>().goal = GameObject.FindGameObjectsWithTag("Goal")[0];
        
    }
}
