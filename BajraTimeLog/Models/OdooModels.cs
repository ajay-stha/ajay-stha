using System.Text.Json.Serialization;

namespace BajraTimeLog.Models;

// ── JSON-RPC envelope ──────────────────────────────────────────────────────

public class OdooJsonRpcRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("method")]
    public string Method { get; set; } = "call";

    [JsonPropertyName("id")]
    public int Id { get; set; } = 1;

    [JsonPropertyName("params")]
    public object Params { get; set; } = new { };
}

// ── Authentication ─────────────────────────────────────────────────────────

public class AuthenticateParams
{
    [JsonPropertyName("db")]
    public string Db { get; set; } = string.Empty;

    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

// ── Domain entities ────────────────────────────────────────────────────────

public class OdooTask
{
    public int    Id          { get; set; }
    public string Name        { get; set; } = string.Empty;
    public int?   ProjectId   { get; set; }
    public string ProjectName { get; set; } = string.Empty;

    public override string ToString() => Name;
}

public class TimeLogEntry
{
    public string Date        { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    public string Description { get; set; } = string.Empty;
    public double Hours       { get; set; }
    public int?   TaskId      { get; set; }
    public int?   ProjectId   { get; set; }
}
