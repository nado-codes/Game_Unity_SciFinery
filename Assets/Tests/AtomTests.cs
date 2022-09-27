
using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class AtomTests
{

    [SetUp]
    public void Init()
    {
        var camera = new GameObject();
        camera.AddComponent<Camera>();
        camera.tag = "MainCamera";
        var root = SceneManager.GetActiveScene().GetRootGameObjects();
    }
    // A Test behaves as an ordinary method
    [UnityTest]
    public IEnumerator ClassificationTest()
    {
        // Use the Assertions.Assert class to test conditions

        var camera = Camera.main;

        var electron = FileSystemLoader.LoadElementsOfType<Particle>().First(p => p.Charge < 0);

        var nonMetalAtom = new Atom()
        {
            ChildIds = new int[] { electron.Id, electron.Id }
        };

        Assert.AreEqual(Classification.NonMetal, nonMetalAtom.GetClassification());

        var metalElectrons = Enumerable.Range(0, 10).Select(n => electron.Id).ToArray();
        var metalAtom = new Atom()
        {
            ChildIds = metalElectrons
        };

        Assert.AreEqual(Classification.Metal, metalAtom.GetClassification());

        yield return null;
    }
}
