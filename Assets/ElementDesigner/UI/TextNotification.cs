using System;
using UnityEngine;
using UnityEngine.UI;

public class TextNotification : MonoBehaviour
{
    private static TextNotification instance;
    private static Text txNotification;
    private static Color startColor;

    public float DelayMS = 3000;

    void Awake()
    {
        instance = this;
        txNotification = GetComponentInChildren<Text>();
        startColor = txNotification.color;

        Show("Welcome to Element Designer");
    }

    static bool VerifyInitialize()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<TextNotification>();
            if (instance == null) return false;

            instance.Awake();
        }

        return true;
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            var textColor = txNotification.color;

            if (textColor.a > 0f)
            {
                textColor.a = Mathf.Lerp(textColor.a, textColor.a - .5f, Time.deltaTime);
                txNotification.color = textColor;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public static void Show(string message)
    {
        if (VerifyInitialize())
        {
            txNotification.color = startColor;
            txNotification.text = message;
            instance.gameObject.SetActive(true);
        }
    }
}