using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarModels001.Controllers
{
    public class CarModelController : ApiController
    {
        CarModels001.Models.CarMakesModels cCMM = new CarModels001.Models.CarMakesModels();

        // GET api/carmodel
        public IHttpActionResult Get()
        {
            Dictionary<string, dynamic> Dict = new Dictionary<string, dynamic>();

            cCMM.Get(ref Dict, true);

            return Json(Dict.Keys.ToArray());
        }

        // GET api/carmodel/{name}
        [Route("api/carmodel/{name}")]
        public IHttpActionResult Get(string name)
        {
            Dictionary<string, dynamic> Dict = new Dictionary<string, dynamic>();
            Dict.Add(name, new List<string>());

            cCMM.Get(ref Dict, false);

            if (Dict[name].Count == 0) return NotFound();

            return Ok(Dict[name].ToArray());
        }

        // GET api/carmodel/all
        [Route("api/carmodel/all")]
        public IHttpActionResult GetAllMakes()
        {
            Dictionary<string, dynamic> Dict = new Dictionary<string, dynamic>();

            cCMM.Get(ref Dict, false);

            return Json(Dict);
        }

        // POST api/carmodel
        public void Post([FromBody]string value)
        {
        }

        // PUT api/carmodel/{name}
        [Route("api/carmodel/{name}")]
        public void Put(string name, [FromBody]string value)
        {
            Dictionary<string, dynamic> Dict = new Dictionary<string, dynamic>();
            Dict.Add(name, new List<string>());
            Dict[name].Add(value);

            cCMM.Set(ref Dict);
        }

        // DELETE api/carmodel/5
        public void Delete(int id)
        {
        }
    }
}
