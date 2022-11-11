using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace VaccineNotifier
{
    internal class WhatsAppManager
    {
        private const string AccountSid = "AC65b81ca0600adf8fa058f70fb1857f06";
        private const string AuthToken = "343f5b24dfc52511b2b52e9ad0600cf3";
        private const string FromNumber = "whatsapp:+14155238886";
        public static void SendMessage(string toNumber, string message)
        {
            TwilioClient.Init(AccountSid, AuthToken);
            MessageResource.Create(from: new Twilio.Types.PhoneNumber(FromNumber), body: message, to: new Twilio.Types.PhoneNumber("whatsapp:" + toNumber));
        }
    }
}
