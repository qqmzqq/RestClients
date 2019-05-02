using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestClients.Controllers
{
    public class CxSastController: Controller
    {
        private const string ServerUrl = "http://<HOST>:<PORT>";
        private const string CxUser = "<USERNAME>";
        private const string CxPassword = "<PASSWORD>";
//        const JObject config = 
        public IActionResult CxSastHome()
        {
            return View();
        }

        public string GetAccessToken()
        {
            
            const string urlSub = "/cxrestapi/auth/identity/connect/token";
            
            var restClient = new RestClient(ServerUrl + urlSub);
            var request = new RestRequest {Method = Method.POST};
            request.AddHeader("Content-Type", "application/json;v=1.0");
            request.AddHeader("cxOrigin", "ASP.Net Core Web Application");

            request.AddParameter("username", CxUser);
            request.AddParameter("password", CxPassword);
            request.AddParameter("grant_type", "password");
            request.AddParameter("scope", "sast_rest_api");
            request.AddParameter("client_id", "resource_owner_client");
            request.AddParameter("client_secret", "014DF517-39D1-4453-B7B3-9930C563627C");

            var response = restClient.Execute(request);

            var content = response.Content;

            return content;
        }
        
        public string GetAllProjectDetials()
        {
            const string urlSub = "/cxrestapi/projects";
            
            var restClient = new RestClient(ServerUrl + urlSub);
            
            var request = new RestRequest {Method = Method.GET};

            var json = (JObject)JsonConvert.DeserializeObject(GetAccessToken());
            
//            Console.WriteLine(json["access_token"]);
            
            request.AddHeader("Content-Type", "application/json;v=2.0");
            request.AddHeader("cxOrigin", "ASP.Net Core Web Application");
            request.AddHeader("Authorization", "Bearer "+json["access_token"]);

            var response = restClient.Execute(request);

            return response.Content;


        }

        public IRestResponse SendRequests(string keyword, string method, string urlSub=null, string data=null)
        {
            var restClient = new RestClient();
            var request = new RestRequest();

            switch (method)
            {
                case "get":
                {
                    restClient.BaseUrl = new Uri(ServerUrl + keyword);
                    request.Method = Method.GET;
                    break;
                }
                case "post":
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
    }
}