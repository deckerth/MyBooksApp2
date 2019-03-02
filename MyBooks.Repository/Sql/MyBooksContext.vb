Imports MyBooks.Models
Imports Microsoft.EntityFrameworkCore

Namespace Global.MyBooks.Repository.Sql

    Public Class MyBooksContext
        Inherits DbContext

        'Creates a new MyBooks DbContext
        Public Sub New(options As DbContextOptions(Of MyBooksContext))
            MyBase.New(options)
        End Sub

        Public Property Books As DbSet(Of Book)

        Public Property Keywords As DbSet(Of Keyword)

        Public Property Storages As DbSet(Of Storage)

        Public Property Authors As DbSet(Of Author)

    End Class

End Namespace
