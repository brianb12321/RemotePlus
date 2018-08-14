using System;
using System.Collections.Generic;
using System.IO;

namespace RemotePlusLibrary.Security.AccountSystem
{
    /// <summary>
    /// Manages all the accounts stored in the system. You can call methods to retrieve accounts, create accounts, etc.
    /// </summary>
    [Serializable]
    public class AccountManager : IAccountManager
    {
        UserCollection _accounts = new UserCollection();
        /// <summary>
        /// Creates a new account and saves its account object file.
        /// </summary>
        /// <param name="cred">The username and password for the account.</param>
        /// <param name="role">The string to a role that is loaded on the server.</param>
        public UserAccount CreateAccount(UserCredentials cred, string role, Configuration.IConfigurationDataAccess da)
        {
            UserAccount account = new UserAccount(cred, role);
            _accounts.Add(account.AID, account);
            da.SaveConfig(account, $"Users\\{account.AID}.uao");
            return account;
        }
        /// <summary>
        /// Gets an account object associated with the account's AID.
        /// </summary>
        /// <param name="AID">The account identification number used to identify unique accounts.</param>
        /// <returns>The UserAccount object that the AID points to. You must have the account be registered into the system.</returns>
        public UserAccount GetAccount(Guid AID)
        {
            try
            {
                return _accounts[AID];
            }
            catch (KeyNotFoundException)
            {
                throw new AccountException($"The account with guid {AID} does not exist.");
            }
        }
        /// <summary>
        /// Refreshes the currently loaded accounts on the system by enumerating the folder with the account objects.
        /// </summary>
        public void RefreshAccountList(Configuration.IConfigurationDataAccess da)
        {
            _accounts.Clear();
            foreach (string file in Directory.GetFiles("Users"))
            {
                if (Path.GetExtension(file) == ".uao")
                {
                    UserAccount account = da.LoadConfig<UserAccount>(file);
                    _accounts.Add(account.AID, account);
                }
            }
        }
        public UserAccount AttemptLogin(UserCredentials cred)
        {
            foreach (UserAccount Account in _accounts.Values)
            {
                if (Account.Verify(cred))
                {
                    return Account;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
    }
}