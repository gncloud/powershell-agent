

namespace com.gncloud.hyperv.agent.Service
{
    using System;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Management;
    using System.Management.Automation;
    using System.Text;
    //using log4net;
    //using log4net.Config;
    using System.Text.RegularExpressions;

    public class PowerShellService
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(sService));

        public PowerShellService()
        {
        }

        public String powerShellScript(String script)
        {
            Collection<PSObject> result = new Collection<PSObject>();
            try
            {
                //log.Info("Result Script: " + script);
                // Execute PowerShell Script for Result Value.
                result = PowerShell.Create()
                        .AddScript(script)
                        .Invoke();
            }
            catch (InvalidOperationException e)
            {
                return "{ \"error\": [InvalidOperationException] \"" + e.ToString() + "\" }";
            }
            catch (ScriptCallDepthException e)
            {
                return "{ \"error\": [ScriptCallDepthException] \"" + e.ToString() + "\" }";
            }
            catch (RuntimeException e)
            {
                return "{ \"error\": [RuntimeException] \"" + e.ToString() + "\" }";
            }
            catch (Exception e)
            {
                return "{ \"error\": \"" + e.ToString() + "\" }";
            }

            if (result.Count == 0)
            {
                return result.ToString();
            }
            else
            {
                return result[0].ToString();
            }
        }
    }
}