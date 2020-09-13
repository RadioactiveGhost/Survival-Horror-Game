using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Common
{
    //helpers
    static int dmg;

    public static int TakeDamage(int damage, int health, int armor)
    {
        dmg = (damage / 2) - armor;
        if (dmg < 0)
        {
            dmg = 0;
        }
        damage = (damage / 2) + dmg;

        return TakeTrueDamage(damage, health);
    }

    public static int TakeTrueDamage(int damage, int health)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
        }

        return health;
    }

    public static int Heal(int health, int amount)
    {
        health += amount;
        return health;
    }

    public static int Attack(int strength, float modifier)
    {
        if (modifier >= 1)
        {
            Debug.LogWarning("Consider balancing dmg, dmg is done: dmg = srength + strength * modifier, current modifier should not go past 1, so it's at max double dmg");
        }
        return (int)(strength + strength * modifier);
    }
}