namespace Networking.Data.ResponseCodes
{
    /// <summary>
    /// These values are sent from the server and represent results of requests.
    /// </summary>
    public enum RegisterResponse
    {
        Success = 0,
        UsernameTaken = 1,
        Failure = 2
    };
}
