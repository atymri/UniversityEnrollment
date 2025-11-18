using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityEnrollment.Core.Common.Results;
using UniversityEnrollment.Core.DTOs.CourseDTOs;
using UniversityEnrollment.Core.ServiceContracts;

namespace UniversityEnrollment.API.Controllers
{
    public class CouorsesController : BaseController
    {

        private readonly ICourseService _courseService;
        public CouorsesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CourseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseService.GetAllAsync();
            return Ok(courses);
        }

        [HttpGet("{courseId:guid}")]
        [ProducesResponseType(typeof(CourseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDTO>> GetCourseById(Guid courseId)
        {
            var result = await _courseService.GetByIdAsync(courseId);
            return result.IsSuccess
                ? Ok(result.Value)
                : Problem(title: result.Error.Code,
                          detail: result.Error.Description,
                          statusCode: StatusCodes.Status404NotFound);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ProducesResponseType(typeof(CourseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CourseDTO>> PostCourse(CreateCourseDTO dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _courseService.CreateAsync(dto);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetCourseById), new { courseId = result.Value.Id }, result.Value)
                : result.Error == CourseErrors.CourseAlreadyExists
                    ? Problem(title: result.Error.Code,
                              detail: result.Error.Description,
                              statusCode: StatusCodes.Status409Conflict)

                    : Problem(title: result.Error.Code,
                              detail: result.Error.Description,
                              statusCode: StatusCodes.Status400BadRequest);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("{courseId:guid}")]
        [ProducesResponseType(typeof(CourseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDTO>> PutCourse(Guid courseId, UpdateCourseDTO dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (courseId != dto.Id)
                return ValidationProblem("Course ID in URL does not match Course ID in the body");

            var result = await _courseService.UpdateAsync(dto);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.Error == CourseErrors.CourseNotFound
                    ? Problem(title: result.Error.Code,
                              detail: result.Error.Description,
                              statusCode: StatusCodes.Status404NotFound)

                    : Problem(title: result.Error.Code,
                              detail: result.Error.Description,
                              statusCode: StatusCodes.Status400BadRequest);
        }

        [Authorize(Roles = "Teacher")]
        [HttpDelete("{courseId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCourse(Guid courseId)
        {
            var result = await _courseService.DeleteAsync(courseId);
            return result.IsSuccess
                ? NoContent()
                : Problem(title: result.Error.Code,
                          detail: result.Error.Description,
                          statusCode: StatusCodes.Status404NotFound);
        }

    }
}
