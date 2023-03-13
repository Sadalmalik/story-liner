using UnityEngine;
using Newtonsoft.Json;

namespace Self.Articy
{
    public static class ArticyProjectImporter
    {
        public static ArticyData ImportFromJsonAsset(TextAsset textAsset)
        {
            return JsonConvert.DeserializeObject<ArticyData>(textAsset.text);
        }
    }
}