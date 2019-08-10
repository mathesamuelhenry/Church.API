using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Church.API.Models;

namespace Church.API.Data.DBContext
{
    public partial class IronChurchContext : DbContext
    {
        public IronChurchContext()
        {
        }

        public IronChurchContext(DbContextOptions<IronChurchContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<ColumnValueDesc> ColumnValueDesc { get; set; }
        public virtual DbSet<Contribution> Contribution { get; set; }
        public virtual DbSet<Contributor> Contributor { get; set; }
        public virtual DbSet<ContributorLoan> ContributorLoan { get; set; }
        public virtual DbSet<SeqControl> SeqControl { get; set; }
        public virtual DbSet<TableColumn> TableColumn { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");

                entity.Property(e => e.AccountId)
                    .HasColumnName("account_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AccountEndDate)
                    .HasColumnName("account_end_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.AccountName)
                    .HasColumnName("account_name")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AccountNumber)
                    .HasColumnName("account_number")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.BankName)
                    .HasColumnName("bank_name")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.DateAdded)
                    .HasColumnName("date_added")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateChanged)
                    .HasColumnName("date_changed")
                    .HasColumnType("datetime");

                entity.Property(e => e.InitialBalance)
                    .HasColumnName("initial_balance")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");
            });

            modelBuilder.Entity<ColumnValueDesc>(entity =>
            {
                entity.ToTable("column_value_desc");

                entity.HasIndex(e => e.TableColumnId)
                    .HasName("table_column_id");

                entity.Property(e => e.ColumnValueDescId)
                    .HasColumnName("column_value_desc_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DateAdded)
                    .HasColumnName("date_added")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateChanged)
                    .HasColumnName("date_changed")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.TableColumnId)
                    .HasColumnName("table_column_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasColumnType("varchar(50)");

                entity.HasOne(d => d.TableColumn)
                    .WithMany(p => p.ColumnValueDesc)
                    .HasForeignKey(d => d.TableColumnId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("column_value_desc_ibfk_1");
            });

            modelBuilder.Entity<Contribution>(entity =>
            {
                entity.ToTable("contribution");

                entity.HasIndex(e => e.ContributorId)
                    .HasName("contribution_ibfk_1");

                entity.Property(e => e.ContributionId)
                    .HasColumnName("contribution_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AccountId)
                    .HasColumnName("account_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Category)
                    .HasColumnName("category")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.CheckNumber)
                    .HasColumnName("check_number")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ContributionName)
                    .HasColumnName("contribution_name")
                    .HasColumnType("varchar(60)");

                entity.Property(e => e.ContributorId)
                    .HasColumnName("contributor_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DateAdded)
                    .HasColumnName("date_added")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateChanged)
                    .HasColumnName("date_changed")
                    .HasColumnType("datetime");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasColumnType("text");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.TransactionDate)
                    .HasColumnName("transaction_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.TransactionMode)
                    .HasColumnName("transaction_mode")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TransactionType)
                    .HasColumnName("transaction_type")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Contributor)
                    .WithMany(p => p.Contribution)
                    .HasForeignKey(d => d.ContributorId)
                    .HasConstraintName("contribution_ibfk_1");
            });

            modelBuilder.Entity<Contributor>(entity =>
            {
                entity.ToTable("contributor");

                entity.Property(e => e.ContributorId)
                    .HasColumnName("contributor_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DateAdded)
                    .HasColumnName("date_added")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateChanged)
                    .HasColumnName("date_changed")
                    .HasColumnType("datetime");

                entity.Property(e => e.FamilyName)
                    .HasColumnName("family_name")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");
            });

            modelBuilder.Entity<ContributorLoan>(entity =>
            {
                entity.ToTable("contributor_loan");

                entity.HasIndex(e => e.ContributorId)
                    .HasName("contributor_id");

                entity.Property(e => e.ContributorLoanId)
                    .HasColumnName("contributor_loan_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ContributorId)
                    .HasColumnName("contributor_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DateAdded)
                    .HasColumnName("date_added")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateChanged)
                    .HasColumnName("date_changed")
                    .HasColumnType("datetime");

                entity.Property(e => e.LoanAmount)
                    .HasColumnName("loan_amount")
                    .HasColumnType("decimal(11,2)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Contributor)
                    .WithMany(p => p.ContributorLoan)
                    .HasForeignKey(d => d.ContributorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("contributor_loan_ibfk_1");
            });

            modelBuilder.Entity<SeqControl>(entity =>
            {
                entity.HasKey(e => e.ObjName)
                    .HasName("PRIMARY");

                entity.ToTable("seq_control");

                entity.Property(e => e.ObjName)
                    .HasColumnName("obj_name")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.NextId)
                    .HasColumnName("next_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<TableColumn>(entity =>
            {
                entity.ToTable("table_column");

                entity.Property(e => e.TableColumnId)
                    .HasColumnName("table_column_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ColumnName)
                    .IsRequired()
                    .HasColumnName("column_name")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.DateAdded)
                    .HasColumnName("date_added")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateChanged)
                    .HasColumnName("date_changed")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasColumnName("table_name")
                    .HasColumnType("varchar(50)");
            });
        }
    }
}
