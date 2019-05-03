using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestClients.Controllers
{    
    public class CxSastController: Controller
    {
        private static readonly JObject Configuration = JObject.Parse(System.IO.File.ReadAllText(@"etc/CxSast/config.json"));
        private static readonly JObject Urls = JObject.Parse(System.IO.File.ReadAllText(@"etc/CxSast/urls.json"));
        //        const JObject config = 
        public IActionResult CxSastHome()
        {
            return View();
        }

        private static IRestResponse SendRequests(string keyword,  string urlSub=null, string data=null)
        {
            var restClient = new RestClient();
            var request = new RestRequest();
            
            request.AddHeader("Content-Type", "application/json;v=" + Urls[keyword]["version"]);
            request.AddHeader("cxOrigin", "ASP.Net Core Web Application");
            request.AddHeader("Authorization", "Bearer " + GetAccessToken());

            switch (Urls[keyword]["http_method"].ToString())
            {
                case "GET":
                {
                    
                    restClient.BaseUrl = new Uri(Configuration["CxServer"].ToString() + Urls[keyword]["url_suffix"]);
                    request.Method = Method.GET;
                    break;
                }
                case "POST":
                {
                    request.Method = Method.POST;
                    break;
                }
                default:
                    Console.WriteLine("Request method not found!");
                    break;
            }

            var response = restClient.Execute(request);
            
            return response;
            

        }

        private static string GetAccessToken()
        {
            var restClient = new RestClient(Configuration["CxServer"].ToString() + Urls["AccessToken"]["url_suffix"]);
            var request = new RestRequest {Method = Method.POST};
//            request.AddHeader("Content-Type", "application/json;v=2.0");
//            request.AddHeader("cxOrigin", "ASP.Net Core Web Application");

            request.AddParameter("username", Configuration["CxUser"]);
            request.AddParameter("password", Configuration["CxPassword"]);
            request.AddParameter("grant_type", "password");
            request.AddParameter("scope", "sast_rest_api");
            request.AddParameter("client_id", "resource_owner_client");
            request.AddParameter("client_secret", "014DF517-39D1-4453-B7B3-9930C563627C");

            var response = restClient.Execute(request);
            
            var json = (JObject)JsonConvert.DeserializeObject(response.Content);

            return json["access_token"].ToString();
        }

//        public JObject Login()

//        {

//            // This method will be deprecated after v8.9.0

//            var restClient = new RestClient(Configuration["CxServer"].ToString() + Urls["login"]["url_suffix"]);

//            var request = new RestRequest {Method = Method.POST};

//

//            request.AddParameter("username", "admin");

//            request.AddParameter("password", "Password01!");

//

//            var response = restClient.Execute(request);

//            Console.WriteLine("================================================");

//            Console.WriteLine(Configuration["CxServer"].ToString() + Urls["login"]["url_suffix"]);

//            Console.WriteLine(response.Headers);

//            Console.WriteLine(response.StatusCode);

//            Console.WriteLine(response.Content);

//            Console.WriteLine("================================================");

//

//            return null;

//        }


        public string GetAllProjectDetials()
        {

            var response = SendRequests("GetAllProjectDetails");

            return response.Content;


        }
    }
}