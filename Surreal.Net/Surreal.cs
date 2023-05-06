using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Surreal.Net;

public class Surreal
{
    private int _id = 0;
    private readonly string _url = "";
    private string _ns = "";
    private string _db = "";

    private SocketsHttpHandler _handler = new();
    private ClientWebSocket _ws = new();



    // Define the cancellation token.
    private CancellationTokenSource _source = new CancellationTokenSource();
    private CancellationToken _token;


    public Surreal(string url)
    {
        _url = url;

        _token = _source.Token;

        _ws.ConnectAsync(new Uri(this._url), new HttpMessageInvoker(_handler), _token).Wait();

    }

    public void SignIn(string user, string pass)
    {
        var result = Send<object>("signin", new SuperUserAuth(user, pass));
    }

    public void Use(string ns, string db)
    {
        var result = Send<object>("use", ns, db);
    }

    public dynamic Ping()
    {
        var result = this.Send<object>("ping");
        return result;
    }

    public dynamic Info(string ns, string db)
    {
        var result = Send<object>("info");
        return result;
    }

    public QueryResult<T> Create<T>(string thing, T data) where T : class, new()
    {
        var result = Send<T>("create", thing, data);
        return result;
    }


    /// <summary>
    /// Updates all records in a table, or a specific record, in the database.
    /// apply UPDATE $thing CONTENT $data;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public QueryResult<T> Update<T>(string thing, T data) where T : class, new()
    {
        return new QueryResult<T>("", "", new List<T>());
    }

    /// <summary>
    /// Modifies all records in a table, or a specific record, in the database.
    /// apply UPDATE $thing MERGE $data;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public QueryResult<T> Change<T>(string thing, object data) where T : class, new()
    {
        return new QueryResult<T>("", "", new List<T>());
    }

    /// <summary>
    /// Applies JSON Patch changes to all records, or a specific record, in the database.
    /// apply UPDATE $thing PATCH $data;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public QueryResult<T> Modify<T>(string thing, List<JsonPatch> data) where T : class, new()
    {
        return new QueryResult<T>("", "", new List<T>());



    }


    public QueryResult<T> Select<T>(string thing) where T : class, new()
    {
        return Send<T>("select", thing);
    }


    public QueryResult<T> Query<T>(string query, object vars) where T : class, new()
    {
        return Send<T>("query", query, vars);
    }


    private QueryResult<T> Send<T>(string method, params object[] parameters) where T : class, new()
    {
        var jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
        
        var dataToSend = new RpcRequest(
            id: (_id++).ToString(),
            method: method,
            parameters: parameters);

        string strToSend = JsonConvert.SerializeObject(dataToSend, jsonSettings);

        byte[] bytesToSend = Encoding.UTF8.GetBytes(strToSend);

        Console.WriteLine(strToSend);

        _ws.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None).Wait();

        byte[] bytesToReceive = new byte[1024];

        WebSocketReceiveResult receiveResult = _ws.ReceiveAsync(new ArraySegment<byte>(bytesToReceive), CancellationToken.None).Result;

        string jsonResponse = Encoding.UTF8.GetString(bytesToReceive, 0, receiveResult.Count);

        QueryResult<T> result = JsonConvert.DeserializeObject<QueryResult<T>>(jsonResponse, jsonSettings) ?? new QueryResult<T>("", "", new List<T>());

        return result;
    }


}
