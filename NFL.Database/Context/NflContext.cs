using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFL.Database.Models;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace NFL.Database.Context
{
    public class NflContext : DbContext
    {
        public NflContext()
        {
        }

        public NflContext(DbContextOptions<NflContext> options) : base(options)
        {
        }

        public DbSet<Season> Seasons { get; set; }

        public DbSet<Conference> Conferences { get; set; }

        public DbSet<Division> Divisions { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Person> People { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                string conn = "Server=(LocalDb)\\\\MSSQLLocalDB; Database=NflProjections; Trusted_Connection=True";

                options.UseSqlServer(conn);
            }

            base.OnConfiguring(options);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                        .HasOne(g => g.HomeTeam)
                        .WithMany(t => t.HomeGames)
                        .HasForeignKey(g => g.HomeTeamId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Game>()
                        .HasOne(g => g.AwayTeam)
                        .WithMany(t => t.AwayGames)
                        .HasForeignKey(g => g.AwayTeamId)
                        .OnDelete(DeleteBehavior.NoAction);

            var decimalProps = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => (System.Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(decimal));

            foreach (var property in decimalProps)
            {
                property.SetPrecision(2);
                property.SetScale(1);
            }
        }
    }
}
