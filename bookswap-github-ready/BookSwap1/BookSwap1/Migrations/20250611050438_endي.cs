using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookSwap1.Migrations
{
    public partial class endي : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // تحقق لو الجدول موجود وعدّله بدل ما تنشئ جديد
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'UserProfileImagePath' AND Object_ID = Object_ID(N'BookComment')) BEGIN ALTER TABLE [BookComment] ADD [UserProfileImagePath] nvarchar(max) NULL END");

            // تأكد من العلاقات لو لزم
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_BookComment_Books_BookId') BEGIN ALTER TABLE [BookComment] ADD CONSTRAINT [FK_BookComment_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([Id]) END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_BookComment_AspNetUsers_UserId') BEGIN ALTER TABLE [BookComment] ADD CONSTRAINT [FK_BookComment_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // عكس التغييرات لو عايزة ترجعي
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'UserProfileImagePath' AND Object_ID = Object_ID(N'BookComment')) BEGIN ALTER TABLE [BookComment] DROP COLUMN [UserProfileImagePath] END");
        }
    }
}