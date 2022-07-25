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
    Rigidbody playerRB;
    public bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        playerRB = this.GetComponent<Rigidbody>();
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
            lm.AS.PlayOneShot(lm.hitSound1, 0.5f);
            playerRB.AddForce((this.transform.position - collision.gameObject.transform.position).normalized, ForceMode.Impulse);
            lm.NextHit();
        }
    }
}
