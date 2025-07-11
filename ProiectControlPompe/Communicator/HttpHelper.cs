using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Communicator
{
    public class HttpHelper
    {
        public static List<ProcessStatusEvent> GetDataFromWebAPI()
        {
            string url = @"http://localhost:5040/api/process";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.ProtocolVersion = HttpVersion.Version11;      
            request.KeepAlive = false;                            
            request.AutomaticDecompression = DecompressionMethods.GZip;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<ProcessStatusEvent>>(json);
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine($"WebException: {ex.Message}");
                if (ex.Response != null)
                {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        Console.WriteLine("Server returned: " + reader.ReadToEnd());
                    }
                }
                return null;
            }
        }

        public static void PostDataToWebAPI(ProcessStatusEvent postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:5040/api/process");
            request.Method = "POST";

            request.ProtocolVersion = HttpVersion.Version11;
            request.KeepAlive = false;
            request.ContentType = "application/json";

            string json = JsonConvert.SerializeObject(postData);
            Console.WriteLine("[HttpHelper] Trimit date: " + json);

            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            request.ContentLength = byteArray.Length;

            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                using (WebResponse response = request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine("Server response: " + result);
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine($"WebException: {ex.Message}");
                if (ex.Response != null)
                {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        Console.WriteLine("Server returned: " + reader.ReadToEnd());
                    }
                }
            }
        }
    }
}