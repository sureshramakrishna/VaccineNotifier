using System;
using System.Linq;
using System.Net;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace VaccineNotifier
{
    internal class VaccineNotifierService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(VaccineNotifierService));
        private const string JobName = "CheckAvailableSlots";
        private const string JobGroup = "Vaccine";
        public void Start()
        {
            Initialize();
            Log.Debug("Started configuring scheduler..");
            var quartzFactory = new StdSchedulerFactory();
            var scheduler = quartzFactory.GetScheduler().Result;
            scheduler.Start();
            var jobKey = new JobKey(JobName, JobGroup);
            var trigger = new SimpleTriggerImpl(JobName, JobGroup, SimpleTriggerImpl.RepeatIndefinitely, new TimeSpan(0, 0, 15));
            IJobDetail jobDetails = new JobDetailImpl(jobKey.Name, jobKey.Group, typeof(CheckSlotsJob));
            scheduler.ScheduleJob(jobDetails, trigger);
            Log.Debug("Finished configuring scheduler.");
            Log.Debug("Service configured successfully. Sit back and relax, we will notify you when free slots are available :)");
        }
        public void Stop()
        {
            Console.WriteLine("Shutting down the service...");
        }

        public void Initialize()
        {
            Console.Clear();
            Console.WriteLine("".PadLeft(70, '*'));
            Console.WriteLine("Copyright (c) 2021, by Suresh R");
            Console.WriteLine("All rights reserved.");
            Console.WriteLine("".PadLeft(70, '*'));
            Log.Debug("Initializing service...");

            //HttpStatusCode statusCode = 0;
            //while (statusCode != HttpStatusCode.OK)
            //{
            //    Console.Write("Please input your mobile number: ");
            //    var mobileNumber = Console.ReadLine();
            //    var response = CowinApi.Api.GenerateOtp(mobileNumber, out statusCode);
            //    if (statusCode != HttpStatusCode.OK)
            //        Console.WriteLine(response + "...Please try again!\n");
            //}
            //Console.WriteLine();
            //statusCode = 0;
            //while (statusCode != HttpStatusCode.OK)
            //{
            //    Console.Write("Please enter OTP: ");
            //    var otp = Console.ReadLine();
            //    var response = CowinApi.Api.VerifyOtp(otp, out statusCode);
            //    if (statusCode != HttpStatusCode.OK)
            //        Console.WriteLine(response + "...Please try again!\n");
            //}
            //Console.WriteLine();
            while (true)
            {
                Console.Write("Enter State (Copy from cowin.gov.in):  ");
                var state = CowinApi.Api.GetState(Console.ReadLine());
                if (state != null)
                {
                    Log.Debug($"State set to:  {state.Name}");
                    break;
                }
            }
            Console.WriteLine();
            while (true)
            {
                Console.Write("Enter District (Copy from cowin.gov.in):  ");
                var district = CowinApi.Api.GetDistrict(Console.ReadLine());
                if (district != null)
                {
                    Log.Debug($"District set to:  {district.Name}");
                    break;
                }
            }
            Console.WriteLine();
        }
    }
}
