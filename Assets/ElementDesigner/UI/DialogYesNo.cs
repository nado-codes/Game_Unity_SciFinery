using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

public delegate T tFN<T>();

public class DialogYesNo : Dialog
{
    public enum YesNo { Yes = 0, No = 1 };

    private static VoidFN _fnYes, _fnNo, _fnOnClose;
    public static void Open(string title, string body, VoidFN fnYes, VoidFN fnNo = null, VoidFN fnOnClose = null)
    {
        _fnYes = fnYes;
        _fnNo = fnNo;
        _fnOnClose = fnOnClose;

        instance.Open(title, body);
    }

    private static YesNo? result = null;

    public static void OpenForResult(string title, string body)
    {
        _fnYes = () => result = YesNo.Yes;
        _fnNo = () => result = YesNo.No;

        instance.Open(title, body);
    }

    public static async Task<YesNo> WaitResult()
    {
        await Task.Run(() => { while (result == null) { } });
        var finalResult = result;
        result = null;
        return (YesNo)finalResult;
    }
    public void HandleYesButtonClicked()
    {
        _fnYes?.Invoke();
        Close();
    }

    public void HandleNoButtonClicked()
    {
        _fnNo?.Invoke();
        Close();
    }

    public override void Close()
    {
        _fnOnClose?.Invoke();
        base.Close();
    }
}
public class DialogYesNo<T> : Dialog
{
    private static tFN<T> _fnYes, _fnNo, _fnOnClose;

    public static void Open(string title, string body, tFN<T> fnYes, tFN<T> fnNo = null, tFN<T> fnOnClose = null)
    {
        _fnYes = fnYes;
        _fnNo = fnNo;
        _fnOnClose = fnOnClose;

        instance.Open(title, body);
    }
    public void HandleYesButtonClicked()
    {
        _fnYes?.Invoke();
        Close();
    }

    public void HandleNoButtonClicked()
    {
        _fnNo?.Invoke();
        Close();
    }

    public override void Close()
    {
        _fnOnClose?.Invoke();
        base.Close();
    }
}
