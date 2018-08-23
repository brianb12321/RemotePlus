using System;
using RemotePlusLibrary.Security.AccountSystem;
using System.Collections.Generic;

namespace RemotePlusServer.UnitTests
{
    public class MockAuthentication : IAccountManager
    {
        UserCollection _fakeAccounts = new UserCollection();
        public UserAccount AttemptLogin(UserCredentials cred)
        {
            foreach (UserAccount Account in _fakeAccounts.Values)
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

        public UserAccount CreateAccount(UserCredentials cred, string role)
        {
            UserAccount account = new UserAccount(cred, role);
            _fakeAccounts.Add(account.AID, account);
            return account;
        }

        public UserAccount GetAccount(Guid AID)
        {
            try
            {
                return _fakeAccounts[AID];
            }
            catch (KeyNotFoundException)
            {
                throw new AccountException($"The account with guid {AID} does not exist.");
            }
        }

        public void RefreshAccountList()
        {
            _fakeAccounts.Clear();
            _fakeAccounts.Add(Guid.NewGuid(), new UserAccount(new UserCredentials("admin", "password"), "Admiin"));
            _fakeAccounts.Add(Guid.NewGuid(), new UserAccount(new UserCredentials("bob", "password"), "Admiin"));
        }
    }
}