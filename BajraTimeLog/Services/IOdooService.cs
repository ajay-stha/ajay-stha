using BajraTimeLog.Models;

namespace BajraTimeLog.Services;

public interface IOdooService
{
    bool IsLoggedIn { get; }

    /// <summary>Authenticate with email and password. Returns (success, errorMessage).</summary>
    Task<(bool Success, string? Error)> LoginAsync(string email, string password);

    /// <summary>Restores a previously persisted session. Returns true if valid session found.</summary>
    Task<bool> RestoreSessionAsync();

    /// <summary>Destroys the current session and clears stored credentials.</summary>
    Task LogoutAsync();

    /// <summary>Returns tasks visible to the logged-in user.</summary>
    Task<List<OdooTask>> GetMyTasksAsync();

    /// <summary>Creates a timesheet entry. Returns (success, errorMessage).</summary>
    Task<(bool Success, string? Error)> SubmitTimeLogAsync(TimeLogEntry entry);

    /// <summary>Returns the display name of the current user from secure storage.</summary>
    Task<string?> GetUserNameAsync();
}
