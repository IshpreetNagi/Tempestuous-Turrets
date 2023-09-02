using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public AudioSource shootSound;
    public AudioSource bounceSound;
    public AudioSource chargeShotSound;

    public GameObject bulletPuffPrefab;
    public Transform trailParticle;
    public Transform chargedParticle;
    private Vector2 trailOffset;

    [HideInInspector] public int bounces;
    [HideInInspector] public Turret player;
    [HideInInspector] public bool charged;

    private Vector2 offset;
    private float chargedTimer;

    private void Start()
    {
        trailOffset = trailParticle.position - transform.position;
    }

    public void ResetBullet()
    {
        bounces = 2;
        GetComponent<Rigidbody2D>().mass = 0.0001f;
        charged = false;
        SpriteRenderer rend = GetComponentInChildren<SpriteRenderer>();
        rend.sprite = player.bulletSprite;
        GetComponent<CircleCollider2D>().radius = 0.2f;

        trailParticle.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        trailParticle.parent = transform;
        trailParticle.position = transform.position + (Vector3)offset;
        trailParticle.GetComponent<ParticleSystem>().Play();

        chargedParticle.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        chargedParticle.parent = transform;
        chargedParticle.position = transform.position + (Vector3)offset;
        chargedParticle.GetComponent<ParticleSystem>().Play();

        chargedParticle.gameObject.SetActive(false);
        trailParticle.gameObject.SetActive(true);
    }

    public void ChangeAudioPitch(int num)
    {
        float pitch = 0.5f + num * 0.2f;

        shootSound.pitch = pitch;
        bounceSound.pitch = pitch;
        chargeShotSound.pitch = pitch;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hit = collision.gameObject;

        chargedTimer = 0f;
        offset = Vector2.zero;

        switch (hit.tag)
        {
            case "Player":
                hit.GetComponent<Turret>().Die();
                DestroyBullet();
                break;

            case "Bullet":
                if (charged && !hit.GetComponent<Bullet>().charged)
                {

                }
                else
                {
                    DestroyBullet();
                }
                break;
            case "Block":
                Block script = hit.GetComponent<Block>();
                int type = script.blockType;
                if (type == 1 || (type == 2 && charged))
                {
                    script.DestroyBlock();
                    DestroyBullet();
                }
                else
                {
                    CheckBullet();
                }
                break;
            case "Wall":

            default:
                CheckBullet();
                break;
        }
    }

    private void CheckBullet()
    {
        if (bounces == 0)
        {
            DestroyBullet();
        }
        else
        {
            bounces--;

        }
        bounceSound.Play();
    }

    private void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 dir = rb.velocity.normalized;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f);

        if (charged)
        {
            chargedTimer += Time.deltaTime;

            Vector2 perp = new Vector2(-dir.y, dir.x);

            Vector2 newOffset = perp * (Mathf.Sin(chargedTimer * 20f)) * 0.1f;


            rb.position = rb.position - offset + newOffset;

            offset = newOffset;

            chargedParticle.gameObject.SetActive(true);
            trailParticle.gameObject.SetActive(false);
        }
    }

    public void DestroyBullet()
    {
        GameObject bulletPuff = Instantiate(bulletPuffPrefab, transform.position, Quaternion.identity);
        bulletPuff.GetComponent<Renderer>().material = GameManager.instance.mats[player.playerNum-1];
        if (charged) bulletPuff.transform.localScale = new Vector3(3f, 3f, 1f);

        chargedParticle.parent = bulletPuff.transform;
        trailParticle.parent = bulletPuff.transform;

        player.BulletDestroyed();
        gameObject.SetActive(false);
    }

    public void PlayShootSound()
    {
        shootSound.Play();
    }
    public void PlayChargeShotSound()
    {
        chargeShotSound.Play();
    }
}
