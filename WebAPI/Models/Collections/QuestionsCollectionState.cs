using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models.Collections
{
    public class QuestionsCollectionState
    {
        public int id { get; set; }
        public string question { get; set; }
        public LinkCollection _links { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public int likes { get; set; }
        public string[] likers { get; set; }
    }
}
