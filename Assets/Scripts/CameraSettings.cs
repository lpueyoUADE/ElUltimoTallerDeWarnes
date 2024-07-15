using UnityEngine;

namespace ArcadeVehicleController
{
    [CreateAssetMenu]
    public class CameraSettings : ScriptableObject
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

        public float Distance => m_Distance;
        public float Height => m_Height;
        public float HeightDamping => m_HeightDamping;
        public float RotationDamping => m_RotationDamping;
        public float MoveSpeed => m_MoveSpeed;
        public float NormalFov => m_NormalFov;
        public float FastFov => m_FastFov;
        public float FovDamping => m_FovDamping;
    }
}