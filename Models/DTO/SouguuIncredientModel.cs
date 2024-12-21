using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace BATTARI_api.Models.DTO;

public class SouguuWebsocketDto
{
    /// <summary>
    /// ユーザーid
    /// </summary>
    public required int id { get; set; }
    public required bool isWelcome { get; set; }
    public required DateTime created { get; set; }
    public required List<SouguuIncredientDataModel> incredients { get; set; }
}

/// <summary>
/// Json デシリアライズ参考
/// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism?pivots=dotnet-8-0
/// </summary>
// [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
// [JsonDerivedType(typeof(SouguuAppIncredientModel), typeDiscriminator: "app")]
// [JsonDerivedType(typeof(SouguuIncredientDataModel), typeDiscriminator: "default")]
[JsonConverter(typeof(SouguuIncredientDataModelConverter))]
public class SouguuIncredientDataModel
{
    public string type { get; set; }
}

// sealedつけたら，deserializeがうまくいった
public class SouguuAppIncredientModel : SouguuIncredientDataModel
{
    public SouguuIncredientDataAppUsageModel appData { get; set; }
}

public class SouguuIncredientDataAppUsageModel
{
    public String appName { get; set; }
    public int useTime { get; set; }
}
public class SouguuIncredientDataModelConverter : JsonConverter<SouguuIncredientDataModel>
{
    public override SouguuIncredientDataModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            var type = root.GetProperty("type").GetString();

            if (type == "app")
            {
                return JsonSerializer.Deserialize<SouguuAppIncredientModel>(root.GetRawText(), options);
            }
            else
            {
                return JsonSerializer.Deserialize<SouguuIncredientDataModel>(root.GetRawText(), options);
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, SouguuIncredientDataModel value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }
}