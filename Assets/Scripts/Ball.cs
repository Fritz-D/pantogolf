using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using System.Threading.Tasks;
using SpeechIO;
public class Ball : MonoBehaviour
{
    public LevelManager lm;
    public PantoHandle handle;
    public bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        handle = lm.switchHandles ? (PantoHandle)GameObject.Find("Panto").GetComponent<UpperHandle>() : (PantoHandle)GameObject.Find("Panto").GetComponent<LowerHandle>();

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated && other.CompareTag("Goal"))
        {
            lm.levels[lm.curlevel].SetActive(false);
            activated = false;
            lm.LevelOver();
        }
        if (activated && other.CompareTag("Hole"))
        {
            lm.levels[lm.curlevel].SetActive(false);
            activated = false;
            lm.FailedLevel();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (activated && collision.gameObject.CompareTag("Club"))
        {
            lm.NextHit();
        }
    }
}
