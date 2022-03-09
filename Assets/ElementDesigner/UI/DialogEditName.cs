using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogEditName : MonoBehaviour
{
    InputField inputName;

    // Start is called before the first frame update
    void Start()
    {
        inputName = transform.Find("inputName").GetComponent<InputField>();
        Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            Accept();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        inputName.text = FileSystem.instance.ActiveElementAs<Atom>().Name;
        HUD.LockedFocus = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HUD.LockedFocus = false;
    }

    public void Accept()
    {
        var nameWithoutVowels = new string(inputName.text.Where(c => !("aeiou").Contains(c)).ToArray());
        var newShortName = (nameWithoutVowels[0].ToString() + nameWithoutVowels[1].ToString()).ToUpper();

        FileSystem.instance.ActiveElementAs<Atom>().Name = inputName.text;
        FileSystem.instance.ActiveElementAs<Atom>().ShortName = newShortName;

        Close();
    }
}
