using System;
using System.Linq;
using System.Web.Mvc;
using Comm_Page.Models;

namespace Comm_Page.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        CPContext mc;

        public HomeController()
        {
            mc = new CPContext();
        }

        public ActionResult Index()
        {
            var persCont = mc.People.FirstOrDefault(p => p.Name == User.Identity.Name);
            if (persCont != null)
            {
                if (DateTime.Today.DayOfYear > persCont.Visited.DayOfYear)
                    persCont.LikesCount += (DateTime.Today.DayOfYear - persCont.Visited.DayOfYear) / 3;
                if (persCont.LikesCount > 10)
                    persCont.LikesCount = 10;

                persCont.Visited = DateTime.Now;
                mc.SaveChanges();
            }
            else
            {
                persCont = new Person
                    {
                        PersonID = Guid.NewGuid().ToString(),
                        Name = User.Identity.Name,
                        LikesCount = 10,
                        Visited = DateTime.Now,
                        Avatar = "/Content/Images/avatar.png"
                    };
                mc.People.Add(persCont);
                mc.SaveChanges();
            }

            var commCont = (from comm in mc.Comments
                            orderby comm.Like - comm.Dislike descending
                            select comm).ToList();

            for (int i = 0; i < commCont.Count; i++)
            {
                var c = commCont[i];
                c.person = mc.People.FirstOrDefault(x => x.PersonID == c.PersonID);
            }
                

            ViewBag.votes = persCont.LikesCount;
            ViewBag.avatar = persCont.Avatar;

            return View(commCont.ToList());
        }


        [System.Web.Http.HttpPost]
        public JsonResult GetData(string json)
        {
            var prsn = mc.People.FirstOrDefault(p => p.Name == User.Identity.Name);
            if(!string.IsNullOrEmpty(json))
            {
                foreach (var i in mc.Comments)
                    if(json == i.CommentText && User.Identity.Name == i.Name) 
                        return Json(new { success = false });

                var c = new Comment
                {
                    CommentID = Guid.NewGuid().ToString(),
                    Name = User.Identity.Name,
                    CommentText = json,
                    Like = 0,
                    Dislike = 0,
                    PersonID = prsn.PersonID
                };
                mc.Comments.Add(c);
                mc.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
                
        }

        public JsonResult GetLike(string name, string comm)
        {
            var persCont = mc.People.FirstOrDefault(p => p.Name == User.Identity.Name);
            var obj = mc.Comments.FirstOrDefault(c => c.Name == name && c.CommentText == comm);

            if (obj.dislikedBy != null && obj.dislikedBy.Contains(User.Identity.Name) || persCont.LikesCount < 1)
                return Json(new { });

            if (!string.IsNullOrEmpty(obj.likedBy))
            {
                if (obj.likedBy.Contains(User.Identity.Name))
                {
                    obj.Like--;
                    obj.likedBy = obj.likedBy.Replace(User.Identity.Name + "; ", "");
                    mc.SaveChanges();

                    persCont.LikesCount = persCont.LikesCount < 10 ? persCont.LikesCount + 1 : 10;
                    ViewBag.votes = persCont.LikesCount;
                    mc.SaveChanges();
                    return Json(new { success = false });
                }
                obj.Like++;
                obj.likedBy += User.Identity.Name + "; ";
                SaveVotes(persCont);
                return Json(new { success = true });
            }
            else
            {
                obj.Like++;
                obj.likedBy = User.Identity.Name + "; ";
                SaveVotes(persCont);
                return Json(new { success = true });
            }
        }

        public JsonResult GetDisLike(string name, string comm)
        {
            var persCont = mc.People.FirstOrDefault(p => p.Name == User.Identity.Name);
            var obj = mc.Comments.FirstOrDefault(c => c.Name == name && c.CommentText == comm);

            if (obj.likedBy != null && obj.likedBy.Contains(User.Identity.Name) || persCont.LikesCount < 1)
                return Json(new { });

            if (!string.IsNullOrEmpty(obj.dislikedBy))
            {
                if (obj.dislikedBy.Contains(User.Identity.Name))
                {
                    obj.Dislike--;
                    obj.dislikedBy = obj.dislikedBy.Replace(User.Identity.Name + "; ", "");
                    mc.SaveChanges();

                    persCont.LikesCount = persCont.LikesCount < 10 ? persCont.LikesCount + 1 : 10;
                    ViewBag.votes = persCont.LikesCount;
                    mc.SaveChanges();
                    return Json(new { success = false });
                }
                obj.Dislike++;
                obj.dislikedBy += User.Identity.Name + "; ";
                SaveVotes(persCont);
                return Json(new { success = true });
            }
            else
            {
                obj.Dislike++;
                obj.dislikedBy = User.Identity.Name + "; ";
                SaveVotes(persCont);
                return Json(new { success = true });
            }
        }

        private void SaveVotes(Person persCont)
        {
            persCont.LikesCount--;
            ViewBag.votes = persCont.LikesCount;
            mc.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            mc.Dispose();
            base.Dispose(disposing);
        }
    }
}