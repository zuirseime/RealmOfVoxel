using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class IconManager
{
    public enum IconColor : byte
    {
        Gray, Blue, Teal, Green, Yellow, Orange, Red, Purple
    }

    public enum IconType : byte
    {
        Circle, Diamond
    }

    private static GUIContent[,] iconTextures;

    public static void SetIcon(GameObject gObj, IconColor color, IconType type)
    {
        if (iconTextures is null)
        {
            iconTextures = LoadTextures();
        }

        Texture2D texture = GetIconTexture(color, type);
        SetIcon(gObj, texture);
    }

    private static Texture2D GetIconTexture(IconColor color, IconType type)
    {
        return iconTextures[(int)color, (int)type].image as Texture2D;
    }

    private static GUIContent[,] LoadTextures()
    {
        byte colors = (byte)Enum.GetValues(typeof(IconColor)).Length;
        byte types = (byte)Enum.GetValues(typeof(IconType)).Length;

        GUIContent[,] textures = new GUIContent[colors, types];

        for (int colorIndex = 0; colorIndex < colors; colorIndex++)
        {
            for (int typeIndex = 0; typeIndex < 2; typeIndex++)
            {
                int textureIndex = colorIndex + (typeIndex * colors);
                textures[colorIndex, typeIndex] = LoadTexture("sv_icon_dot", textureIndex, "_pix16_gizmo");
            }
        }

        return textures;
    }

    private static GUIContent LoadTexture(string baseName, int index, string postfix)
    {
        return EditorGUIUtility.IconContent(baseName + index + postfix);
    }

    private static void SetIcon(GameObject gObj, Texture2D texture)
    {
        EditorGUIUtility.SetIconForObject(gObj, texture );
    }
}
