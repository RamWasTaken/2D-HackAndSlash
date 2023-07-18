using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character, IDamagable
{
    public virtual void ApplyDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <=0)
        {
            Die();
        }
    }
}
