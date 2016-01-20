using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Comment
    {
 
        public string PostId { get; set; }
        
        public string Content { get; set; }
        public string Author { get; set; }
        public System.DateTime CreatedAtUtc { get; set; }
    }
}