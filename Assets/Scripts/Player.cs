using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float playerSpeed;
    private Rigidbody playerRb;
    public bool hasPowerUp;
    private GameObject focalPoint;
    public float powerupStrength = 15.0f;
    public GameObject powerupIndicator;
    GameController gameController;

    public PowerUp.PowerUpType currentPowerUp = PowerUp.PowerUpType.None;
    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;

    // Smash
    public float explosionForce;
    public float explosionRadius;
    public float hangTime;
    public float smashSpeed;
    public bool isOnGround;
    float floorY;

    // Start is called before the first frame update
    void Start()
    {
        isOnGround = true;
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");

        gameController = FindObjectOfType<GameController>(gameController);
    }
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -5)
        {
            gameController.SetIsGameOver(true);
        }
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * playerSpeed * verticalInput);
        powerupIndicator.transform.position = transform.position + new Vector3 (0, -0.5f, 0);

        if (Input.GetKeyDown(KeyCode.D) && currentPowerUp == PowerUp.PowerUpType.Rockets)
        {
            //Instantiate(rocketPrefab, transform.position + Vector3.forward, rocketPrefab.transform.rotation);
            LauchRockets();
        }

        if (Input.GetKeyDown(KeyCode.Space)&& isOnGround && currentPowerUp == PowerUp.PowerUpType.Smash) 
        {
            isOnGround = false;
            playerRb.AddForce(Vector3.up * smashSpeed, ForceMode.Impulse);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            Destroy(other.gameObject);
            StartCoroutine(PowerUpCountDownRoutine());
            powerupIndicator.gameObject.SetActive(true);
            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerUpCountDownRoutine());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp ==  PowerUp.PowerUpType.Pushback)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;
            Debug.Log("Collided with " + collision.gameObject.name + " with powerup set to " + hasPowerUp);
            enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            Debug.Log("Player collided with " + collision.gameObject.name + " with powerup set to " + currentPowerUp.ToString());
        }
        if (collision.gameObject.CompareTag("Ground") && !isOnGround && currentPowerUp == PowerUp.PowerUpType.Smash)
        {
            isOnGround = true;
            Collider[] coliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider nearby in coliders)
            {
                Rigidbody m_rigidbody = nearby.GetComponent<Rigidbody>();
                if (m_rigidbody != null)
                {
                    m_rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.05f, ForceMode.Impulse);
                }
            }
        }
        
    }
    /*private void SmashPowerUp()
    {
        Collider[] coliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in coliders)
        {
            Rigidbody m_rigidbody = nearby.GetComponent<Rigidbody>();
            if (m_rigidbody != null)
            {
                m_rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.05f, ForceMode.Impulse);
            }
        }
      
    }*/
    void LauchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.forward, rocketPrefab.transform.rotation);
            tmpRocket.GetComponent<Rockets>().Fire(enemy.transform);
        }
    }
    IEnumerator PowerUpCountDownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerUp = false;
        currentPowerUp = PowerUp.PowerUpType.None;
        powerupIndicator.gameObject.SetActive(false);
    }
}
