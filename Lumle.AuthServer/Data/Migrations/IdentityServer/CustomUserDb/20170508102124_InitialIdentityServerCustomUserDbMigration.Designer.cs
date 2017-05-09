﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Lumle.AuthServer.Data.Contexts;

namespace Lumle.AuthServer.Data.Migrations.IdentityServer.CustomUserDb
{
    [DbContext(typeof(UserDbContext))]
    [Migration("20170508102124_InitialIdentityServerCustomUserDbMigration")]
    partial class InitialIdentityServerCustomUserDbMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Lumle.AuthServer.Data.Entities.CustomUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Email");

                    b.Property<string>("Gender");

                    b.Property<bool>("IsBlocked");

                    b.Property<bool>("IsEmailVerified");

                    b.Property<bool>("IsStaff");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<string>("PasswordSalt")
                        .IsRequired();

                    b.Property<string>("PhoneNo");

                    b.Property<string>("ProfileUrl");

                    b.Property<string>("Provider")
                        .IsRequired();

                    b.Property<string>("SubjectId")
                        .IsRequired();

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("PublicUser_CustomUser");
                });
        }
    }
}
