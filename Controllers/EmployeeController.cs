using DormAPI.Database.Ado.Repositories;
using DormAPI.Database.EF.Contexts;
using DormAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DormAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeRepository _repository;
        private readonly DormContext _dormDbContext;

        public EmployeeController(EmployeeRepository repository, DormContext context)
        {
            _repository = repository;
            _dormDbContext = context;
        }

        [HttpGet("get/all/employees")]
        public async Task<ActionResult<EmployeePaginationResponseDTO>> GetEmployees([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            try
            {

                //var response = await _repository.GetEmployeesAsync(page, pageSize);

                var query = _dormDbContext.ViewEmployees
                    .AsNoTracking()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                var response = query.ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("add/employee")]
        public async Task<ActionResult> AddEmployee([FromBody] AddEmployeeDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bool isSuccess = await _repository.AddEmployeeAsync(dto);
                if (isSuccess)
                {
                    return Ok(new { message = "Employee added successfully." });
                }
                return BadRequest("Failed to add employee.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get/all/employee-types")]
        public async Task<ActionResult<List<EmployeeTypeDTO>>> GetEmployeeTypes()
        {
            try
            {
                //var response = await _repository.GetEmployeeTypesAsync();
                var response = await _dormDbContext.EmployeeTypes.Select(et => new
                {
                    et.TypeId,
                    et.TypeName,
                    et.Description
                }).ToListAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/employee/{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            try
            {
                bool isSuccess = await _repository.DeleteEmployeeAsync(id);
                if (isSuccess)
                {
                    return Ok(new { message = "Employee deleted successfully." });
                }
                return NotFound(new { message = "Employee not found or could not be deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("get/contract/active/{employeeId}")]
        public async Task<ActionResult<EmployeeContractDTO>> GetActiveEmployeeContract(int employeeId)
        {
            try
            {
                // var response = await _repository.GetActiveEmployeeContractByIdAsync(employeeId);
                var response = await _dormDbContext.ViewEmployeesContracts
                    .AsNoTracking()
                    .Where(ve => ve.EndDate >= DateOnly.FromDateTime(DateTime.Now) && ve.EmployeeId == employeeId)
                    .FirstOrDefaultAsync();
                if (response != null)
                {
                    return Ok(response);
                }
                return NotFound(new { message = "Active contract not found for this employee." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("add/contract")]
        public async Task<ActionResult> AddEmployeeContract([FromBody] AddEmployeeContractDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                bool isSuccess = await _repository.AddEmployeeContractAsync(dto);
                if (isSuccess)
                {
                    return Ok(new { message = "Employee contract added successfully." });
                }
                return BadRequest("Failed to add employee contract.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("total/employee")]
        public async Task<ActionResult> GetTotalEmployee()
        {
            // Example: return total employees count
            var total = await _dormDbContext.Employees.CountAsync();
            return Ok(new { TotalEmployees = total });
        }


        [HttpPut("update/contract")]
        public async Task<ActionResult> UpdateEmployeeContract([FromBody] UpdateEmployeeContractDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bool isSuccess = await _repository.UpdateEmployeeContractAsync(dto);
                if (isSuccess)
                {
                    return Ok(new { message = "Employee contract updated successfully." });
                }
                return BadRequest("Failed to update employee contract.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get/all/shifts")]
        public async Task<ActionResult<List<ShiftDTO>>> GetShifts()
        {
            try
            {
                var response = await _dormDbContext.Shifts
                    .AsNoTracking()
                    .Select(s => new
                    {
                        s.ShiftId,
                        s.ShiftName

                    }).ToListAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("get/leaves/{employeeId}")]
        public async Task<IActionResult> GetEmployeeLeaves(int employeeId)
        {
            //var leaves = await _repository.GetEmployeeLeavesAsync(employeeId);
            var leaves = await _dormDbContext.ViewEmployeeLeaves
                .AsNoTracking()
                .Where(el => el.EmployeeId == employeeId)
                .ToListAsync();
            return Ok(leaves);
        }

        [HttpPost("add/leave")]
        public async Task<IActionResult> RequestLeave([FromBody] AddEmployeeLeaveDTO dto)
        {
            var leaveId = await _repository.AddEmployeeLeaveAsync(dto);
            if (leaveId > 0)
            {
                return Ok(new { LeaveId = leaveId });
            }
            return BadRequest("Failed to add leave.");
        }


        [HttpPut("refuse/leave/{leaveId}")]
        public async Task<IActionResult> RefuseLeave(int leaveId)
        {
            var leave = await _dormDbContext.EmployeeLeaves.Where(el => el.LeaveId == leaveId && el.StatusId == 1).FirstOrDefaultAsync();
            if (leave == null)
                {
                return NotFound(new { message = "Leave not found." });
            }

            leave.StatusId = 3; // Assuming 3 is the status ID for "refused"

            _dormDbContext.EmployeeLeaves.Update(leave);

            if (await _dormDbContext.SaveChangesAsync() > 0)
            {
                return Ok(new { message = "Leave refused successfully." });
            }
            return NotFound(new { message = "Leave not found or could not be refused." });
        }

        [HttpPut("accept/leave/{leaveId}")]
        public async Task<IActionResult> AcceptLeave(int leaveId)
        {
            var leave = await _dormDbContext.EmployeeLeaves.Where(el => el.LeaveId == leaveId && el.StatusId == 1).FirstOrDefaultAsync();
            if (leave == null)
            {
                return NotFound(new { message = "Leave not found." });
            }

            leave.StatusId = 2; //  2 is the status ID for "Approved"

            _dormDbContext.EmployeeLeaves.Update(leave);

            if (await _dormDbContext.SaveChangesAsync() > 0)
            {
                return Ok(new { message = "Leave Approved successfully." });
            }
            return NotFound(new { message = "Leave not found or could not be refused." });
        }

        [HttpGet("get/leave/types")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLeaveTypes()
        {
            var leaveTypes = await _dormDbContext
                .LeaveTypes
                .AsNoTracking()
                .Select(lt => new
            {
                lt.LeaveTypeId,
                lt.LeaveTypeName,
            }).ToListAsync();

            if(leaveTypes == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "No leave types found.");
            }

            return Ok(leaveTypes);
        }

        [HttpGet("get/status/types")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStatusTypes()
        {
            var statusTypes = await _dormDbContext
                .StatusTypes
                .AsNoTracking()
                .Select(st => new
                {
                    st.StatusTypeId,
                    st.StatusTypeName,
                }).ToListAsync();

            if (statusTypes == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "No status types found.");
            }

            return Ok(statusTypes);
        }


        [HttpGet("get/employees/statistics/overview")]
        public async Task<IActionResult> GetEmployeesStatisticsOverview()
        {

            var data = await _repository.GetEmployeesStatisticsOverview();

            if (data == null)
            {
                return NotFound(new { message = "No statistics data found." });
            }

            return Ok(data);
        }

    }
}
