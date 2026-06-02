using UnityEngine;
using UnityEngine.Rendering;

public class CameraBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;

    [Header("Walk Bob")]
    [SerializeField] private float walkFrequency = 1.8f;
    [SerializeField] private float walkAmplitudeY = 0.012f;
    [SerializeField] private float walkAmplitudeX = 0.006f;

    [Header("Run Bob")]
    [SerializeField] private float runFrequency = 2.8f;
    [SerializeField] private float runAmplitudeY = 0.026f;
    [SerializeField] private float runAmplitudeX = 0.013f;
    [SerializeField] private float runSpeedThreshold = 5.5f;

    [Header("Idle Breathe")]
    [SerializeField] private float breathFrequency = 0.4f;
    [SerializeField] private float breathAmplitudeY = 0.003f;
    [SerializeField] private float swayFrequency = 0.27f;
    [SerializeField] private float swayAmplitudeX = 0.002f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 8f;

    private float bobTimer;
    private Vector3 currentOffset;
    private Vector3 savedPos;
    private Quaternion savedRot;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginRender;
        RenderPipelineManager.endCameraRendering += OnEndRender;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginRender;
        RenderPipelineManager.endCameraRendering -= OnEndRender;
    }

    void Update()
    {
        float speed = new Vector3(
            characterController.velocity.x, 0f, characterController.velocity.z).magnitude;

        bool grounded = characterController.isGrounded;
        bool moving = speed > 0.1f && grounded;
        bool running = speed >= runSpeedThreshold && grounded;

        Vector3 targetOffset;

        if (moving)
        {
            float freq = running ? runFrequency : walkFrequency;
            float ampY = running ? runAmplitudeY : walkAmplitudeY;
            float ampX = running ? runAmplitudeX : walkAmplitudeX;

            bobTimer += Time.deltaTime * freq * (speed / (running ? runSpeedThreshold : 2f));

            float sinY = Mathf.Sin(bobTimer * Mathf.PI * 2f);
            float sinX = Mathf.Sin(bobTimer * Mathf.PI);

            targetOffset = new Vector3(sinX * ampX, sinY * ampY, 0f);
        }
        else
        {
            float breathY = Mathf.Sin(Time.time * breathFrequency * Mathf.PI * 2f) * breathAmplitudeY;
            float swayX = Mathf.Sin(Time.time * swayFrequency * Mathf.PI * 2f) * swayAmplitudeX;
            targetOffset = new Vector3(swayX, breathY, 0f);
        }

        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);
    }

    void OnBeginRender(ScriptableRenderContext ctx, Camera cam)
    {
        if (cam != mainCamera) return;
        savedPos = cam.transform.position;
        savedRot = cam.transform.rotation;
        cam.transform.position += cam.transform.right * currentOffset.x
                                 + cam.transform.up    * currentOffset.y;
    }

    void OnEndRender(ScriptableRenderContext ctx, Camera cam)
    {
        if (cam != mainCamera) return;
        cam.transform.position = savedPos;
        cam.transform.rotation = savedRot;
    }
}
