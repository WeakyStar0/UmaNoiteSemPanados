using System.Collections;
using UnityEngine;

public class GarageDoor : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Vector3 openLocalPosition;
    [SerializeField] private float openDuration = 1.8f;
    [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] [Range(0f, 1f)] private float openVolume = 1f;

    [Header("Debug")]
    [SerializeField] private bool isOpen;

    private Vector3 _closedLocalPosition;
    private bool _wasOpen;
    private Coroutine _moveCoroutine;

    void Start()
    {
        _closedLocalPosition = transform.localPosition;
        _wasOpen = isOpen;
    }

    void Update()
    {
        if (isOpen == _wasOpen) return;
        _wasOpen = isOpen;
        if (isOpen) Open();
        else Close();
    }

    public void Open()
    {
        isOpen = true;
        _wasOpen = true;
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        gameObject.SetActive(true);
        if (openSound != null)
            AudioSource.PlayClipAtPoint(openSound, transform.position, openVolume);
        _moveCoroutine = StartCoroutine(MoveRoutine(transform.localPosition, openLocalPosition, true));
    }

    void Close()
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        gameObject.SetActive(true);
        transform.localPosition = openLocalPosition;
        _moveCoroutine = StartCoroutine(MoveRoutine(openLocalPosition, _closedLocalPosition, false));
    }

    IEnumerator MoveRoutine(Vector3 from, Vector3 to, bool disableOnComplete)
    {
        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(from, to, openCurve.Evaluate(Mathf.Clamp01(elapsed / openDuration)));
            yield return null;
        }
        transform.localPosition = to;
        if (disableOnComplete) gameObject.SetActive(false);
    }
}
