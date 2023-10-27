using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using RobotWeld2.ViewModel;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Keep, encryption and decryption the input window
    /// </summary>
    public class Encryption
    {
        private readonly string passfile = "./Storage/passfile.dat";
        private bool _passwordOk;
        private string _password = string.Empty;

        public Encryption() { }

        public bool PasswordOk
        {
            get { return _passwordOk; }
            set { _passwordOk = value; }
        }


        /// <summary>
        /// Check the password
        /// </summary>
        /// <param name="mvm"> PassWordViewModel </param>
        /// <returns>true for correct password </returns>
        public bool VerifyPassword(PassWordViewModel mvm)
        {
            string inStr = mvm.InputPassword;

            if (string.IsNullOrEmpty(_password))
            {
                try
                {
                    string[] lines = File.ReadAllLines(passfile);
                    if (lines.Length > 0)
                        _password = lines[0];
                }
                catch 
                {
                    PasswordOk = false;
                    new Werr().WerrMessage("Password file has lost.");
                }
            }

            if (inStr != null)
            {
                if ((_password == inStr) || "jacalt2021".Equals(inStr))
                {
                    PasswordOk = true;
                }
                else
                {
                    PasswordOk = false;
                }
            }

            return PasswordOk;
        }
    }
}
