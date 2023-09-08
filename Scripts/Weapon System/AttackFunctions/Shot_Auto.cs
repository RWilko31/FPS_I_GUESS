using System.Collections;
using UnityEngine;
using System;

public class Shot_Auto : AttackFunction
{
    private bool reloading = false;
    private bool cancelReload = false;
    private bool hold = false;
    private bool isShooting = false;
    [SerializeField] public Rigidbody projectile;
    [SerializeField] public Transform barrelposition;
    public override bool UsesAmmo()
    { return true; }
    public override void Attack()
    { hold = true; }
    public override void AttackCancelled()
    { hold = false; }
    public override void Reload()
    {
        if (ammoData == null) return;
        if (ammoData.ReserveAmmo == 0) return;
        if (reloading) return;
        StartCoroutine(Reloading());
    }

    private void Update()
    {
        if (hold && !isShooting) StartCoroutine(FullAuto());
    }
    IEnumerator FullAuto()
    {
        isShooting = true;
        Shoot();
        yield return new WaitForSeconds(ammoData.RateOfFire);
        isShooting = false;
    }

    void Shoot()
    {
        cancelReload = true;
        if (ammoData == null) return;
        else if (ammoData.MagAmmo == 0) { Reload(); return; }

        ammoData.MagAmmo--;
        float x = Screen.width / 2;
        float y = Screen.height / 2;
        var ray = playerCamera.ScreenPointToRay(new Vector3(x, y, 0));

        //Debug.Log("shoot");
        Rigidbody instantiatedProjectile = Instantiate(projectile, barrelposition.transform.position, playerCamera.transform.rotation) as Rigidbody;
        instantiatedProjectile.tag = "PlayerRangedAttack";
        instantiatedProjectile.AddForce(ray.direction * 80f, ForceMode.Impulse);
        Destroy(instantiatedProjectile.gameObject, 0.1f);

        RaycastHit hitInfo;
        float rayDistance;
        if (Physics.Raycast(ray, out hitInfo, 10000f, 256)) //Blue Laser
        { rayDistance = hitInfo.distance; }
        else rayDistance = 10000f;

        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance, 1024); //Yellow Laser
        if (Physics.Raycast(ray, rayDistance, 1024))
        {
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            foreach (RaycastHit obj in hits)
            {
                if (obj.transform.GetComponent<HealthScript>() != null)
                { SendAttackData(obj.transform.gameObject); }
            }
        }
    }
    private void SendAttackData(GameObject obj)
    {
        HealthScript healthScript = obj.transform.GetComponent<HealthScript>();
        bool critical = false; //temporary - sets all attacks to not be critical
        healthScript.DamageEntity("Player", name, critical);
    }

    IEnumerator Reloading()
    {
        int addAmmo;
        cancelReload = false;
        reloading = true;
        yield return new WaitForSeconds(ammoData.ReloadTime);
        if (!cancelReload)
        {
            addAmmo = ammoData.MagMax - ammoData.MagAmmo;
            if (ammoData.MagAmmo + ammoData.ReserveAmmo > ammoData.MagMax) { ammoData.ReserveAmmo -= addAmmo; ammoData.MagAmmo = ammoData.MagMax; }
            else if (ammoData.ReserveAmmo > 0) { ammoData.MagAmmo = ammoData.ReserveAmmo + ammoData.MagAmmo; ammoData.ReserveAmmo = 0; }
        }
        else { addAmmo = 0; }
        reloading = false;
    }
}