using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using WebApp.Models;
using WebApp.Models.Home;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var blogContext = new BlogContext();
            var filter = new BsonDocument();
            var sort = Builders<Models.Post>.Sort.Descending("CreatedAtUtc");

            var recentPosts = await blogContext.Posts.Find(filter).Sort(sort).Limit(10).ToListAsync();
            var model = new IndexModel
            {
                RecentPosts =recentPosts
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult NewPost()
        {
            return View(new NewPostModel());
        }

        [HttpPost]
        public async Task<ActionResult> NewPost(NewPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var blogContext = new BlogContext();
            var newPost = new Post();
            newPost.Content = model.Content;
            newPost.Tags =  model.Tags ;
            newPost.Title =  model.Title;
            newPost.Comments = new List<Comment>();
            newPost.Author = this.User.Identity.Name;
            newPost.CreatedAtUtc = DateTime.UtcNow;

            await blogContext.Posts.InsertOneAsync(newPost);
            return RedirectToAction("Post", new { id = newPost.Id });
        }

        [HttpGet]
        public async Task<ActionResult> Post(string id)
        {
            var blogContext = new BlogContext();
            var post = await blogContext.Posts.Find(x => x.Id == id).SingleOrDefaultAsync();
            if (post == null)
            {
                return RedirectToAction("Index");
            }

            var model = new PostModel
            {
                Post = post
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Posts(string tag = null)
        {
            var blogContext = new BlogContext();
            var sort = Builders<Models.Post>.Sort.Ascending("Tags");
            List<Models.Post> listx = null;
            if (tag==null)
            {
                var filter = new BsonDocument();
                listx = await blogContext.Posts.Find(filter).Sort(sort).ToListAsync();
            }
            else
            {
                var builder = Builders<Models.Post>.Filter;
                var filter = builder.Eq("Tags", tag);

                listx = await blogContext.Posts.Find(filter).Sort(sort).ToListAsync();
            }
            return View(listx);

        }

        [HttpPost]
        public async Task<ActionResult> NewComment(NewCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { id = model.PostId });
            }

            var blogContext = new BlogContext();
            var post = await blogContext.Posts.Find(x => x.Id == model.PostId).SingleOrDefaultAsync();

            var newComments = new Comment
            {
                Author = this.User.Identity.Name,
                Content= model.Content,
                PostId = model.PostId,
                CreatedAtUtc = DateTime.UtcNow
        };
            post.Comments.Add(newComments);

            var filter = Builders<Models.Post>.Filter.Eq("Id", model.PostId);


            var update = Builders<Models.Post>.Update
                .AddToSet("Comments", newComments);

            await blogContext.Posts.UpdateOneAsync(filter, update);
            return RedirectToAction("Post", new { id = model.PostId });
        }
    }
}