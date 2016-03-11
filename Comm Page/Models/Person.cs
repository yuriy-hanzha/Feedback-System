using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Comm_Page.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("People")]
    public class Person
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string ID { get; set; }
        public string Name { get; set; }
        public int LikesCount { get; set; }
        public DateTime Visited { get; set; }
    }

    public class PeopleContext : DbContext
    {
        public PeopleContext ()
            : base("DefaultConnection")
        {
        }
        public DbSet<Person> People { get; set; }
    }
}