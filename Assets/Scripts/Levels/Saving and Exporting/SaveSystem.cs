using System;
using System.Collections.Generic;
using RhythmLevel;

public static class SaveSystem
{

    public static LevelExportData CreateExportData(LevelData createdLevel)
    {
        // TODO
        return null;
    }

    public static void CreateExport(LevelData data, out LevelExportData exporter)
    {
        exporter = null;
    }

    public static bool ExportLevel(string author, string songFilepath, List<LevelExportData> levelDifficulties)
    {
        // TODO
        // if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir); // Ensure directory exists
        // var json = JsonUtility.ToJson(data, true);        // Pretty print for readability
        // File.WriteAllText(PathFor(data.slotIndex), json);
        return false;
    }

    public static bool LoadLevel(string zippedLevelFilepath, string difficulty, out LevelMetadata metadata, out LevelExportData import)
    {
        // TODO
        // var slotPath = PathFor(slot);
        // if (!File.Exists(slotPath)) { data = null; return false; }   // Safe check prevents crashes
        // data = JsonUtility.FromJson<LevelExportData>(File.ReadAllText(slotPath));
        // return data != null;
        metadata = null;
        import = null;
        return false;
    }

    public static bool LoadMetadata(string zippedLevelFilepath, out LevelMetadata metadata)
    {
        // TODO
        metadata = null;
        return false;
    }
}

[Serializable]
public class LevelExportData
{
    public string difficulty;
    public float bpm;
    public int numLanes;
    public float noteRushTime; 
    public List<LevelEventExportData> events;
    public int presongTickAmount;
}

[Serializable]
public class LevelEventExportData
{
    public int lane;
    public double beatTime;
    public int eventType;
    public string optionalBeatId;
}

[Serializable]
public class LevelMetadata
{
    public string author;
    public string songFilepath;
    public long createdTime;
    public List<KeyedValue<string, string>> difficultyFilepaths;
}