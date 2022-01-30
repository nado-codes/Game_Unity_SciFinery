using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IsotopeGridItem : PeriodicTableGridItem
{
    Text nameText;
    Button button;

    void Start()
    {
        nameText = transform.Find("ShortName").GetComponent<Text>();
        button = GetComponent<Button>();

        SetActive(false);
    }
    void Awake()
    {
        nameText = transform.Find("ShortName").GetComponent<Text>();
        button = GetComponent<Button>();

        SetActive(false); 
    }

    public override void SetActive(bool active)
    {
        nameText.gameObject.SetActive(active);
        button.interactable = active;
    }

    public void SetName(string newName)
    {
        var nameWithoutVowels = new string(newName.Where(c => !("aeiou").Contains(c)).ToArray());
        var newShortName = (nameWithoutVowels[0].ToString() + nameWithoutVowels[1].ToString()).ToUpper();
        nameText.text = newShortName;

        SetActive((newName != null && newName != string.Empty));
    }
}