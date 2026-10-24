using System.Data;
using DormAPI.DTOs;
using DormAPI.Database.EF.Contexts;
using DormAPI.Database.EF.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace DormAPI.Database.Ado.Repositories
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;
        private readonly DormContext _context;

        public EmployeeRepository(IConfiguration configuration, DormContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }

        //------------------------------
        //      Employee Section
        // ------------------------------
        public async Task<EmployeePaginationResponseDTO> GetEmployeesAsync(int page, int pageSize)
        {
            var response = new EmployeePaginationResponseDTO
            {
                Page = page,
                PageSize = pageSize,
                Employees = new List<EmployeeDTO>()
            };

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAllEmployees", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Page", page);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Handle potential total count column, some pagination SPs return it in each row or as out param
                                // For now we parse it if exists, else default to 0 and calculate later if needed
                                if (response.TotalCount == 0 && Enumerable.Range(0, reader.FieldCount).Any(i => reader.GetName(i).Equals("TotalCount", StringComparison.OrdinalIgnoreCase)))
                                {
                                    response.TotalCount = reader["TotalCount"] != DBNull.Value ? Convert.ToInt32(reader["TotalCount"]) : 0;
                                }

                                var employee = new EmployeeDTO
                                {
                                    EmployeeID = reader["EmployeeID"] != DBNull.Value ? Convert.ToInt32(reader["EmployeeID"]) : 0,
                                    TypeID = reader["TypeID"] != DBNull.Value ? Convert.ToInt32(reader["TypeID"]) : 0,
                                    TypeName = reader["TypeName"]?.ToString() ?? "",
                                    Description = reader["Description"]?.ToString() ?? "",
                                    PersonID = reader["PersonID"] != DBNull.Value ? Convert.ToInt32(reader["PersonID"]) : 0,
                                    FirstName = reader["FirstName"]?.ToString() ?? "",
                                    SecondName = reader["SecondName"]?.ToString() ?? "",
                                    ThirdName = reader["ThirdName"]?.ToString() ?? "",
                                    LastName = reader["LastName"]?.ToString() ?? "",
                                    DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : DateTime.MinValue,
                                    Gender = reader["Gender"] != DBNull.Value && Convert.ToBoolean(reader["Gender"]),
                                    PassportNumber = reader["PassportNumber"]?.ToString() ?? "",
                                    IdentityNumber = reader["IdentityNumber"]?.ToString() ?? "",
                                    NationalityID = reader["NationalityID"] != DBNull.Value ? Convert.ToInt32(reader["NationalityID"]) : 0,
                                    ContactID = reader["ContactID"] != DBNull.Value ? Convert.ToInt32(reader["ContactID"]) : 0,
                                    Email = reader["Email"]?.ToString() ?? "",
                                    PhoneNumber = reader["PhoneNumber"]?.ToString() ?? "",
                                    Address = reader["Address"]?.ToString() ?? "",
                                    EmergencyContactName = reader["EmergencyContactName"]?.ToString() ?? "",
                                    EmergencyContactPhone = reader["EmergencyContactPhone"]?.ToString() ?? "",
                                    RelationshipTypeID = reader["RelationshipTypeID"] != DBNull.Value ? Convert.ToInt32(reader["RelationshipTypeID"]) : 0,
                                    RelationshipName = reader["RelationshipName"]?.ToString() ?? ""
                                };
                                response.Employees.Add(employee);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetEmployeesAsync: {ex.Message}");
                    }
                }
            }
            return response;
        }

        public async Task<bool> AddEmployeeAsync(AddEmployeeDTO dto)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddEmployeeComplete", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@EmployeeTypeID", dto.EmployeeTypeID);

                    command.Parameters.AddWithValue("@FirstName", dto.FirstName);
                    command.Parameters.AddWithValue("@SecondName", (object)dto.SecondName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ThirdName", (object)dto.ThirdName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", dto.LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", dto.DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", dto.Gender);
                    command.Parameters.AddWithValue("@PassportNumber", dto.PassportNumber);
                    command.Parameters.AddWithValue("@IdentityNumber", dto.IdentityNumber);
                    command.Parameters.AddWithValue("@NationalityID", dto.NationalityID);

                    command.Parameters.AddWithValue("@Email", dto.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", dto.PhoneNumber);
                    command.Parameters.AddWithValue("@Address", dto.Address);
                    command.Parameters.AddWithValue("@EmergencyContactName", dto.EmergencyContactName);
                    command.Parameters.AddWithValue("@EmergencyContactPhone", dto.EmergencyContactPhone);
                    command.Parameters.AddWithValue("@RelationshipTypeID", dto.RelationshipTypeID);

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in AddEmployeeAsync: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task<List<EmployeeTypeDTO>> GetEmployeeTypesAsync()
        {
            var types = new List<EmployeeTypeDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT TypeID, TypeName, Description FROM EmployeeTypes", connection))
                {
                    command.CommandType = CommandType.Text;
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                types.Add(new EmployeeTypeDTO
                                {
                                    TypeID = reader["TypeID"] != DBNull.Value ? Convert.ToInt32(reader["TypeID"]) : 0,
                                    TypeName = reader["TypeName"]?.ToString() ?? "",
                                    Description = reader["Description"]?.ToString() ?? ""
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetEmployeeTypesAsync: {ex.Message}");
                    }
                }
            }
            return types;
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in DeleteEmployeeAsync: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task<EmployeeContractDTO> GetActiveEmployeeContractByIdAsync(int employeeId)
        {
            // ... (keep existing implementation)
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetActiveEmployeeContractById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new EmployeeContractDTO
                                {
                                    EmployeeContractId = reader["EmployeeContractId"] != DBNull.Value ? Convert.ToInt32(reader["EmployeeContractId"]) : 0,
                                    EmployeeID = reader["EmployeeID"] != DBNull.Value ? Convert.ToInt32(reader["EmployeeID"]) : 0,
                                    StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : DateTime.MinValue,
                                    EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : DateTime.MinValue,
                                    ShiftId = reader["ShiftId"] != DBNull.Value ? Convert.ToInt32(reader["ShiftId"]) : 0,
                                    Salary = reader["Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Salary"]) : null,
                                    WorkingHours = reader["WorkingHours"] != DBNull.Value ? Convert.ToInt32(reader["WorkingHours"]) : 0,
                                    WorkingDaysId = reader["WorkingDaysId"] != DBNull.Value ? Convert.ToInt32(reader["WorkingDaysId"]) : 0,
                                    Monday = reader["Monday"] != DBNull.Value && Convert.ToBoolean(reader["Monday"]),
                                    Tuesday = reader["Tuesday"] != DBNull.Value && Convert.ToBoolean(reader["Tuesday"]),
                                    Wednesday = reader["Wednesday"] != DBNull.Value && Convert.ToBoolean(reader["Wednesday"]),
                                    Thursday = reader["Thursday"] != DBNull.Value && Convert.ToBoolean(reader["Thursday"]),
                                    Friday = reader["Friday"] != DBNull.Value && Convert.ToBoolean(reader["Friday"]),
                                    Saturday = reader["Saturday"] != DBNull.Value && Convert.ToBoolean(reader["Saturday"]),
                                    Sunday = reader["Sunday"] != DBNull.Value && Convert.ToBoolean(reader["Sunday"]),
                                    AssignedBuildingId = reader["AssignedBuildingId"] != DBNull.Value ? Convert.ToInt32(reader["AssignedBuildingId"]) : 0
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetActiveEmployeeContractByIdAsync: {ex.Message}");
                    }
                }
            }
            return null;
        }

        public async Task<bool> AddEmployeeContractAsync(AddEmployeeContractDTO dto)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddEmployeeContract", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeId", dto.EmployeeId);
                    command.Parameters.AddWithValue("@StartDate", dto.StartDate);
                    command.Parameters.AddWithValue("@EndDate", dto.EndDate);
                    command.Parameters.AddWithValue("@ShiftId", (object)dto.ShiftId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Salary", dto.Salary);
                    command.Parameters.AddWithValue("@WorkingHours", (object)dto.WorkingHours ?? DBNull.Value);
                    command.Parameters.AddWithValue("@WorkingDays", (object)dto.WorkingDays ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AssignedBuildingId", (object)dto.AssignedBuildingId ?? DBNull.Value);

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in AddEmployeeContractAsync: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task<bool> UpdateEmployeeContractAsync(UpdateEmployeeContractDTO dto)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateEmployeeContract", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeContractId", dto.EmployeeContractId);
                    command.Parameters.AddWithValue("@StartDate", dto.StartDate);
                    command.Parameters.AddWithValue("@EndDate", dto.EndDate);
                    command.Parameters.AddWithValue("@ShiftId", (object)dto.ShiftId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Salary", dto.Salary);
                    command.Parameters.AddWithValue("@WorkingHours", (object)dto.WorkingHours ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Monday", dto.Monday);
                    command.Parameters.AddWithValue("@Tuesday", dto.Tuesday);
                    command.Parameters.AddWithValue("@Wednesday", dto.Wednesday);
                    command.Parameters.AddWithValue("@Thursday", dto.Thursday);
                    command.Parameters.AddWithValue("@Friday", dto.Friday);
                    command.Parameters.AddWithValue("@Saturday", dto.Saturday);
                    command.Parameters.AddWithValue("@Sunday", dto.Sunday);
                    command.Parameters.AddWithValue("@AssignedBuildingId", (object)dto.AssignedBuildingId ?? DBNull.Value);

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in UpdateEmployeeContractAsync: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task<List<ShiftDTO>> GetShiftsAsync()
        {
            var shifts = new List<ShiftDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("Select * from shifts", connection))
                {
                    command.CommandType = CommandType.Text;

                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                shifts.Add(new ShiftDTO
                                {
                                    ShiftId = reader["ShiftId"] != DBNull.Value ? Convert.ToInt32(reader["ShiftId"]) : 0,
                                    ShiftName = reader["ShiftName"]?.ToString() ?? "",
                                    
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetShiftsAsync: {ex.Message}");
                    }
                }
            }
            return shifts;
        }
        public async Task<List<EmployeeLeaveDTO>> GetEmployeeLeavesAsync(int employeeId)
        {
            var leaves = new List<EmployeeLeaveDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM ViewEmployeeLeaves WHERE EmployeeId = @EmployeeId ORDER BY CreatedAt DESC";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                leaves.Add(new EmployeeLeaveDTO
                                {
                                    LeaveId = Convert.ToInt32(reader["LeaveId"]),
                                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    LeaveTypeId = Convert.ToInt32(reader["LeaveTypeId"]),
                                    LeaveTypeName = reader["LeaveTypeName"].ToString(),
                                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                                    Reason = reader["Reason"].ToString(),
                                    StatusId = Convert.ToInt32(reader["StatusId"]),
                                    StatusTypeName = reader["StatusTypeName"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetEmployeeLeavesAsync: {ex.Message}");
                    }
                }
            }
            return leaves;
        }

        public async Task<int> AddEmployeeLeaveAsync(AddEmployeeLeaveDTO dto)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddEmployeeLeave", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeId", dto.EmployeeId);
                    command.Parameters.AddWithValue("@LeaveTypeId", dto.LeaveTypeId);
                    command.Parameters.AddWithValue("@StartDate", dto.StartDate);
                    command.Parameters.AddWithValue("@EndDate", dto.EndDate);
                    command.Parameters.AddWithValue("@Reason", dto.Reason);
                    
                    SqlParameter outputParam = new SqlParameter("@Output", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        return (int)outputParam.Value;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in AddEmployeeLeaveAsync: {ex.Message}");
                        return 0;
                    }
                }
            }
        }

        public async Task<List<LeaveTypeDTO>> GetLeaveTypesAsync()
        {
            var leaveTypes = new List<LeaveTypeDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM LeaveTypes";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                leaveTypes.Add(new LeaveTypeDTO
                                {
                                    LeaveTypeID = Convert.ToInt32(reader["LeaveTypeID"]),
                                    LeaveTypeName = reader["LeaveTypeName"].ToString()
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetLeaveTypesAsync: {ex.Message}");
                    }
                }
            }
            return leaveTypes;
        }


        // =============================== 
        //     Employee Overview Section
        // ===============================
        public async Task<Dictionary<string, dynamic>> GetEmployeesStatisticsOverview()
        {
            var result = new Dictionary<string, dynamic>();


            using (var connection = new SqlConnection(_connectionString))
            {

                List<Dictionary<string, dynamic>> listTypesOverview = new List<Dictionary<string, dynamic>>();

                Dictionary<string, dynamic> overview = new Dictionary<string, dynamic>();
                Dictionary<string, dynamic> statis = new Dictionary<string, dynamic>();

                try
                {
                    await connection.OpenAsync();

                    using (var command1 = new SqlCommand("usp_GetEmployeesOverview", connection))
                    {
                        command1.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command1.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    overview[reader.GetName(i)] = reader.GetValue(i);
                                }

                            }
                            result["Overview"] = overview;
                        }
                    }

                    using (var command2 = new SqlCommand("usp_GetEmployeesTypesOverview", connection))
                    {
                        command2.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command2.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                statis["TypeName"] = reader["TypeName"]?.ToString() ?? "";
                                statis["Staff"] = reader["Staff"] ?? 0;
                                listTypesOverview.Add(new Dictionary<string, dynamic>(statis));
                            }

                            result["EmployeesTypesOverview"] = listTypesOverview;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GetEmployeesStatisticsOverview: {ex.Message}");

                }
            }
            return result;
        }



        // =============================
        //      Employee EF Section
        // =============================
        public async Task<int> GetTotalEmployees()
        {
            int totalEmployees = await _context.Employees.CountAsync();

            return totalEmployees;
        }
    }
}
