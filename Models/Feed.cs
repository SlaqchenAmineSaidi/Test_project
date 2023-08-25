using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test_project.Models
{
    public class Feed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        // Other properties

        public List<Article> Articles { get; set; }
    }
}
