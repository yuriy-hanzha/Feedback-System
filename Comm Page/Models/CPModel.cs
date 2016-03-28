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
        public string CommentID { get; set; }
        public string Name { get; set; }
        public string CommentText { get; set; }
        public int Like { get; set; }
        public int Dislike { get; set; }
        public string likedBy { get; set; }
        public string dislikedBy { get; set; }
        public Person person { get; set; }
        public string PersonID { get; set; }
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("People")]
    public class Person
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string PersonID { get; set; }
        public string Name { get; set; }
        public int LikesCount { get; set; }
        public DateTime Visited { get; set; }
        public string Avatar { get; set; }
    }

    public class CPContext : DbContext
    {
        public CPContext ()
            : base("DefaultConnection")
        {
        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Person> People { get; set; }
    }
}