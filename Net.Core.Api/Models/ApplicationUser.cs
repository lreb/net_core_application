using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Net.Core.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationUser
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
    }
}
