using NVMP.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NVMP.Authenticator.Basic
{
    internal class BasicAuthenticatorImpl : IBasicAuthenticator
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly string AuthenticatorName = "basic";

        private ICollection<string> BannedIPs;
        private string FileName = "bannedips.txt";

        static ulong CalculateHash(string read)
        {
            ulong hashedValue = 3074457345618258791ul;
            for (int i = 0; i < read.Length; i++)
            {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }

        /// <summary>
        /// </summary>
        public BasicAuthenticatorImpl()
        {
            NativeSettings.SetStringValue("Server", "Authenticator", AuthenticatorName);

            BannedIPs = new List<string>();
            ReadBannedIPsFile();
        }

        /// <summary>
        /// Implements any additional logging that needs to be fed to your server administrators
        /// </summary>
        /// <param name="message"></param>
        virtual public void Log(string message)
        {
            Debugging.Write(message);
        }

        virtual public string GetAuthenticationURL()
        {
            return null;
        }

        virtual public string GetClientID()
        {
            return null;
        }

        virtual public void Update()
        {
        }

        virtual public void PlayerLeft(NetPlayer player)
        {
        }

        virtual public void SetupAuthentication(NetPlayer player)
        {
            player.Name = "Lonesome Courier";
            player.Authenticated = true;
            player["UniqueID"] = CalculateHash(player.AuthenticationToken).ToString();
        }

        virtual public bool IsScopeValid(NetPlayer player, string scope)
        {
            return true;
        }

        virtual public bool IsAuthenticationValid(NetPlayer player, string authenticationToken, ref string badauthReason)
        {
            if (BannedIPs.Contains(player.IP))
            {
                badauthReason = "IP is banned";
                return false;
            }

            return true;
        }

        protected void ReadBannedIPsFile()
        {
            if (!File.Exists(FileName))
            {
                File.Create(FileName).Dispose();
            }

            using (StreamReader sr = new StreamReader(FileName))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    BannedIPs.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
        }

        protected void WriteBannedIPsFile()
        {
            if (!File.Exists(FileName))
            {
                File.Create(FileName).Dispose();
            }

            using (StreamWriter sr = new StreamWriter(FileName))
            {
                foreach (string ip in BannedIPs)
                {
                    sr.WriteLine(ip);
                }
                sr.Close();
            }
        }

        virtual public void Ban(NetPlayer player, string reason = null)
        {
            if (!BannedIPs.Contains(player.IP))
            {
                BannedIPs.Add(player.IP);
                WriteBannedIPsFile();
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
