using System;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private CinemachineVirtualCameraBase cam;

    [SerializeField]
    private float camMoveSpeed = 2f;
    [SerializeField] private float tolerance = 0.1f;
    [SerializeField] private CinemachineSplineDolly splineDolly;
    private float targetPosition;
    private bool isMoving;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        targetPosition = splineDolly.CameraPosition;
    }

    public void MoveCamToNextLocation(Transform _newLocation)
    {
        if (!_newLocation) { return;}
        cam.transform.position = _newLocation.position;
        cam.transform.rotation = _newLocation.rotation;
    }

    private void Update()
    {
        if (!isMoving) { return; }
        float _newPos = Mathf.MoveTowards(splineDolly.CameraPosition, targetPosition, camMoveSpeed * Time.deltaTime);
        splineDolly.CameraPosition = _newPos;

        if (!(math.abs(splineDolly.CameraPosition- targetPosition) <= tolerance)) { return; }
        splineDolly.CameraPosition = targetPosition;
        isMoving = false;
    }

    public void MoveToNewLocation(float _distToMove)
    {
        targetPosition = _distToMove;
        isMoving = true;
    }
}
