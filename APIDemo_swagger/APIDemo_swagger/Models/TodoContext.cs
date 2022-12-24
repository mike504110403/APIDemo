using System;
using System.Collections.Generic;
using APIDemo_swagger.Dtos;
using Microsoft.EntityFrameworkCore;

namespace APIDemo_swagger.Models;

public partial class TodoContext : DbContext
{
    public TodoContext()
    {
    }

    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Division> Divisions { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<JobTitle> JobTitles { get; set; }

    public virtual DbSet<TodoList> TodoLists { get; set; }

    public virtual DbSet<UploadFile> UploadFiles { get; set; }

    public virtual DbSet<TodoListSelectDto> TodoListSelectDto { get; set; } // sql撈Dto物件

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoListSelectDto>().HasNoKey(); // sql撈Dto物件

        modelBuilder.Entity<Division>(entity =>
        {
            entity.HasKey(e => e.DivisionId).HasName("PK__Division__20EFC6A8F99D2514");

            entity.ToTable("Division");

            entity.Property(e => e.DivisionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F114A9BEF50");

            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Division).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_ToTable_1");

            entity.HasOne(d => d.JobTitle).WithMany(p => p.Employees)
                .HasForeignKey(d => d.JobTitleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_ToTable");
        });

        modelBuilder.Entity<JobTitle>(entity =>
        {
            entity.HasKey(e => e.JobTitleId).HasName("PK__JobTitle__35382FE932476E76");

            entity.ToTable("JobTitle");

            entity.Property(e => e.JobTitleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TodoList>(entity =>
        {
            entity.HasKey(e => e.TodoId).HasName("PK__Table__95862552FC49C675");

            entity.ToTable("TodoList");

            entity.Property(e => e.TodoId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.InsertTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.InsertEmployee).WithMany(p => p.TodoListInsertEmployees)
                .HasForeignKey(d => d.InsertEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Todo_ToTable");

            entity.HasOne(d => d.UpdateEmployee).WithMany(p => p.TodoListUpdateEmployees)
                .HasForeignKey(d => d.UpdateEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Todo_ToTable_1");
        });

        modelBuilder.Entity<UploadFile>(entity =>
        {
            entity.HasKey(e => e.UploadFileId).HasName("PK__Table__6F0F98BF69AEBA07");

            entity.ToTable("UploadFile");

            entity.Property(e => e.UploadFileId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Todo).WithMany(p => p.UploadFiles)
                .HasForeignKey(d => d.TodoId)
                //.OnDelete(DeleteBehavior.ClientSetNull) //拔掉才能刪子資料
                .HasConstraintName("FK_File_ToTable");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
