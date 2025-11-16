using UniversityEnrollment.Core.Common.Results;

namespace UniversityEnrollment.Core.Common.Results
{
    public static class RoleErrors
    {
        public static readonly Error RoleNotFound = new Error(
            "Role.NotFound",
            "The specified role was not found."
        );

        public static readonly Error RoleAlreadyExists = new Error(
            "Role.AlreadyExists",
            "A role with the specified name already exists."
        );
    }
}
