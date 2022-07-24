﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using SpeechIO;
using System.Threading.Tasks;
public class LevelManager : MonoBehaviour
{
    public GameObject panto;
    public GameObject ball;
    public GameObject club;
    public GameObject collisionHelper;
    public GameObject[] levels;
    public SpeechOut sOut;
    public SpeechIn sIn;
    public bool switchHandles = true;
    public bool collisionHelp = true;
    int maxlevel = -1;
    public int curlevel = -1;
    int hitCount = 0;
    bool levelSelect = false;
    Dictionary<string, List<string>> commandDict = new Dictionary<string, List<string>>(){
            { "select", new List<string>(){ "level select", "course select"} },
            { "one", new List<string>(){ "course one", "level one"} },
            { "two", new List<string>(){ "course two", "level two" } },
            { "three", new List<string>(){ "course three", "level three" } },
            { "reset", new List<string>(){ "level reset", "reset", "restart", "retry" } }
        };
    // Start is called before the first frame update
    void Start()
    {
        sOut = new SpeechOut();
        sIn = new SpeechIn(onSpeechRecognized);

        List<string> strings = new List<string>();
        foreach(var kvp in commandDict)
        {
            foreach(string s in kvp.Value)
            {
                strings.Add(s); // build list of strings according to dict
            }
        }

        sIn.StartListening(strings.ToArray()); //init listener
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(false);
        }
        if (switchHandles)
        {
            ObjectOfInterest[] gos = UnityEngine.Object.FindObjectsOfType<ObjectOfInterest>(true);
            for (int i = 0; i < gos.Length; i++)
            {

                gos[i].isOnUpper = !gos[i].isOnUpper;
            }
        }

        LoadLevel(0);
    }
    async void onSpeechRecognized(string command)
    {
        
        sOut.Stop(false);
        Debug.Log(command);
        if (commandDict["select"].Contains(command))
        {
            await sOut.Speak(command);
            club.GetComponent<Club>().handle.Freeze();
            collisionHelper.GetComponent<CollisionHelper>().handle.Free();
            collisionHelper.GetComponent<CollisionHelper>().handle.Freeze();

            levelSelect = true;
        }
        if(levelSelect)
        {
            if (commandDict["one"].Contains(command)) { 
                LoadLevel(0); 
            }
            if (commandDict["two"].Contains(command)) {
                if (maxlevel >= 0)
                {
                    LoadLevel(1);
                }
                else
                {
                    await sOut.Speak("Beat course one first");
                }
            }
            if (commandDict["three"].Contains(command)) {
                if (maxlevel >= 1)
                {
                    LoadLevel(2);
                }
                else
                {
                    await sOut.Speak("Beat course two first");
                }
            }
        }
        if(commandDict["reset"].Contains(command))
        {
            LoadLevel(curlevel);
        }
        
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnApplicationQuit()
    {
        sOut.Stop();
        sIn.StopListening();
    }
    async public void LevelOver()
    {
        collisionHelper.GetComponent<CollisionHelper>().handle.Freeze();
        club.GetComponent<Club>().handle.Freeze();
        levels[curlevel].SetActive(false);
        await LevelOverSound(curlevel);
        
        if (curlevel > maxlevel)
        {
            maxlevel = curlevel;
        }

        collisionHelper.GetComponent<CollisionHelper>().handle.Free();
        club.GetComponent<Club>().handle.Free();
        if (curlevel == levels.Length-1) { await sOut.Speak("You win the game.");  return; }
        LoadLevel(curlevel + 1);
    }

    async public void LoadLevel(int levelnum)
    {
        if (levelnum < 0 || levelnum > levels.Length - 1) { return; }
        club.GetComponent<Club>().activated = false;
        ball.GetComponent<Ball>().activated = false;
        collisionHelper.GetComponent<CollisionHelper>().handle.Free();
        ball.transform.position = new Vector3(0.0f, 0.0f, -6.5f);
        club.transform.position = new Vector3(0.0f, 0.0f, -4.0f);
        club.GetComponent<Rigidbody>().velocity = Vector3.zero;

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(false);
        }
        levels[levelnum].SetActive(true);
        await sOut.Speak("Course " + (levelnum+1).ToString());
        await IntroduceLevel();

        curlevel = levelnum;
        hitCount = 0;
        
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        collisionHelper.GetComponent<CollisionHelper>().updatePos();
        await collisionHelper.GetComponent<CollisionHelper>().Activate();

        
        await club.GetComponent<Club>().handle.MoveToPosition(club.transform.position, 100.0f);

        club.GetComponent<Club>().activated = true;
        club.GetComponent<Club>().handle.Free();
        ball.GetComponent<Ball>().activated = true;
    }

    public async void NextHit()
    {
        hitCount++;
        await sOut.Speak(hitCount.ToString(), 2);
    }

    public async Task LevelOverSound(int levelnum)
    {
        if(hitCount == 0)
        {
            return;
        }
        int[] parScore = { 2, 4, 5 };
        string[] underScore = { "par", "eagle", "albatross", "condor" };
        string[] overScore = { "", "double ", "triple ", "quadrouple ", "quintouple ", "sextouple ", "septouple ", "octouple "};
        string score = "";
        if(hitCount == 1) 
        { 
            score = "ace"; 
        }
        else if(parScore[levelnum] - hitCount >= 0)
        {
            if (parScore[levelnum] - hitCount < underScore.Length)
            {
                score = underScore[parScore[levelnum] - hitCount];
            }
            else 
            { 
                score = "insane"; 
            }
        }
        else
        {
            if (hitCount - parScore[levelnum] < 9)
            {
                score = overScore[hitCount - parScore[levelnum]] + "bogey";
            }
            else
            {
                score = "horrendous bogey";
            }
        }
        await sOut.Speak(score);
    }
    async Task IntroduceLevel()
    {
        Level l = new Level();
        collisionHelper.GetComponent<CollisionHelper>().handle.Free();
        club.GetComponent<Club>().handle.Free();
        await l.PlayIntroduction(10, 300);
    }
}
