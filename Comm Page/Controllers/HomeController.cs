using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Comm_Page.Models;

namespace Comm_Page.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        CommentsContext cc = new CommentsContext();
        PeopleContext pc = new PeopleContext();


        public ActionResult Index()
        {
            var persCont = pc.People.Where(p => p.Name == User.Identity.Name).FirstOrDefault();
            if (persCont != null)
            {
                if (DateTime.Today.DayOfYear > persCont.Visited.DayOfYear)
                    persCont.LikesCount += (DateTime.Today.DayOfYear - persCont.Visited.DayOfYear) / 3;
                if (persCont.LikesCount > 10)
                    persCont.LikesCount = 10;

                persCont.Visited = DateTime.Now;
                pc.SaveChanges();
            }
            else
            {
                persCont = new Person
                    {
                        ID = Guid.NewGuid().ToString(),
                        Name = User.Identity.Name,
                        LikesCount = 10,
                        Visited = DateTime.Now
                    };
                pc.People.Add(persCont);
                pc.SaveChanges();
            }

            var commCont = from comm in cc.Comments
                           orderby comm.Like - comm.Dislike descending
                           select comm;
            ViewBag.votes = persCont.LikesCount;

            return View(commCont.ToList());
        }


        [System.Web.Http.HttpPost]
        public JsonResult GetData(string json)
        {
            if(json != null && json != "")
            {
                foreach (var i in cc.Comments)
                    if(json == i.CommentText && User.Identity.Name == i.Name) 
                        return Json(new { success = false });

                var c = new Comment
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = User.Identity.Name,
                    CommentText = json,
                    Like = 0,
                    Dislike = 0
                };
                cc.Comments.Add(c);
                cc.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
                
        }

        public JsonResult GetLike(string name, string comm)
        {
            var persCont = pc.People.Where(p => p.Name == User.Identity.Name).FirstOrDefault();
            var obj = cc.Comments.Where(c => c.Name == name && c.CommentText == comm).FirstOrDefault();

            if (obj.dislikedBy != null && obj.dislikedBy.Contains(User.Identity.Name) || persCont.LikesCount < 1)
                return Json(new { });

            if (obj.likedBy != null && obj.likedBy != "")
            {
                if (obj.likedBy.Contains(User.Identity.Name))
                {
                    obj.Like--;
                    obj.likedBy = obj.likedBy.Replace(User.Identity.Name + "; ", "");
                    cc.SaveChanges();

                    persCont.LikesCount = persCont.LikesCount < 10 ? persCont.LikesCount + 1 : 10;
                    ViewBag.votes = persCont.LikesCount;
                    pc.SaveChanges();
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
            var persCont = pc.People.Where(p => p.Name == User.Identity.Name).FirstOrDefault();
            var obj = cc.Comments.Where(c => c.Name == name && c.CommentText == comm).FirstOrDefault();

            if (obj.likedBy != null && obj.likedBy.Contains(User.Identity.Name) || persCont.LikesCount < 1)
                return Json(new { });

            if (obj.dislikedBy != null && obj.dislikedBy != "")
            {
                if (obj.dislikedBy.Contains(User.Identity.Name))
                {
                    obj.Dislike--;
                    obj.dislikedBy = obj.dislikedBy.Replace(User.Identity.Name + "; ", "");
                    cc.SaveChanges();

                    persCont.LikesCount = persCont.LikesCount < 10 ? persCont.LikesCount + 1 : 10;
                    ViewBag.votes = persCont.LikesCount;
                    pc.SaveChanges();
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
            cc.SaveChanges();

            persCont.LikesCount--;
            ViewBag.votes = persCont.LikesCount;
            pc.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            cc.Dispose();
            pc.Dispose();
            base.Dispose(disposing);
        }
    }
}

//public JsonResult GetLike(string name, string comm)
//{
//    var obj = cc.Comments.Where(c => c.Name == name && c.CommentText == comm).FirstOrDefault();
//    return VoteCalc(obj.Like, obj.Dislike, obj.likedBy, obj.dislikedBy);
//}

//public JsonResult GetDisLike(string name, string comm)
//{

//    var obj = cc.Comments.Where(c => c.Name == name && c.CommentText == comm).FirstOrDefault();
//    return VoteCalc(obj.Dislike, obj.Like, obj.dislikedBy, obj.likedBy);
//}
//public JsonResult VoteCalc(int vote1, int vote2, string voted1, string voted2)
//{
//    var persCont = pc.People.Where(p => p.Name == User.Identity.Name).FirstOrDefault();
//    if (voted2 != null && voted2.Contains(User.Identity.Name) || persCont.LikesCount < 1)
//        return Json(new { });

//    if (voted1 != null && voted1 != "")
//    {
//        if (voted1.Contains(User.Identity.Name))
//        {
//            vote1--;
//            voted1 = voted1.Replace(User.Identity.Name + "; ", "");
//            cc.SaveChanges();

//            persCont.LikesCount = persCont.LikesCount < 10 ? persCont.LikesCount + 1 : 10;
//            ViewBag.votes = persCont.LikesCount;
//            pc.SaveChanges();
//            return Json(new { success = false });
//        }
//        vote1++;
//        voted1 += User.Identity.Name + "; ";
//        SaveVotes(persCont);
//        return Json(new { success = true });
//    }
//    else
//    {
//        vote2++;
//        voted1 = User.Identity.Name + "; ";
//        SaveVotes(persCont);
//        return Json(new { success = true });
//    }
//}