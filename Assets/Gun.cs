using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int damage = 1;
    public bool infiniteBullets = false;
    public int currentBullets = 6;
    public int maxBullets = 6;

    public bool drawDebugLines;
    
    public GameObject muzzleFlash;
    public GameObject bulletHolePrefab;
    public GameObject bulletOrigin;
    [SerializeField] private Animator gunAnimator;
    private AudioSource _audioSource;
    
    public bool isReloading;
    private static readonly int Shoot1 = Animator.StringToHash("Shoot");

    private void Start()
    {
        muzzleFlash.gameObject.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void Shoot()
    {
        if (gunAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Shoot")
            return;
        
        if (infiniteBullets || (!isReloading && currentBullets > 0))
            StartCoroutine(ShootCoroutine());
        else if (!isReloading)
            Reload();
    }

    public void Reload()
    {
        
    }

    private IEnumerator ShootCoroutine()
    {
        currentBullets--;
        // animation
        gunAnimator.SetTrigger(Shoot1);
        // muzzleflash
        muzzleFlash.transform.Rotate(new Vector3(Random.Range(0, 360),0,0));
        var randomScale = Random.Range(0.08f, 0.12f);
        muzzleFlash.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        muzzleFlash.gameObject.SetActive(true);
        // bang
        _audioSource.Play();
        ShootBullet();
        // raycast to target
        yield return new WaitForSeconds(0.08f);
        muzzleFlash.gameObject.SetActive(false);
    }

    private void ShootBullet()
    {
        var dir = bulletOrigin.transform.forward;
        var ray =
            new Ray(bulletOrigin.transform.position, dir);
        if (drawDebugLines)
            Debug.DrawRay(ray.origin, ray.direction * 1000.0f, Color.cyan, 0.5f);
        
        if (!Physics.Raycast(ray, out var hit)) return;
        
        if (hit.collider.isTrigger) return;

        var health = hit.collider.GetComponent<Health>();
        if (health)
            health.TakeDamage(damage);

        var point = hit.point;
        point -= (dir * 0.1f);
        var bulletHole = Instantiate(bulletHolePrefab, hit.collider.transform);
        bulletHole.transform.position = point;
        bulletHole.transform.LookAt(point + hit.normal);
    }
}
