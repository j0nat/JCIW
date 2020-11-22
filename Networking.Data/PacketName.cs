namespace Networking.Data
{
    // Request = expecting response
    // Post = not expecting response
    // Re = return / reply 

    /// <summary>
    /// The names of all the packets used by the project.
    /// </summary>
    public enum PacketName
    {
        RequestLogin,
        RequestRegisterAccount,
        ReRegisterAccount,
        ReLoginResult,
        ReSessionId,
        RequestSessionVerification,
        ReSessionVerificationResult,
        RequestModuleList,
        ReModuleList,
        PostEnableService,
        PostDisableService,
        PostDeleteService,
        RequestServiceCommand,
        ReServiceResponse,
        RequestServiceLog,
        ReServiceLog,
        RequestAccountList,
        RequestDeleteAccount,
        ReDeleteAccount,
        ReAccountList,
        RequestUserGroupList,
        RequestGroupList,
        ReUserGroupList,
        RequestUpdatePassword,
        RequestUpdateAccountInformation,
        RequestDeleteGroupFromUser,
        RequestAddGroupToUser,
        ReAddGroupToUser,
        ReUpdatePassword,
        ReUpdateAccountInformation,
        ReDeleteGroupFromUser,
        RequestDeleteGroup,
        RequestAddGroup,
        ReDeleteGroup,
        ReAddGroup,
        RequestUserAppList,
        RequestAllAppList,
        ReAllAppList,
        RequestAppFile,
        ReAppFile,
        RequestAppGroupList,
        ReAppGroupList,
        PostAddGroupToApp,
        PostRemoveGroupFromApp,
        PostEnableApp,
        PostDisableApp,
        ReUnauthorized
    };
}
