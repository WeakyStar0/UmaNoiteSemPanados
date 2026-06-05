using System.Collections;
using UnityEngine;

public class KeypadButton : MonoBehaviour
{
    public char Key;
    public Light ButtonLight;

    private KeypadController _keypad;

    void Start()
    {
        _keypad = GetComponentInParent<KeypadController>();
        if (ButtonLight != null)
            ButtonLight.enabled = false;
    }

    public void Press()
    {
        _keypad?.PressKey(Key);
        if (ButtonLight != null)
            StartCoroutine(BlinkOnce());
    }

    public void TriggerFailBlink() => StartCoroutine(BlinkTimes(Color.red, 3, 0.15f, 0.12f));

    IEnumerator BlinkOnce()
    {
        if (ButtonLight == null) yield break;
        ButtonLight.color = Color.white;
        ButtonLight.enabled = true;
        yield return new WaitForSeconds(0.12f);
        ButtonLight.enabled = false;
    }

    IEnumerator BlinkTimes(Color color, int count, float onDuration, float offDuration)
    {
        if (ButtonLight == null) yield break;
        for (int i = 0; i < count; i++)
        {
            ButtonLight.color = color;
            ButtonLight.enabled = true;
            yield return new WaitForSeconds(onDuration);
            ButtonLight.enabled = false;
            if (i < count - 1)
                yield return new WaitForSeconds(offDuration);
        }
    }
}
