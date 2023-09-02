using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Turret : MonoBehaviour
{
    [HideInInspector] public int playerNum;

    public GameObject explosionPrefab;

    public GameObject bulletPrefab;
    private GameObject[] bullets;
    private int bulletsRemaining;

    public ParticleSystem barrelSmoke;

    public Sprite bulletSprite;
    public Sprite chargeShotSprite;


    public SpriteRenderer barrelRend;
    public SpriteRenderer topRend;
    public Transform barrel;
    public Transform endOfBarrel;
    public Animator barrelAnim;

    
    
    public float bulletSpeed;
    public float chargeShotTime;
    private float barrelMovementTimer;

    private bool canMove;

    private KeyCode shootButton;

    private float startingRot;

    [HideInInspector] public bool isAlive;

    public AudioSource chargeUpSound;
    public AudioSource chargeLoopSound;

    [HideInInspector] public bool buttonTap;
    [HideInInspector] public bool buttonHold;

    private void Start()
    {

        CreateBulletPool();
    }
    private void CreateBulletPool()
    {
        bullets = new GameObject[5];
        bulletsRemaining = 5;
        for (int i = 0; i < 5; i++)
        {
            bullets[i] = Instantiate(bulletPrefab, Vector2.zero, Quaternion.identity);
            bullets[i].GetComponentInChildren<SpriteRenderer>().material = GameManager.instance.mats[playerNum - 1];

            Bullet script = bullets[i].GetComponent<Bullet>();
            script.player = this;
            script.ResetBullet();
            bullets[i].SetActive(false);
        }
    }

    // make barrel face middle
    public void SetBarrelRotation()
    {
        Vector2 dir = -transform.position;
        startingRot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        barrel.transform.rotation = Quaternion.Euler(0f, 0f, startingRot);

        
    }

    public void SetCanMove(bool ableToMove)
    {
        canMove = ableToMove;
        buttonTap = false;
        buttonHold = false;
    }

    public void Setup(int num)
    {
        playerNum = num;

        SetBarrelRotation();
        GetComponent<SpriteRenderer>().color = GameManager.instance.colours[num - 1];
        barrelRend.color = GameManager.instance.colours[num - 1];
        topRend.color = GameManager.instance.colours[num - 1];

        shootButton = GameManager.instance.controls[num-1];

        isAlive = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(shootButton))
        {
            buttonTap = true;
            buttonHold = true;
        }
        if (Input.GetKeyUp(shootButton))
        {
            buttonHold = false;
        }

        if (!canMove)
            return;

        //barrel.transform.Rotate(Time.deltaTime * turnSpeed * turnDir * Vector3.forward);
        barrel.transform.rotation = Quaternion.Euler(0f, 0f, startingRot + Mathf.Sin(barrelMovementTimer) * 100f);
        barrelMovementTimer += Time.deltaTime;


        if (buttonTap && bulletsRemaining > 0)
        {
            buttonTap = false;
            StartCoroutine(ChargeShot());
        }

        
    }

    private IEnumerator ChargeShot()
    {
        float timer = 0f;
        bool fullyCharged = false;
        bool playingChargeSound = false;

        barrelAnim.SetBool("Charging", true);
        
        
        while (buttonHold)
        {
            timer += Time.deltaTime;

            if (timer >= 0.1f && !playingChargeSound)
            {
                chargeUpSound.Play();
                playingChargeSound = true;
            }

            if (timer >= chargeShotTime && !fullyCharged)
            {
                fullyCharged = true;
                chargeUpSound.Stop();
                chargeLoopSound.Play();
            }

            yield return null;
        }

        Shoot(fullyCharged);
    }
    private void Shoot(bool chargedShot)
    {
        chargeUpSound.Stop();
        chargeLoopSound.Stop();

        barrelAnim.SetBool("Charging", false);
        GameObject bullet = null;
        for (int i = 0; i < 5; i++)
        {
            if (!bullets[i].activeSelf)
            {
                bullet = bullets[i];
                break;
            }
        }
        bulletsRemaining--;
        if (bullet == null) return;
        bullet.SetActive(true);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.ResetBullet();

        bullet.transform.position = endOfBarrel.transform.position;

        barrelSmoke.Play();

        bulletScript.ChangeAudioPitch(playerNum);


        Vector2 dir = (endOfBarrel.transform.position - transform.position).normalized;
        bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;

        if (chargedShot)
        {
            bullet.GetComponent<Rigidbody2D>().velocity *= 0.7f;
            bulletScript.charged = true;
            bullet.GetComponent<Rigidbody2D>().mass = 10000000f;
            SpriteRenderer rend = bullet.GetComponentInChildren<SpriteRenderer>();
            rend.sprite = chargeShotSprite;
            bullet.GetComponent<CircleCollider2D>().radius = 0.44f;

            bulletScript.PlayChargeShotSound();
            GameManager.instance.ShakeCamera(3f, 0.2f);
        }
        else
        {
            bulletScript.PlayShootSound();
            GameManager.instance.ShakeCamera(1f, 0.2f);
        }

        //turnDir *= -1;
    }
    public void BulletDestroyed()
    {
        bulletsRemaining++;
    }

    public void Die()
    {
        isAlive = false;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
        GameManager.instance.PlayerDied();

        GameManager.instance.ShakeCamera(5f, 0.2f);
    }

}
