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
    private bool gameStarted = false;
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
        Level level = GameObject.Find("Panto").GetComponent<Level>();
        await level.PlayIntroduction();
        
        await GameObject.FindObjectOfType<Club>().Activate();
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
        await ball.GetComponent<GolfBall>().Activate(panto);
    }
    void LoadLevel(int n)
    {
        if (n < 0 || n >= levels.Length)
        {
            return;
        }
        LevelData ld = levels[n].GetComponent<LevelData>();
        club.transform.position = ld.clubPos;
        club.transform.rotation = Quaternion.Euler(ld.clubRot);
        if (ball is not null) { panto.GetComponent<LowerHandle>().Free(); Destroy(ball); }
        ball = Instantiate(ballPrefab, ld.ballPos, Quaternion.Euler(ld.ballRot));
        ball.GetComponent<GolfBall>().lm = this;
        ActivateBall();

        //panto.GetComponentInChildren<Camera>().transform.position = ld.cameraPos;
        //panto.GetComponentInChildren<Camera>().transform.rotation = Quaternion.Euler(ld.cameraRot);
        //panto.GetComponentInChildren<Camera>().orthographicSize = ld.cameraSize;
        levels[n].SetActive(true);
        ball.GetComponent<GolfBall>().goal = GameObject.FindGameObjectsWithTag("Goal")[0];
        
    }
}
