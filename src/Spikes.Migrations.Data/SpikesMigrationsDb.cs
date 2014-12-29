﻿using System.Data.Common;
using System.Data.Entity;
using Spike.Migrations.Model;
using Spikes.Migrations.BaseData;

namespace Spikes.Migrations.Data
{
    public class SpikesMigrationsDb : SpikesMigrationsBaseDb
    {
        public SpikesMigrationsDb()
        {
        }

        public SpikesMigrationsDb(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbSet<Asset> Assets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Main");
        }
    }
}