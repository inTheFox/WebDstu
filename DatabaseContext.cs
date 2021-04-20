using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using WebDstu.Models;

namespace WebDstu.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<DSTUSaved> Saved { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=37.140.192.97;user=u1333340_dstubot;password=SECURITY;database=u1333340_dstu;",
                new MariaDbServerVersion(new Version(10, 5, 8))
            );
        }
    }
}
