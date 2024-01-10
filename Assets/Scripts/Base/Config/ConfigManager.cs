using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class ConfigManager
{
    public static T GetConfigByType<T>(string configName = "") where T : ConfigBase
    {
        if (string.IsNullOrEmpty(configName))
        {
            configName = $"{typeof(T)}.json";
        }

        var path = Path.Combine(Application.streamingAssetsPath, "Configs", configName);
        var result = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        return result;
    }
}
