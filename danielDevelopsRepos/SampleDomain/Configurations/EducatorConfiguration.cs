using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDomain.Configurations
{
    public class EducatorConfiguration : IEntityTypeConfiguration<Educator>
    {
        public void Configure(EntityTypeBuilder<Educator> builder)
        {
            builder.AddModInfoConfiguration();
            builder.Property(t => t.FirstName).IsRequired().HasMaxLength(250);
            builder.Property(t => t.LastName).IsRequired().HasMaxLength(250);
        }
    }
}
