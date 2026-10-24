namespace DormAPI.DTOs.AI
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    // DTOs/OllamaResponseDto.cs

    public class OllamaResponseDto
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("message")]
        public OllamaMessageDto Message { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; }
    }

    public class OllamaMessageDto
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }


        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("tool_calls")]
        public List<OllamaToolCallDto>? ToolCalls { get; set; }
    }

    public class OllamaToolCallDto
    {
        [JsonPropertyName("function")]
        public OllamaFunctionCallDto Function { get; set; }
    }

    public class OllamaFunctionCallDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        // Using JsonElement allows us to capture the dynamic JSON object 
        // returned by the AI and convert it to a string later.
        [JsonPropertyName("arguments")]
        public JsonElement Arguments { get; set; }
    }
}
