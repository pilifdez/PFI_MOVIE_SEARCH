using Newtonsoft.Json;
using System.IO;
using System.Net.Mime;
using System.Net;
using System;

namespace PFI_MOVIE_SEARCH.Themoviedb
{
    public class Connector
    {
        private const string URL_FORMAT = "{0}/{1}/{2}";
        private const string CONTENT_TYPE = "application/json";
        private string baseUrl { get; set; }
        private string accessToken { get; set; }
        private int deleteLogDays { get; set; }
        public Connector(string baseUrl, string accessToken, int deleteLogDays=7)
        {
            this.baseUrl = baseUrl;
            this.accessToken = accessToken;
            this.deleteLogDays = deleteLogDays;
        }

        public TResult Request<TResult>(out HttpStatusCode status, string resource, string operation, string method, object body = null, string path = null) where TResult : class
        {
            status = HttpStatusCode.NoContent;
            try
            {
                string url = string.Format(URL_FORMAT, baseUrl, operation, resource);
                if (!string.IsNullOrEmpty(path))
                {
                    url += path;
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.Timeout = 30000;
                request.Headers.Add("Authorization", "Bearer " + accessToken);

                if (body != null)
                {
                    request.ContentType = CONTENT_TYPE;
                    string content = JsonConvert.SerializeObject(body);
                    using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                    {
                        sw.Write(content);
                    }
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response != null &&
                    response.StatusCode == HttpStatusCode.OK)
                {
                    status = response.StatusCode;
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string result = sr.ReadToEnd();
                        if (!string.IsNullOrEmpty(result))
                        {                            
                            TResult res = JsonConvert.DeserializeObject<TResult>(result);
                            TResult objRes = JsonConvert.DeserializeObject<TResult>(result);
                            res = objRes;
                            return res;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string Message_Error = ex.Message + "\n\n";
                try
                {
                    WebException webEx = ex as WebException;
                    if (webEx != null)
                    {
                        HttpWebResponse response = (HttpWebResponse)webEx.Response;
                        if (response != null) status = response.StatusCode;
                        using (StreamReader sr = new StreamReader(webEx.Response.GetResponseStream()))
                        {
                            string bodyError = sr.ReadToEnd();
                            Message_Error += "Body -> " + bodyError;
                        }
                    }                    
                }
                catch { }
            }

            return null;
        }
    }
}
