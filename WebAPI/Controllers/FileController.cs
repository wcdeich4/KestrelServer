using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using BasicObjects;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private SingletonDictionary singletonDictionary ;

        public FileController()
        {
            singletonDictionary = SingletonDictionary.GetInstance();
        }

        // GET ~/File/name
        [EnableCors("*")] //??? doing anything?
        [HttpGet("{filename}")]
        public IEnumerable<string> FileString(string filename)
        {
            string fileContents = null;
            try
            {
                string envContentRootPath = (string) singletonDictionary["env.ContentRootPath"];
                string fileFolder = (string) singletonDictionary["FileFolder"];
                string fileFolderPath = System.IO.Path.Combine(envContentRootPath, fileFolder);
                string filePath = System.IO.Path.Combine(fileFolder, filename);
                fileContents = System.IO.File.ReadAllText(filePath);

                //System.Console.WriteLine("Path.Combine reuslt = " + combinedPath);
                //System.Console.WriteLine("folder path: " + FileFolderPath);
               // string envWebRootPath = (string) singletonDictionary["env.WebRootPath"];
               // System.Console.WriteLine("env.WebRootPath: " + envWebRootPath);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }


       //     System.IO.File.WriteAllText("localfile.txt", "text");


            return new string[] { fileContents };
            //return fileContents;
        }
    }
}
