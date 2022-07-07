using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DualPantoFramework;
using SpeechIO;

public class Club : MonoBehaviour
{
    private UpperHandle uhandle;
    private SpeechIn speech;
    public int speed = 10;
    private Rigidbody playerRb;
    private bool shoot = false;
    private float shoot_mag = 0;
    private bool activated = false;
    private bool intangible = false;
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        //await ActivatePlayer();
        speech = new SpeechIn(onSpeechRecognized);
        speech.StartListening(new string[] { "Bang" });
    }

    void onSpeechRecognized(string command)
    {
        if (command == "Bang") { 
            shoot = true;
            shoot_mag = (GameObject.FindGameObjectWithTag("Ball").transform.position - transform.position).magnitude;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!activated) { return; }
        if (uhandle.GetComponentInParent<DualPantoSync>().debug)
        {
            if (Input.GetKey("space"))
            {
                shoot = true;
                shoot_mag = (GameObject.FindGameObjectWithTag("Ball").transform.position - transform.position).magnitude;
                playerRb.velocity = Vector3.zero;
            }
            if (shoot)
            {
                Vector3 vec = GameObject.FindGameObjectWithTag("Ball").transform.position - transform.position;
                playerRb.AddForce(vec * shoot_mag, ForceMode.Impulse);
                return;
            }
            float forwardInput = Input.GetAxis("Vertical");
            float sidewaysInput = Input.GetAxis("Horizontal");
            playerRb.velocity = (transform.forward * sidewaysInput + transform.right * -forwardInput) * speed;
        }
        else
        {
            Vector3 err = uhandle.GetPosition() - transform.position;
            playerRb.velocity = 10f * err;
        }
    }

    public async Task Activate()
    {
        uhandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        await uhandle.MoveToPosition(gameObject.transform.position);
        activated = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            shoot = false;
            if (intangible == false)
            {
                intangible = true;
                playerRb.mass = 0.00001f;
                Invoke(nameof(resetLayer), 1);
            }
        }
    }
    private void resetLayer()
    {
        intangible = false;
        playerRb.mass = 5f;
    }
}