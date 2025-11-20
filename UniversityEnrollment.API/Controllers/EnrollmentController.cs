using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityEnrollment.Core.DTOs.EnrollmentDTOs;
using UniversityEnrollment.Core.ServiceContracts;

namespace UniversityEnrollment.API.Controllers
{
    [Authorize(Roles = "Student")]
    public class EnrollmentsController : BaseController
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(EnrollmentDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEnrollment(CreateEnrollmentDTO createEnrollmentDto)
        {
            var result = await _enrollmentService.CreateAsync(createEnrollmentDto);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetEnrollmentById), new { id = result.Value.Id }, result.Value)
                : Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status400BadRequest);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EnrollmentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEnrollmentById(Guid id)
        {
            var result = await _enrollmentService.GetByIdAsync(id);

            return result.IsSuccess
                ? Ok(result.Value)
                : Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status404NotFound);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<EnrollmentDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var result = await _enrollmentService.GetAllAsync();
            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEnrollment(Guid id)
        {
            var result = await _enrollmentService.DeleteAsync(id);

            return result.IsSuccess
                ? NoContent()
                : Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status404NotFound);
        }
    }
}