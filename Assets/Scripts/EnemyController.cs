using UnityEngine;

public class EnemyController : Entity
{
    [SerializeField] private Transform newCamPos;
    protected override void die()
    {
        GameManager.Instance.MoveCamToNextLocation(newCamPos);
        Destroy(gameObject);
    }
}
