using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Net.Core.Api.Models
{
    /// <summary>
    /// General context for manage entities
    /// </summary>
    public class ApplicationContext : DbContext
    {
        /// <summary>
        /// constructor of applicationcontext
        /// </summary>
        /// <param name="options"></param>
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        /// <summary>
        /// Users entity
        /// </summary>
        public DbSet<ApplicationUser> Users { get; set; }
    }
}
