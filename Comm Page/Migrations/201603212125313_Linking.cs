namespace Comm_Page.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Linking : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        CommentText = c.String(),
                        Like = c.Int(nullable: false),
                        Dislike = c.Int(nullable: false),
                        likedBy = c.String(),
                        dislikedBy = c.String(),
                        PersonID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CommentID)
                .ForeignKey("dbo.People", t => t.PersonID)
                .Index(t => t.PersonID);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        PersonID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        LikesCount = c.Int(nullable: false),
                        Visited = c.DateTime(nullable: false),
                        Avatar = c.String(),
                    })
                .PrimaryKey(t => t.PersonID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "PersonID", "dbo.People");
            DropIndex("dbo.Comments", new[] { "PersonID" });
            DropTable("dbo.People");
            DropTable("dbo.Comments");
        }
    }
}
