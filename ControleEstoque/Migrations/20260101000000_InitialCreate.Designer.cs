using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ControleEstoque.Data;

#nullable disable

namespace ControleEstoque.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260101000000_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ControleEstoque.Models.Material", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<DateTime>("AtualizadoEm")
                    .HasColumnType("timestamp with time zone");

                b.Property<DateTime>("CriadoEm")
                    .HasColumnType("timestamp with time zone");

                b.Property<DateTime?>("DataValidade")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Localizacao")
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnType("character varying(300)");

                b.Property<string>("Nome")
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnType("character varying(300)");

                b.Property<bool>("PossuiValidade")
                    .HasColumnType("boolean");

                b.Property<decimal>("PrecoUnitario")
                    .HasPrecision(18, 2)
                    .HasColumnType("numeric(18,2)");

                b.Property<decimal>("Quantidade")
                    .HasPrecision(18, 2)
                    .HasColumnType("numeric(18,2)");

                b.HasKey("Id");

                b.ToTable("Materiais");
            });

            modelBuilder.Entity("ControleEstoque.Models.Saida", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<DateTime>("DataHora")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Identificacao")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)");

                b.Property<int>("MaterialId")
                    .HasColumnType("integer");

                b.Property<string>("NomeRetirante")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("character varying(200)");

                b.Property<decimal>("Quantidade")
                    .HasPrecision(18, 2)
                    .HasColumnType("numeric(18,2)");

                b.Property<int>("SetorId")
                    .HasColumnType("integer");

                b.Property<int>("TipoIdentificacao")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.HasIndex("MaterialId");

                b.HasIndex("SetorId");

                b.ToTable("Saidas");
            });

            modelBuilder.Entity("ControleEstoque.Models.Setor", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CriadoEm")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Nome")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("character varying(200)");

                b.HasKey("Id");

                b.HasIndex("Nome")
                    .IsUnique();

                b.ToTable("Setores");
            });

            modelBuilder.Entity("ControleEstoque.Models.Usuario", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<string>("Login")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)");

                b.Property<string>("SenhaHash")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("character varying(200)");

                b.HasKey("Id");

                b.HasIndex("Login")
                    .IsUnique();

                b.ToTable("Usuarios");
            });

            modelBuilder.Entity("ControleEstoque.Models.Saida", b =>
            {
                b.HasOne("ControleEstoque.Models.Material", "Material")
                    .WithMany("Saidas")
                    .HasForeignKey("MaterialId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("ControleEstoque.Models.Setor", "Setor")
                    .WithMany("Saidas")
                    .HasForeignKey("SetorId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.Navigation("Material");

                b.Navigation("Setor");
            });

            modelBuilder.Entity("ControleEstoque.Models.Material", b =>
            {
                b.Navigation("Saidas");
            });

            modelBuilder.Entity("ControleEstoque.Models.Setor", b =>
            {
                b.Navigation("Saidas");
            });
#pragma warning restore 612, 618
        }
    }
}
