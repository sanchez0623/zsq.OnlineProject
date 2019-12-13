using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using zsq.Project.Domain.AggregatesModel;

namespace zsq.Project.Infrastructure.EntityConfiguration
{
    public class ProjectVisibleRuleConfiguration : IEntityTypeConfiguration<ProjectVisibleRule>
    {
        public void Configure(EntityTypeBuilder<ProjectVisibleRule> builder)
        {
            builder.ToTable("ProjectVisibleRules").HasKey(p => p.Id);
        }
    }
}
