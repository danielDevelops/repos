﻿using danielDevelops.CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModels
{
    public class Course : IEntity<int>, IModInfo
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual Educator Educator { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; }
    }
}
