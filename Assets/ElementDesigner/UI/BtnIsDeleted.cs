using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BtnIsDeleted : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Element data;
    private Image deletedIdle, deletedUndo;
    private Button button;
    private Image buttonImage;
    void Start()
    {
        deletedIdle = transform.Find("DeletedIdle").GetComponent<Image>();
        Assertions.AssertNotNull(deletedIdle, "deletedIdle");
        deletedUndo = transform.Find("DeletedUndo").GetComponent<Image>();
        Assertions.AssertNotNull(deletedUndo, "deletedUndo");
        button = GetComponent<Button>();
        Assertions.AssertNotNull(button, "button");
        buttonImage = GetComponent<Image>();
        Assertions.AssertNotNull(buttonImage, "buttonImage");

        button.onClick.AddListener(HandleRecycleClicked);
        // SetActive(false);
    }
    public void OnPointerEnter(PointerEventData ev)
    {
        Debug.Log("POINTER ENTER");
        // if (data != null && data.IsDeleted)
        // {
        deletedIdle.gameObject.SetActive(false);
        deletedUndo.gameObject.SetActive(true);
        //}
    }

    public void OnPointerExit(PointerEventData ev)
    {
        // if (data != null && data.IsDeleted)
        setIdle();
    }

    void setIdle()
    {
        deletedIdle.gameObject.SetActive(true);
        deletedUndo.gameObject.SetActive(false);
    }

    public void HandleRecycleClicked()
    {
        if (data == null || !data.IsDeleted)
            return;

        FileSystem.RecycleRestoreElement(data, false);
        data = null;
    }
    public void SetData(Element element)
    {
        data = element;
    }

    public void SetActive(bool value)
    {
        button.enabled = value;
        buttonImage.enabled = value;

        if (value)
            setIdle();
        else
        {
            deletedIdle.gameObject.SetActive(false);
            deletedUndo.gameObject.SetActive(false);
        }
    }
}