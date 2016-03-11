using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Comm_Page.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("Comments")]
    public class Comment
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string ID { get; set; }
        public string Name { get; set; }
        public string CommentText { get; set; }
        public int Like { get; set; }
        public int Dislike { get; set; }
        public string likedBy { get; set; }
        public string dislikedBy { get; set; }
    }

    public class CommentsContext : DbContext
    {
        public CommentsContext ()
            : base("DefaultConnection")
        {
        }
        public DbSet<Comment> Comments { get; set; }
    }
}