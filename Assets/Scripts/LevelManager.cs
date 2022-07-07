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
    async void LoadLevel(int n)
    {
        if (n < 0 || n >= levels.Length)
        {
            return;
        }
        if (ball is not null) { panto.GetComponent<LowerHandle>().Free(); Destroy(ball); }
        LevelData ld = levels[n].GetComponent<LevelData>();
        levels[n].SetActive(true);
        club.SetActive(false);
        Level level = GameObject.Find("Panto").GetComponent<Level>();
        await level.PlayIntroduction();
        await panto.GetComponent<UpperHandle>().MoveToPosition(ld.cspawn.transform.position);
        club.transform.position = panto.GetComponent<UpperHandle>().GetPosition();
        club.SetActive(true);
        //club.transform.position = ld.cspawn.transform.position;
        //club.transform.rotation = ld.cspawn.transform.rotation;
        ActivateBall();

        //panto.GetComponentInChildren<Camera>().transform.position = ld.cameraPos;
        //panto.GetComponentInChildren<Camera>().transform.rotation = Quaternion.Euler(ld.cameraRot);
        //panto.GetComponentInChildren<Camera>().orthographicSize = ld.cameraSize;
        
        ball.GetComponent<GolfBall>().goal = GameObject.FindGameObjectsWithTag("Goal")[0];
        
    }
}