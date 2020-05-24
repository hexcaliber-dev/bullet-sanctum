using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public List<Sprite> strafeSprites, reloadSprites, healthSprites;
    public Image strafeMeter, reloadMeter, healthMeter;
    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    IEnumerator ReloadWeapon(Weapon weapon) {
        for (int step = 0; step < reloadSprites.Count; step += 1) {
            reloadMeter.sprite = reloadSprites[step];
            yield return new WaitForSeconds (weapon.cooldownTime / reloadSprites.Count);
        }
    }

    public void UpdateRechargeMeter(Weapon weapon) {
        StartCoroutine(ReloadWeapon(weapon));
    }

    public void SetStrafeAmount(int amt) {
        strafeMeter.sprite = strafeSprites[amt];
    }

    public void SetHealthAmount(int health) {
        healthMeter.sprite = healthSprites[health];
    }
}