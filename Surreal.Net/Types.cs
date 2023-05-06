using Newtonsoft.Json;

public sealed record QueryResult<T>
(
    [JsonProperty("time")] string Time,
    [JsonProperty("status")] string Status,
    [JsonProperty("result")] List<T> Result,
    [JsonProperty("error")] ResultError? Error = null
) where T : class, new();

public sealed record ResultError(
    [JsonProperty("code")] int Code, 
    [JsonProperty("message")] string Message);

public class JsonPatch
{
    [JsonProperty("op")] 
    public string Op { get; set; } = "";
    [JsonProperty("from")] 
    public string? From { get; set; } = null;
    [JsonProperty("path")] 
    public string Path { get; set; } = "";
    [JsonProperty("value")] 
    public object? Value { get; set; } = null;
}

public sealed class RpcRequest
{
    public RpcRequest(string id, string method, params object[] parameters)
    {
        Id = id;
        Method = method;
        Parameters = parameters;
    }

    [JsonProperty("id")] 
    public string Id {get;}
    [JsonProperty("method")] 
    public string Method {get;}
    [JsonProperty("params")] 
    public object[] Parameters {get;}
}

public sealed record SuperUserAuth(
    string user,
    string pass
);