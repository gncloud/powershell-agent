/**/

namespace com.gncloud.hyperv.agent.Controllers
{
    using System;
    using System.Web.Http;
    using System.Net;
    using System.Net.Http;
    using log4net;
    using Newtonsoft.Json.Linq;
    using com.gncloud.hyperv.agent.Service;

    public class PowerShellController : ApiController
    {
        ILog log = log4net.LogManager.GetLogger(typeof(PowerShellController));

        [HttpGet]
        [Route("service/check")]
        public HttpResponseMessage CheckService()
        {
            String returnStr = "{\"message\":\"Service Check OK.\"}";
            JObject returnJobj = JObject.Parse(returnStr);
            var response = Request.CreateResponse(HttpStatusCode.OK, returnJobj);
            
            response.Headers.Add("Content", "Application/json");
            log.Debug("service/check 호출");
            return response;
        }
        
        [HttpPost]
        [Route("powershell/execute")]
        public HttpResponseMessage GetExecuteScript(String script = "")
        {
            PowerShellService ctrl = new PowerShellService();
            
            // 스크립트 실행 및 리턴값 받아오기
            JObject returnJObj = ctrl.powerShellScript(script);

            var response = Request.CreateResponse(HttpStatusCode.OK, returnJObj);
            response.Headers.Add("Content", "Application/json");

            log.Debug("powershell/execute 호출");
            return response;
        }
    }
}