using System.Collections;
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
    public AudioSource AS;
    public AudioClip failSound;
    public AudioClip hitSound1;
    public AudioClip hitSound2;
    public bool switchHandles = true;
    public bool collisionHelp = true;
    public bool introductions = true;
    public int maxlevel = -1;
    public int curlevel = -1;
    int hitCount = 0;
    bool levelSelect = false;
    Dictionary<string, List<string>> commandDict = new Dictionary<string, List<string>>(){
            { "select", new List<string>(){ "level wählen", "kurs wählen"} },
            { "one", new List<string>(){ "kurs eins", "level eins"} },
            { "two", new List<string>(){ "kurs zwei", "level zwei" } },
            { "three", new List<string>(){ "kurs drei", "level drei" } },
            { "four", new List<string>(){ "kurs vier", "level vier" } },
            { "reset", new List<string>(){ "level zurücksetzen", "kurs zurücksetzen", "zurücksetzen", "neustarten", "wiederversuchen" } },
            { "introduction", new List<string>(){ "Einführungen wechseln" } },
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
        if (commandDict["introduction"].Contains(command))
        {
            introductions = !introductions;
        }
        if (commandDict["select"].Contains(command))
        {
            await sOut.Speak(command, 1.0f, SpeechBase.LANGUAGE.GERMAN);
            club.GetComponent<Club>().handle.Freeze();
            collisionHelper.GetComponent<CollisionHelper>().handle.Free();
            collisionHelper.GetComponent<CollisionHelper>().handle.Freeze();

            levelSelect = true;
        }
        if(levelSelect)
        {
            if (commandDict["one"].Contains(command)) { 
                LoadLevel(0);
                levelSelect = false;
            }
            if (commandDict["two"].Contains(command)) {
                if (maxlevel >= 0)
                {
                    LoadLevel(1);
                    levelSelect = false;
                }
                else
                {
                    await sOut.Speak("Spiel level eins zuerst", 1.0f, SpeechBase.LANGUAGE.GERMAN);
                }
            }
            if (commandDict["three"].Contains(command)) {
                if (maxlevel >= 1)
                {
                    LoadLevel(2);
                    levelSelect = false;
                }
                else
                {
                    await sOut.Speak("Spiel level zwei zuerst", 1.0f, SpeechBase.LANGUAGE.GERMAN);
                }
            }
            if (commandDict["four"].Contains(command))
            {
                if (maxlevel >= 2)
                {
                    LoadLevel(3);
                    levelSelect = false;
                }
                else
                {
                    await sOut.Speak("Spiel level drei zuerst", 1.0f, SpeechBase.LANGUAGE.GERMAN);
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
        collisionHelper.GetComponent<CollisionHelper>().handle.Free();
        club.GetComponent<Club>().handle.Free();
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
        if (curlevel == levels.Length-1) { await sOut.Speak("Du gewinnst.", 1.0f, SpeechBase.LANGUAGE.GERMAN);  return; }
        LoadLevel(curlevel + 1);
    }

    async public void LoadLevel(int levelnum)
    {
        if (levelnum < 0 || levelnum > levels.Length - 1) { return; }
        club.GetComponent<Club>().activated = false;
        ball.GetComponent<Ball>().activated = false;
        collisionHelper.GetComponent<CollisionHelper>().handle.Free();
        collisionHelper.GetComponent<CollisionHelper>().handle.Freeze();
        ball.transform.position = new Vector3(0.0f, 0.0f, -6.5f);
        club.transform.position = new Vector3(0.0f, 0.0f, -4.0f);
        club.GetComponent<Rigidbody>().velocity = Vector3.zero;

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(false);
        }
        levels[levelnum].SetActive(true);
        await sOut.Speak("Kurs " + (levelnum+1).ToString(), 1.0f, SpeechBase.LANGUAGE.GERMAN);
        await IntroduceLevel();

        curlevel = levelnum;
        hitCount = 0;
        
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        collisionHelper.GetComponent<CollisionHelper>().updatePos();
        collisionHelper.GetComponent<CollisionHelper>().handle.Free();
        await collisionHelper.GetComponent<CollisionHelper>().Activate();

        
        await club.GetComponent<Club>().handle.MoveToPosition(club.transform.position, 10.0f);
        club.GetComponent<Club>().handle.Free();
        club.GetComponent<Club>().activated = true;
        
        ball.GetComponent<Ball>().activated = true;
    }

    async public void FailedLevel()
    {
        AS.PlayOneShot(failSound);
        club.GetComponent<Club>().activated = false;
        ball.GetComponent<Ball>().activated = false;
        collisionHelper.GetComponent<CollisionHelper>().handle.Free();
        club.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.position = new Vector3(0.0f, 0.0f, -6.5f);
        club.transform.position = new Vector3(0.0f, 0.0f, -4.0f);
        
        await sOut.Speak("", 1.0f, SpeechBase.LANGUAGE.GERMAN);

        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        collisionHelper.GetComponent<CollisionHelper>().updatePos();
        await collisionHelper.GetComponent<CollisionHelper>().Activate();

        await club.GetComponent<Club>().handle.MoveToPosition(club.transform.position, 10.0f);

        levels[curlevel].SetActive(true);

        Invoke(nameof(ReactivateAll), 500);
    }

    public async void NextHit()
    {
        hitCount++;
        await sOut.Speak(hitCount.ToString(), 2, SpeechBase.LANGUAGE.GERMAN);
    }

    public async Task LevelOverSound(int levelnum)
    {
        if(hitCount == 0)
        {
            return;
        }
        int[] parScore = { 2, 4, 5, 4 };
        string[] underScore = { "par", "birdie",  "eagle", "albatross", "condor" };
        string[] overScore = { "", "double ", "triple ", "quadrouple ", "quintouple ", "sextouple ", "septouple ", "octouple " };
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
                score = overScore[hitCount - parScore[levelnum]-1] + "bogey";
            }
            else
            {
                score = "birdie";
            }
        }
        await sOut.Speak(score, 1.0f, SpeechBase.LANGUAGE.ENGLISH);
    }
    async Task IntroduceLevel()
    {
        if (!introductions) { return; }
        Level l = new Level();
        collisionHelper.GetComponent<CollisionHelper>().handle.Free();
        club.GetComponent<Club>().handle.Free();
        await l.PlayIntroduction(2.5f, 1000);
        collisionHelper.GetComponent<CollisionHelper>().handle.Free();
        club.GetComponent<Club>().handle.Free();
        await sOut.Speak("Now!");
    }

    void ReactivateAll()
    {
        club.GetComponent<Club>().activated = true;
        ball.GetComponent<Ball>().activated = true;
    }
}
