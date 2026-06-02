namespace BajraTimeLog;

/// <summary>
/// Application-wide constants.
/// </summary>
public static class Constants
{
    /// <summary>Base URL of the Bajra Technologies Odoo instance.</summary>
    public const string BaseUrl = "https://bajratechnologies.com";

    // SecureStorage keys
    public const string SessionIdKey  = "odoo_session_id";
    public const string UserIdKey     = "odoo_user_id";
    public const string UserNameKey   = "odoo_user_name";
    public const string DatabaseKey   = "odoo_database";

    // Odoo model names
    public const string TaskModel      = "bajra_scrum.task";
    public const string TimesheetModel = "account.analytic.line";
}
