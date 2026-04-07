using UnityEngine;

namespace ColbyO.Untitled.Player
{
    [CreateAssetMenu(fileName = "DefaultMovementSettings", menuName = "Player/MovementSettings")]
    public class MovementSettings : ScriptableObject
    {
        [Header("Look")]
        [Min(0)] public float ControllerSensitivityScaleFactor = 2f;
        [Min(0)] public float Sensitivity = 3f;
        public Vector2 YLookLimit = new Vector2(-80f, 80f);
        public bool InvertLookY = false;
        public bool InvertLookX = false;

        [Header("Movement")]
        [Min(0)] public float Speed = 10f;
        [Min(0)] public float SprintSpeedMul = 1.5f;
        [Range(0f, 1f)] public float BackwardSpeedMul = 0.5f;
        [Range(0f, 1f)] public float StrafingSpeedMul = 1f;
        [Min(0)] public float GravityMul = 1f;
        [Min(0)] public float InputSmoothing = 15f;
        [Range(0f, 1f)] public float AirControl = 0.2f;
        [Min(0)] public float TerminalVelocity = 50f;
    }
}
