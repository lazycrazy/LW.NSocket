using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using LW.NGSSocketServer;

namespace NGSSocketService
{
    public partial class NGSSocketService : ServiceBase
    {
        public NGSSocketService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LW.NSocket.SocketBase.Log.Trace.EnableDiagnostic();
            NGSServ.Start();
        }

        protected override void OnStop()
        {
            NGSServ.Stop();
        }
    }
}
