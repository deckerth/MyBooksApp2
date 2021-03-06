﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyBooks.ContextProvider;

namespace MyBooks.ContextProvider.Migrations
{
    [DbContext(typeof(MyBooksContext))]
    [Migration("20190426121759_NewColumnASIN")]
    partial class NewColumnASIN
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("MyBooks.Models.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("MyBooks.Models.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ASIN");

                    b.Property<string>("Authors");

                    b.Property<string>("BorrowedDate");

                    b.Property<string>("BorrowedTo");

                    b.Property<string>("DLCNo");

                    b.Property<string>("GoogleBooksUrl");

                    b.Property<string>("ISBN");

                    b.Property<string>("Keywords");

                    b.Property<int>("Medium");

                    b.Property<string>("NBACN");

                    b.Property<string>("OCLCNo");

                    b.Property<string>("OriginalTitle");

                    b.Property<string>("Published");

                    b.Property<string>("Storage");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("MyBooks.Models.Keyword", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Keywords");
                });

            modelBuilder.Entity("MyBooks.Models.Storage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Storages");
                });
#pragma warning restore 612, 618
        }
    }
}
