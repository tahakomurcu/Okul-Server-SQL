using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using App.Api.Data;
using App.Api.Data.Entities;
using App.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public StudentsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }       

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentEntity))]
        public async Task<IActionResult> Get()
        {
            var students = await _dbContext.Students.ToListAsync();
            return Ok(students);
        }

        [HttpGet("{number}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentEntity))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> GetStudent([FromRoute] int number)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.Number == number);

            if (student is null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentEntity))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> NewStudent([FromBody] NewStudent student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var studentNumber = _dbContext.Students.FirstOrDefault(s => s.Number == student.Number);

            if (studentNumber is not null)
            {
                return BadRequest(studentNumber);
            }

            var item = new StudentEntity
            {
                Number = student.Number,
                Name = student.Name,
                Surname = student.Surname,
                Class = student.Class,
                Grade = student.Grade,
            };

            _dbContext.Students.Add(item);

            await _dbContext.SaveChangesAsync();

            return Ok(item);
        }

        [HttpPut("{number}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> ChangeStudent([FromRoute] int number, [FromBody] NewStudent student)
        {
            var existStudent = await _dbContext.Students.FirstOrDefaultAsync(x => x.Number == number);

            if (existStudent is null)
            {
                return NotFound("Böyle bir öğrenci bulunamadı.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Geçersiz veri girdiniz.");
            }

            // 🔎 Güncellenmek istenen numara başka bir öğrenciye ait mi?
            var sameNumberExists = await _dbContext.Students
                .AnyAsync(x => x.Number == student.Number && x.Number != number);

            if (sameNumberExists)
            {
                return BadRequest($"{student.Number} numarası başka bir öğrenciye ait.");
            }

            // ✅ Güncelleme işlemi
            existStudent.Number = student.Number;
            existStudent.Name = student.Name;
            existStudent.Surname = student.Surname;
            existStudent.Class = student.Class;
            existStudent.Grade = student.Grade;

            await _dbContext.SaveChangesAsync();

            return Ok("Öğrenci güncellendi.");
        }


        [HttpDelete("delete/{number}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(string))]
        public async Task<IActionResult> DeleteStudent([FromRoute] int number)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.Number == number);

            if (student is null)
            {
                return NotFound();
            }

            _dbContext.Students.Remove(student);

            await _dbContext.SaveChangesAsync();

            return Ok("Öğrenci silindi.");

        }
    }
}
