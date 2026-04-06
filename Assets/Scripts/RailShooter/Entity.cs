using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float damagePerShot;
	
    
    protected float health;

    public virtual void TakeDamage(float _damage)
    {
	    health -= _damage;
	    if (health <= 0)
	    {
		    die();
	    }
    }
    public virtual void Heal(float _heal)
    {
        health += _heal;
    }
    protected virtual void die(){}
}
