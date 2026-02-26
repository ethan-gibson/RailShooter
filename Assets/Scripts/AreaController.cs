using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    [SerializeField] private float newCamPos;
    [SerializeField] private List<EnemyController>  enemies;

    private int totalEnemiesInArea;

    private void Awake()
    {
        totalEnemiesInArea = enemies.Count;
        foreach (var _enemy in enemies)
        {
            _enemy.Initialize(this);
        }
    }

    public void EnemyDied()
    {
        totalEnemiesInArea--;
        if (totalEnemiesInArea == 0)
        {
            GameManager.Instance.MoveToNewLocation(newCamPos);
        }
    }
}
