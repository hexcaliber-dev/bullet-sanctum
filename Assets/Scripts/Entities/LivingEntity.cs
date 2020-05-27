using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base interface for all entities (enemies, bosses, and players).
public abstract class LivingEntity : MonoBehaviour {
    private Dictionary<Type, string> typeToString = new Dictionary<Type, string> ();

    public string entityName;
    public enum Type { Player, Enemy, Boss }
    public Type entityType;
    // LET IT GOOOOOOOO
    public bool frozen = false;
    public float speed;

    public int health;

    protected virtual void Start () {
        typeToString.Add (Type.Player, "player");
        typeToString.Add (Type.Enemy, "enemy");
        typeToString.Add (Type.Boss, "boss");
        print ("test");
    }

    public List<LivingEntity> getEntities (string name) {
        List<LivingEntity> result = new List<LivingEntity> ();
        LivingEntity[] allEntities = GameObject.FindObjectsOfType<LivingEntity> ();
        foreach (LivingEntity e in allEntities) {
            if (e.entityName.Equals (name)) {
                result.Add (e);
            }
        }
        return result;
    }

    public LivingEntity getEntity (string name) {
        List<LivingEntity> allWithName = getEntities (name);
        if (allWithName.Count > 0) {
            return allWithName[0];
        }
        Debug.LogError ("Didn't find any entities with name " + name);
        return null;
    }

    public virtual void TakeDamage (Bullet b) {
        TakeDamage(b.damage);
    }

    public virtual void TakeDamage (int damage) {
        health -= damage;
        if (health <= 0) {
            OnDeath ();
        }
    }

    public abstract void Attack ();
    public abstract void OnSpawn ();
    public virtual void OnDeath () {
        Destroy (gameObject);
    }

    void OnCollisionEnter2D (Collision2D col) {
        if (col.gameObject.tag.Equals ("Bullet")) {
            TakeDamage (col.gameObject.GetComponent<Bullet> ());
        }
    }
}