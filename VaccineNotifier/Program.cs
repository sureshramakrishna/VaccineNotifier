using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Topshelf;

namespace VaccineNotifier
{
    class Program
    {
        private static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<VaccineNotifierService>(s =>
                {
                    s.ConstructUsing(name => new VaccineNotifierService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.SetServiceName("VaccineNotifierService");
                x.SetDisplayName("Vaccine Notifier");
                x.RunAsLocalSystem(); 
                x.StartAutomatically();
            });
        }
    }
}
