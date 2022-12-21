using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIDemo_swagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        // GET: api/<HelloWorldController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello", "萱" };
        }

        // GET api/<HelloWorldController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HelloWorldController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HelloWorldController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HelloWorldController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
