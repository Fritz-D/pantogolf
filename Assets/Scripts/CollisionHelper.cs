using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using System.Threading.Tasks;
public class CollisionHelper : MonoBehaviour
{
    public LevelManager lm;
    public GameObject ball;
    public PantoHandle handle;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = ball.transform.position + colMap();
        handle = lm.switchHandles ? (PantoHandle)GameObject.Find("Panto").GetComponent<UpperHandle>() : (PantoHandle)GameObject.Find("Panto").GetComponent<LowerHandle>();
    }

    // Update is called once per frame
    void Update()
    {
        updatePos();
    }
    
    public async Task Activate()
    {
        await handle.SwitchTo(gameObject, 10.0f);
    }
    public void updatePos()
    {
        transform.position = colMap();
    }
    Vector3 colMap()
    {
        float x = ball.transform.position.x;
        float y = ball.transform.position.y;
        float z = ball.transform.position.z;
        if (lm.collisionHelp)
        {
            z = lm.switchHandles ? z+(ball.transform.position.z + 15.0f) / 5.0f : z - (ball.transform.position.z + 15.0f) / 5.0f;
        }
        return new Vector3(x, y, z);
    }

}