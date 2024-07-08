using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float m_Distance = 10.0f;
    [SerializeField] private float m_Height = 5.0f;
    [SerializeField] private float m_HeightDamping = 2.0f;
    [SerializeField] private float m_RotationDamping = 3.0f;
    [SerializeField] private float m_MoveSpeed = 1.0f;
    [SerializeField] private float m_NormalFov = 60.0f;
    [SerializeField] private float m_FastFov = 90.0f;
    [SerializeField] private float m_FovDamping = 0.25f;

    [SerializeField] private CarController FollowTarget;

    private Camera mCamera;
    public float SpeedRatio { get; set; }

    private void Start()
    {
        mCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        SpeedRatio = FollowTarget.Velocity.magnitude / FollowTarget.Settings.MaxSpeed;

        if (Input.GetMouseButton(2))
        {
            int turnLeft = Input.mousePosition.x > Screen.width / 2 ? 1: -1;
            transform.RotateAround(transform.position, Vector3.up, 360 * turnLeft * Time.deltaTime);
            return;
        }
    }
    public void LateUpdate()
    {
        if (FollowTarget == null)
        {
            return;
        }


        float wantedRotationAngle = FollowTarget.transform.eulerAngles.y;
        float wantedHeight = FollowTarget.transform.position.y + m_Height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, m_RotationDamping * Time.deltaTime);

        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, m_HeightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0.0f);

        Vector3 desiredPosition = FollowTarget.transform.position;
        desiredPosition -= currentRotation * Vector3.forward * m_Distance;
        desiredPosition.y = currentHeight;

        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * m_MoveSpeed);

        transform.LookAt(FollowTarget.transform);

        const float FAST_SPEED_RATIO = 0.9f;
        float targetFov = SpeedRatio > FAST_SPEED_RATIO ? m_FastFov : m_NormalFov;
        mCamera.fieldOfView = Mathf.Lerp(mCamera.fieldOfView, targetFov, Time.deltaTime * m_FovDamping);
    }
}
