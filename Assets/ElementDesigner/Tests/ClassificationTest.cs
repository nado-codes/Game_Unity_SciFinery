using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PanelNameTests
{

    [SetUp]
    public void Init()
    {
        Debug.Log("RAN SETUP");
    }
    // A Test behaves as an ordinary method
    [Test]
    public void ClassificationTest()
    {
        // Use the Assertions.Assert class to test conditions
        // var panelName = new Panel();

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MyTestWithEnumeratorPasses()
    {
        // Use the Assertions.Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
