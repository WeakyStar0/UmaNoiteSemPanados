using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color exhaustedColor = new Color(0.9f, 0.2f, 0.2f);
    [SerializeField] private float colorLerpSpeed = 4f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        if (StaminaSystem.Instance == null) return;

        float normalized = StaminaSystem.Instance.CurrentStamina / StaminaSystem.Instance.MaxStamina;
        slider.value = normalized;

        Color targetColor = StaminaSystem.Instance.IsExhausted ? exhaustedColor : normalColor;
        fillImage.color = Color.Lerp(fillImage.color, targetColor, Time.deltaTime * colorLerpSpeed);

        float targetAlpha = normalized >= 1f && !StaminaSystem.Instance.IsExhausted ? 0f : 1f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 5f);
    }
}
