using UnityEngine;

public class Settings
{
    private static Settings _instance;
    public static Settings Instance => _instance ??= new Settings();

    private Settings()
    {
        
    }

    public InputSettings Input { get; private set; } = new();
}

public class InputSettings
{
    public KeyCode Spell1 { get; set; } = KeyCode.Q;
    public KeyCode Spell2 { get; set; } = KeyCode.W;
    public KeyCode Spell3 { get; set; } = KeyCode.E;
    public KeyCode Spell4 { get; set; } = KeyCode.R;

    public KeyCode SwapWeapons { get; set; } = KeyCode.Tab;

    public KeyCode Interact { get; set; } = KeyCode.F;
}