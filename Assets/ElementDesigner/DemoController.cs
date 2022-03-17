using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoController : MonoBehaviour
{
    public Editor editor;

    public int timerLimit = 250;
    private int timer = 0;
    private bool isDemo = false;
    private List<Atom> prevLoaded = new List<Atom>();
    private Text timerText;
    private Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        toggle = GameObject.Find("toggleDemo").GetComponent<Toggle>();
        isDemo = toggle.isOn;

        timerText = GameObject.Find("textDemoTimer").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDemo)
        {
            if (timer > 0 && timer % timerLimit == 0)
                loadRandomAtom();

            timer++;
        }

        var timerString = Math.Round((decimal)(timer % timerLimit) / timerLimit * 100, 0).ToString();
        timerText.text = timerString + "%";
    }

    public void HandleToggleDemo()
    {
        if (!toggle.isOn)
            timer = 0;

        isDemo = toggle.isOn;
    }
    private void loadRandomAtom()
    {
        var atomCount = FileSystem.instance.LoadedAtoms.Count;

        var allAtomsNotPreviouslyLoaded = FileSystem.instance.LoadedAtoms.Where(atom => !prevLoaded.Any(prevAtom => prevAtom.Weight == atom.Weight));
        var randomAtomIndex = UnityEngine.Random.Range(0, allAtomsNotPreviouslyLoaded.Count());
        var randomAtom = allAtomsNotPreviouslyLoaded.ElementAt(randomAtomIndex);
        editor.LoadElementData(randomAtom);

        if (prevLoaded.Count >= atomCount / 2)
            prevLoaded.RemoveAt(0);

        timerLimit = 240 * Editor.Particles.Count;
        timer = 0;

        prevLoaded.Add(randomAtom);
    }
}
