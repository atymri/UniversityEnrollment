using UniversityEnrollment.Core.Common.Results;

namespace UniversityEnrollment.Core.Common.Results
{
    public static class RoleErrors
    {
        public static readonly Error RoleNotFound = new Error(
            "ApplicationRole.NotFound",
            "The specified role was not found."
        );

        public static readonly Error RoleAlreadyExists = new Error(
            "ApplicationRole.AlreadyExists",
            "A role with the specified name already exists."
        );
    }
}
