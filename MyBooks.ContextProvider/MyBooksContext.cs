using Microsoft.EntityFrameworkCore;
using MyBooks.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyBooks.ContextProvider
{
    public class MyBooksContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Author> Authors { get; set; }

        public MyBooksContext() : base()
        {

        }

        public MyBooksContext(DbContextOptions<MyBooksContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite("Data Source=MyBooks.db");
        }

    }
}
