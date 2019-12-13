using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using zsq.Project.Domain.AggregatesModel;

namespace zsq.Project.Infrastructure.EntityConfiguration
{
    public class ProjectPropertyConfiguration : IEntityTypeConfiguration<ProjectProperty>
    {
        public void Configure(EntityTypeBuilder<ProjectProperty> builder)
        {
            builder.ToTable("ProjectProperty").HasKey(p => new { p.ProjectId, p.Key, p.Value });

            builder.Property(p => p.Key).HasMaxLength(100);
            builder.Property(p => p.Value).HasMaxLength(100);
        }
    }
}
