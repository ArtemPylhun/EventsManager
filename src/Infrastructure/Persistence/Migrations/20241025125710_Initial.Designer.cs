﻿// <auto-generated />
using System;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241025125710_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Attendances.Attendance", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_id");

                    b.Property<DateTime>("RegistrationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("registration_date")
                        .HasDefaultValueSql("timezone('utc', now())");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_attendances");

                    b.HasIndex("EventId")
                        .HasDatabaseName("ix_attendances_event_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_attendances_user_id");

                    b.ToTable("attendances", (string)null);
                });

            modelBuilder.Entity("Domain.Categories.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_categories");

                    b.ToTable("categories", (string)null);
                });

            modelBuilder.Entity("Domain.Events.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("category_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("description");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date");

                    b.Property<Guid>("LocationId")
                        .HasColumnType("uuid")
                        .HasColumnName("location_id");

                    b.Property<Guid>("OrganizerId")
                        .HasColumnType("uuid")
                        .HasColumnName("organizer_id");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_date");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_events");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_events_category_id");

                    b.HasIndex("LocationId")
                        .HasDatabaseName("ix_events_location_id");

                    b.HasIndex("OrganizerId")
                        .HasDatabaseName("ix_events_organizer_id");

                    b.ToTable("events", (string)null);
                });

            modelBuilder.Entity("Domain.EventsTags.EventTag", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_id");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uuid")
                        .HasColumnName("tag_id");

                    b.HasKey("Id")
                        .HasName("pk_events_tags");

                    b.HasIndex("EventId")
                        .HasDatabaseName("ix_events_tags_event_id");

                    b.HasIndex("TagId")
                        .HasDatabaseName("ix_events_tags_tag_id");

                    b.ToTable("events_tags", (string)null);
                });

            modelBuilder.Entity("Domain.Locations.Location", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("address");

                    b.Property<int>("Capacity")
                        .HasColumnType("integer")
                        .HasColumnName("capacity");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("city");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("country");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_locations");

                    b.ToTable("locations", (string)null);
                });

            modelBuilder.Entity("Domain.Profiles.Profile", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Address")
                        .HasColumnType("varchar(100)")
                        .HasColumnName("address");

                    b.Property<DateTime?>("BirthDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birth_date")
                        .HasDefaultValueSql("timezone('utc', now())");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("full_name");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(100)")
                        .HasColumnName("phone_number");

                    b.HasKey("Id")
                        .HasName("pk_profiles");

                    b.ToTable("profiles", (string)null);
                });

            modelBuilder.Entity("Domain.Roles.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.ToTable("roles", (string)null);
                });

            modelBuilder.Entity("Domain.Tags.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_tags");

                    b.ToTable("tags", (string)null);
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("email");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("password_hash");

                    b.Property<Guid>("ProfileId")
                        .HasColumnType("uuid")
                        .HasColumnName("profile_id");

                    b.Property<DateTime>("RegisteredOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("registered_on")
                        .HasDefaultValueSql("timezone('utc', now())");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid")
                        .HasColumnName("role_id");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("ProfileId")
                        .IsUnique()
                        .HasDatabaseName("ix_users_profile_id");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_users_role_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Domain.Attendances.Attendance", b =>
                {
                    b.HasOne("Domain.Events.Event", "Event")
                        .WithMany("Attendances")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_attendances_events_id");

                    b.HasOne("Domain.Users.User", "User")
                        .WithMany("Attendances")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_attendances_users_id");

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Events.Event", b =>
                {
                    b.HasOne("Domain.Categories.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_events_categories_id");

                    b.HasOne("Domain.Locations.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_events_locations_id");

                    b.HasOne("Domain.Users.User", "Organizer")
                        .WithMany()
                        .HasForeignKey("OrganizerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_events_users_id");

                    b.Navigation("Category");

                    b.Navigation("Location");

                    b.Navigation("Organizer");
                });

            modelBuilder.Entity("Domain.EventsTags.EventTag", b =>
                {
                    b.HasOne("Domain.Events.Event", "Event")
                        .WithMany("EventsTags")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_events_tags_events_id");

                    b.HasOne("Domain.Tags.Tag", "Tag")
                        .WithMany("EventsTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_events_tags_tags_id");

                    b.Navigation("Event");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.HasOne("Domain.Profiles.Profile", "Profile")
                        .WithOne()
                        .HasForeignKey("Domain.Users.User", "ProfileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_users_profile_id");

                    b.HasOne("Domain.Roles.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_users_roles_id");

                    b.Navigation("Profile");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Domain.Events.Event", b =>
                {
                    b.Navigation("Attendances");

                    b.Navigation("EventsTags");
                });

            modelBuilder.Entity("Domain.Tags.Tag", b =>
                {
                    b.Navigation("EventsTags");
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.Navigation("Attendances");
                });
#pragma warning restore 612, 618
        }
    }
}
