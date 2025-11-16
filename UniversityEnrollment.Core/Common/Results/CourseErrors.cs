using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityEnrollment.Core.Common.Results
{
    public static class CourseErrors
    {
        public static readonly Error CourseNotFound =
    new("Enrollment.CourseNotFound",
        "The specified course does not exist.");

        public static readonly Error CourseAlreadyExists =
            new("Enrollment.CourseAlreadyExists",
                "A course with the specified course code already exists.");
    }
}
