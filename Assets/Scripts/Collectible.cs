using UnityEngine;

// Represents an item that the player can pick up.
public class Collectible : MonoBehaviour {

    public enum CollectibleType { Weapon, Fragment }
    public CollectibleType type;
    public int collectibleWeapon; // Does not need to be assigned if this is not a weapon

    void OnCollisionEnter2D (Collision2D col) {
        if (col.gameObject.tag.Equals ("Player")) {
            if (type == CollectibleType.Weapon) {
                GameObject.FindObjectOfType<PlayerShoot>().GetNewWeapon(collectibleWeapon);
            } else if (type == CollectibleType.Fragment) {
                GameObject.FindObjectOfType<PlayerBounty> ().CollectFragment ();
            }

            Destroy (gameObject);
        }
    }
}