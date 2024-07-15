using ArcadeVehicleController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{
    private static readonly Wheel[] s_Wheels = new Wheel[]
    {
        Wheel.FrontLeft, Wheel.FrontRight, Wheel.BackLeft, Wheel.BackRight
    };

    private static readonly Wheel[] s_FrontWheels = new Wheel[] { Wheel.FrontLeft, Wheel.FrontRight };
    private static readonly Wheel[] s_BackWheels = new Wheel[] { Wheel.BackLeft, Wheel.BackRight };

    [Header("Car Settings")]
    [SerializeField] private VehicleSettings m_Settings;

    [Header("Wheels")]
    [SerializeField] private Transform m_WheelFrontLeft;
    [SerializeField] private Transform m_WheelFrontRight;
    [SerializeField] private Transform m_WheelBackLeft;
    [SerializeField] private Transform m_WheelBackRight;
    [SerializeField] private float m_WheelsSpinSpeed;
    [SerializeField] private float m_WheelYWhenSpringMin;
    [SerializeField] private float m_WheelYWhenSpringMax;

    [Header("Lights")]
    [SerializeField] private Light frontLight_Right;
    [SerializeField] private Light frontLight_Left;

    [SerializeField] private Material frontLight_Material;
    [SerializeField] private Material backLight_Material;

    [Header("SkidMarks")]
    [SerializeField] private TrailRenderer[] skidMarks = new TrailRenderer[2];
    [SerializeField] private float skidMarkThreshold;

    [Header("Exhaust")]
    [SerializeField] private ParticleSystem smokeParticles;

    [Header("Sounds")]
    [SerializeField] private AudioSource hornSound;
    [SerializeField] private AudioSource skidmarkSound;
    [SerializeField] private AudioSource revingSound;
    [SerializeField] private float maxVolume;

    private bool lightsOn;
    private Color INITIAL_BACKLIGHT_EMISSIVE_COLOR;

    private bool handBrake;
    private float handBrakeSlipFactor = 0.5f;

    private bool resetCar;
    private bool horn;

    private bool speedBoosted;

    private Quaternion m_WheelFrontLeftRoll;
    private Quaternion m_WheelFrontRightRoll;


    public bool IsMovingForward { get; set; }

    public float ForwardSpeed { get; set; }

    public float AccelerateInput { get; set; }
    public float SteerInput { get; set; }

    public float SteerAngle { get; set; }

    public float SpringsRestLength { get; set; }

    public bool IsBreaking { get{ return Input.GetAxisRaw("Vertical") < 0; }}

    public bool IsAccelerating { get { return Input.GetAxisRaw("Vertical") > 0; } }

    public bool SpeedBoosted { get; set; }

    public Dictionary<Wheel, float> SpringsCurrentLength { get; set; } = new()
        {
            { Wheel.FrontLeft, 0.0f },
            { Wheel.FrontRight, 0.0f },
            { Wheel.BackLeft, 0.0f },
            { Wheel.BackRight, 0.0f }
        };

    private Transform m_Transform;
    private BoxCollider m_BoxCollider;
    private Rigidbody m_Rigidbody;
    private Dictionary<Wheel, SpringData> m_SpringDatas;

    public VehicleSettings Settings => m_Settings;
    public Vector3 Forward => m_Transform.forward;
    public Vector3 Velocity => m_Rigidbody.velocity;

    public Vector3 LocalVelocity => transform.InverseTransformDirection(m_Rigidbody.velocity);

    public static event Action ItemPickedAction;
    public static event Action TimeStopBoostAction;
    public static event Action SpeedBoostAction;
    private void Start()
    {
        Application.targetFrameRate = 60;

        m_WheelFrontLeftRoll = m_WheelFrontLeft.localRotation;
        m_WheelFrontRightRoll = m_WheelFrontRight.localRotation;

        SpringsRestLength = Settings.SpringRestLength;
        SteerAngle = Settings.SteerAngle;

        SpringsCurrentLength[Wheel.FrontLeft] = GetSpringCurrentLength(Wheel.FrontLeft);
        SpringsCurrentLength[Wheel.FrontRight] = GetSpringCurrentLength(Wheel.FrontRight);
        SpringsCurrentLength[Wheel.BackLeft] = GetSpringCurrentLength(Wheel.BackLeft);
        SpringsCurrentLength[Wheel.BackRight] = GetSpringCurrentLength(Wheel.BackRight);

        lightsOn = false;
        INITIAL_BACKLIGHT_EMISSIVE_COLOR = backLight_Material.GetColor("_EmissionColor");

        SpeedBoosted = false;
    }

    private void Update()
    {
        AccelerateInput = Mathf.Clamp(Input.GetAxis("Vertical"), -1.0f, 1.0f); 
        SteerInput = Mathf.Clamp(Input.GetAxis("Horizontal"), -1.0f, 1.0f);
        lightsOn = Input.GetKeyDown(KeyCode.L)? !lightsOn : lightsOn;
        handBrake = Input.GetKey(KeyCode.Space);
        resetCar = Input.GetKeyDown(KeyCode.Escape);
        horn = Input.GetKeyDown(KeyCode.H);

        ResetCar();
        UpdateHorn();
        UpdateLights();
        UpdateWheelVisuals();
        UpdateSkidmarks();
        UpdateSmokeParticles();
    }
    private class SpringData
    {
        public float CurrentLength;
        public float CurrentVelocity;
    }

    private void Awake()
    {
        m_Transform = transform;
        InitializeCollider();
        InitializeBody();

        m_SpringDatas = new Dictionary<Wheel, SpringData>();
        foreach (Wheel wheel in s_Wheels)
        {
            m_SpringDatas.Add(wheel, new());
        }
    }

    private void FixedUpdate()
    {
        UpdateSuspension();

        UpdateSteering();

        UpdateAccelerate();

        UpdateBrakes();

        UpdateAirResistance();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            GameObject go = other.gameObject;
            var pc = go.GetComponent<PickableController>();
            pc.Interact();

            ItemPickedAction?.Invoke();

        } else if (other.CompareTag("TimeStopBoost"))
        {
            GameObject go = other.gameObject;
            var tsbc = go.GetComponent<TimeStopBoostController>();
            tsbc.Interact();

           TimeStopBoostAction?.Invoke();
        }

        else if (other.CompareTag("SpeedBoost"))
        {
            GameObject go = other.gameObject;
            var sbc = go.GetComponent<SpeedBoostController>();
            sbc.Interact();

            SpeedBoostAction?.Invoke();
        }
    }
    private void ResetCar()
    {
        if(resetCar)
        {
            this.transform.rotation = Quaternion.identity;
        }
    }
    private void UpdateHorn()
    {
        if (horn)
            hornSound.Play();
    }

    private void UpdateLights()
    {
        frontLight_Right.enabled = lightsOn;
        frontLight_Left.enabled = lightsOn;

        if(lightsOn)
        {
            frontLight_Material.EnableKeyword("_EMISSION");
            backLight_Material.EnableKeyword("_EMISSION");
        }
        else
        {
            frontLight_Material.DisableKeyword("_EMISSION");
            backLight_Material.DisableKeyword("_EMISSION");
        }

        if (IsBreaking)
        {
            backLight_Material.EnableKeyword("_EMISSION");
            backLight_Material.SetColor("_EmissionColor", INITIAL_BACKLIGHT_EMISSIVE_COLOR);
        } else
        {
            backLight_Material.SetColor("_EmissionColor", INITIAL_BACKLIGHT_EMISSIVE_COLOR);
        }
    }

    private void UpdateSkidmarks()
    {
        skidMarks[0].emitting = IsGrounded(Wheel.BackLeft) && Mathf.Abs(LocalVelocity.x) > skidMarkThreshold;
        skidMarks[1].emitting = IsGrounded(Wheel.BackRight) && Mathf.Abs(LocalVelocity.x) > skidMarkThreshold;

        skidmarkSound.mute = !IsBreaking && !skidMarks[0].emitting && !skidMarks[1].emitting;
    }

    private void UpdateSmokeParticles()
    {
        var emission = smokeParticles.emission;
        emission.rateOverTime = 5 + 15 * Mathf.Abs(AccelerateInput);
    }
    private void UpdateWheelVisuals() {

        float forwardSpeed = Vector3.Dot(Forward, Velocity);
        ForwardSpeed = forwardSpeed;
        IsMovingForward = forwardSpeed > 0.0f;

        SpringsCurrentLength[Wheel.FrontLeft] = GetSpringCurrentLength(Wheel.FrontLeft);
        SpringsCurrentLength[Wheel.FrontRight] = GetSpringCurrentLength(Wheel.FrontRight);
        SpringsCurrentLength[Wheel.BackLeft] = GetSpringCurrentLength(Wheel.BackLeft);
        SpringsCurrentLength[Wheel.BackRight] = GetSpringCurrentLength(Wheel.BackRight);

        if (SpringsCurrentLength[Wheel.FrontLeft] < SpringsRestLength)
        {
            m_WheelFrontLeftRoll *= Quaternion.AngleAxis(ForwardSpeed * m_WheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (SpringsCurrentLength[Wheel.FrontRight] < SpringsRestLength)
        {
            m_WheelFrontRightRoll *= Quaternion.AngleAxis(ForwardSpeed * m_WheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (SpringsCurrentLength[Wheel.BackLeft] < SpringsRestLength)
        {
            m_WheelBackLeft.localRotation *= Quaternion.AngleAxis(ForwardSpeed * m_WheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (SpringsCurrentLength[Wheel.BackRight] < SpringsRestLength)
        {
            m_WheelBackRight.localRotation *= Quaternion.AngleAxis(ForwardSpeed * m_WheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        m_WheelFrontLeft.localRotation = Quaternion.AngleAxis(SteerInput * SteerAngle, Vector3.up) * m_WheelFrontLeftRoll;
        m_WheelFrontRight.localRotation = Quaternion.AngleAxis(SteerInput * SteerAngle, Vector3.up) * m_WheelFrontRightRoll;

        float springFrontLeftRatio = SpringsCurrentLength[Wheel.FrontLeft] / SpringsRestLength;
        float springFrontRightRatio = SpringsCurrentLength[Wheel.FrontRight] / SpringsRestLength;
        float springBackLeftRatio = SpringsCurrentLength[Wheel.BackLeft] / SpringsRestLength;
        float springBackRightRatio = SpringsCurrentLength[Wheel.BackRight] / SpringsRestLength;

        m_WheelFrontLeft.localPosition = new Vector3(m_WheelFrontLeft.localPosition.x,
            m_WheelYWhenSpringMin + (m_WheelYWhenSpringMax - m_WheelYWhenSpringMin) * springFrontLeftRatio,
            m_WheelFrontLeft.localPosition.z);

        m_WheelFrontRight.localPosition = new Vector3(m_WheelFrontRight.localPosition.x,
            m_WheelYWhenSpringMin + (m_WheelYWhenSpringMax - m_WheelYWhenSpringMin) * springFrontRightRatio,
            m_WheelFrontRight.localPosition.z);

        m_WheelBackRight.localPosition = new Vector3(m_WheelBackRight.localPosition.x,
            m_WheelYWhenSpringMin + (m_WheelYWhenSpringMax - m_WheelYWhenSpringMin) * springBackRightRatio,
            m_WheelBackRight.localPosition.z);

        m_WheelBackLeft.localPosition = new Vector3(m_WheelBackLeft.localPosition.x,
            m_WheelYWhenSpringMin + (m_WheelYWhenSpringMax - m_WheelYWhenSpringMin) * springBackLeftRatio,
            m_WheelBackLeft.localPosition.z);
    }

    public float GetSpringCurrentLength(Wheel wheel)
    {
        return m_SpringDatas[wheel].CurrentLength;
    }

    private void InitializeCollider()
    {
        if (!TryGetComponent(out m_BoxCollider))
        {
            m_BoxCollider = gameObject.AddComponent<BoxCollider>();
        }

        m_BoxCollider.center = Vector3.zero;
        m_BoxCollider.size = new Vector3(m_Settings.Width, m_Settings.Height, m_Settings.Length);
        m_BoxCollider.isTrigger = false;
        m_BoxCollider.enabled = true;
    }

    private void InitializeBody()
    {
        if (!TryGetComponent(out m_Rigidbody))
        {
            m_Rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        const int WHEELS_COUNT = 4;
        m_Rigidbody.mass = m_Settings.ChassiMass + m_Settings.TireMass * WHEELS_COUNT;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;
        m_Rigidbody.drag = 0.0f;
        m_Rigidbody.angularDrag = 0.0f;
        m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        m_Rigidbody.constraints = RigidbodyConstraints.None;
    }

    // To be called once per physics frame per spring.
    // Updates the spring currentVelocity and currentLength.
    private void CastSpring(Wheel wheel)
    {
        Vector3 position = GetSpringPosition(wheel);

        float previousLength = m_SpringDatas[wheel].CurrentLength;

        float currentLength;

        if (Physics.Raycast(position, -m_Transform.up, out var hit, m_Settings.SpringRestLength))
        {
            currentLength = hit.distance;
        }
        else
        {
            currentLength = m_Settings.SpringRestLength;
        }

        m_SpringDatas[wheel].CurrentVelocity = (currentLength - previousLength) / Time.fixedDeltaTime;
        m_SpringDatas[wheel].CurrentLength = currentLength;
    }

    private Vector3 GetSpringRelativePosition(Wheel wheel)
    {
        Vector3 boxSize = m_BoxCollider.size;
        float boxBottom = boxSize.y * -0.5f;

        float paddingX = m_Settings.WheelsPaddingX;
        float paddingZ = m_Settings.WheelsPaddingZ;

        switch (wheel)
        {
            case Wheel.FrontLeft:
                return new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (0.5f - paddingZ));
            case Wheel.FrontRight:
                return new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (0.5f - paddingZ));
            case Wheel.BackLeft:
                return new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (paddingZ - 0.5f));
            case Wheel.BackRight:
                return new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (paddingZ - 0.5f));
            default:
                return default;
        }
    }

    private Vector3 GetSpringPosition(Wheel wheel)
    {
        return m_Transform.localToWorldMatrix.MultiplyPoint3x4(GetSpringRelativePosition(wheel));
    }

    private Vector3 GetSpringHitPosition(Wheel wheel)
    {
        Vector3 vehicleDown = -m_Transform.up;
        return GetSpringPosition(wheel) + m_SpringDatas[wheel].CurrentLength * vehicleDown;
    }

    private Vector3 GetWheelRollDirection(Wheel wheel)
    {
        bool frontWheel = wheel == Wheel.FrontLeft || wheel == Wheel.FrontRight;

        if (frontWheel)
        {
            var steerQuaternion = Quaternion.AngleAxis(SteerInput * m_Settings.SteerAngle, Vector3.up);
            return steerQuaternion * m_Transform.forward;
        }
        else
        {
            return m_Transform.forward;
        }
    }

    private Vector3 GetWheelSlideDirection(Wheel wheel)
    {
        Vector3 forward = GetWheelRollDirection(wheel);
        return Vector3.Cross(m_Transform.up, forward);
    }

    private Vector3 GetWheelTorqueRelativePosition(Wheel wheel)
    {
        Vector3 boxSize = m_BoxCollider.size;

        float paddingX = m_Settings.WheelsPaddingX;
        float paddingZ = m_Settings.WheelsPaddingZ;

        switch (wheel)
        {
            case Wheel.FrontLeft:
                return new Vector3(boxSize.x * (paddingX - 0.5f), 0.0f, boxSize.z * (0.5f - paddingZ));
            case Wheel.FrontRight:
                return new Vector3(boxSize.x * (0.5f - paddingX), 0.0f, boxSize.z * (0.5f - paddingZ));
            case Wheel.BackLeft:
                return new Vector3(boxSize.x * (paddingX - 0.5f), 0.0f, boxSize.z * (paddingZ - 0.5f));
            case Wheel.BackRight:
                return new Vector3(boxSize.x * (0.5f - paddingX), 0.0f, boxSize.z * (paddingZ - 0.5f));
            default:
                return default;
        }
    }

    private Vector3 GetWheelTorquePosition(Wheel wheel)
    {
        return m_Transform.localToWorldMatrix.MultiplyPoint3x4(GetWheelTorqueRelativePosition(wheel));
    }

    private float GetWheelGripFactor(Wheel wheel)
    {
        bool frontWheel = wheel == Wheel.FrontLeft || wheel == Wheel.FrontRight;
        return frontWheel ? m_Settings.FrontWheelsGripFactor : (handBrake ? handBrakeSlipFactor : m_Settings.RearWheelsGripFactor);
    }

    private bool IsGrounded(Wheel wheel)
    {
        return m_SpringDatas[wheel].CurrentLength < m_Settings.SpringRestLength;
    }

    private void UpdateSuspension()
    {
        foreach (Wheel id in m_SpringDatas.Keys)
        {
            CastSpring(id);
            float currentLength = m_SpringDatas[id].CurrentLength;
            float currentVelocity = m_SpringDatas[id].CurrentVelocity;

            float force = SpringMath.CalculateForceDamped(currentLength, currentVelocity,
                m_Settings.SpringRestLength, m_Settings.SpringStrength,
                m_Settings.SpringDamper);

            m_Rigidbody.AddForceAtPosition(force * m_Transform.up, GetSpringPosition(id));
        }
    }

    private void UpdateSteering()
    {
        foreach (Wheel wheel in s_Wheels)
        {
            if (!IsGrounded(wheel))
            {
                continue;
            }

            Vector3 springPosition = GetSpringPosition(wheel);

            Vector3 slideDirection = GetWheelSlideDirection(wheel);
            float slideVelocity = Vector3.Dot(slideDirection, m_Rigidbody.GetPointVelocity(springPosition));

            float desiredVelocityChange = -slideVelocity * GetWheelGripFactor(wheel);
            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;

            Vector3 force = desiredAcceleration * m_Settings.TireMass * slideDirection;
            m_Rigidbody.AddForceAtPosition(force, GetWheelTorquePosition(wheel));
        }
    }

    private void UpdateAccelerate()
    {
        revingSound.volume = 0;
        revingSound.pitch = 1;

        if (Mathf.Approximately(AccelerateInput, 0.0f))
        {
            return;
        }

        float forwardSpeed = Vector3.Dot(m_Transform.forward, m_Rigidbody.velocity);
        bool movingForward = forwardSpeed > 0.0f;
        float speed = Mathf.Abs(forwardSpeed);

        float maxSpeed = SpeedBoosted ? m_Settings.MaxSpeed + 350 : m_Settings.MaxSpeed;

        float speedRatio = speed / maxSpeed;

        revingSound.volume = speedRatio * maxVolume;
        revingSound.pitch = 1 + speedRatio;

        if (movingForward && speed > maxSpeed)
        {
            return;
        }
        else if (!movingForward && speed > m_Settings.MaxReverseSpeed)
        {
            return;
        }

        foreach (Wheel wheel in s_Wheels)
        {
            if (!IsGrounded(wheel))
            {
                continue;
            }

            Vector3 position = GetWheelTorquePosition(wheel);
            Vector3 wheelForward = GetWheelRollDirection(wheel);

            float acceleratePower = SpeedBoosted ? m_Settings.AcceleratePower + 500 : m_Settings.AcceleratePower;
            m_Rigidbody.AddForceAtPosition(AccelerateInput * acceleratePower * wheelForward, position);
        }
    }

    private void UpdateBrakes()
    {
        float forwardSpeed = Vector3.Dot(m_Transform.forward, m_Rigidbody.velocity);
        float speed = Mathf.Abs(forwardSpeed);

        float brakesRatio;

        const float ALMOST_STOPPING_SPEED = 2.0f;
        bool almostStopping = speed < ALMOST_STOPPING_SPEED;
        if (almostStopping)
        {
            brakesRatio = 1.0f;
        }
        else
        {
            if (handBrake)
            {
                brakesRatio = 1.5f;
            }
            else
            {
                bool accelerateContrary =
                    !Mathf.Approximately(AccelerateInput, 0.0f) &&
                    Vector3.Dot(AccelerateInput * m_Transform.forward, m_Rigidbody.velocity) < 0.0f;
                if (accelerateContrary)
                {
                    brakesRatio = 1.0f;
                }
                else if (Mathf.Approximately(AccelerateInput, 0.0f)) // No accelerate input
                {
                    brakesRatio = 0.1f;
                }
                else
                {
                    return;
                }
            }
        }

        foreach (Wheel wheel in s_BackWheels)
        {
            if (!IsGrounded(wheel))
            {
                continue;
            }

            Vector3 springPosition = GetSpringPosition(wheel);
            Vector3 rollDirection = GetWheelRollDirection(wheel);
            float rollVelocity = Vector3.Dot(rollDirection, m_Rigidbody.GetPointVelocity(springPosition));

            float desiredVelocityChange = -rollVelocity * m_Settings.BrakesPower * brakesRatio;
            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;

            Vector3 force = desiredAcceleration * m_Settings.TireMass * rollDirection;
            m_Rigidbody.AddForceAtPosition(force, GetWheelTorquePosition(wheel));
        }
    }

    private void UpdateAirResistance()
    {
        m_Rigidbody.AddForce(m_BoxCollider.size.magnitude * m_Settings.AirResistance * -m_Rigidbody.velocity);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector3 vehicleDown = -transform.up;

            foreach (Wheel wheel in m_SpringDatas.Keys)
            {
                // Spring
                Vector3 position = GetSpringPosition(wheel);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(position, position + vehicleDown * m_Settings.SpringRestLength);
                Gizmos.color = Color.red;
                Gizmos.DrawCube(GetSpringHitPosition(wheel), Vector3.one * 0.08f);

                // Wheel
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(position, GetWheelRollDirection(wheel));
                Gizmos.color = Color.red;
                Gizmos.DrawRay(position, GetWheelSlideDirection(wheel));
            }
        }
        else
        {
            if (m_Settings != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position,
                    new Vector3(
                        m_Settings.Width,
                        m_Settings.Height,
                        m_Settings.Length));
            }
        }
    }
#endif
}
