using UnityEngine;
using System.Collections.Generic;

/** Manages persistent storage and gamewide static data.
In the future, we'll add save file behaviors here. */
public class PersistentDataManager : MonoBehaviour {

    // Rooms that the player has gone to. Resets when 
    public static List<string> visitedRooms = new List<string>();
}