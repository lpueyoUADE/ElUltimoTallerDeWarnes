using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] List<CameraSettings> cameras;

    [Header("Target")]
    public CarController FollowTarget;

    private CameraSettings selectedCamera;
    private int selectedCameraIndex;
    private Camera mCamera;
    public float SpeedRatio { get; set; }

    private void Start()
    {
        mCamera = GetComponent<Camera>();
        selectedCameraIndex = 0;
        selectedCamera = cameras[selectedCameraIndex];
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

        if(Input.GetKeyDown(KeyCode.C)) {
            selectedCameraIndex = (selectedCameraIndex + 1) % cameras.Count;
            selectedCamera = cameras[selectedCameraIndex];
        }
    }
    public void LateUpdate()
    {
        if (FollowTarget == null)
        {
            return;
        }


        float wantedRotationAngle = FollowTarget.transform.eulerAngles.y;
        float wantedHeight = FollowTarget.transform.position.y + selectedCamera.Height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, selectedCamera.RotationDamping * Time.deltaTime);

        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, selectedCamera.HeightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0.0f);

        Vector3 desiredPosition = FollowTarget.transform.position;
        desiredPosition -= currentRotation * Vector3.forward * selectedCamera.Distance;
        desiredPosition.y = currentHeight;

        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * selectedCamera.MoveSpeed);

        transform.LookAt(FollowTarget.transform);

        const float FAST_SPEED_RATIO = 0.9f;
        float targetFov = SpeedRatio > FAST_SPEED_RATIO ? selectedCamera.FastFov : selectedCamera.NormalFov;
        mCamera.fieldOfView = Mathf.Lerp(mCamera.fieldOfView, targetFov, Time.deltaTime * selectedCamera.FovDamping);
    }
}
