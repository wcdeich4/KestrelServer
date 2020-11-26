using System;
//using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {

        [EnableCors("*")] //??? doing anything?
        [HttpGet("{name}")]
        public IEnumerable<string> Get(string name)
        {

           // var x = System.Configuration.ConfigurationManager.AppSettings["ResetIISEncryptWebconfig"];
            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
                var Configuration = builder.Build();
                var connectionString = Configuration["ConnectionStrings:YourConnectionString"];
                System.Console.WriteLine(connectionString);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }


            System.IO.File.WriteAllText("localfile.txt", "text");

            return new string[] { "gfdafsxfxcgcgfghkfdfdsdfsdfg" };
            //https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readalltext?view=net-5.0
            //
        }
    }
}
