using UnityEngine;
using TMPro;

class UpdateWeaponUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI weaponNameText;
    [SerializeField] WeaponSystem ws;
    AmmoData ad;
    string lastString = "";

    private void Update()
    {
        try
        {
            ad = ws.weaponScript.AttackScript.ammoData;
            ammoCount();
            weaponName();
        }
        catch { };
        
        
    }
    void weaponName()
    {
        string name = ws.weaponScript.weapon.ToString();
        if (weaponNameText.text != name) weaponNameText.text = name;
    }

    void ammoCount()
    {
        string newString = ad.MagAmmo.ToString() + " / " + ad.ReserveAmmo.ToString();
        if (newString != lastString)
        {
            ammoText.text = newString;
            lastString = newString;
        }
    }
}