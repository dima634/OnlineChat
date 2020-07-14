using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Helpers
{
    public static class ApiControllerExtension
    {
        public static ContentResult JsonContent(this ControllerBase controller, object obj, TypeNameHandling typeNameHandling)
            => controller.Content(JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                TypeNameHandling = typeNameHandling
            }), "application/json");
    }
}
