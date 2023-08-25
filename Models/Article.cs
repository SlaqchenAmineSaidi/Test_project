using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test_project.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        // Other properties

        public int FeedId { get; set; }
        public Feed Feed { get; set; }
    }
}
