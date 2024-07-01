using UnityEngine;

public class PauseButton : Button
{
    public override void Do()
    {
        UIManager.Pause();
    }
}
