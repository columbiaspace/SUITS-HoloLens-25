// using System;
// using System.Collections.Generic;
// using UnityEngine;

// [Serializable]
// public class RockAnalysisEntry
// {
//     public int id;
//     public string timestamp;
//     public Dictionary<string, float> chemicalData;

//     public RockAnalysisEntry(int id, Dictionary<string, float> chemicalData)
//     {
//         this.id = id;
//         this.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//         this.chemicalData = chemicalData;
//     }
// }


using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RockAnalysisEntry
{
    public int id;
    public string timestamp;
    public Dictionary<string, float> chemicalData;

    // New fields for astronaut notes
    public string color;
    public string size;
    public string texture;
    public string otherNotes;

    public RockAnalysisEntry(int id, Dictionary<string, float> chemicalData)
    {
        this.id = id;
        this.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.chemicalData = chemicalData;

        // Initialize notes as empty
        this.color = "";
        this.size = "";
        this.texture = "";
        this.otherNotes = "";
    }
}
