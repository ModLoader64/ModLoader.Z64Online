using System.Text.Json.Serialization;

namespace ModLoader.Z64Online;

[Configuration]
public class Configuration
{
    public int version { get; set; } = 0;
    public string test { get; set; } = "this is a test";

    [JsonIgnore]
    string ignore = "don't save me";
}
