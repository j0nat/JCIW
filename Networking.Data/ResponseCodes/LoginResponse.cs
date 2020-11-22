
namespace Networking.Data.ResponseCodes
{
    /// <summary>
    /// These values are sent from the server and represent results of requests.
    /// </summary>
    public enum LoginResponse
    {
        Success = 0,
        WrongUsernamePassword = 1,
        NoAdminAccess
    };
}
