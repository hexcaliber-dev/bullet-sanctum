using UnityEngine;

// Should be present in every room. Handles stuff like entering/leaving, enemy spawn locations, etc
public class Room : MonoBehaviour {

    // Assign in inspector
    public string roomID;

    // Room ID's for neighboring rooms. Corresponds to Left, Right, Up, Down
    public string[] neighboringRooms = new string[] {null, null, null, null};

    void Start() {

    }

    void Update() {

    }

    public Enemy[] getEnemies () {
        return GameObject.FindObjectsOfType<Enemy> ();
    }

}