using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldManager : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color common;

    [ColorUsage(true, true)]
    public Color uncommon;

    [ColorUsage(true, true)]
    public Color rare;

    [ColorUsage(true, true)]
    public Color epic;

    [ColorUsage(true, true)]
    public Color legendary;

    public static List<Color> rarity = new List<Color>();

    void Start()
    {
        rarity.Add(common);
        rarity.Add(uncommon);
        rarity.Add(rare);
        rarity.Add(epic);
        rarity.Add(legendary);
    }

    void Update()
    {
        
    }
}
