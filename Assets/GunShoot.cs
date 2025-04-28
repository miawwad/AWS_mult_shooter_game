using UnityEngine;

public class GunShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;

    private bool punchDisabled = false;

    void Update()
    {
        // Disable punch if gun is active
        if (!punchDisabled)
        {
            PlayerController pc = GetComponentInParent<PlayerController>();
            if (pc != null)
            {
                pc.hasGun = true;           // disables punch in PlayerController
                punchDisabled = true;       // only runs once
            }
        }

        if (Input.GetMouseButtonDown(0)) // Left click to shoot
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
    }
}
