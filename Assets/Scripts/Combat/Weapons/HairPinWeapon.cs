using UnityEngine;

public class HairPinWeapon : WeaponBase
{   
    [SerializeField] private PlayerController playerController;
    [SerializeField] private HairPinProjectile hairPinPrefab;
    [SerializeField] private float projectileSpeed = 8f;

    protected override void Awake()
    {
        base.Awake();

        playerController = GetComponentInParent<PlayerController>();
    }

    protected override void Attack()
    {
        Vector2 shootDirection = playerController.LastMoveDirection;
        HairPinProjectile hairPinProjectile = Instantiate(
            hairPinPrefab, 
            transform.position, 
            Quaternion.identity);
        hairPinProjectile.Initialize(shootDirection, Damage, projectileSpeed);
    }
}
