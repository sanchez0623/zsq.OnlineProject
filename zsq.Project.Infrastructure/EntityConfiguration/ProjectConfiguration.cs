using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using ProjectEntity = zsq.Project.Domain.AggregatesModel.Project;

namespace zsq.Project.Infrastructure.EntityConfiguration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<ProjectEntity>
    {
        public void Configure(EntityTypeBuilder<ProjectEntity> builder)
        {
            builder.ToTable("Projects").HasKey(p => p.Id);

            //使用mysql的情况下，如果有bool类型的字段，在db中存为bit类型
            //如果没有进行如下设置，则在保存时会报错：
            //InvalidCastException: Unable to cast object of type System.Boolean to type System.Int16
            builder.Property(s => s.ShowSecurityInfo).HasColumnType("bit");
            builder.Property(s => s.OnPlatform).HasColumnType("bit");
        }
    }
}
