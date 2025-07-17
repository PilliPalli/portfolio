using System.Text.Json.Serialization;

public class FunFactResponseService
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
}