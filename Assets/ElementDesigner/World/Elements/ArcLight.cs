using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ArcLight : MonoBehaviour
{
    private Light _light;
    private bool enableLight = true;
    private float lightNum = 0, lightBaseIntensity = 0;

    private AudioSource _audio;
    private float audioBasePitch = 0;

    public bool Active = true;
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponentInChildren<Light>();
        Assertions.AssertNotNull(_light, "_light");
        _audio = GetComponent<AudioSource>();
        Assertions.AssertNotNull(_audio, "_audio");
        lightBaseIntensity = _light.intensity;
        audioBasePitch = _audio.pitch;
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
        if (value && !_audio.isPlaying)
        {
            _audio.Play();
        }
        if (!value)
            _audio.Stop();
    }
}
