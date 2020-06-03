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
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.AddModInfoConfiguration();
            builder.Property(t => t.FirstName).HasMaxLength(250).IsRequired();
            builder.Property(t => t.LastName).HasMaxLength(250).IsRequired();
        }
    }
}
