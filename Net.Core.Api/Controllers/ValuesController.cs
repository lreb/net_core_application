using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Net.Core.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    ///[Authorize]
    ///[AllowAnonymous]
    [Authorize]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        public enum eValueType
        {
            /// <summary>
            /// 
            /// </summary>
            Number,
            /// <summary>
            /// 
            /// </summary>
            Text
        }

        // GET api/values
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Value> Get()
        {
            //return new string[] { "value1", "value2" };
            return new Value[] { new Value { Id = 1, Text = "T1" }, new Value { Id = 2, Text = "T2" } };
        }

        // GET api/values/5
        /// <summary>
        /// Get API values by ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">Type of Value</param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public Value Get(int id, eValueType type)
        {
            //return "value";
            return new Value { Id = id, Text = "Value" };
        }

        // POST api/values
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(typeof(Value))]
        public IActionResult Post([FromBody]Value value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //redirect after create
            return CreatedAtAction("Get", new { id = value.Id }, value);
        }

        // PUT api/values/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        /// <summary>
        /// Delete API Value
        /// </summary>
        /// <remarks>This API will delete the values.</remarks>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public class Value
        {
            /// <summary>
            /// 
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [Required]
            public string Text { get; set; }
        }
    }
}
