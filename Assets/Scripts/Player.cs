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

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");

        gameController = FindObjectOfType<GameController>(gameController);
    }
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
        {
            gameController.SetIsGameOver(true);
            //Destroy(gameObject);
        }
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * playerSpeed * verticalInput);
        powerupIndicator.transform.position = transform.position + new Vector3 (0, -0.5f, 0);

        if (Input.GetKeyDown(KeyCode.F) && currentPowerUp == PowerUp.PowerUpType.Rockets)
        {
            LauchRockets();
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
    }
    void LauchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.forward, Quaternion.identity);
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
