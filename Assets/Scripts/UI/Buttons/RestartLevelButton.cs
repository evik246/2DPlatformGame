using UnityEngine;

public class RestartLevelButton : Button
{
    public override void Do()
    {
        UIManager.RestartLevel();
    }
}
