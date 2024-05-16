using System.Collections.Generic;
using System.IO;
using Colossal.PSI.Environment;
using Game.Prefabs;
using Game.Rendering;
using Newtonsoft.Json;
using Unity.Entities;
using UnityEngine;

namespace CarColorChanger;

public record PrefabEntry
{
    public string Name;
    public DynamicBuffer<ColorVariation> ColorVariations;
}

public record Entry
{
    [JsonConverter(typeof(ColorHandler))] public Color color;
    public byte probability;
}

public record VariationPack
{
    public string Name;
    public Dictionary<string, List<Entry>> Entries;

    public VariationPack()
    {

    }

    public static VariationPack Default()
    {
        VariationPack pack = new VariationPack();
        pack.Name = "Default";
        pack.Entries = new Dictionary<string, List<Entry>>();
        pack.Entries["default"] = new List<Entry>
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

    public static List<string> GetVariationPackNames()
    {
        var modPath = Path.GetDirectoryName(Mod.path);
        var path = Path.Combine(modPath, "Resources");
        if (!Directory.Exists(path))
            return new List<string>();

        var files = Directory.GetFiles(path, "*.json");
        List<string> names = new List<string>();
        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            names.Add(name);
        }
        return names;
    }

    public static VariationPack Rdm()
    {
        VariationPack pack = new VariationPack();
        pack.Name = "Random";
        pack.Entries = new Dictionary<string, List<Entry>>();
        pack.Entries["default"] = new List<Entry>();

        for (int i = 0; i < 100; i++)
        {
            var r = Random.value;
            var g = Random.value;
            var b = Random.value;
            pack.Entries["default"].Add(new Entry()
            {
                color = new Color(r, g, b),
                probability = 1
            });
        }

        return pack;
    }

    public void FillColorVariations(string prefabName, ref DynamicBuffer<ColorVariation> buffer)
    {
        if (Entries == null || !Entries.ContainsKey(prefabName))
        {
            buffer.Clear();
            if (Entries.ContainsKey("default"))
            {
                foreach (Entry e in Entries["default"])
                {
                    var elem = new ColorVariation
                    {
                        m_ColorSet = new ColorSet(e.color),
                        m_Probability = e.probability
                    };
                    buffer.Add(elem);
                }
            }
            return;
        }
        buffer.Clear();
        foreach (Entry e in Entries[prefabName])
        {
            var elem = new ColorVariation
            {
                m_ColorSet = new ColorSet(e.color),
                m_Probability = e.probability
            };
            buffer.Add(elem);
        }
    }

    public static VariationPack Load(string name)
    {
        var modPath = Path.GetDirectoryName(Mod.path);
        var path = Path.Combine(modPath, "Resources", name + ".json");
        if (!File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<VariationPack>(json);
    }

    public void Save()
    {
        // Save data to json
        var modPath = Path.GetDirectoryName(Mod.path);
        var path = Path.Combine(modPath, "Resources", Name + ".json");
        if (!Directory.Exists(Path.GetDirectoryName(path)))
            Directory.CreateDirectory(Path.GetDirectoryName(path));

        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public void SavePrefabVariations(string prefabName, DynamicBuffer<ColorVariation> colorVariations)
    {
        if (Entries == null)
            Entries = new Dictionary<string, List<Entry>>();

        var entries = new List<Entry>();
        foreach (ColorVariation cv in colorVariations)
        {
            entries.Add(new Entry
            {
                color = cv.m_ColorSet[0],
                probability = cv.m_Probability
            });
        }

        Entries[prefabName] = entries;
    }
}