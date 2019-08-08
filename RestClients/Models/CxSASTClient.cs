using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestClients.Models
{
    public class CxSastClient
    {
        private static readonly JObject Configuration = JObject.Parse(System.IO.File.ReadAllText(@"Properties/CxSast/config.json"));
        private static readonly JObject Urls = JObject.Parse(System.IO.File.ReadAllText(@"Properties/CxSast/urls.json"));
        private static string _token;

        public CxSastClient()
        {
            _token = GetAccessToken();
        }

        private static async Task<HttpResponseMessage> SendRequests(string keyword,  JObject urlSub=null, JObject data=null)
        {
            Console.WriteLine("* Token: " + _token);
            if (urlSub == null)
            {
                Console.WriteLine("=== URL sub is null ===");
                urlSub = new JObject
                ( 
                    new JProperty("pattern", ""),
                    new JProperty("value", "")
                );
            }

            var urlParameters = Urls[keyword];
            
            var url = Configuration["CxServer"] + 
                      Regex.Replace
                      (
                          urlParameters["urlSuffix"].ToString(), 
                          urlSub["pattern"].ToString(), 
                          urlSub["value"].ToString() 
                      );
            
//            Console.WriteLine("=== url: " + url + " ===");
//            
//            var restClient = new RestClient(url);
//            var request = new RestRequest();
//
//            request.AddHeader("Content-Type", "application/json;v=" + Urls[keyword]["version"]);
//            request.AddHeader("cxOrigin", "ASP.Net Core Web Application");
//            request.AddHeader("Authorization", "Bearer " + GetAccessToken());

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Content-Type", "application/json;v=" + Urls[keyword]["version"]);
            client.DefaultRequestHeaders.Add("cxOrigin", "ASP.Net Core Web Application");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);

            HttpResponseMessage response = null;

            switch (Urls[keyword]["method"].ToString())
            {
                case "GET":
                {
                    response = await client.GetAsync(url);
                    break;
                }
                case "POST":
                {
                    response = await client.PostAsJsonAsync(url, data);
                    break;
                }
                case "PUT":
                {
                    response = await client.PutAsJsonAsync(url, data);
                    break;
                }
                default:
                    throw new Exception("Request method not found!");
            }

            
            return response;
            

        }

        private static string GetAccessToken()
        {
            var url = Configuration["CxServer"].ToString() + Urls["AccessToken"]["urlSuffix"];
            var data = new JObject
            {
                new JProperty("username", Configuration["CxUser"].ToString()),
                new JProperty("password", Configuration["CxPassword"].ToString()),
                new JProperty("grant_type", "password"),
                new JProperty("scope", "sast_rest_api"),
                new JProperty("client_id", "resource_owner_client"),
                new JProperty("client_secret", "014DF517-39D1-4453-B7B3-9930C563627C")
            };
            using (var handler = new HttpClientHandler{ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator})
            {
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Clear();
                
                var response = client.PostAsJsonAsync(url, data).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(content);

            }
            
            return "";
        }


        public static async Task<string> GetAllProjectDetials()
        {

            var response = await SendRequests("GetAllProjectDetails");

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> CreateProjectWithDefaultConfiguration(string name, string owningTeam, bool isPublic)
        {
            var data = new JObject
                (
                    new JProperty("name", name),
                    new JProperty("owningTeam", owningTeam),
                    new JProperty("isPublic", isPublic)
                );
            
            var response = await SendRequests("CreateProjectWithDefaultConfiguration", data:data);

            return await response.Content.ReadAsStringAsync();;
        }

        public static async Task<string> GetProjectDetailsById(string id)
        {
            var urlSub = new JObject
                ( 
                    new JProperty("pattern", "{id}"),
                    new JProperty("value", id)
                );
            var response = await SendRequests("GetProjectDetailsById", urlSub);

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> UpdateProjectById(string id, string name, string owningTeam, bool customFields=false, int customFieldId=0, string value=null)
        {
            /*
              {
                "name": "string",
                "owningTeam": "string",
                "customFields": [
                  {
                    "id": 0,
                    "value": "string"
                  }
                ]
              }
             */
            var urlSub = new JObject(
                    new JProperty("pattern", "{id}"),
                    new JProperty("value", id)
                );
            var data = new JObject(
                    new JProperty("name", name),
                    new JProperty("owningTeam", owningTeam)
                );
            if (customFields)
            {
                data.Add(
                        new JProperty
                        (
                            "customFields", new JArray(new JObject
                                (
                                    new JProperty("id", customFieldId),
                                    new JProperty("value", value)
                                ))
                        )
                    );
            }

            var response = await SendRequests("UpdateProjectById", urlSub, data);

            return await response.Content.ReadAsStringAsync();
        }
    }
}