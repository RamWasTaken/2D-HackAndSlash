using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private Player player = null;
    public Player Player
    {
        get{ return player; }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<Player>())
            {
                player = collision.GetComponent<Player>();
            }
            Activate();
        }
    }
    public virtual void Activate()
    {
        Destroy(gameObject);
    }
}
