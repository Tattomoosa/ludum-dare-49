using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    public int damage = 1;
    public int criticalHitDamage = 10;
    public bool infiniteBullets = false;
    public int currentBullets = 6;
    public int maxBullets = 6;
    public LayerMask bulletInteractionLayers;

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
        var clipInfo = gunAnimator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0 && clipInfo[0].clip.name == "Shoot")
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
        
        // if (!Physics.Raycast(ray, out var hit)) return;
        if (!Physics.Raycast(ray, out var hit, 200.0f, bulletInteractionLayers)) return;

        if (hit.collider.isTrigger)
        {
            Debug.LogWarning(
                $"BULLET HIT TRIGGER? GameObject: {hit.collider.gameObject} Layer: {hit.collider.gameObject.layer}");
            return;
        }

        var rigidBody = hit.collider.attachedRigidbody;
        if (rigidBody)
        {
            var health = rigidBody.GetComponent<Health>();
            if (health)
            {
                var dmg = damage;
                if (hit.collider.GetComponent<CriticalHitArea>())
                    dmg = criticalHitDamage;

                health.TakeDamage(dmg);
            }
        }

        var point = hit.point;
        point -= (dir * 0.1f);
        var bulletHole = Instantiate(bulletHolePrefab);
        if (rigidBody)
            bulletHole.transform.SetParent(hit.collider.transform);
        bulletHole.transform.position = point;
        bulletHole.transform.LookAt(point + hit.normal);
        // this is dependent on the bullet from blender having weird rotation to start with... lol
        // bulletHole.transform.Rotate(Vector3.right, Random.Range(0.0f, 360.0f));
        bulletHole.transform.Rotate(Vector3.forward, Random.Range(0.0f, 360.0f), Space.Self);
    }
}
