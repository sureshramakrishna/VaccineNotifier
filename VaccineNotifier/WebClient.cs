using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using VaccineNotifier.Models;

namespace VaccineNotifier
{
    internal class WebClient
    {
        private static WebClient _client;
        public static WebClient Client => _client ?? (_client = new WebClient());

        private WebRequest CreateRequest<T>(string uri, HttpMethod method, T content) where T : class
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(uri));
            request.Method = method.Method;
            request.ContentType = "application/json";
            if (content == null)
                return request;
            using (var sr = request.GetRequestStream())
            {
                var bytes = new ObjectContent<T>(content, new JsonMediaTypeFormatter()).ReadAsByteArrayAsync().Result;
                sr.Write(bytes, 0, bytes.Length);
                return request;
            }
        }
        public string GetResponse<T>(string apiPath, HttpMethod method, T content, out HttpStatusCode statusCode) where T : class
        {
            HttpWebResponse response = null;
            statusCode = HttpStatusCode.OK;
            try
            {
                var request = CreateRequest(apiPath, method, content);
                response = (HttpWebResponse) request.GetResponse();
                return ReadResponse(response);
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
                if (response == null)
                    throw new Exception(e.Message);
                return ReadResponse(response);
            }
            finally
            {
                if (response != null) 
                    statusCode = response.StatusCode;
            }
        }

        private string ReadResponse(HttpWebResponse response)
        {
            var responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                string responseString;
                if (string.IsNullOrEmpty(response.ContentEncoding))
                    using (var streamReader = new StreamReader(responseStream))
                        responseString = streamReader.ReadToEnd();
                else
                    throw new InvalidOperationException("Invalid content encoding");
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception(responseString + "\n" + response.ResponseUri.AbsoluteUri);

                if (!(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.NoContent))
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadGateway:
                        case HttpStatusCode.RequestTimeout:
                            throw new Exception("Request Timeout");
                        case HttpStatusCode.BadRequest:
                        {
                            var jsonValue = JsonConvert.DeserializeObject<ErrorDetails>(responseString);
                            throw new Exception(jsonValue.Error);
                        }
                        case HttpStatusCode.GatewayTimeout:
                            throw new Exception(!string.IsNullOrEmpty(response.StatusDescription) ? response.StatusDescription : "Bad Gateway");
                        case HttpStatusCode.ServiceUnavailable:
                            throw new Exception("Service Unavailable, please try again later.");
                        case HttpStatusCode.Unauthorized:
                        case HttpStatusCode.Forbidden:
                            throw new Exception(string.IsNullOrEmpty(responseString) ? response.StatusDescription : responseString);
                        case HttpStatusCode.SeeOther:
                        case HttpStatusCode.MovedPermanently:
                            throw new Exception(responseString + response.Headers["Location"]);
                        default:
                            throw new Exception(responseString);
                    }
                }
                return responseString;
            }
            return null;
        }
    }

}
