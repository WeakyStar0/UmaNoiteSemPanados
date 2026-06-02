using UnityEngine;
using StarterAssets;

public class ItemBobSway : MonoBehaviour
{
    [Header("Bob")]
    [SerializeField] private float bobFrequency = 10f;
    [SerializeField] private float bobAmountY = 0.004f;
    [SerializeField] private float bobAmountX = 0.002f;
    [SerializeField] private float bobSmoothing = 12f;
    [SerializeField] private float sprintBobMultiplier = 2f;

    [Header("Sway")]
    [SerializeField] private float swayPositionAmount = 0.0008f;
    [SerializeField] private float swayMaxAmount = 0.05f;
    [SerializeField] private float swayPositionSmoothing = 8f;
    [SerializeField] private float swayRotationAmount = 0.3f;
    [SerializeField] private float swayRotationSmoothing = 8f;

    [Header("References")]
    [SerializeField] private Transform itemHandPos;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private StarterAssetsInputs inputs;

    private float bobTimer;
    private Vector3 bobOffset;
    private Vector3 swayPosOffset;
    private Quaternion swayRotOffset = Quaternion.identity;
    private Vector3 baseLocalPos;

    void Start()
    {
        baseLocalPos = itemHandPos.localPosition;
    }

    void Update()
    {
        UpdateBob();
        UpdateSway();
        Apply();
    }

    void UpdateBob()
    {
        float speed = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z).magnitude;
        bool moving = speed > 0.1f && characterController.isGrounded;

        if (moving)
        {
            float multiplier = inputs.sprint ? sprintBobMultiplier : 1f;
            bobTimer += Time.deltaTime * bobFrequency * multiplier;
            Vector3 target = new Vector3(
                Mathf.Sin(bobTimer / 2f) * bobAmountX * multiplier,
                Mathf.Sin(bobTimer) * bobAmountY * multiplier,
                0f
            );
            bobOffset = Vector3.Lerp(bobOffset, target, Time.deltaTime * bobSmoothing);
        }
        else
        {
            bobOffset = Vector3.Lerp(bobOffset, Vector3.zero, Time.deltaTime * bobSmoothing);
        }
    }

    void UpdateSway()
    {
        Vector2 look = inputs.look;

        Vector3 swayTarget = Vector3.ClampMagnitude(
            new Vector3(look.x * swayPositionAmount, -look.y * swayPositionAmount, 0f),
            swayMaxAmount
        );
        swayPosOffset = Vector3.Lerp(swayPosOffset, swayTarget, Time.deltaTime * swayPositionSmoothing);

        Quaternion rotTarget = Quaternion.Euler(
            -look.y * swayRotationAmount,
             look.x * swayRotationAmount,
            -look.x * swayRotationAmount
        );
        swayRotOffset = Quaternion.Slerp(swayRotOffset, rotTarget, Time.deltaTime * swayRotationSmoothing);
    }

    void Apply()
    {
        itemHandPos.localPosition = baseLocalPos + bobOffset + swayPosOffset;
        itemHandPos.localRotation = swayRotOffset;
    }
}
