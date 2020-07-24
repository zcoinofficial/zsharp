﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NBitcoin;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Zsharp.LightweightIndexer.Postgres;

namespace Zsharp.LightweightIndexer.Postgres.Migrations
{
    [DbContext(typeof(DbContext))]
    partial class DbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.Block", b =>
                {
                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<uint256>("Hash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<uint256>("MerkleRoot")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<int>("Nonce")
                        .HasColumnType("integer");

                    b.Property<int>("Target")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Height");

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.BlockTransaction", b =>
                {
                    b.Property<uint256>("BlockHash")
                        .HasColumnType("bytea");

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<uint256>("TransactionHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("BlockHash", "Index");

                    b.HasIndex("TransactionHash");

                    b.ToTable("BlockTransactions");
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.ElysiumTransaction", b =>
                {
                    b.Property<uint256>("TransactionHash")
                        .HasColumnType("bytea");

                    b.Property<string>("Receiver")
                        .HasColumnType("text");

                    b.Property<string>("Sender")
                        .HasColumnType("text");

                    b.Property<byte[]>("Serialized")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("TransactionHash");

                    b.ToTable("ElysiumTransactions");
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.Input", b =>
                {
                    b.Property<uint256>("TransactionHash")
                        .HasColumnType("bytea");

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<uint256>("OutputHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<int>("OutputIndex")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Script")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<int>("Sequence")
                        .HasColumnType("integer");

                    b.HasKey("TransactionHash", "Index");

                    b.ToTable("Inputs");
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.MtpData", b =>
                {
                    b.Property<uint256>("BlockHash")
                        .HasColumnType("bytea");

                    b.Property<uint256>("Hash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<uint256>("Reserved1")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<uint256>("Reserved2")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("BlockHash");

                    b.ToTable("MtpData");
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.Output", b =>
                {
                    b.Property<uint256>("TransactionHash")
                        .HasColumnType("bytea");

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Script")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<long>("Value")
                        .HasColumnType("bigint");

                    b.HasKey("TransactionHash", "Index");

                    b.ToTable("Outputs");
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.Transaction", b =>
                {
                    b.Property<uint256>("Hash")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("ExtraPayload")
                        .HasColumnType("bytea");

                    b.Property<int>("LockTime")
                        .HasColumnType("integer");

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.Property<short>("Version")
                        .HasColumnType("smallint");

                    b.HasKey("Hash");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.BlockTransaction", b =>
                {
                    b.HasOne("Zsharp.LightweightIndexer.Entity.Block", "Block")
                        .WithMany("Transactions")
                        .HasForeignKey("BlockHash")
                        .HasPrincipalKey("Hash")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zsharp.LightweightIndexer.Entity.Transaction", "Transaction")
                        .WithMany("Blocks")
                        .HasForeignKey("TransactionHash")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.ElysiumTransaction", b =>
                {
                    b.HasOne("Zsharp.LightweightIndexer.Entity.Transaction", null)
                        .WithOne("Elysium")
                        .HasForeignKey("Zsharp.LightweightIndexer.Entity.ElysiumTransaction", "TransactionHash")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.Input", b =>
                {
                    b.HasOne("Zsharp.LightweightIndexer.Entity.Transaction", null)
                        .WithMany("Inputs")
                        .HasForeignKey("TransactionHash")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.MtpData", b =>
                {
                    b.HasOne("Zsharp.LightweightIndexer.Entity.Block", null)
                        .WithOne("MtpData")
                        .HasForeignKey("Zsharp.LightweightIndexer.Entity.MtpData", "BlockHash")
                        .HasPrincipalKey("Zsharp.LightweightIndexer.Entity.Block", "Hash")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Zsharp.LightweightIndexer.Entity.Output", b =>
                {
                    b.HasOne("Zsharp.LightweightIndexer.Entity.Transaction", null)
                        .WithMany("Outputs")
                        .HasForeignKey("TransactionHash")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
