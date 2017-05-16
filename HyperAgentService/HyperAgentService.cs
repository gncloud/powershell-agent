using System;
using System.ServiceProcess;
using System.Web.Http;
using System.Web.Http.SelfHost;
using log4net;

namespace HyperAgentService
{
    public partial class HyperAgentService : ServiceBase
    {
        HttpSelfHostServer server;
        ILog log = log4net.LogManager.GetLogger(typeof(HyperAgentService));

        public HyperAgentService()
        {
            InitializeComponent();

            this.CanStop = true;
            this.AutoLog = true;
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var config = new HttpSelfHostConfiguration("http://127.0.0.1:" + Properties.Settings.Default.ProcessPort);
                config.MapHttpAttributeRoutes();
                /*
                config.Routes.MapHttpRoute("API Default", "api/{controller}/{id}",
                    new { id = RouteParameter.Optional }
                );
                */
                log4net.Config.XmlConfigurator.Configure();
                log.Info("-------- Gncloud Hyper-V Agent Service Start --------");
                log.Info("접근 포트 설정: " + Properties.Settings.Default.ProcessPort);

                server = new HttpSelfHostServer(config);
                server.OpenAsync().Wait();
            }
            catch (Exception ex)
            {
                log.Error("Exception: ", ex);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
