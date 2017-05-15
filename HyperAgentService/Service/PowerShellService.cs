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

namespace com.gncloud.hyperv.agent.Service
{
    public class PowerShellService
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(sService));

        public PowerShellService()
        {
        }

        public String powerShellScript(String script)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\system.log"))
            {
                try
                {
                    // Execute PowerShell Script for Result Value.
                    PowerShell PowerShellInstance = PowerShell.Create().AddScript(script);
                    PowerShellInstance.AddScript(script);
                    // invoke execution on the pipeline (collecting output)
                    Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

                    // check the other output streams (for example, the error stream)
                    if (PowerShellInstance.Streams.Error.Count > 0)
                    {
                        // error records were written to the error stream.
                        // do something with the items found.
                        String error = "{ \"error\": \"";
                        foreach (System.Management.Automation.ErrorRecord record in PowerShellInstance.Streams.Error)
                        {
                            error += record.ToString();
                        }
                        error += "\" }";
                        
                        //return error;
                    }

                    String result = "";
                    // loop through each output object item
                    foreach (PSObject outputItem in PSOutput)
                    {

                        // if null object was dumped to the pipeline during the script then a null
                        // object may be present here. check for null to prevent potential NRE.
                        if (outputItem != null)
                        {
                            //TODO: do something with the output item
                            // outputItem.BaseOBject
                            result += outputItem.BaseObject;
                        }
                    }

                    return result;
                }
                catch (InvalidOperationException e)
                {
                    file.WriteLine("InvalidOperationException: " + e.Message);
                    return "{ \"error\": \"[InvalidOperationException] " + e.Message + "\" }";
                }
                catch (ScriptCallDepthException e)
                {
                    file.WriteLine("ScriptCallDepthException: " + e.Message);
                    return "{ \"error\": \"[ScriptCallDepthException] " + e.Message + "\" }";
                }
                catch (RuntimeException e)
                {
                    file.WriteLine("RuntimeException: " + e.Message);
                    return "{ \"error\": \"[RuntimeException] " + e.Message + "\" }";
                }
                catch (Exception e)
                {
                    file.WriteLine("RuntimeException: " + e.Message);
                    return "{ \"error\": \"" + e.Message + "\" }";
                }
            }
        }
    }
}