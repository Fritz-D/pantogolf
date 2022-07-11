using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

public class testball : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        startgame();
    }
    
    async void startgame()
    {
        LowerHandle lhandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        lhandle.Free();
        await lhandle.SwitchTo(gameObject,20.0f);
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * Input.GetAxis("Horizontal");
        transform.position += Vector3.forward * Input.GetAxis("Vertical");
    }
}
