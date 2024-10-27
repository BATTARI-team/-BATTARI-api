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
    public int id { get; set; }
    public bool isWelcome { get; set; }
    public List<SouguuIncredientDataModel> incredients { get; set; }
}

/// <summary>
/// Json デシリアライズ参考
/// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism?pivots=dotnet-8-0
/// </summary>
// [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
// [JsonDerivedType(typeof(SouguuAppIncredientModel), typeDiscriminator: "app")]
// [JsonDerivedType(typeof(SouguuIncredientDataModel), typeDiscriminator: "default")]
public class SouguuIncredientDataModel
{
    public string Type { get; set; }
}

public class SouguuIncredientModel
{
    public String name { get; set; }
    public SouguuIncredientDataModel data { get; set; }
}

// sealedつけたら，deserializeがうまくいった
public class SouguuAppIncredientModel : SouguuIncredientDataModel
{
    public SouguuIncredientDataAppUsageModel AppData { get; set; }
}

public class SouguuIncredientDataAppUsageModel
{
    public String appName { get; set; }
    public int useTime { get; set; }
}
