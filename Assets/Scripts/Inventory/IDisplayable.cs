using System.Collections.Generic;
using UnityEngine;

public interface IDisplayable
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Dictionary<string, string> Stats { get; set; }

    public Sprite Sprite { get; set; }
}
