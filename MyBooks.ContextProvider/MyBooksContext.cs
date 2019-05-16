using Microsoft.EntityFrameworkCore;
using MyBooks.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyBooks.ContextProvider
{
    public class MyBooksContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Author> Authors { get; set; }

        private bool MassUpdateMode = false;
        private bool UnsavedChangesExist = false;

        public MyBooksContext() : base()
        {

        }

        public MyBooksContext(DbContextOptions<MyBooksContext> options) : base(options)
        {
        }

        public void StartMassUpdate()
        {
            MassUpdateMode = true;
        }

        public async Task<int> EndMassUpdateModeAsync()
        {
            MassUpdateMode = false;
            if (UnsavedChangesExist)
            {
                UnsavedChangesExist = false;
                return await SaveChangesAsync();
            } else
            {
                return 0;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (MassUpdateMode)
            {
                UnsavedChangesExist = true;
                return 0;
            }
            else
            {
                try
                {
                    return await base.SaveChangesAsync(cancellationToken);
                } catch(Exception ex)
                {
                    return 0;
                }

            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite("Data Source=MyBooks.db");
        }

    }
}
