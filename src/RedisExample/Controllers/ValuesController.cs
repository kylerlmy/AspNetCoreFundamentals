using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        private IDistributedCache _cache;
        public ValuesController(IDistributedCache cache)
        {
            _cache = cache;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            //var helloRedis = Encoding.UTF8.GetBytes("Hello World");
            //HttpContext.Session.Set("hellokey", helloRedis);

            //var gethello = default(byte[]);
            //HttpContext.Session.TryGetValue("hellokey", out gethello);
            //var result = Encoding.UTF8.GetString(gethello);


            var cachedTimeUTC = "Cached Time Expired";
            var encodedCachedTimeUTC = _cache.Get("redis-mastercahcedTimeUTC");

            if (encodedCachedTimeUTC != null)
            {
                cachedTimeUTC = Encoding.UTF8.GetString(encodedCachedTimeUTC);
            }

            return Json(cachedTimeUTC);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
