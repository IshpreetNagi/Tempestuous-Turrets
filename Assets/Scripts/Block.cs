using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    /*
     * 1 light
     * 2 heavy
     * 3 steel
     * 4 explosive
    */
    public int blockType;
    public GameObject explosionPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Bullet"))
            return;

        if (blockType == 4)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2.5f);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.CompareTag("Block") && hit.gameObject != gameObject)
                {
                    Block script = hit.GetComponent<Block>();
                    if (script.blockType != 3)
                    {
                        script.StartDestroyDelay(transform.position);
                    }
                }
            }

            collision.GetComponent<Bullet>().DestroyBullet();
            DestroyBlock();
            GameManager.instance.ShakeCamera(5f, 0.2f);
        }
    }

    public void StartDestroyDelay(Vector3 TNTpos)
    {
        StartCoroutine(DestroyDelay(TNTpos));
    }

    public IEnumerator DestroyDelay(Vector3 TNTpos)
    {
        float dist = (TNTpos - transform.position).magnitude;

        yield return new WaitForSeconds(dist / 20f);

        DestroyBlock();
    }

    public void DestroyBlock()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
