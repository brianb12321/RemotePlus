using System;

namespace RemotePlusLibrary.Security.AccountSystem
{
    /// <summary>
    /// Provides methods that retrieves account data.
    /// </summary>
    public interface IAccountManager
    {
        UserAccount GetLoggedInUser();
        UserAccount CreateAccount(UserCredentials cred);
        /// <summary>
        /// Gets an account object associated with the account's AID.
        /// </summary>
        /// <param name="AID">The account identification number used to identify unique accounts.</param>
        /// <returns>The UserAccount object that the AID points to. You must have the account be registered into the system.</returns>
        UserAccount GetAccount(Guid AID);
        /// <summary>
        /// Refreshes the currently loaded accounts on the system by enumerating the folder with the account objects.
        /// </summary>
        void RefreshAccountList();
        UserAccount AttemptLogin(UserCredentials cred);
    }
}