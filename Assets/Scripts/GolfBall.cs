using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DualPantoFramework;

public class GolfBall : MonoBehaviour
{
    public LevelManager lm;
    private LowerHandle lhandle;
    public GameObject goal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   if(goal is null) { return; }
        //float rot = Quaternion.LookRotation((transform.position - goal.transform.position)).y * 360;
        //lhandle.Rotate(rot);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            lm.NextLevel();
        }
    }
    public async Task Activate()
    {
        lhandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        lhandle.Free();
        await lhandle.SwitchTo(gameObject);
    }
}
