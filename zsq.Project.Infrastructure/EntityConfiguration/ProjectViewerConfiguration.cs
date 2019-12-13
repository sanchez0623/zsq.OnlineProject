using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using zsq.Project.Domain.AggregatesModel;

namespace zsq.Project.Infrastructure.EntityConfiguration
{
    public class ProjectViewerConfiguration : IEntityTypeConfiguration<ProjectContributor>
    {
        public void Configure(EntityTypeBuilder<ProjectContributor> builder)
        {
            builder.ToTable("ProjectViewers").HasKey(p => p.Id);
        }
    }
}
