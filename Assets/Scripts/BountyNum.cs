using UnityEngine;

// Manages the instantiation of bounty number indicators on enemy death.
public class BountyNum : MonoBehaviour {
    public GameObject numPrefab;

    public void ShowNumInternal(Vector3 pos, int number) {
        GameObject num = GameObject.Instantiate(numPrefab, pos, Quaternion.identity);
        num.GetComponent<TMPro.TMP_Text>().text = "+" + number;
    }

    public static void ShowNum(Vector3 position, int number) {
        GameObject.FindObjectOfType<BountyNum>().ShowNumInternal(position, number);
    }
}