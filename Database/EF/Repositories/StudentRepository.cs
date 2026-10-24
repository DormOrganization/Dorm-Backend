using DormAPI.Database.EF.Contexts;
using DormAPI.Database.EF.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace DormAPI.Database.EF.Repositories
{
    public class StudentRepository
    {
        private readonly DormContext _context;
        public StudentRepository(DormContext context)
        {
            _context = context;
        }
        public async Task<List<dynamic>> GetUnpaidStudents(int month, int year)
        {


            var students = await _context.RentInstallmentsLedgers
                .AsNoTracking()
                .Where(r => !r.IsPaid && r.DueDate.Month == month && r.DueDate.Year == year)
                .Select(r => new
                {
                    StudentId = r.Contract.StudentId,
                    StudentName = r.Contract.Student.Person.FirstName + " " + r.Contract.Student.Person.LastName,
                    ContractId = r.ContractId,
                    DueDate = r.DueDate,
                    AmountDue = r.Contract.MonthlyAmount
                })
                .ToListAsync();


      

            return students.Cast<dynamic>().ToList();
        }
        public async Task<int> GetTotalStudent()
        {
            int totalStudent = await  _context.Students.AsNoTracking().CountAsync();

            return totalStudent;
        }


        public async Task<List<dynamic>> GetUnsignedRoomStudents()
        {
            var unsignedStudents = await _context.Students.AsNoTracking()
                .Where(s => !s.RoomAllocations.Any())
                .Select(s => new
                {
                    StudentId = s.StudentId,
                    StudentName = s.Person.FirstName + " " + s.Person.LastName,

                })
                .ToListAsync();

            return unsignedStudents.Cast<dynamic>().ToList();
        }


       
    }
}
