using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity;
using UnityEngine.UI;
using UnityEngine;

public class WorldParticle : WorldElement
{
  private WorldElementMotor motor;
  private WorldElementMotor Motor
  {
    get
    {
      if (motor == null)
        motor = GetComponent<WorldElementMotor>();
      if (motor == null)
        throw new ApplicationException("WorldParticle requires a WorldElementMotor to work properly. Please add one first.");

      return motor;
    }
  }

  private GameObject _orbNoCharge, _orbCharge, _orbNegCharge;

  protected override void Start()
  {
    base.Start();
    VerifyInitialize();
  }

  protected override void VerifyInitialize()
  {
    _orbNoCharge = BodyTransform.Find("Orb_NoCharge")?.gameObject;
    Assertions.AssertNotNull(_orbNoCharge, "_orbNoCharge");
    _orbCharge = BodyTransform.Find("Orb_Charge")?.gameObject;
    Assertions.AssertNotNull(_orbNoCharge, "_orbCharge");
    _orbNegCharge = BodyTransform.Find("Orb_NegCharge")?.gameObject;
    Assertions.AssertNotNull(_orbNoCharge, "_orbNegCharge");

    base.VerifyInitialize();
  }

  protected override void Update()
  {
    var audioSource = GetComponentsInChildren<AudioSource>().FirstOrDefault(a => a.clip.name == "PowerlineNoise");
    if (Data.Charge < 0 && audioSource != null)
    {
      if (!audioSource.isPlaying)
        audioSource.Play();
    }
    base.Update();
  }

  // TODO: Everything is a WorldElement but not every WorldElement can move or has a trail
  // WorldElementMotor - For movement
  // WorldElementTrail - For trails

  public void SetParticleData(Particle particleData)
  {
    base.VerifyInitialize();
    base.SetData(particleData);

    if (particleData.Charge < 0)
    {
      _orbCharge.SetActive(false);
      _orbNoCharge.SetActive(false);
      _orbNegCharge.SetActive(true);
    }
    else if (particleData.Charge > 0)
    {
      _orbNoCharge.SetActive(false);
      _orbNegCharge.SetActive(false);
      _orbCharge.SetActive(true);
    }
    else
    {
      _orbNegCharge.SetActive(false);
      _orbCharge.SetActive(false);
      _orbNoCharge.SetActive(true);
    }

    var pSize = particleData.Size;
    BodyTransform.localScale = new Vector3(pSize, pSize, pSize);

    ColorUtility.TryParseHtmlString(particleData.ColorHex, out Color particleColor);
    SetColor(particleColor);
  }
}