using UnityEngine;

public class EnemyController : Entity
{
    private AreaController areaController;
    protected override void die()
    {
        areaController.EnemyDied(this);
        Destroy(gameObject);
    }

    public void Initialize(AreaController _areaController)
    {
        areaController = _areaController;
    }
}
