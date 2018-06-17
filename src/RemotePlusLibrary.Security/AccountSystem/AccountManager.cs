using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusLibrary.Security.AccountSystem
{
    /// <summary>
    /// Manages all the accounts stored in the system. You can call methods to retrieve accounts, create accounts, etc.
    /// </summary>
    [Serializable]
    public static class AccountManager
    {
        static UserCollection _accounts = new UserCollection();
        /// <summary>
        /// Creates a new account and saves its account object file.
        /// </summary>
        /// <param name="cred">The username and password for the account.</param>
        /// <param name="role">The string to a role that is loaded on the server.</param>
        public static void CreateAccount(UserCredentials cred, string role)
        {
            UserAccount account = new UserAccount(cred, role);
            _accounts.Add(account.AID, account);
            account.Save();
        }
        /// <summary>
        /// Gets an account object associated with the account's AID.
        /// </summary>
        /// <param name="AID">The account identification number used to identify unique accounts.</param>
        /// <returns>The UserAccount object that the AID points to. You must have the account be registered into the system.</returns>
        public static UserAccount GetAccount(Guid AID)
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
        public static void RefreshAccountList()
        {
            _accounts.Clear();
            foreach (string file in Directory.GetFiles("Users"))
            {
                if (Path.GetExtension(file) == ".uao")
                {
                    UserAccount account = UserAccount.Load(file);
                    _accounts.Add(account.AID, account);
                }
            }
        }
        public static UserAccount AttemptLogin(UserCredentials cred)
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
