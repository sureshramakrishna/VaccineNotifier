using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VaccineNotifier.Models;

namespace VaccineNotifier
{
    class CowinApi
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(CowinApi));
        private const string BaseUri = "https://cdn-api.co-vin.in/api/v2/";
        private static CowinApi _cowinApi;
        private string _transactionId = "4cace144-4b34-4008-8a66-e8b6a491539d";
        private string _token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX25hbWUiOiIyM2IwNzBhOS03MTA1LTQwODQtOGY1Yy02MGExNDVjMDlkY2IiLCJ1c2VyX3R5cGUiOiJCRU5FRklDSUFSWSIsInVzZXJfaWQiOiIyM2IwNzBhOS03MTA1LTQwODQtOGY1Yy02MGExNDVjMDlkY2IiLCJtb2JpbGVfbnVtYmVyIjo5NzM4NzAwMTI1LCJiZW5lZmljaWFyeV9yZWZlcmVuY2VfaWQiOjE2Mjk0NzM5MzA0OTgyLCJ0eG5JZCI6IjRjYWNlMTQ0LTRiMzQtNDAwOC04YTY2LWU4YjZhNDkxNTM5ZCIsImlhdCI6MTYyMDg4MDgyNCwiZXhwIjoxNjIwODgxNzI0fQ.qNU1OdsLyi0iDlppelHyE4tYsINvu1mJfRc0Ll7Lqdc";
        private State[] _states;
        private readonly Dictionary<int, District[]> _districts;
        private State _selectedState;
        private District _selectedDistrict;

        private CowinApi()
        {
            _districts = new Dictionary<int, District[]>();
        }
        public static CowinApi Api => _cowinApi ?? (_cowinApi = new CowinApi());
        public string GenerateOtp(string mobileNumber, out HttpStatusCode statusCode)
        {
            try
            {
                dynamic value = new System.Dynamic.ExpandoObject();
                value.mobile = mobileNumber;
                var uri = $"{BaseUri}auth/public/generateOTP";
                var response = WebClient.Client.GetResponse(uri, HttpMethod.Post, value, out statusCode);
                if (statusCode != HttpStatusCode.OK)
                    return response;
                var r = JsonConvert.DeserializeObject(response);
                _transactionId = r["txnId"].Value;
                return _transactionId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string VerifyOtp(string otp, out HttpStatusCode statusCode)
        {
            try
            {
                dynamic value = new System.Dynamic.ExpandoObject();
                value.otp = ComputeHash(otp);
                value.txnId = _transactionId;
                var uri = $"{BaseUri}auth/public/confirmOTP";
                var response = WebClient.Client.GetResponse(uri, HttpMethod.Post, value, out statusCode);
                if (statusCode != HttpStatusCode.OK)
                    return response;
                var r = JsonConvert.DeserializeObject(response);
                _token = r["token"].Value;
                return _token;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        public State GetState(string name)
        {
            var states = GetStates();
            _selectedState = states.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            return _selectedState;
        }
        public State[] GetStates()
        {
            try
            {
                if (_states != null)
                    return _states;
                Log.Debug("Fetching states from server...");
                var uri = $"{BaseUri}admin/location/states";
                var response = WebClient.Client.GetResponse<string>(uri, HttpMethod.Get, null, out _);
                var content = JObject.Parse(response);
                _states = content["states"].ToObject<State[]>();
                Log.Debug("Successfully fetched states from server...");
                return _states;
            }
            catch (Exception e)
            {
                Log.Error("Failed to fetch states from server: " + e.Message);
                throw;
            }
        }

        public District GetDistrict(string name)
        {
            var districts = GetDistricts(_selectedState.Id);
            _selectedDistrict = districts.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            return _selectedDistrict;
        }
        public District[] GetDistricts(int stateId)
        {
            try
            {
                if (!_districts.TryGetValue(stateId, out var districts))
                {
                    Log.Debug("Fetching districts from server...");
                    var uri = $"{BaseUri}admin/location/districts/{stateId}";
                    var response = WebClient.Client.GetResponse<string>(uri, HttpMethod.Get, null, out _);
                    var content = JObject.Parse(response);
                    _districts[stateId] = districts = content["districts"].ToObject<District[]>();
                    Log.Debug("Successfully fetched districts from server...");
                }
                return districts;
            }
            catch (Exception e)
            {
                Log.Error("Failed to fetch districts from server: " + e.Message);
                throw;
            }
        }

        public List<Center> GetSchedule(DateTime dateTime)
        {
            try
            {
                Log.Debug($"Fetching slots data from server (Date: {dateTime:dd-MM-yyyy})");
                var uri = $"{BaseUri}appointment/sessions/public/calendarByDistrict?district_id={_selectedDistrict.Id}&date={dateTime:dd-MM-yyyy}";
                var response = WebClient.Client.GetResponse<string>(uri, HttpMethod.Get, null, out HttpStatusCode _);
                var content = JObject.Parse(response);
                Log.Debug($"Fetching data successful (Date: {dateTime:dd-MM-yyyy})");
                var centers = content["centers"].ToObject<List<Center>>();
                return centers.Where(x => x.Sessions.Any(session => session.AvailableCapacity > 0 && session.MinAgeLimit < 45)).ToList();
            }
            catch (Exception e)
            {
                Log.Error("Failed to fetch data from server:  "+ e.Message);
            }
            return new List<Center>();
        }
        private static string ComputeHash(string value)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(value));

                foreach (var b in result)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
