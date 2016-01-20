using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp.Models.Home
{
    public class IndexModel
    {
        public List<Post> RecentPosts { get; set; }
    }
}