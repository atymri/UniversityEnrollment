namespace UniversityEnrollment.Core.Common.Results
{
    public static class EnrollmentErrors
    {
        public static readonly Error AlreadyEnrolled =
            new("Enrollment.AlreadyEnrolled",
                "Student is already enrolled in the specified course.");

        public static readonly Error EnrollmentNotFound =
            new("Enrollment.EnrollmentNotFound",
                "The specified enrollment does not exist.");
    }
}
