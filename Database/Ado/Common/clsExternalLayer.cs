using System.Data;
using System.Data.Common;
using DormAPI.Database.Ado.Extensions;
using DormAPI.DTOs;
using DormAPI.Helpers;
using DormAPI.Models;
using Microsoft.Data.SqlClient;

namespace DormAPI.Database.Ado.Common
{
    public class clsExternalLayer
    {
        private readonly string _connectionString;
        public clsExternalLayer(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        //------------------------------
        //      Contract Section
        // ------------------------------
        public bool CreateContract(ContractDTO contractDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_CreateContract", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ContractName", contractDTO.contractName);
                    command.Parameters.AddWithValue("@ContractStartDate", contractDTO.contractStartDate);
                    command.Parameters.AddWithValue("@ContractEndDate", contractDTO.contractEndDate);
                    command.Parameters.AddWithValue("@TotalAmount", contractDTO.totalAmount);
                    command.Parameters.AddWithValue("@AdditionalFees", contractDTO.additionalFees);
                    command.Parameters.AddWithValue("@Discount", contractDTO.discount);
                    command.Parameters.AddWithValue("@MonthlyAmount", contractDTO.monthlyAmount);
                    command.Parameters.AddWithValue("@NumberOfInstallments", contractDTO.numberOfInstallments);
                    command.Parameters.AddWithValue("@StudentID", contractDTO.studentId);
                    int rowsAffected = 0;
                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return rowsAffected > 0;
                }
            }
        }
        public async Task<List<Dictionary<string, dynamic>>> GetStudentContracts( int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetContractsByStudentID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentID", studentId);

                    List<Dictionary<string, dynamic>> contractsList = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var dict = new Dictionary<string, dynamic>();
                                dict["ContractID"] = reader[0];
                                dict["ContractName"] = reader[1].ToString();
                                dict["ContractStartDate"] = reader[2];
                                dict["ContractEndDate"] = reader[3];
                                dict["TotalAmount"] = reader[4];
                                dict["AdditionalFees"] = reader[5];
                                dict["Discount"] = reader[6];
                                dict["MonthlyAmount"] = reader[7];
                                dict["NumberOfInstallments"] = reader[8];
                                dict["StudentID"] = reader[9];
                                contractsList.Add(dict);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return contractsList;
                }
            }
        }


        public async Task<List<Dictionary<string, dynamic>>>  GetStudentInstallments(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetInstallmentsByStudentID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    List<Dictionary<string, dynamic>> installmentsList = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var dict = new Dictionary<string, dynamic>();
                                dict["RentInstallmentID"] = reader[0];
                                dict["ContractID"] = reader[1];
                                dict["InstallmentNumber"] = reader[2];
                                dict["InstallmentAmount"] = reader[3];
                                dict["DueDate"] = reader[4];
                                dict["PaidDate"] = reader[5];
                                dict["IsPaid"] = reader[6];
                                dict["PaymentMethodName"] = reader[7];

                                installmentsList.Add(dict);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return installmentsList;
                }
            }
        }


        public bool PayRent(PaymentDTO paymentDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_PayRent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RentInstallmentID", paymentDTO.rentInstallmentId);
                    command.Parameters.AddWithValue("@PaymentMethodID", paymentDTO.paymentMethodId);
                    command.Parameters.AddWithValue("@Notes", paymentDTO.notes);
                    int rowsAffected = 0;
                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return rowsAffected > 0;
                }
            }
        }
        public async Task<List<Dictionary<string, dynamic>>> GetAllPaymentMethodAsync()
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM PaymentMethod", connection))


                {

                    List<Dictionary<string, dynamic>> methodList = new List<Dictionary<string, dynamic>>();

                    try
                    {
                        await connection.OpenAsync();

                        using(SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var dict = new Dictionary<string, dynamic>();
                                dict["PaymentMethodID"] = reader[0];
                                dict["PaymentMethodName"] = reader[1].ToString();

                                methodList.Add(dict);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }

                    return methodList;
                }
            }
        }

        public async Task<List<Dictionary<string, dynamic>>> GetPaymentHistory(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_PaymentHistory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    List<Dictionary<string, dynamic>> paymentHistory = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var dict = new Dictionary<string, dynamic>();
                                dict["PaymentID"] = reader[0];
                                dict["RentInstallmentID"] = reader[1];
                                dict["PaymentMethodID"] = reader[2];
                                dict["PaymentDate"] = reader[3];
                                //dict["AmountPaid"] = reader[4];
                                dict["Notes"] = reader[4].ToString();
                                paymentHistory.Add(dict);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return paymentHistory;
                }
            }
        }
        
        public async Task<Dictionary<string, dynamic>> GetAccommodationInfoByStudentID(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using(SqlCommand command = new SqlCommand("SP_GetAccommodationInfoByStudentID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@StudentID", studentId);

                    Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();

                    try
                    {
                        await connection.OpenAsync();
                        using(SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                dict["BuildingName"] = reader[0];
                                dict["FloorNumber"] = reader[1];
                                dict["RoomNumber"] = reader[2];
                                dict["BedNumber"] = reader[3];
                            }
                        }
                    }catch(Exception ex)
                    {

                    }

                    return dict;
                }
            }
        }

        // ==========================================
        //              Files Section
        // ==========================================
        public bool UploadFilePath(int studentId, string filePath, string fileName, string? fileType)
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                using(SqlCommand command = new SqlCommand("SP_AddDocumentPath", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@DocumentPath", filePath);
                    command.Parameters.AddWithValue("@DocumentName", fileName);
                    command.Parameters.AddWithValue("@DocumentType", fileType);
                    command.Parameters.AddWithValue("@StudentID", studentId);

                    int rowAffected = 0;
                    try
                    {
                        connection.Open();

                        rowAffected = command.ExecuteNonQuery();
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    return rowAffected > 0;


                }
            }
        }

        public bool UploadEmployeeFilePath(int employeeId, string filePath, string fileName, string? fileType)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddEmployeeDocumentPath", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@DocumentPath", filePath);
                    command.Parameters.AddWithValue("@DocumentName", fileName);
                    command.Parameters.AddWithValue("@DocumentType", fileType);
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    int rowAffected = 0;
                    try
                    {
                        connection.Open();

                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    return rowAffected > 0;


                }
            }
        }
        public List<Dictionary<string, dynamic>> GetAllStudentDocuments(int studentId)
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                using(SqlCommand command = new SqlCommand("SP_GetAllStudentDocuments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.AddWithValue("@StudentID", studentId);

                    List<Dictionary<string, dynamic>> listDocumentsPath = new List<Dictionary<string, dynamic>>();

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dict = new Dictionary<string, dynamic>();
                               
                                     dict["DocumentPathID"]  = reader[0];
                                     dict["DocumentPath"]  = reader[1].ToString();
                                     dict["DocumentName"]  = reader[2].ToString();
                                     dict["DocumentType"]  = reader[3].ToString();
                                     dict["StudentID"]  = reader[4];


                                listDocumentsPath.Add(dict);
        
                            }
                        }
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                    }

                    return listDocumentsPath;
                }

                
            }
        }

        public List<Dictionary<string, dynamic>> GetAllEmployeeDocuments(int employeeId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAllEmployeeDocuments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    List<Dictionary<string, dynamic>> listDocumentsPath = new List<Dictionary<string, dynamic>>();

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dict = new Dictionary<string, dynamic>();

                                dict["DocumentPathID"] = reader[0];
                                dict["DocumentPath"] = reader[1].ToString();
                                dict["DocumentName"] = reader[2].ToString();
                                dict["DocumentType"] = reader[3].ToString();
                                dict["EmployeeID"] = reader[4];


                                listDocumentsPath.Add(dict);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                    }

                    return listDocumentsPath;
                }


            }
        }
        public  async Task<(byte[], string, string)> GetDocumentByPathAsync(string path)
        {
            if(!File.Exists(path))
            {
                return ([], "", "");
            }

            var fileBytes = File.ReadAllBytes(path);

            var contentType = GetContentType.GetContentTypeDocument(path);

            

            return (fileBytes, contentType, path);


        }

        // Students without room allocation
        public List<Dictionary<string, dynamic>> GetStudentsWithoutAllocation()
        {
            var sql = @"SELECT s.*, p.FirstName, p.SecondName, p.ThirdName, p.LastName,
                                 d.DepartmentName, u.university_name
                          FROM Students s
                          INNER JOIN People p ON p.PersonID = s.PersonID
                          INNER JOIN Departments d ON d.DepartmentID = s.DepartmentID
                          INNER JOIN universities u ON u.university_id = s.UniversityID
                          WHERE s.RoomAllocationID IS NULL";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                var list = new List<Dictionary<string, dynamic>>();
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dict = new Dictionary<string, dynamic>();
                            if (reader.HasColumn("StudentID"))
                                dict["StudentID"] = reader.IsDBNull(reader.GetOrdinal("StudentID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("StudentID"));
                            if (reader.HasColumn("StudentNumber"))
                                dict["StudentNumber"] = reader.IsDBNull(reader.GetOrdinal("StudentNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("StudentNumber"));
                            if (reader.HasColumn("PersonID"))
                                dict["PersonID"] = reader.IsDBNull(reader.GetOrdinal("PersonID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PersonID"));
                            if (reader.HasColumn("DepartmentID"))
                                dict["DepartmentID"] = reader.IsDBNull(reader.GetOrdinal("DepartmentID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("DepartmentID"));
                            if (reader.HasColumn("UniversityID"))
                                dict["UniversityID"] = reader.IsDBNull(reader.GetOrdinal("UniversityID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UniversityID"));
                            if (reader.HasColumn("YearOfStudy"))
                                dict["YearOfStudy"] = reader.IsDBNull(reader.GetOrdinal("YearOfStudy")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("YearOfStudy"));
                            if (reader.HasColumn("StartedDate"))
                                dict["StartedDate"] = reader.IsDBNull(reader.GetOrdinal("StartedDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("StartedDate"));
                            if (reader.HasColumn("EndDate"))
                                dict["EndDate"] = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndDate"));

                            // person fields
                            if (reader.HasColumn("FirstName"))
                                dict["FirstName"] = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FirstName"));
                            if (reader.HasColumn("SecondName"))
                                dict["SecondName"] = reader.IsDBNull(reader.GetOrdinal("SecondName")) ? string.Empty : reader.GetString(reader.GetOrdinal("SecondName"));
                            if (reader.HasColumn("ThirdName"))
                                dict["ThirdName"] = reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? string.Empty : reader.GetString(reader.GetOrdinal("ThirdName"));
                            if (reader.HasColumn("LastName"))
                                dict["LastName"] = reader.IsDBNull(reader.GetOrdinal("LastName")) ? string.Empty : reader.GetString(reader.GetOrdinal("LastName"));

                            // department and university
                            if (reader.HasColumn("DepartmentName"))
                                dict["DepartmentName"] = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? string.Empty : reader.GetString(reader.GetOrdinal("DepartmentName"));
                            if (reader.HasColumn("university_name"))
                                dict["UniversityName"] = reader.IsDBNull(reader.GetOrdinal("university_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("university_name"));

                            list.Add(dict);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return list;
            }
        }

        // Assign student to a specific room bed
        public bool AssignStudentToRoom(AssignStudentToRoomDTO dto, out string? errorMessage)
        {
            errorMessage = null;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_AssignStudentToRoom", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@StudentID", dto.StudentID);
                command.Parameters.AddWithValue("@RoomID", dto.RoomID);
                command.Parameters.AddWithValue("@RoomBedID", dto.RoomBedID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex)
                {
                    // map our THROW error numbers to friendly messages
                    if (ex.Number == 50001)
                        errorMessage = "Student is already assigned to a room.";
                    else if (ex.Number == 50002)
                        errorMessage = "The room has no available beds.";
                    else
                        errorMessage = ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }
        }

        // Remvoe student from the room 
        public bool RemoveStudentFromRoom(int studentId, out string? errorMessage) 
        {
            errorMessage = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using(SqlCommand command = new SqlCommand("SP_RemoveStudentFromRoom", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentID", studentId);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        return true;
                    }
                    catch (SqlException ex)

                    {
                        // map our THROW error numbers to friendly messages
                        if (ex.Number == 50001)
                            errorMessage = "Student does not exist.";
                        else if (ex.Number == 50002)
                            errorMessage = "Student is not assigned to any room.";
                        else if (ex.Number == 50003)
                            errorMessage = "Room allocation does not exist or is invalid.";
                        else
                            errorMessage = ex.Message;
                        return false;
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        return false;
                    }
                }
            }
        }

        // Room details by RoomID (allocations)
        public List<Dictionary<string, dynamic>> GetRoomDetailsById(int floorID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetRoomDetailsByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    // SP was altered to accept parameter named @FloorID (maps to RoomID in query)
                    command.Parameters.AddWithValue("@FloorID", floorID);

                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dict = new Dictionary<string, dynamic>();

                                // RoomAllocation columns
                                if (reader.HasColumn("RoomAllocationID"))
                                    dict["RoomAllocationID"] = reader.IsDBNull(reader.GetOrdinal("RoomAllocationID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RoomAllocationID"));
                                if (reader.HasColumn("RoomID"))
                                    dict["RoomID"] = reader.IsDBNull(reader.GetOrdinal("RoomID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RoomID"));
                                if (reader.HasColumn("BedNumber"))
                                    dict["BedNumber"] = reader.IsDBNull(reader.GetOrdinal("BedNumber")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BedNumber"));
                                if (reader.HasColumn("StudentID"))
                                    dict["StudentID"] = reader.IsDBNull(reader.GetOrdinal("StudentID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("StudentID"));

                                // Room columns (joined)
                                if (reader.HasColumn("RoomNumber"))
                                    dict["RoomNumber"] = reader.IsDBNull(reader.GetOrdinal("RoomNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("RoomNumber"));
                                if (reader.HasColumn("FloorID"))
                                    dict["FloorID"] = reader.IsDBNull(reader.GetOrdinal("FloorID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("FloorID"));
                                if (reader.HasColumn("Capacity"))
                                    dict["Capacity"] = reader.IsDBNull(reader.GetOrdinal("Capacity")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Capacity"));
                                if (reader.HasColumn("AvailableBeds"))
                                    dict["AvailableBeds"] = reader.IsDBNull(reader.GetOrdinal("AvailableBeds")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AvailableBeds"));
                                if (reader.HasColumn("RoomStatusID"))
                                    dict["RoomStatusID"] = reader.IsDBNull(reader.GetOrdinal("RoomStatusID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RoomStatusID"));

                                // Student and person info (optional)
                                if (reader.HasColumn("StudentNumber"))
                                    dict["StudentNumber"] = reader.IsDBNull(reader.GetOrdinal("StudentNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("StudentNumber"));
                                if (reader.HasColumn("FirstName"))
                                    dict["FirstName"] = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FirstName"));
                                if (reader.HasColumn("SecondName"))
                                    dict["SecondName"] = reader.IsDBNull(reader.GetOrdinal("SecondName")) ? string.Empty : reader.GetString(reader.GetOrdinal("SecondName"));
                                if (reader.HasColumn("ThirdName"))
                                    dict["ThirdName"] = reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? string.Empty : reader.GetString(reader.GetOrdinal("ThirdName"));
                                if (reader.HasColumn("LastName"))
                                    dict["LastName"] = reader.IsDBNull(reader.GetOrdinal("LastName")) ? string.Empty : reader.GetString(reader.GetOrdinal("LastName"));

                                listDict.Add(dict);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }

        // Student counts overview (single row)
        public Dictionary<string, dynamic> GetStudentCountsOverview()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetStudentCountsOverview", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                var dict = new Dictionary<string, dynamic>();
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dict["TotalStudents"] = reader.IsDBNull(reader.GetOrdinal("TotalStudents")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalStudents"));
                            dict["ActiveStudents"] = reader.IsDBNull(reader.GetOrdinal("ActiveStudents")) ? 0 : reader.GetInt32(reader.GetOrdinal("ActiveStudents"));
                            dict["StudentsWithoutContract"] = reader.IsDBNull(reader.GetOrdinal("StudentsWithoutContract")) ? 0 : reader.GetInt32(reader.GetOrdinal("StudentsWithoutContract"));
                            dict["UnpaidInstallmentsTotal"] = reader.IsDBNull(reader.GetOrdinal("UnpaidInstallmentsTotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("UnpaidInstallmentsTotal"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return dict;
            }
        }

        // Dashboard Overview
        public DashboardOverviewDTO GetDashboardOverview()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetDashboardOverview", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                var overview = new DashboardOverviewDTO();
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            overview.TotalStudents = reader.IsDBNull(reader.GetOrdinal("TotalStudents")) ? 0 : Convert.ToInt32(reader["TotalStudents"]);
                            overview.TotalBeds = reader.IsDBNull(reader.GetOrdinal("TotalBeds")) ? 0 : Convert.ToInt32(reader["TotalBeds"]);
                            overview.StudentsWithoutRoom = reader.IsDBNull(reader.GetOrdinal("StudentsWithoutRoom")) ? 0 : Convert.ToInt32(reader["StudentsWithoutRoom"]);
                            overview.OccupiedBeds = reader.IsDBNull(reader.GetOrdinal("OccupiedBeds")) ? 0 : Convert.ToInt32(reader["OccupiedBeds"]);
                            overview.AvailableBeds = reader.IsDBNull(reader.GetOrdinal("AvailableBeds")) ? 0 : Convert.ToInt32(reader["AvailableBeds"]);
                            overview.TotalBuildings = reader.IsDBNull(reader.GetOrdinal("TotalBuildings")) ? 0 : Convert.ToInt32(reader["TotalBuildings"]);
                            overview.TotalRooms = reader.IsDBNull(reader.GetOrdinal("TotalRooms")) ? 0 : Convert.ToInt32(reader["TotalRooms"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GetDashboardOverview: {ex.Message}");
                }
                return overview;
            }
        }

        public Dictionary<string, dynamic> GetAllBuildingsStatistics()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM ViewBuildingsStatistics", connection))
            {

                var dict = new Dictionary<string, dynamic>();


                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            
                            if (reader.HasColumn("TotalBuildings"))
                                dict["TotalBuildings"] = reader.IsDBNull(reader.GetOrdinal("TotalBuildings")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TotalBuildings"));
                            if (reader.HasColumn("TotalFloors"))
                                dict["TotalFloors"] = reader.IsDBNull(reader.GetOrdinal("TotalFloors")) ? string.Empty : reader.GetInt32(reader.GetOrdinal("TotalFloors"));
                            if (reader.HasColumn("TotalRooms"))
                                dict["TotalRooms"] = reader.IsDBNull(reader.GetOrdinal("TotalRooms")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalRooms"));
                            if (reader.HasColumn("TotalBeds"))
                                dict["TotalBeds"] = reader.IsDBNull(reader.GetOrdinal("TotalBeds")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalBeds"));
                            if (reader.HasColumn("OccupiedBeds"))
                                dict["OccupiedBeds"] = reader.IsDBNull(reader.GetOrdinal("OccupiedBeds")) ? 0 : reader.GetInt32(reader.GetOrdinal("OccupiedBeds"));
                            if (reader.HasColumn("AvailableBeds"))
                                dict["AvailableBeds"] = reader.IsDBNull(reader.GetOrdinal("AvailableBeds")) ? 0 : reader.GetInt32(reader.GetOrdinal("AvailableBeds"));



                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return dict;
            }
        }

        // Building statistics (optionally for a specific building)
        public List<Dictionary<string, dynamic>> GetBuildingStatistics(int? buildingId = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_GetBuildingStatistics", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@BuildingID", (object?)buildingId ?? DBNull.Value);

                var list = new List<Dictionary<string, dynamic>>();
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dict = new Dictionary<string, dynamic>();
                            if (reader.HasColumn("BuildingID"))
                                dict["BuildingID"] = reader.IsDBNull(reader.GetOrdinal("BuildingID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BuildingID"));
                            if (reader.HasColumn("BuildingName"))
                                dict["BuildingName"] = reader.IsDBNull(reader.GetOrdinal("BuildingName")) ? string.Empty : reader.GetString(reader.GetOrdinal("BuildingName"));
                            if (reader.HasColumn("FloorCount"))
                                dict["FloorCount"] = reader.IsDBNull(reader.GetOrdinal("FloorCount")) ? 0 : reader.GetInt32(reader.GetOrdinal("FloorCount"));
                            if (reader.HasColumn("RoomCount"))
                                dict["RoomCount"] = reader.IsDBNull(reader.GetOrdinal("RoomCount")) ? 0 : reader.GetInt32(reader.GetOrdinal("RoomCount"));
                            if (reader.HasColumn("TotalCapacity"))
                                dict["TotalCapacity"] = reader.IsDBNull(reader.GetOrdinal("TotalCapacity")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalCapacity"));
                            if (reader.HasColumn("OccupiedBeds"))
                                dict["OccupiedBeds"] = reader.IsDBNull(reader.GetOrdinal("OccupiedBeds")) ? 0 : reader.GetInt32(reader.GetOrdinal("OccupiedBeds"));
                            if (reader.HasColumn("AvailableBeds_ByCapacity"))
                                dict["AvailableBeds_ByCapacity"] = reader.IsDBNull(reader.GetOrdinal("AvailableBeds_ByCapacity")) ? 0 : reader.GetInt32(reader.GetOrdinal("AvailableBeds_ByCapacity"));
                            if (reader.HasColumn("AvailableBeds_Column"))
                                dict["AvailableBeds_Column"] = reader.IsDBNull(reader.GetOrdinal("AvailableBeds_Column")) ? 0 : reader.GetInt32(reader.GetOrdinal("AvailableBeds_Column"));

                            list.Add(dict);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return list;
            }
        }

        public bool UpdateFullStudent(UpdateFullStudentDTO dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_UpdateFullStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentID", dto.StudentID);

                    // Student params
                    command.Parameters.AddWithValue("@StudentNumber", dto.StudentNumber);
                    command.Parameters.AddWithValue("@StartedDate", dto.StartedDate);
                    command.Parameters.AddWithValue("@EndDate", dto.EndDate);
                    command.Parameters.AddWithValue("@YearOfStudy", dto.YearOfStudy);
                    command.Parameters.AddWithValue("@UniversityID", dto.UniversityID);
                    command.Parameters.AddWithValue("@FacultyID", dto.FacultyID);
                    command.Parameters.AddWithValue("@DepartmentID", dto.DepartmentID);

                    // Person params
                    command.Parameters.AddWithValue("@FirstName", dto.FirstName);
                    command.Parameters.AddWithValue("@SecondName", (object?)dto.SecondName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ThirdName", (object?)dto.ThirdName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", dto.LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", dto.DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", dto.Gender);
                    command.Parameters.AddWithValue("@PassportNumber", dto.PassportNumber);
                    command.Parameters.AddWithValue("@IdentityNumber", dto.IdentityNumber);
                    command.Parameters.AddWithValue("@NationalityID", (object?)dto.NationalityID ?? DBNull.Value);

                    // Contact params
                    command.Parameters.AddWithValue("@Email", dto.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", dto.PhoneNumber);
                    command.Parameters.AddWithValue("@Address", (object?)dto.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EmergencyContactName", (object?)dto.EmergencyContactName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EmergencyContactPhone", (object?)dto.EmergencyContactPhone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RelationshipTypeID", (object?)dto.RelationshipTypeID ?? DBNull.Value);

                    try
                    {
                        // capture stored procedure return value because SP sets NOCOUNT ON
                        var returnParam = new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
                        command.Parameters.Add(returnParam);

                        connection.Open();
                        command.ExecuteNonQuery();

                        var ret = returnParam.Value == DBNull.Value ? -1 : Convert.ToInt32(returnParam.Value);
                        // SP returns 0 for success, -1 for error
                        return ret == 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error executing SP_UpdateFullStudent: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        // Education levels
        public List<Dictionary<string, dynamic>> GetEducationLevels()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetEducationLevels", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // helper local function to check column existence
                                // (can't declare nested methods here easily) use extension method HasColumn
                                int levelId = reader.GetInt32(reader.GetOrdinal("LevelID"));
                                string levelName = reader.GetString(reader.GetOrdinal("LevelName"));
                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "LevelID", levelId },
                                    { "LevelName", levelName }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }

        // Floors with Rooms for a specific building
        public List<Dictionary<string, dynamic>> GetFloorsWithRooms(int buildingId)
        {
            var sql = @"SELECT f.FloorID, f.BuildingID, f.FloorNumber,
                               r.RoomID, r.RoomNumber, r.Capacity, r.AvailableBeds, r.RoomStatusID
                        FROM Floor f
                        INNER JOIN Room r ON r.FloorID = f.FloorID
                        WHERE f.BuildingID = @BuildingID
                        ORDER BY f.FloorID, r.RoomID";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@BuildingID", buildingId);

                var floors = new Dictionary<int, Dictionary<string, dynamic>>();
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int floorId = reader.GetInt32(reader.GetOrdinal("FloorID"));
                            if (!floors.TryGetValue(floorId, out var floorDict))
                            {
                                floorDict = new Dictionary<string, dynamic>
                                {
                                    { "FloorID", floorId },
                                    { "BuildingID", reader.GetInt32(reader.GetOrdinal("BuildingID")) },
                                    { "FloorNumber", reader.GetInt32(reader.GetOrdinal("FloorNumber")) },
                                    { "Rooms", new List<Dictionary<string, dynamic>>() }
                                };
                                floors[floorId] = floorDict;
                            }

                            var rooms = (List<Dictionary<string, dynamic>>)floorDict["Rooms"];
                            var room = new Dictionary<string, dynamic>()
                            {
                                { "RoomID", reader.GetInt32(reader.GetOrdinal("RoomID")) },
                                { "RoomNumber", reader.IsDBNull(reader.GetOrdinal("RoomNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("RoomNumber")) },
                                { "Capacity", reader.IsDBNull(reader.GetOrdinal("Capacity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Capacity")) },
                                { "AvailableBeds", reader.IsDBNull(reader.GetOrdinal("AvailableBeds")) ? 0 : reader.GetInt32(reader.GetOrdinal("AvailableBeds")) },
                                { "RoomStatusID", reader.IsDBNull(reader.GetOrdinal("RoomStatusID")) ? 0 : reader.GetInt32(reader.GetOrdinal("RoomStatusID")) }
                            };

                            rooms.Add(room);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                return floors.Values.ToList();
            }
        }

        // View: Building details (floors, rooms, capacity, available, occupied)
        public List<Dictionary<string, dynamic>> GetBuildingDetails()
        {
            var sql = @"SELECT
    b.BuildingID,
    b.BuildingName,

    COUNT(DISTINCT f.FloorID) AS TotalFloors,

    COUNT(DISTINCT r.RoomID) AS TotalRooms,

    COUNT(ra.RoomAllocationID) AS TotalCapacity,

    COUNT(CASE
        WHEN ra.RoomAllocationID IS NOT NULL
         AND ra.StudentID IS NULL
        THEN 1
    END) AS Available,

    COUNT(CASE
        WHEN ra.RoomAllocationID IS NOT NULL
         AND ra.StudentID IS NOT NULL
        THEN 1
    END) AS Occupied

FROM Building b

LEFT JOIN Floor f
    ON f.BuildingID = b.BuildingID

LEFT JOIN Room r
    ON r.FloorID = f.FloorID

LEFT JOIN RoomAllocation ra
    ON ra.RoomID = r.RoomID

GROUP BY
    b.BuildingID,
    b.BuildingName;";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var buildingId = reader.GetInt32(reader.GetOrdinal("BuildingID"));
                                var buildingName = reader.IsDBNull(reader.GetOrdinal("BuildingName")) ? string.Empty : reader.GetString(reader.GetOrdinal("BuildingName"));
                                var totalFloors = reader.IsDBNull(reader.GetOrdinal("TotalFloors")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalFloors"));
                                var totalRooms = reader.IsDBNull(reader.GetOrdinal("TotalRooms")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalRooms"));
                                var totalCapacity = reader.IsDBNull(reader.GetOrdinal("TotalCapacity")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalCapacity"));
                                var available = reader.IsDBNull(reader.GetOrdinal("Available")) ? 0 : reader.GetInt32(reader.GetOrdinal("Available"));
                                var occupied = reader.IsDBNull(reader.GetOrdinal("Occupied")) ? 0 : reader.GetInt32(reader.GetOrdinal("Occupied"));

                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "BuildingID", buildingId },
                                    { "BuildingName", buildingName },
                                    { "TotalFloors", totalFloors },
                                    { "TotalRooms", totalRooms },
                                    { "TotalCapacity", totalCapacity },
                                    { "Available", available },
                                    { "Occupied", occupied }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }

        // Relationship Types
        public List<Dictionary<string, dynamic>> GetRelationshipTypes()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetRelationshipTypes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("RelationshipTypeID"));
                                string name = reader.IsDBNull(reader.GetOrdinal("RelationshipName")) ? string.Empty : reader.GetString(reader.GetOrdinal("RelationshipName"));
                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "RelationshipTypeID", id },
                                    { "RelationshipName", name }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }

        // Universities
        public List<Dictionary<string, dynamic>> GetUniversities()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetUniversities", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("university_id"));
                                string name = reader.IsDBNull(reader.GetOrdinal("university_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("university_name"));
                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "UniversityID", id },
                                    { "UniversityName", name }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }

        // Faculties
        public List<Dictionary<string, dynamic>> GetFaculties()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetFaculties", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("FacultyID"));
                                string name = reader.IsDBNull(reader.GetOrdinal("FacultyName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FacultyName"));
                                string code = reader.IsDBNull(reader.GetOrdinal("FacultyCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("FacultyCode"));
                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "FacultyID", id },
                                    { "FacultyName", name },
                                    { "FacultyCode", code }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }

        // Departments
        public List<Dictionary<string, dynamic>> GetDepartments()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetDepartments", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("DepartmentID"));
                                string name = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? string.Empty : reader.GetString(reader.GetOrdinal("DepartmentName"));
                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "DepartmentID", id },
                                    { "DepartmentName", name }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }

        // Nationalities
        public List<Dictionary<string, dynamic>> GetNationalities()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetNationalities", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int nationalityId = reader.GetInt32(reader.GetOrdinal("NationalityID"));
                                string country = reader.IsDBNull(reader.GetOrdinal("Country")) ? string.Empty : reader.GetString(reader.GetOrdinal("Country"));
                                string nationality = reader.IsDBNull(reader.GetOrdinal("Nationality")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nationality"));
                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "NationalityID", nationalityId },
                                    { "Country", country },
                                    { "Nationality", nationality }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }

        public (bool Success, int NewStudentId, int NewPersonId, int NewContactId) AddFullStudent(AddFullStudentDTO dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_AddFullStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Contact
                    command.Parameters.AddWithValue("@Email", dto.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", dto.PhoneNumber);
                    command.Parameters.AddWithValue("@Address", (object?)dto.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EmergencyContactName", (object?)dto.EmergencyContactName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EmergencyContactPhone", (object?)dto.EmergencyContactPhone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RelationshipTypeID", (object?)dto.RelationshipTypeID ?? DBNull.Value);

                    // Person
                    command.Parameters.AddWithValue("@FirstName", dto.FirstName);
                    command.Parameters.AddWithValue("@SecondName", (object?)dto.SecondName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ThirdName", (object?)dto.ThirdName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", dto.LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", dto.DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", dto.Gender);
                    command.Parameters.AddWithValue("@PassportNumber", dto.PassportNumber);
                    command.Parameters.AddWithValue("@IdentityNumber", dto.IdentityNumber);
                    command.Parameters.AddWithValue("@NationalityID", (object?)dto.NationalityID ?? DBNull.Value);

                    // Student
                    command.Parameters.AddWithValue("@StudentNumber", dto.StudentNumber);
                    command.Parameters.AddWithValue("@StartedDate", dto.StartedDate);
                    command.Parameters.AddWithValue("@EndDate", dto.EndDate);
                    command.Parameters.AddWithValue("@YearOfStudy", dto.YearOfStudy);
                    command.Parameters.AddWithValue("@UniversityID", dto.UniversityID);
                    command.Parameters.AddWithValue("@FacultyID", dto.FacultyID);
                    command.Parameters.AddWithValue("@DepartmentID", dto.DepartmentID);
                    command.Parameters.AddWithValue("@EducationLevelID", dto.EducationLevelID);

                    // Outputs
                    var outContact = new SqlParameter("@NewContactID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var outPerson = new SqlParameter("@NewPersonID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    var outStudent = new SqlParameter("@NewStudentID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(outContact);
                    command.Parameters.Add(outPerson);
                    command.Parameters.Add(outStudent);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        int newContactId = outContact.Value != DBNull.Value ? Convert.ToInt32(outContact.Value) : 0;
                        int newPersonId = outPerson.Value != DBNull.Value ? Convert.ToInt32(outPerson.Value) : 0;
                        int newStudentId = outStudent.Value != DBNull.Value ? Convert.ToInt32(outStudent.Value) : 0;
                        return (true, newStudentId, newPersonId, newContactId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error executing SP_AddFullStudent: {ex.Message}");
                        return (false, -1, -1, -1);
                    }
                }
            }
        }

        // Student Section/Operations
        public List<Student> GetAllStudents(int page, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetAllStudents", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Page", page);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    var list = new List<Student>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var student = new Student();

                                student.studentId = reader.GetInt32(reader.GetOrdinal("StudentID"));
                                student.StudentNumber = reader.GetString(reader.GetOrdinal("StudentNumber"));
                                student.StartedDate = reader.GetDateTime(reader.GetOrdinal("StartedDate"));
                                student.EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                                student.YearOfStudy = reader.GetInt32(reader.GetOrdinal("YearOfStudy"));
                                student.UniversityID = reader.IsDBNull(reader.GetOrdinal("UniversityID")) ? 0 : reader.GetInt32(reader.GetOrdinal("UniversityID"));
                                student.UniversityName =  reader.GetString(reader.GetOrdinal("university_name"));
                                student.FacultyID = reader.IsDBNull(reader.GetOrdinal("FacultyID")) ? 0 : reader.GetInt32(reader.GetOrdinal("FacultyID"));
                                student.FacultyName = reader.IsDBNull(reader.GetOrdinal("FacultyName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FacultyName"));
                                student.DepartmentID = reader.IsDBNull(reader.GetOrdinal("DepartmentID")) ? 0 : reader.GetInt32(reader.GetOrdinal("DepartmentID"));
                                student.DepartmentName = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? string.Empty : reader.GetString(reader.GetOrdinal("DepartmentName"));
                                student.EducationLevelID = reader.IsDBNull(reader.GetOrdinal("EducationLevelID")) ? 0 : reader.GetInt32(reader.GetOrdinal("EducationLevelID"));
                                student.EducationLevelName = reader.IsDBNull(reader.GetOrdinal("LevelName")) ? string.Empty : reader.GetString(reader.GetOrdinal("LevelName"));

                                // person fields
                                student.personId = reader.GetInt32(reader.GetOrdinal("PersonID"));
                                student.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                student.SecondName = reader.IsDBNull(reader.GetOrdinal("SecondName")) ? null : reader.GetString(reader.GetOrdinal("SecondName"));
                                student.ThirdName = reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? null : reader.GetString(reader.GetOrdinal("ThirdName"));
                                student.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                                student.DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth"));
                                student.Gender = reader.GetBoolean(reader.GetOrdinal("Gender"));
                                student.PassportNumber = reader.IsDBNull(reader.GetOrdinal("PassportNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("PassportNumber"));
                                student.IdentityNumber = reader.IsDBNull(reader.GetOrdinal("IdentityNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("IdentityNumber"));
                                student.NationalityID = reader.IsDBNull(reader.GetOrdinal("NationalityID")) ? null : reader.GetInt32(reader.GetOrdinal("NationalityID"));
                                student.Nationality =  reader.GetString(reader.GetOrdinal("Nationality"));
                                student.ContactID = reader.GetInt32(reader.GetOrdinal("ContactID"));

                                // Contact
                                var contact = new Contact();
                                contact.contactId = student.ContactID;
                                contact.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email"));
                                contact.PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("PhoneNumber"));
                                contact.Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address"));
                                contact.EmergencyContactName = reader.IsDBNull(reader.GetOrdinal("EmergencyContactName")) ? null : reader.GetString(reader.GetOrdinal("EmergencyContactName"));
                                contact.EmergencyContactPhone = reader.IsDBNull(reader.GetOrdinal("EmergencyContactPhone")) ? null : reader.GetString(reader.GetOrdinal("EmergencyContactPhone"));

                                contact.RelationshipTypeID = reader.IsDBNull(reader.GetOrdinal("RelationshipTypeID")) ? null : reader.GetInt32(reader.GetOrdinal("RelationshipTypeID"));
                                // optional relationship name column
                                if (reader.HasColumn("RelationshipName"))
                                {
                                    contact.RelationshipType = reader.IsDBNull(reader.GetOrdinal("RelationshipName")) ? null : reader.GetString(reader.GetOrdinal("RelationshipName"));
                                }
                                else if (reader.HasColumn("RelationshipType") )
                                {
                                    contact.RelationshipType = reader.IsDBNull(reader.GetOrdinal("RelationshipType")) ? null : reader.GetString(reader.GetOrdinal("RelationshipType"));
                                }

                                student.Contact = contact;

                                list.Add(student);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return list;
                }
            }
        }

        public int GetStudentsCount()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT COUNT(*) FROM Students", connection))
                {
                    command.CommandType = CommandType.Text;
                    try
                    {
                        connection.Open();
                        var result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            return Convert.ToInt32(result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return 0;
                }
            }
        }


        // building Section/Operations
        public bool AddBuilding(Building building)
        {
            using (var connection = new SqlConnection(_connectionString))
            {

                using (var command = new SqlCommand("SP_CreateBuilding", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BuildingName", building.buildingName);

                    // Output parameter to get THE Building ID
                    SqlParameter BuidlingIdParam = new SqlParameter("@Output", SqlDbType.Int);

                    BuidlingIdParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(BuidlingIdParam);

                    
                    int rowAffected = 0;
                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                        // Read the output value
                        building.buildingId = Convert.ToInt32(BuidlingIdParam.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                   
                    return rowAffected > 0;

                }

            }
        }

        public List<Dictionary<string, dynamic>> GetAllBuildings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {

                using (var command = new SqlCommand("SP_GetAllBuildings", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    var dict = new Dictionary<string, dynamic>();
                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int buildingId = reader.GetInt32(reader.GetOrdinal("buildingId"));
                                string buildingName = reader.GetString(reader.GetOrdinal("buildingName"));
                                int floorCount = reader.GetInt32(reader.GetOrdinal("floor_count"));
                                int roomCount = reader.GetInt32(reader.GetOrdinal("room_count"));
                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "BuildingId", buildingId },
                                    { "BuildingName", buildingName },
                                    { "floor_count" , floorCount },
                                    { "room_count" , roomCount },
                                }
                                );

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                    return listDict;

                }

            }
        }

        public bool UpdateBuilding(Building building)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_UpdateBuilding", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BuildingID", building.buildingId);
                    command.Parameters.AddWithValue("@BuildingName", building.buildingName);
                    int rowAffected = 0;
                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return rowAffected > 0;
                }
            }
        }

        public bool DeleteBuilding(int buildingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_DeleteBuilding", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BuildingID", buildingId);
                    int rowAffected = 0;
                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return rowAffected > 0;
                }
            }
        }

        // Floor Section/Operations
        public bool AddFloor(Floor floor)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_AddFloor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BuildingID", floor.buildingId);
                    command.Parameters.AddWithValue("@FloorNumber", floor.floorNumber);

                    // Output parameter to get THE Building ID
                    SqlParameter floorIdParam = new SqlParameter("@Output", SqlDbType.Int);

                    floorIdParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(floorIdParam);

                    int rowAffected = 0;
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        if (floorIdParam.Value == DBNull.Value)
                            return false;

                        floor.floorId = Convert.ToInt32(floorIdParam.Value);

                        return floor.floorId > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        return false;
                    }
                }
            }
        }
        public List<Dictionary<string, dynamic>> GetAllFloors(int buildingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetAllFloors", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BuildingID", buildingId);
                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int floorId = reader.GetInt32(reader.GetOrdinal("floorId"));
                                buildingId = reader.GetInt32(reader.GetOrdinal("buildingId"));
                                int floorNumber = reader.GetInt32(reader.GetOrdinal("floorNumber"));
                                int roomCount = reader.GetInt32(reader.GetOrdinal("roomCount"));
                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "FloorID", floorId },
                                    { "BuildingID", buildingId },
                                    { "FloorNumber", floorNumber },
                                    { "RoomCount", roomCount }
                                }
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }
        public bool UpdateFloor(Floor floor)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_UpdateFloor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FloorID", floor.floorId);
                    command.Parameters.AddWithValue("@FloorNumber", floor.floorNumber);
                    command.Parameters.AddWithValue("@BuildingID", floor.buildingId);


                    int rowAffected = 0;
                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return rowAffected > 0;
                }
            }
        }

        public bool DeleteFloor(Floor floor)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_DeleteFloor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FloorID", floor.floorId);
                    int rowAffected = 0;
                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return rowAffected > 0;
                }
            }

        }


        // ==========================        
        // Room Section/Operations
        // =========================
        public bool AddRoom(Room room)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_AddRoom", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RoomNumber", room.roomNumber);
                    command.Parameters.AddWithValue("@FloorID", room.floorId);
                    command.Parameters.AddWithValue("@Capacity", room.capacity);
                    command.Parameters.AddWithValue("@RoomStatusID", room.roomStatusId);

                    SqlParameter roomIdParam = new SqlParameter("@output", SqlDbType.Int);
                    roomIdParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(roomIdParam);
                    
                    int rowAffected = 0;
                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                        room.roomId = Convert.ToInt32(roomIdParam.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return rowAffected > 0;
                }
            }
        }

        public List<Dictionary<string, dynamic>> GetAllRooms(int floorId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetAllRooms", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FloorID", floorId);
                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int roomId = reader.GetInt32(reader.GetOrdinal("roomId"));
                                string roomNumber = reader.GetString(reader.GetOrdinal("roomNumber"));
                                int capacity = reader.GetInt32(reader.GetOrdinal("capacity"));
                                int availableBeds = reader.GetInt32(reader.GetOrdinal("availableBeds"));
                                int roomStatusId = reader.GetInt32(reader.GetOrdinal("roomStatusId"));
                                string roomStatus = reader.GetString(reader.GetOrdinal("roomStatus"));
                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "RoomID", roomId },
                                    { "RoomNumber", roomNumber },
                                    { "Capacity", capacity },
                                    { "AvailableBeds", availableBeds },
                                    { "RoomStatusID", roomStatusId },
                                    { "RoomStatus", roomStatus }
                                }
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }

        public bool UpdateRoom(Room room)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_UpdateRoom", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RoomID", room.roomId);
                    command.Parameters.AddWithValue("@RoomNumber", room.roomNumber);
                    command.Parameters.AddWithValue("@FloorID", room.floorId);
                    command.Parameters.AddWithValue("@Capacity", room.capacity);
                    command.Parameters.AddWithValue("RoomStatusID", room.roomStatusId);

                    int rowAffected = 0;
                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return rowAffected > 0;
                }
            }
        }

        public bool DeleteRoom(Room room)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_DeleteRoom", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RoomID", room.roomId);
                    command.Parameters.AddWithValue("@FloorID", room.floorId);
                    int rowAffected = 0;
                    try
                    {
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return rowAffected > 0;
                }
            }
        }

        public List<Dictionary<string, dynamic>> GetRoomStatus()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT * FROM RoomStatus ", connection))
                {
                    command.CommandType = CommandType.Text;

                    var listDict = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int roomStatusId = reader.GetInt32(reader.GetOrdinal("roomStatusId"));
                                string  roomStatus= reader.GetString(reader.GetOrdinal("roomStatus"));

                                listDict.Add(new Dictionary<string, dynamic>()
                                {
                                    { "RoomStatusID", roomStatusId },
                                    { "RoomStatus", roomStatus }

                                });
                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    return listDict;
                }
            }
        }

        public async Task<List<Dictionary<string, dynamic>>> GetAvailableRooms()
        {

            List<Dictionary<string, dynamic>> listAvailableRooms = new List<Dictionary<string, dynamic>>();

            using (var connection = new SqlConnection(_connectionString)) {
                using (var command = new SqlCommand("usp_GetAvailableRooms", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;


                    try
                    {

                       

                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Dictionary<string, dynamic> room = new Dictionary<string, dynamic>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    room[reader.GetName(i)] = reader.GetValue(i);
                                }

                               

                                listAvailableRooms.Add(room);

                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetEmployeesStatisticsOverview: {ex.Message}");
                    }

                    
                }
            }
            return listAvailableRooms;

        }

        //------------------------------
        //      Financial / Income Section
        // ------------------------------
        public async Task<List<IncomeOverviewDTO>> GetIncomesOverview()
        {
            var list = new List<IncomeOverviewDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT IncomeID, IncomeCategoryID, IncomeAmount, IncomeDate, CategoryName FROM ViewIcomesTotalOverview ORDER BY IncomeDate DESC", connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new IncomeOverviewDTO
                                {
                                    IncomeID = reader.GetInt32(reader.GetOrdinal("IncomeID")),
                                    IncomeCategoryID = reader.GetInt32(reader.GetOrdinal("IncomeCategoryID")),
                                    IncomeAmount = reader.GetDecimal(reader.GetOrdinal("IncomeAmount")),
                                    IncomeDate = reader.GetDateTime(reader.GetOrdinal("IncomeDate")),
                                    CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? string.Empty : reader.GetString(reader.GetOrdinal("CategoryName"))
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching IncomesOverview: {ex.Message}");
                    }
                }
            }
            return list;
        }

        public async Task<List<IncomeCategoryDTO>> GetIncomeCategories()
        {
            var list = new List<IncomeCategoryDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT CategoryID, CategoryName FROM IncomeCategories ORDER BY CategoryName ASC", connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new IncomeCategoryDTO
                                {
                                    CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching IncomeCategories: {ex.Message}");
                    }
                }
            }
            return list;
        }

        public async Task<bool> AddIncome(AddIncomeDTO incomeDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddIncome", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IncomeCategoryID", incomeDTO.IncomeCategoryID);
                    command.Parameters.AddWithValue("@IncomeAmount", incomeDTO.IncomeAmount);
                    command.Parameters.AddWithValue("@IncomeDate", incomeDTO.IncomeDate.Date);

                    int rowsAffected = 0;
                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in AddIncome: {ex.Message}");
                        return false;
                    }
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<IncomeStatisticsDTO> GetIncomeStatisticsAsync()
        {
            var stats = new IncomeStatisticsDTO();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    // 1. Total Deposit held
                    using (SqlCommand cmd1 = new SqlCommand("SELECT ISNULL(SUM(DepositAmount), 0) AS TotalDeposit FROM StudentDeposit WHERE isRefunded = 0", connection))
                    {
                        var result = await cmd1.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            stats.TotalDeposit = Convert.ToDecimal(result);
                        }
                    }

                    // 2. Pending Student Rent
                    using (SqlCommand cmd2 = new SqlCommand("SELECT ISNULL(SUM(InstallmentAmount), 0) AS PendingStudentRent FROM RentInstallmentsLedger WHERE IsPaid = 0", connection))
                    {
                        var result = await cmd2.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            stats.PendingStudentRent = Convert.ToDecimal(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GetIncomeStatisticsAsync: {ex.Message}");
                }
            }
            return stats;
        }

        public async Task<FinancialOverviewBundleDTO> GetFinancialOverviewBundleAsync()
        {
            var bundle = new FinancialOverviewBundleDTO();
            
            // 1. Fetch Incomes
            bundle.Incomes = await GetIncomesOverview();
            
            // 2. Fetch Expenses
            bundle.Expenses = await GetExpensesOverview();
            
            // 3. Fetch Statistics (Deposit and Pending Rent)
            var stats = await GetIncomeStatisticsAsync();
            bundle.TotalDepositHeld = stats.TotalDeposit;
            bundle.PendingStudentRent = stats.PendingStudentRent;

            // 4. Calculate monthly and all-time totals
            var now = DateTime.Now;
            
            bundle.TotalIncomeAllTime = bundle.Incomes.Sum(i => i.IncomeAmount);
            bundle.TotalIncomeThisMonth = bundle.Incomes
                .Where(i => i.IncomeDate.Year == now.Year && i.IncomeDate.Month == now.Month)
                .Sum(i => i.IncomeAmount);

            bundle.TotalExpenseAllTime = bundle.Expenses.Sum(e => e.ExpenseAmount);
            bundle.TotalExpenseThisMonth = bundle.Expenses
                .Where(e => e.ExpenseDate.Year == now.Year && e.ExpenseDate.Month == now.Month)
                .Sum(e => e.ExpenseAmount);

            bundle.NetBalanceThisMonth = bundle.TotalIncomeThisMonth - bundle.TotalExpenseThisMonth;

            return bundle;
        }

        //------------------------------
        //      Financial / Expense Section
        // ------------------------------
        public async Task<List<ExpenseOverviewDTO>> GetExpensesOverview()
        {
            var list = new List<ExpenseOverviewDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT ExpenseID, ExpenseGategoryID, CategoryName, ExpenseAmount, ExpenseDate, CreatedAt FROM ViewExpenseOverview ORDER BY ExpenseDate DESC", connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new ExpenseOverviewDTO
                                {
                                    ExpenseID = reader.GetInt32(reader.GetOrdinal("ExpenseID")),
                                    ExpenseGategoryID = reader.GetInt32(reader.GetOrdinal("ExpenseGategoryID")),
                                    CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? string.Empty : reader.GetString(reader.GetOrdinal("CategoryName")),
                                    ExpenseAmount = reader.GetDecimal(reader.GetOrdinal("ExpenseAmount")),
                                    ExpenseDate = reader.GetDateTime(reader.GetOrdinal("ExpenseDate")),
                                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching ExpensesOverview: {ex.Message}");
                    }
                }
            }
            return list;
        }

        public async Task<List<ExpenseCategoryDTO>> GetExpenseCategories()
        {
            var list = new List<ExpenseCategoryDTO>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT CategoryID, CategoryName FROM ExpenseCategories ORDER BY CategoryName ASC", connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new ExpenseCategoryDTO
                                {
                                    CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching ExpenseCategories: {ex.Message}");
                    }
                }
            }
            return list;
        }

        public async Task<bool> AddExpense(AddExpenseDTO expenseDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddExpense", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExpenseGategoryID", expenseDTO.ExpenseGategoryID);
                    command.Parameters.AddWithValue("@ExpenseAmount", expenseDTO.ExpenseAmount);
                    command.Parameters.AddWithValue("@ExpenseDate", expenseDTO.ExpenseDate.Date);

                    int rowsAffected = 0;
                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in AddExpense: {ex.Message}");
                        return false;
                    }
                    return rowsAffected > 0;
                }
            }
        }

        // ================================
        //      Student Deposit Section
        // ================================

        public async Task<bool> AddStudentDepositAsync(AddStudentDepositDTO depositDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddStudentDeposit", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentID", depositDTO.StudentID);
                    command.Parameters.AddWithValue("@DepositAmount", depositDTO.DepositAmount);
                    command.Parameters.AddWithValue("@DepositDate", depositDTO.DepositDate.Date);

                    int rowsAffected = 0;
                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in AddStudentDepositAsync: {ex.Message}");
                        return false;
                    }
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<List<Dictionary<string, dynamic>>> GetStudentDepositsAsync(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetStudentDeposits", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentID", studentId);

                    var list = new List<Dictionary<string, dynamic>>();
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var dict = new Dictionary<string, dynamic>();
                                dict["DepositID"] = reader["DepositID"];
                                dict["StudentID"] = reader["StudentID"];
                                dict["DepositAmount"] = reader["DepositAmount"];
                                dict["DepositDate"] = reader["DepositDate"];
                                dict["isRefunded"] = reader["isRefunded"];
                                dict["CreatedAt"] = reader["CreatedAt"];
                                list.Add(dict);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetStudentDepositsAsync: {ex.Message}");
                    }
                    return list;
                }
            }
        }

        public async Task<bool> RefundStudentDepositAsync(int depositId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_RefundStudentDeposit", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DepositID", depositId);

                    int rowsAffected = 0;
                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in RefundStudentDepositAsync: {ex.Message}");
                        return false;
                    }
                    return rowsAffected > 0;
                }
            }
        }        
        
        

        

       
    }
}
