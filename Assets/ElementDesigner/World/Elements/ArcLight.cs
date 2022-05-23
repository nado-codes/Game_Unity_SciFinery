using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcLight : MonoBehaviour
{
    private Light _light;
    private AudioSource _audio;
    private float lightNum = 0;
    private float lightMaxIntensity = 1;
    private bool enableLight = true;
    public bool Active = true;
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponentInChildren<Light>();
        _audio = GetComponent<AudioSource>();
        lightMaxIntensity = _light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            lightNum += Time.deltaTime;

            if (Mathf.Round(lightNum * 10f) % 1 == 0)
                enableLight = !enableLight;
        }

        _light.enabled = enableLight && Active;
    }

    public void SetActive(bool value)
    {
        if (value)
            _audio.Play();
        else
            _audio.Stop();
    }
}
