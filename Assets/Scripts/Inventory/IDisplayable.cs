using UnityEngine;
using UnityEngine.Rendering;

public interface IDisplayable
{
    public string Title { get; set; }
    public string Description { get; set; }
    public SerializedDictionary<string, string> Stats { get; set; }

    public Sprite Sprite { get; set; }
}
