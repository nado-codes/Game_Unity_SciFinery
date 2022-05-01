using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogEditName : MonoBehaviour
{
    InputField inputName;
    Text title;

    // Start is called before the first frame update
    void Start()
    {
        inputName = transform.Find("inputName").GetComponent<InputField>();

        var panelHeader = transform.Find("panelHeader");
        Assertions.AssertNotNull(panelHeader, "panelHeader");
        title = panelHeader.transform.Find("Title")?.GetComponent<Text>();
        Assertions.AssertNotNull(title, "title");

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
        title.text = "Name your " + FileSystem.ActiveElement.ElementType.ToString();
        inputName.text = FileSystem.ActiveElement.Name;
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

        FileSystem.ActiveElement.Name = inputName.text;
        FileSystem.UpdateActiveElement();
        PanelName.SetElementData(FileSystem.ActiveElement);

        Close();
    }
}
