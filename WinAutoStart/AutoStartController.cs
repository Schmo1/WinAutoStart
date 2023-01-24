using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace WinAutoStart
{
    public class AutoStartController
    {

        // The path to the key where Windows looks for startup applications
        private readonly RegistryKey startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private readonly string _appName;
        private string _args;

        public bool AutoStartActiv { get => IsAutoStartActiv(); }


        /// <summary>
        /// Set some Arguments
        /// </summary>
        public string Arguments { get => _args; set => _args = value; }


        /// <summary>
        /// Creates some Instance
        /// </summary>
        public AutoStartController()
        {
            _appName = Assembly.GetEntryAssembly().GetName().Name;
        }


        /// <summary>
        /// Create some Instance
        /// </summary>
        public AutoStartController(string args) : this() //call the base constructor
        {
            _args = args;
        }

        /// <summary>
        /// Create some Instance
        /// </summary>
        public AutoStartController(string args, string appName)
        {
            _appName = appName;
            _args = args;
        }



        private bool IsAutoStartActiv()
        {
            object objValue;
            try
            {
                objValue = startupKey.GetValue(_appName);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            // Check to see the current state (running at startup or not)
            if (objValue == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                return false;
            }
            else
            {
                // The value exists, the application is set to run at startup                 
                return true;
            }
        }

        /// <summary>
        /// Returns the current value
        /// </summary>
        public object GetValue()
        {
            if (startupKey.GetValue(_appName) != null)
                return startupKey.GetValue(_appName);
            else
                return "";
        }


        /// <summary>
        /// Enables autostart. If it's exist, key will be updated.
        /// </summary>
        public void EnableAutoStart()
        {
            try
            {
                //Get Full Path
                string regValue = GetRegValue();
                string currValue = GetValue().ToString();

                if (!currValue.Equals(regValue) && _appName != null)
                {
                    //Update path
                    // Add the value in the registry so that the application runs at startup
                    startupKey?.SetValue(_appName, regValue);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Disables autostart if Key does exists
        /// </summary>
        public void DisableAutoStart()
        {
            
            try
            {
                // Remove the value from the registry so that the application doesn't start
                startupKey?.DeleteValue(_appName, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Switch the current state
        /// </summary>
        public void SwitchAutoStartChecked()
        {
            if (AutoStartActiv)
                DisableAutoStart();   // Remove the value from the registry so that the application doesn't start        
            else
                EnableAutoStart();

        }


        private string GetRegValue()
        {
            //Get Full Path
            string dirInfo = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            //add quotation marks and argument
            dirInfo = $"\"{dirInfo}\"" + Arguments;

            return dirInfo;
        }
    }
}
