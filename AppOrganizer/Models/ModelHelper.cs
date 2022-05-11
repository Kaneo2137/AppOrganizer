using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using AppOrganizer.Models;

public static class ModelHelper
{
    const string JsonPath = "profiles.json";
    public static void SaveModel(this IEnumerable<ProfileModel> model)
    {
        File.WriteAllText(JsonPath, JsonSerializer.Serialize(model));
    }

    public static IEnumerable<ProfileModel> LoadModel()
    {
#pragma warning disable CS8603 // Possible null reference return.
        if (!File.Exists(JsonPath))
            return new List<ProfileModel>();
        return JsonSerializer.Deserialize<IEnumerable<ProfileModel>>(File.ReadAllText(JsonPath));
    }
}
