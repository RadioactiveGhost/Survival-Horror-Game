using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common
{
    //Stats
    private int maxHealth;
    private int healthPoints;
    private int strength;
    private int armor;
    public bool dead;

    //helpers
    int dmg;

    public Common(int MaxHealth, int Strength, int Armor)
    {
        SetMaxHealth(MaxHealth);
        SetStrength(Strength);
        SetArmor(Armor);
    }

    public void SetMaxHealth(int Maxhealth)
    {
        maxHealth = Maxhealth;
        healthPoints = Maxhealth;
    }

    public void SetStrength(int Strength)
    {
        strength = Strength;
    }

    public void SetArmor(int Armor)
    {
        armor = Armor;
    }

    public bool TakeDamage(int damage, GameObject entity, bool destroy)
    {
        dmg = damage - armor;
        if (dmg > 0)//did damage
        {
            healthPoints -= dmg;
        }
        else
        {
            return false;
        }
        if (healthPoints <= 0)//died
        {
            dead = true;
            if (destroy)
            {
                Destroy(entity);
            }
            return true;
        }
        return false;
    }

    public bool TakeTrueDamage(int damage, GameObject entity, bool destroy)
    {
        if (dmg > 0)//did damage
        {
            healthPoints -= damage;
        }
        else
        {
            return false;
        }
        if (healthPoints <= 0)//died
        {
            dead = true;
            if (destroy)
            {
                Destroy(entity);
            }
            return true;
        }
        return false;
    }

    public void Destroy(GameObject entity)
    {
        //insert death animation

        GameObject.Destroy(entity);
    }

    public void Heal(int healthPointsRegained)
    {
        healthPoints += healthPointsRegained;
    }

    public void FullHeal()
    {
        healthPoints = maxHealth;
    }

    public int Attack(float modifier)
    {
        if (modifier >= 1)
        {
            Debug.LogWarning("Consider balancing dmg, dmg is done: dmg = dmg + dmg * modifier, current modifier goes past 1, so it's at least double dmg");
        }
        return (int)(strength + strength * modifier);
    }
}