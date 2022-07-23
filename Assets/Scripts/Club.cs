using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

public class Club : MonoBehaviour
{
    public LevelManager lm;
    Rigidbody playerRb;
    public PantoHandle handle;
    public bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        handle = !lm.switchHandles ? (PantoHandle)GameObject.Find("Panto").GetComponent<UpperHandle>() : (PantoHandle)GameObject.Find("Panto").GetComponent<LowerHandle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            Vector3 err = handle.GetPosition() - transform.position;
            playerRb.velocity = 10f * err;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        Invoke(nameof(reenableClub), 1);
    }

    void reenableClub()
    {
        this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
    }
}
