using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        

        public ValuesController()
        {

        }


        // GET api/values
        [EnableCors("*")] //??? doing anything?
        [HttpGet]
        public IEnumerable<string> Get() //worked as text & JSON
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
		[EnableCors("*")]
        [HttpGet("{id}")]
        public string Get(int id) //did not work when called by RxJS - not res.getJSON(), nor x.Result
        {
            return "Hello from .NET Core WebAPI";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
