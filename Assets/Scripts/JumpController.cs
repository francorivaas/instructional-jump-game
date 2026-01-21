using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpController : MonoBehaviour
{
    [Header("Charge")]
    public float maxCharge = 1f;
    public float chargeSpeed = 1f;

    [Header("Force")]
    public float horizontalStrength = 8f;
    public float verticalStrength = 14f;

    [Header("Preview")]
    public LineRenderer lineRenderer;
    public float previewLength = 3f;

    [Header("UI")]
    public JumpPowerUI powerUI;

    [Header("Max Charge Shake")]
    public Transform visual;
    public float shakeIntensity = 0.05f;
    public float shakeSpeed = 40f;

    private float charge;
    private bool isGrounded;
    private bool isCharging;

    private Rigidbody2D rb;
    private Vector3 visualOriginalLocalPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        visualOriginalLocalPos = visual.localPosition;
    }

    void Update()
    {
        if (!isGrounded)
        {
            ResetVisuals();
            return;
        }

        // 👉 CANCELAR SALTO (BOTÓN DERECHO)
        if (Input.GetMouseButtonDown(1))
        {
            CancelCharge();
            return;
        }

        // 👉 CARGA DE SALTO
        if (Input.GetMouseButton(0))
        {
            isCharging = true;

            charge += chargeSpeed * Time.deltaTime;
            charge = Mathf.Clamp(charge, 0f, maxCharge);

            UpdatePreview();
            powerUI.SetValue(charge / maxCharge);

            if (charge >= maxCharge)
                ApplyShake();
        }

        // 👉 SOLTAR SALTO
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            Jump();
        }
    }

    void LateUpdate()
    {
        // Seguridad absoluta: siempre derecho
        transform.rotation = Quaternion.identity;
    }

    void Jump()
    {
        isGrounded = false;
        isCharging = false;

        float horizontal = GetHorizontalInput();

        Vector2 force = new Vector2(
            horizontal * horizontalStrength,
            verticalStrength
        ) * charge;

        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        charge = 0f;
        ResetVisuals();
    }

    void CancelCharge()
    {
        charge = 0f;
        isCharging = false;
        ResetVisuals();
    }

    void UpdatePreview()
    {
        lineRenderer.enabled = true;

        Vector3 start = transform.position;
        Vector3 dir = new Vector3(GetHorizontalInput(), 1f, 0f).normalized;
        float length = previewLength * (charge / maxCharge);

        Vector3 end = start + dir * length;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    void ApplyShake()
    {
        float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
        float offsetY = Mathf.Cos(Time.time * shakeSpeed) * shakeIntensity;

        visual.localPosition =
            visualOriginalLocalPos + new Vector3(offsetX, offsetY, 0f);
    }

    void ResetVisuals()
    {
        lineRenderer.enabled = false;
        powerUI.SetVisible(false);
        visual.localPosition = visualOriginalLocalPos;
    }

    float GetHorizontalInput()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float dir = mouseWorld.x - transform.position.x;
        return Mathf.Clamp(dir, -1f, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.6f)
            {
                isGrounded = true;
                break;
            }
        }
    }
}
