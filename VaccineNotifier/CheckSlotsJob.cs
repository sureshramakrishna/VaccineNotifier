using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using VaccineNotifier.Models;

namespace VaccineNotifier
{
    internal class CheckSlotsJob : IJob
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(CheckSlotsJob));
        public Task Execute(IJobExecutionContext _)
        {
            return CheckIfAnySlotsAreAvailable();
        }
        public Task CheckIfAnySlotsAreAvailable()
        {
            return Task.Factory.StartNew(() =>
            {
                Log.Debug("Searching for free slots");
                var presentData = CowinApi.Api.GetSchedule(DateTime.Now);
                if (presentData.Any())
                {
                    CheckFreeSotsInternal(presentData);
                    return;
                }
                var nextWeekData = CowinApi.Api.GetSchedule(DateTime.Today.AddDays(7));
                if (nextWeekData.Any())
                {
                    CheckFreeSotsInternal(nextWeekData);
                    return;
                }
                Log.Debug("No free slots available, re-trying in 30 seconds...");
            });
        }

        public void CheckFreeSotsInternal(List<Center> centers)
        {
            if (centers.Any())
            {
                Log.Debug("Found free slots, find details below:  ");
                var sb = new StringBuilder();
                foreach (var center in centers)
                {
                    if (sb.Length > 1500)
                        break;
                    sb.AppendLine($"{center.Name}: {center.Address}\nAge Limit: {center.Sessions.Min(x => x.MinAgeLimit)}\nCapacity:  {center.Sessions.Capacity}");
                }

                Console.WriteLine(sb.ToString());
                Log.Debug("Sending alert message to WhatsApp");
                WhatsAppManager.SendMessage("+918123470121", sb.ToString());
                Log.Debug("Sent alert message to WhatsApp");
            }
        }
    }
}

