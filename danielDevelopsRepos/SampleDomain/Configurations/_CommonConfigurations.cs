using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDomain.Configurations
{
    public static class CommonConfigurations
    {
        public static void AddModInfoConfiguration<T>(this EntityTypeBuilder<T> entity) 
            where T : class, IModInfo
        {
            entity.Property(t => t.Created).IsRequired();
            entity.Property(t => t.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(t => t.Modified).IsRequired(false);
            entity.Property(t => t.ModifiedBy).HasMaxLength(100).IsRequired(false);
        }
    }
}
