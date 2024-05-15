using System.Collections.Generic;
using Game.Prefabs;
using Game.Rendering;
using Unity.Entities;
using UnityEngine;

namespace CarColorChanger;

public record Entry
{
    public Color color;
    public byte probability;
}

public class VariationPack
{
    private string _name;
    private List<Entry> _entries;
    public VariationPack[] VariationPacks = new VariationPack[]
    {

    };

    public VariationPack()
    {

    }

    public static VariationPack Default()
    {
        VariationPack pack = new VariationPack();
        pack._name = "Default";
        pack._entries = new List<Entry>
        {
            new()
            {
                color = Color.white,
                probability = 26,
            },
            new()
            {
                color = Color.black,
                probability = 22,
            },
            new()
            {
                color = Color.gray,
                probability = 25,
            },
            new()
            {
                color = Color.blue,
                probability = 10,
            },
            new()
            {
                color = Color.red,
                probability = 9,
            },
            new()
            {
                color = Color.red,
                probability = 1,
            },
            new()
            {
                color = Color.green,
                probability = 1,
            },
            new()
            {
                color = new Color(255, 165, 0),
                probability = 1,
            },

        };
        return pack;
    }

    public static VariationPack Rdm()
    {
        VariationPack pack = new VariationPack();
        pack._name = "Random";
        for (int i = 0; i < 100; i++)
        {
            var r = Random.value;
            var g = Random.value;
            var b = Random.value;
            pack._entries.Add(new Entry()
            {
                color = new Color(r, g, b),
                probability = 1
            });
        }
        return pack;
    }

    public void FillColorVariations(ref DynamicBuffer<ColorVariation> buffer)
    {
        buffer.Clear();
        foreach (Entry e in _entries)
        {
            var elem = new ColorVariation
            {
                m_ColorSet = new ColorSet(e.color),
                m_Probability = e.probability
            };
            buffer.Add(elem);
        }
    }

    public void Save()
    {

    }

    public static void SaveDefault(DynamicBuffer<ColorVariation> colorVariations)
    {
        VariationPack pack = new VariationPack();
        foreach (ColorVariation cv in colorVariations)
        {
            pack._entries.Add(new Entry
            {
                color = cv.m_ColorSet[0],
                probability = cv.m_Probability
            });
        }
        pack.Save();
    }
}