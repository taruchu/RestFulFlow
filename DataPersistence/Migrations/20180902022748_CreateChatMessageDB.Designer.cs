﻿// <auto-generated />
using System;
using DataPersistence.Services.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataPersistence.Migrations
{
    [DbContext(typeof(SQLDataBaseBoardChatMessage))]
    [Migration("20180902022748_CreateChatMessageDB")]
    partial class CreateChatMessageDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataPersistence.Models.ChatMessage.Channel", b =>
                {
                    b.Property<long>("ChannelID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ChannelName");

                    b.HasKey("ChannelID");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("DataPersistence.Models.ChatMessage.ChatMessage", b =>
                {
                    b.Property<long>("ChatMessageID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChannelID");

                    b.Property<string>("ChatMessageBody");

                    b.Property<DateTime>("CreatedDateTime");

                    b.Property<DateTime>("ModifiedDateTime");

                    b.Property<string>("SenderUserName");

                    b.HasKey("ChatMessageID");

                    b.HasIndex("ChannelID");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("DataPersistence.Models.ChatMessage.ChatMessage", b =>
                {
                    b.HasOne("DataPersistence.Models.ChatMessage.Channel", "Channel")
                        .WithMany("ChatMessages")
                        .HasForeignKey("ChannelID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
