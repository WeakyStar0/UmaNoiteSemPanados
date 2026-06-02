using UnityEngine;
using StarterAssets;

public class StaminaSystem : MonoBehaviour
{
    public static StaminaSystem Instance { get; private set; }

    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float drainRate = 20f;
    [SerializeField] private float regenRate = 15f;
    [SerializeField] private float regenDelay = 1f;

    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;
    public bool IsExhausted { get; private set; }
    public bool CanSprint => !IsExhausted;

    private StarterAssetsInputs inputs;
    private CharacterController characterController;
    private float regenDelayTimer;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        CurrentStamina = maxStamina;
    }

    void Start()
    {
        inputs = GetComponent<StarterAssetsInputs>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (IsExhausted) inputs.sprint = false;

        float horizontalSpeed = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z).magnitude;
        bool actuallysprinting = inputs.sprint && horizontalSpeed > 0.1f && characterController.isGrounded;

        if (actuallysprinting)
        {
            regenDelayTimer = regenDelay;
            CurrentStamina -= drainRate * Time.deltaTime;

            if (CurrentStamina <= 0f)
            {
                CurrentStamina = 0f;
                IsExhausted = true;
            }
        }
        else
        {
            if (regenDelayTimer > 0f)
            {
                regenDelayTimer -= Time.deltaTime;
            }
            else
            {
                CurrentStamina = Mathf.Min(maxStamina, CurrentStamina + regenRate * Time.deltaTime);

                if (IsExhausted && CurrentStamina >= maxStamina * 0.9f)
                    IsExhausted = false;
            }
        }
    }
}
