using UnityEngine;

public class RotationOscillator : MonoBehaviour
{
    [System.Serializable]
    public class AxisOscillation
    {
        public bool enabled = false;

        [Tooltip("Rango máximo de rotación en grados (positivo y negativo)")]
        [Range(0f, 360f)]
        public float range = 45f;

        [Tooltip("Velocidad de oscilación (ciclos por segundo)")]
        [Range(0.01f, 10f)]
        public float speed = 1f;

        [Tooltip("Desplazamiento del ángulo central")]
        public float angleOffset = 0f;
    }

    [Header("Oscilación por Eje")]
    public AxisOscillation axisX;
    public AxisOscillation axisY;
    public AxisOscillation axisZ;

    private Quaternion _initialRotation;

    void Start()
    {
        _initialRotation = transform.localRotation;
    }

    void Update()
    {
        float t = Time.time;

        float angleX = axisX.enabled ? Mathf.Sin(t * axisX.speed * Mathf.PI * 2f) * axisX.range + axisX.angleOffset : 0f;
        float angleY = axisY.enabled ? Mathf.Sin(t * axisY.speed * Mathf.PI * 2f) * axisY.range + axisY.angleOffset : 0f;
        float angleZ = axisZ.enabled ? Mathf.Sin(t * axisZ.speed * Mathf.PI * 2f) * axisZ.range + axisZ.angleOffset : 0f;

        Quaternion oscillation = Quaternion.AngleAxis(angleX, Vector3.right)
                               * Quaternion.AngleAxis(angleY, Vector3.up)
                               * Quaternion.AngleAxis(angleZ, Vector3.forward);

        transform.localRotation = _initialRotation * oscillation;
    }

    void OnDrawGizmosSelected()
    {
        if (axisX.enabled) DrawAxisGizmo(Vector3.right,   Color.red);
        if (axisY.enabled) DrawAxisGizmo(Vector3.up,      Color.green);
        if (axisZ.enabled) DrawAxisGizmo(Vector3.forward, Color.blue);
    }

    void DrawAxisGizmo(Vector3 axis, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(axis) * 1.5f);
    }
}