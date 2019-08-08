using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestClients.Models;

namespace RestClients.Controllers
{    
    public class CxSastController: Controller
    {
        private readonly CxSastClient _cx = new CxSastClient();
        public IActionResult CxSastHome()
        {
            return View();
        }

        
        public async Task<string> GetAllProjectDetials()
        {
            return await CxSastClient.GetAllProjectDetials();
        }

        public async Task<string> CreateProjectWithDefaultConfiguration(string name, string owningTeam, bool isPublic = true)
        {
            return await CxSastClient.CreateProjectWithDefaultConfiguration(name, owningTeam, isPublic);
        }

        public async Task<string> GetProjectDetailsById(string id)
        {
            return await CxSastClient.GetProjectDetailsById(id);
        }

        public async Task<string> UpdateProjectById(string id, string name, string owningTeam,
            bool customFields = false, int customFieldId = 0, string value = null)
        {
            return await CxSastClient.UpdateProjectById(id, name, owningTeam, customFields, customFieldId, value);
        }
    }
}