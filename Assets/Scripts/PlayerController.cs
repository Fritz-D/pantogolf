/// Hint: Commenting or uncommenting in VS
/// On Mac: CMD + SHIFT + 7
/// On Windows: CTRL + K and then CTRL + C
    
using UnityEngine;
using DualPantoFramework;
using System.Threading.Tasks;
using SpeechIO;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed = 10f;
    public GameObject focalPoint;
    public bool hasPowerup;
    public float powerupStrength = 15f;
    public int powerupTime = 7;
    public GameObject powerupIndicator;
    public UpperHandle uhandle;
    public SpawnManager sm;
    private SpeechIn speech;
    private bool movementFrozen = false;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        sm = GameObject.FindObjectOfType<SpawnManager>();
        speech = new SpeechIn(onSpeechRecognized);
        speech.StartListening(new string[]{"help", "resume"});
        //await ActivatePlayer();
    }

    public async void onSpeechRecognized(string command)
    {
        if(command == "resume" && movementFrozen) { ResumeAfterPause(); }
        else if(command == "help" && !movementFrozen) 
        { 
            ToggleMovementFrozen();
            var powerups = GameObject.FindGameObjectsWithTag("Powerup");
            if(powerups.Length > 0)
            {
                await GameObject.Find("Panto").GetComponent<LowerHandle>().SwitchTo(powerups[0]);
            }
        }
    
    }
    public void ToggleMovementFrozen() 
    {
        RigidbodyConstraints cons = movementFrozen ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeAll;
        playerRb.constraints = cons;
        foreach(var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<Rigidbody>().constraints = cons;
        }
        movementFrozen = !movementFrozen;
    }
   async public void ResumeAfterPause() 
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if(enemy != null)
        {
            await GameObject.Find("Panto").GetComponent<LowerHandle>().SwitchTo(enemy);
        }
        ToggleMovementFrozen();
    }

    public async Task ActivatePlayer()
    {
        uhandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        await uhandle.SwitchTo(gameObject);
        uhandle.FreeRotation();
    }

    void Update()
    {
        if (!sm.gameStarted) { return; }
        powerupIndicator.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
    }

    void FixedUpdate()
    {
        if (!sm.gameStarted) { return; }
        //float forwardInput = Input.GetAxis("Vertical");
        //playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);
        PantoMovement();
    }
    
    void PantoMovement()
    {
        float rotation = uhandle.GetRotation();
        transform.eulerAngles = new Vector3(0, rotation, 0);
        playerRb.velocity = speed * transform.forward;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);
            CancelInvoke(nameof(PowerupCountdown)); // if we previously picked up an powerup
            Invoke(nameof(PowerupCountdown), powerupTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        /// challenge: when collision has tag "Enemy" and we have a powerup
        /// get the enemyRigidbody and push the enemy away from the player
        if (other.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = other.transform.position - transform.position;
            enemyRigidbody.AddForce(awayFromPlayer.normalized * powerupStrength, ForceMode.Impulse);
        }
    }

    void PowerupCountdown()
    {
        hasPowerup = false;
        powerupIndicator.gameObject.SetActive(false);
    }
}
