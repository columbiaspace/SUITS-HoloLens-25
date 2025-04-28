using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RockAnalysisEntry
{
    public int id;
    public string timestamp;
    public Dictionary<string, float> chemicalData;

    public RockAnalysisEntry(int id, Dictionary<string, float> chemicalData)
    {
        this.id = id;
        this.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.chemicalData = chemicalData;
    }
}
