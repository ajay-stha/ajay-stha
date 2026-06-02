using System.Net;
using System.Text;
using System.Text.Json;
using BajraTimeLog.Models;

namespace BajraTimeLog.Services;

/// <summary>
/// Communicates with the Bajra Technologies Odoo backend via JSON-RPC.
/// Session is maintained via an HttpClientHandler CookieContainer and
/// persisted across app launches in SecureStorage.
/// </summary>
public class OdooService : IOdooService
{
    private readonly HttpClient      _http;
    private readonly CookieContainer _cookies;
    private string?  _database;
    private int      _userId;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OdooService()
    {
        _cookies = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = _cookies,
            UseCookies = true
        };
        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(Constants.BaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
        _http.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public bool IsLoggedIn => _userId > 0;

    // ── Login ──────────────────────────────────────────────────────────────

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    {
        _database = await ResolveDatabase();
        if (_database is null)
            return (false, "Unable to reach the server. Check your internet connection.");

        var req = new OdooJsonRpcRequest
        {
            Params = new AuthenticateParams
            {
                Db       = _database,
                Login    = email,
                Password = password
            }
        };

        try
        {
            using var resp = await PostJsonAsync("/web/session/authenticate", req);
            using var doc  = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var errEl))
            {
                var msg = errEl.TryGetProperty("data", out var d) &&
                          d.TryGetProperty("message", out var m)
                    ? m.GetString()
                    : "Authentication failed";
                return (false, msg ?? "Authentication failed");
            }

            if (!root.TryGetProperty("result", out var result) ||
                result.ValueKind is JsonValueKind.False or JsonValueKind.Null)
                return (false, "Invalid email or password.");

            _userId = result.TryGetProperty("uid", out var uid) ? uid.GetInt32() : 0;
            if (_userId <= 0)
                return (false, "Invalid email or password.");

            var name      = result.TryGetProperty("name",       out var n)   ? n.GetString()   : email;
            var sessionId = result.TryGetProperty("session_id", out var sid) ? sid.GetString() : null;

            // Restore cookie explicitly in case Set-Cookie was absent
            if (!string.IsNullOrEmpty(sessionId))
                _cookies.Add(new Uri(Constants.BaseUrl), new Cookie("session_id", sessionId));

            // Persist for future launches
            await SecureStorage.Default.SetAsync(Constants.SessionIdKey, sessionId ?? string.Empty);
            await SecureStorage.Default.SetAsync(Constants.UserIdKey,    _userId.ToString());
            await SecureStorage.Default.SetAsync(Constants.UserNameKey,  name ?? email);
            await SecureStorage.Default.SetAsync(Constants.DatabaseKey,  _database);

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Connection error: {ex.Message}");
        }
    }

    // ── Session restore ────────────────────────────────────────────────────

    public async Task<bool> RestoreSessionAsync()
    {
        try
        {
            var sessionId = await SecureStorage.Default.GetAsync(Constants.SessionIdKey);
            var userIdStr = await SecureStorage.Default.GetAsync(Constants.UserIdKey);
            var database  = await SecureStorage.Default.GetAsync(Constants.DatabaseKey);

            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(database))
                return false;

            _database = database;
            _userId   = int.Parse(userIdStr);
            _cookies.Add(new Uri(Constants.BaseUrl), new Cookie("session_id", sessionId));

            return await VerifySession();
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> VerifySession()
    {
        try
        {
            var req = new OdooJsonRpcRequest { Params = new { } };
            using var resp = await PostJsonAsync("/web/session/get_session_info", req);
            using var doc  = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            return root.TryGetProperty("result", out var result) &&
                   result.TryGetProperty("uid",    out var uid)   &&
                   uid.ValueKind != JsonValueKind.False            &&
                   uid.ValueKind != JsonValueKind.Null             &&
                   uid.GetInt32() > 0;
        }
        catch
        {
            return false;
        }
    }

    // ── Logout ─────────────────────────────────────────────────────────────

    public async Task LogoutAsync()
    {
        try
        {
            var req = new OdooJsonRpcRequest { Params = new { } };
            await PostJsonAsync("/web/session/destroy", req);
        }
        catch { /* ignore – clear local state regardless */ }
        finally
        {
            _userId   = 0;
            _database = null;
            SecureStorage.Default.Remove(Constants.SessionIdKey);
            SecureStorage.Default.Remove(Constants.UserIdKey);
            SecureStorage.Default.Remove(Constants.UserNameKey);
            SecureStorage.Default.Remove(Constants.DatabaseKey);
        }
    }

    // ── Tasks ──────────────────────────────────────────────────────────────

    public async Task<List<OdooTask>> GetMyTasksAsync()
    {
        var tasks = new List<OdooTask>();
        try
        {
            var req = new OdooJsonRpcRequest
            {
                Params = new
                {
                    model  = Constants.TaskModel,
                    method = "search_read",
                    args   = new object[]
                    {
                        // Domain: non-folded stages (active tasks)
                        new object[] { new object[] { "stage_id.fold", "=", false } }
                    },
                    kwargs = new
                    {
                        fields = new[] { "id", "name", "project_id" },
                        limit  = 200,
                        order  = "name asc"
                    }
                }
            };

            using var resp = await PostJsonAsync("/web/dataset/call_kw", req);
            using var doc  = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            if (!root.TryGetProperty("result", out var result) || result.ValueKind != JsonValueKind.Array)
                return tasks;

            foreach (var item in result.EnumerateArray())
            {
                var id   = item.TryGetProperty("id",   out var idEl)   ? idEl.GetInt32()   : 0;
                var name = item.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : null;
                if (id == 0 || name is null) continue;

                var task = new OdooTask { Id = id, Name = name };

                if (item.TryGetProperty("project_id", out var projEl) &&
                    projEl.ValueKind == JsonValueKind.Array)
                {
                    var arr = projEl.EnumerateArray().ToList();
                    if (arr.Count >= 2)
                    {
                        task.ProjectId   = arr[0].GetInt32();
                        task.ProjectName = arr[1].GetString() ?? string.Empty;
                    }
                }

                tasks.Add(task);
            }
        }
        catch { /* return whatever was collected */ }

        return tasks;
    }

    // ── Time log submission ────────────────────────────────────────────────

    public async Task<(bool Success, string? Error)> SubmitTimeLogAsync(TimeLogEntry entry)
    {
        try
        {
            var values = new Dictionary<string, object?>
            {
                ["date"]        = entry.Date,
                ["name"]        = entry.Description,
                ["unit_amount"] = entry.Hours
            };
            if (entry.TaskId.HasValue)    values["task_id"]    = entry.TaskId.Value;
            if (entry.ProjectId.HasValue) values["project_id"] = entry.ProjectId.Value;

            var req = new OdooJsonRpcRequest
            {
                Params = new
                {
                    model  = Constants.TimesheetModel,
                    method = "create",
                    args   = new object[] { values },
                    kwargs = new { }
                }
            };

            using var resp = await PostJsonAsync("/web/dataset/call_kw", req);
            using var doc  = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var errEl))
            {
                var msg = errEl.TryGetProperty("data", out var d) &&
                          d.TryGetProperty("message", out var m)
                    ? m.GetString()
                    : "Failed to submit time log";
                return (false, msg ?? "Failed to submit time log");
            }

            // result is the new record's integer ID
            if (root.TryGetProperty("result", out var result) &&
                result.ValueKind == JsonValueKind.Number)
                return (true, null);

            return (false, "Unexpected server response. Please try again.");
        }
        catch (Exception ex)
        {
            return (false, $"Connection error: {ex.Message}");
        }
    }

    // ── User name ──────────────────────────────────────────────────────────

    public async Task<string?> GetUserNameAsync() =>
        await SecureStorage.Default.GetAsync(Constants.UserNameKey);

    // ── Helpers ────────────────────────────────────────────────────────────

    private async Task<HttpResponseMessage> PostJsonAsync(string path, object body)
    {
        var json    = JsonSerializer.Serialize(body, JsonOpts);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp    = await _http.PostAsync(path, content);
        resp.EnsureSuccessStatusCode();
        return resp;
    }

    /// <summary>
    /// Resolves the Odoo database name.
    /// Priority: SecureStorage → /web/database/list → fallback constant.
    /// </summary>
    private async Task<string?> ResolveDatabase()
    {
        // 1. Previously stored
        var stored = await SecureStorage.Default.GetAsync(Constants.DatabaseKey);
        if (!string.IsNullOrEmpty(stored)) return stored;

        // 2. Ask the server
        try
        {
            var req = new OdooJsonRpcRequest { Params = new { } };
            using var resp = await PostJsonAsync("/web/database/list", req);
            using var doc  = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            if (root.TryGetProperty("result", out var res) && res.ValueKind == JsonValueKind.Array)
            {
                var dbs = res.EnumerateArray().ToList();
                if (dbs.Count == 1) return dbs[0].GetString();

                // Prefer a name that contains "bajra"
                foreach (var db in dbs)
                {
                    var n = db.GetString() ?? string.Empty;
                    if (n.Contains("bajra", StringComparison.OrdinalIgnoreCase))
                        return n;
                }
                if (dbs.Count > 0) return dbs[0].GetString();
            }
        }
        catch { /* fall through to default */ }

        // 3. Best-guess fallback
        return "bajra_technologies";
    }
}
