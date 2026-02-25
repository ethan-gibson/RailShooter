using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Entity
{
    private PlayerInput playerInput;
    [SerializeField] private Camera cam;

    
    private Vector2 lookInput;
    private float xRotation;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Attack"].performed += shoot;
    }

    private void OnDisable()
    {
        playerInput.actions["Attack"].performed -= shoot;
    }

    private void shoot(InputAction.CallbackContext _ctx)
    {
        Debug.Log("Shoot");
        Ray _ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (!Physics.Raycast(_ray, out var _hit, Mathf.Infinity)) { return; }
        Debug.DrawRay(_ray.origin, _ray.direction * 100, Color.red, 0.5f);
        if (_hit.transform.gameObject.TryGetComponent<EnemyController>(out var _enemyController))
        {
            _enemyController.TakeDamage(damagePerShot);
        }
    }
}
