using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management;
using System.Management.Automation;
using System.Text;
using log4net;
using log4net.Config;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace com.gncloud.hyperv.agent.Service
{

    
    public class PowerShellService
    {
        log4net.ILog log = log4net.LogManager.GetLogger(typeof(PowerShellService));

        public PowerShellService() {}

        public JObject powerShellScript(String script)
        {
            try
            {
                // Execute PowerShell Script for Result Value.
                PowerShell PowerShellInstance = PowerShell.Create();
                PowerShellInstance.AddScript(script);
                // invoke execution on the pipeline (collecting output)
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                log.Debug("실행된 스크립트: " + script);

                // check the other output streams (for example, the error stream)
                if (PowerShellInstance.Streams.Error.Count > 0)
                {
                    // error records were written to the error stream.
                    // do something with the items found.
                    JObject error = new JObject();
                    String logError = "";
                    foreach (System.Management.Automation.ErrorRecord record in PowerShellInstance.Streams.Error)
                    {
                        logError += record.ToString();
                    }
                    log.Error("에러 발생: " + logError);
                    error.Add("error", logError);
                    return error;
                }

                // Warning 메세지도 로그에 남긴다.
                if (PowerShellInstance.Streams.Warning.Count > 0)
                {
                    // error records were written to the error stream.
                    // do something with the items found.
                    String logWarning = "";
                    foreach (System.Management.Automation.WarningRecord record in PowerShellInstance.Streams.Warning)
                    {
                        logWarning += record.ToString();
                    }
                    log.Warn("에러 발생: " + logWarning);
                }

                JObject result = new JObject();
                if (PSOutput.Count == 0)
                {
                    result = new JObject();
                }
                else if (PSOutput.Count == 1)
                {
                    JArray resultJArray = new JArray();
                    try
                    {
                        result = JObject.Parse(PSOutput[0].ToString());
                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                        try
                        {
                            resultJArray = JArray.Parse(PSOutput[0].ToString());
                        }
                        catch (Newtonsoft.Json.JsonReaderException jarrerr)
                        {
                            result.Add("result", PSOutput[0].ToString());
                        }
                    }

                    if (resultJArray.Count > 0)
                    {
                        result.Add("resultArray", resultJArray);
                    }
                }
                else
                {
                    JArray returnArr = new JArray();
                    // loop through each output object item
                    foreach (PSObject outputItem in PSOutput)
                    {
                        // if null object was dumped to the pipeline during the script then a null
                        // object may be present here. check for null to prevent potential NRE.
                        if (outputItem != null)
                        {
                            //TODO: do something with the output item
                            // outputItem.BaseOBject
                            returnArr.Add(JToken.Parse(outputItem.BaseObject.ToString()));
                        }
                        result.Add("returnArray", returnArr);
                    }
                }

                return result;
            }
            catch (InvalidOperationException e)
            {
                log.Error("InvalidOperationException: ", e);
                JObject error = new JObject();
                error.Add("error", e.Message);
                return error;
            }
            catch (ScriptCallDepthException e)
            {
                log.Error("ScriptCallDepthException: ", e);
                JObject error = new JObject();
                error.Add("error", e.Message);
                return error;
            }
            catch (RuntimeException e)
            {
                log.Error("RuntimeException: ", e);
                JObject error = new JObject();
                error.Add("error", e.Message);
                return error;
            }
            catch (Exception e)
            {
                log.Error("RuntimeException: ", e);
                JObject error = new JObject();
                error.Add("error", e.Message);
                return error;
            }
        }
    }
}