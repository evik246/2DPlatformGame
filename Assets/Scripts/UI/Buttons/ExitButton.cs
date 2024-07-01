using UnityEngine;

public class ExitButton : Button
{
    public override void Do()
    {
        UIManager.Quit();
    }
}
