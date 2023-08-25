using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test_project.Models;

namespace Test_project.Data
{
    public class Test_projectContext : DbContext
    {
        public Test_projectContext (DbContextOptions<Test_projectContext> options)
            : base(options)
        {
        }

        public DbSet<Test_project.Models.Article> Article { get; set; }

        public DbSet<Test_project.Models.Feed> Feed { get; set; }
        public IEnumerable<object> Feeds { get; internal set; }
        public IEnumerable<object> Articles { get; internal set; }
    }
}
