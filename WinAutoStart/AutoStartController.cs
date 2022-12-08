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
        public string Arguments { get { return _args; } set { _args = value; EnableAutoStart(); } }



        public AutoStartController()
        {
            _appName = Assembly.GetEntryAssembly().GetName().Name;
        }


        public AutoStartController(string args) : this() //call the base constructor
        {
            _args = args;
        }


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

        public object GetValue()
        {
            if (startupKey.GetValue(_appName) != null)
                return startupKey.GetValue(_appName);
            else
                return "";
        }


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

        public void ChangeAutoStartChecked()
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
