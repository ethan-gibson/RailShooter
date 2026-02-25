using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Camera cam;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void MoveCamToNextLocation(Transform _newLocation)
    {
        if (!_newLocation) { return;}
        cam.transform.position = _newLocation.position;
        cam.transform.rotation = _newLocation.rotation;
    }
}
