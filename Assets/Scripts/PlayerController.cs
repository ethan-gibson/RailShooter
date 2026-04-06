using System;
using Cysharp.Threading.Tasks;
using FiringRange;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Entity
{
    private PlayerInput playerInput;
    [SerializeField] private Camera cam;
    [SerializeField] private int maxAmmo;

    private int currentAmmo;
    private Vector2 lookInput;
    private float xRotation;
    private bool magOut;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Attack"].performed += shoot;
        playerInput.actions["Reload"].performed += _ => reload();
        currentAmmo = maxAmmo;
        init().Forget();
    }
    
    private async UniTaskVoid init()
    {
        await UniTask.WaitUntil(() => HUDManager.Instance);
        HUDManager.Instance.UpdateAmmoCount($"{currentAmmo}/{maxAmmo}");
    }

    private void OnDisable()
    {
        playerInput.actions["Attack"].performed -= shoot;
        playerInput.actions["Reload"].performed -= _ => reload();
    }

    private void shoot(InputAction.CallbackContext _ctx)
    {
        if (currentAmmo <= 0) { return;}
        currentAmmo--;
        HUDManager.Instance.UpdateAmmoCount($"{currentAmmo}/{maxAmmo}");
        Ray _ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (!Physics.Raycast(_ray, out var _hit, Mathf.Infinity)) { return; }
        Debug.DrawRay(_ray.origin, _ray.direction * 100, Color.red, 0.5f);
        if (_hit.transform.gameObject.TryGetComponent<EnemyController>(out var _enemyController))
        {
            _enemyController.TakeDamage(damagePerShot);
        }

        if (_hit.transform.gameObject.TryGetComponent<IDummy>(out var _dummy))
        {
            _dummy.OnHit();
        }
    }

    private void reload()
    {
        if (magOut)
        {
            magOut = false;
            currentAmmo = maxAmmo;
        }
        else
        {
            magOut = true;
            currentAmmo = 0;
        }
        HUDManager.Instance.UpdateAmmoCount($"{currentAmmo}/{maxAmmo}");
    }
}
