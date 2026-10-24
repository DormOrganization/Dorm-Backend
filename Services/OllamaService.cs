using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using DormAPI.Database.Ado.Repositories;
using DormAPI.Database.EF.Repositories;
using DormAPI.DTOs.AI;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DormAPI.Services
{




    public class OllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly EmployeeRepository _employeeRepo;
        private readonly StudentRepository _studentRepo;
        private readonly ChatBotRepository _chatBotRepo;
        private readonly string _modelName;

        // HttpClient is already configured with http://localhost:11434/ in Program.cs
        public OllamaService(HttpClient httpClient, EmployeeRepository employeeRepo, StudentRepository studentRepo, ChatBotRepository chatBotRepository, IConfiguration config)
        {
            _httpClient = httpClient;
            _employeeRepo = employeeRepo;
            _studentRepo = studentRepo;
            _chatBotRepo = chatBotRepository;
            // Fetches "llama3.2" from your appsettings.json
            _modelName = config["OllamaConfig:ModelName"] ?? "llama3.2";
        }

        private static readonly string _systemPrompt = @"
You are a SQL Server T-SQL query-generation assistant for a Dormitory Management System.
Translate the user's natural-language question into one valid, read-only T-SQL SELECT query.

### 1. PRIORITY VIEW (ALWAYS USE THIS FIRST)

whenever the user asks about students personal information and total students
- ViewStudents (
    StudentID int, 
    StudentNumber varchar(20), 
    StartedDate date,
    EndDate date,
    YearOfStudy int,
    UniversityID int,
    university_name nvarchar(255), 
    FacultyID int,
    FacultyName nvarchar(100), 
    DepartmentID int, 
    DepartmentName nvarchar(100), EducationLevelID int,
    LevelName nvarchar(50), 
    PersonID int, FirstName varchar(50), SecondName varchar(50), ThirdName varchar(50), LastName varchar(50), 
    DateOfBirth date, Gender bit, PassportNumber varchar(20), IdentityNumber varchar(20), NationalityID int, 
    Nationality nvarchar(100), ContactID int, Email varchar(100), PhoneNumber varchar(13), Address varchar(255), 
    EmergencyContactName varchar(100), EmergencyContactPhone varchar(13), RelationshipTypeID int, RelationshipName nvarchar(50)
  )

Whenever the user asks about room assignments, which room/bed/floor/building a student lives in, or students in a specific building/floor/room, DO NOT join RoomAllocation, Room, Floor, Building, Students, or People. You MUST query this view directly:
- ViewRoomAllocationDetails (
    RoomAllocationID int,
    RoomID int,
    RoomNumber int,
    FloorNumber int,
    BuildingName nvarchar(50),
    BedNumber int,
    StudentID int,
    StudentNumber nvarchar(30),
    FirstName nvarchar(40),
    SecondName nvarchar(40),
    ThirdName nvarchar(40),
    LastName nvarchar(40)
  )



### 2. NAME FILTERING & TABLE NAME RULES
- When filtering by a student's two-part name (e.g., 'Mohammed Kalumian'), match FirstName = 'Mohammed' AND (LastName = 'Kalumian' OR SecondName = 'Kalumian').
- Exact Table Names: The room table is named `Room` (NEVER `Rooms`). The building table is `Building` (NEVER `Buildings`). The floor table is `Floor` (NEVER `Floors`).
- The `Students` table does NOT have `FirstName` or `LastName` columns. Names only exist in `ViewRoomAllocationDetails` or `People`.

### 3. BASE TABLES (USE ONLY IF THE VIEW DOES NOT HAVE THE DATA)
1. Building and Room Management
- Building (BuildingID [PK], BuildingName)
- Floor (FloorID [PK], FloorNumber, BuildingID [FK -> Building.BuildingID])
- Room (RoomID [PK], RoomNumber, FloorID [FK -> Floor.FloorID], Capacity, AvailableBeds, RoomStatusID [FK -> RoomStatus.RoomStatusID])
- RoomStatus (RoomStatusID [PK], RoomStatus)
- RoomAllocation (RoomAllocationID [PK], RoomID [FK -> Room.RoomID], BedNumber, StudentID [FK -> Students.StudentID])

2. People and Contact Information
- People (PersonID [PK], FirstName, SecondName, ThirdName, LastName, DateOfBirth, Gender, PassportNumber, IdentityNumber, NationalityID [FK -> Nationalities.NationalityID], ContactID [FK -> Contacts.ContactID])
- Contacts (ContactID [PK], Email, PhoneNumber, Address, EmergencyContactName, EmergencyContactPhone, RelationshipTypeID [FK -> RelationshipTypes.RelationshipTypeID])
- RelationshipTypes (RelationshipTypeID [PK], RelationshipName)
- Nationalities (NationalityID [PK], Country, Nationality)

3. Student Management
- Students (StudentID [PK], StudentNumber, StartedDate, EndDate, YearOfStudy, PersonID [FK -> People.PersonID], UniversityID [FK -> universities.university_id], FacultyID [FK -> Faculties.FacultyID], DepartmentID [FK -> Departments.DepartmentID], EducationLevelID [FK -> EducationLevel.LevelID], RoomAllocationID [FK -> RoomAllocation.RoomAllocationID])
- Faculties (FacultyID [PK], FacultyName, FacultyCode)
- Departments (DepartmentID [PK], DepartmentName)
- universities (university_id [PK], university_name)
- EducationLevel (LevelID [PK], LevelName)
- DocumentsPath (DocumentPathID [PK], DocumentPath, DocumentName, DocumentType, StudentID [FK -> Students.StudentID])
- StudentDeposit (DepositID [PK], StudentID [FK -> Students.StudentID], DepositAmount, DepositDate, isRefunded, CreatedAt)

4. Contracts and Rent Installments
- Contracts (ContractID [PK], ContractName, ContractStartDate, ContractEndDate, TotalAmount, AdditionalFees, Discount, MonthlyAmount, NumberOfInstallments, CreatedAt, IsActive, StudentID [FK -> Students.StudentID])
- RentInstallmentsLedger (RentInstallmentID [PK], ContractID [FK -> Contracts.ContractID], InstallmentNumber, InstallmentAmount, DueDate, PaidDate, IsPaid, CreatedAt, PaymentMethodID [FK -> PaymentMethod.PaymentMethodID])
- Payments (PaymentID [PK], RentInstallmentID [FK -> RentInstallmentsLedger.RentInstallmentID], PaymentMethodID [FK -> PaymentMethod.PaymentMethodID], PaymentDate, Notes)
- PaymentMethod (PaymentMethodID [PK], PaymentMethodName)

### 4. EXAMPLES (FOLLOW THESE PATTERNS STRICTLY)
User: Where did Mohammed Kalumian assign which Room?
Output: {""sql"":""SELECT BuildingName, FloorNumber, RoomNumber, BedNumber FROM ViewRoomAllocationDetails WHERE FirstName = 'Mohammed' AND (LastName = 'Kalumian' OR SecondName = 'Kalumian')""}

User: How many students do we have in building B19?
Output: {""sql"":""SELECT COUNT( DISTINCT StudentID ) AS TotalStudents FROM ViewRoomAllocationDetails WHERE BuildingName = 'B19'""}

User: Who are the students in building B19? Only give their names.
Output: {""sql"":""SELECT FirstName, SecondName, LastName FROM ViewRoomAllocationDetails WHERE BuildingName = 'B19'""}

User: How many rooms are there?
Output: {""sql"":""SELECT COUNT(*) AS TotalRooms FROM Room""}

User: How many available beds are there across all rooms?
Output: {""sql"":""SELECT SUM(AvailableBeds) AS TotalAvailableBeds FROM Room""}

### 5. SECURITY & OUTPUT FORMAT
- Generate read-only single SELECT queries only.
- If the request cannot be answered using the provided schema, return:
  {""error"":""The requested information cannot be retrieved from the provided schema.""}
- Return ONLY one valid JSON object: {""sql"":""SELECT ...""}
" + DateTime.Now.ToString("yyyy-MM-dd").ToString()
        ;

        private readonly object _systemMessage = new
        {
            role = "system",
            content = "You are a helpful dormitory management assistant. You have permission to only generate select" + _systemPrompt,
        };



        private readonly object _systemMessageFunction = new
        {
            role = "system",
            content = "You are a dormitory management assistant for the dorm owner. You are strictly permitted to read, select, and normalize dormitory data using the provided tools and return clear responses. Do not modify or delete any data. If the user's request cannot be answered using the available tools or is unrelated to dormitory management, do not call any tools and reply with: 'I am a dorm assistant and cannot help you with that.'"
        };




        private static readonly string _normalizerSystemPrompt = @"
You are a helpful Dormitory Management Assistant.
You will be given a User Question and the Raw Database Result (JSON or text).
Your task:
- Answer the user's question clearly, naturally, and concisely based ONLY on the Raw Database Result.
- Do NOT mention SQL queries, table names, database schemas, or JSON formatting in your response.
- If the database result is empty or '[]', politely inform the user that no matching records were found.
";

        public async Task<string> ProcessChatSqlAcync(string userPrompt)
        {
            // 1. Send the prompt to Ollama to generate the SQL JSON
            var initialRequest = new
            {
                model = _modelName, // e.g., "llama3.2"
                messages = new object[]
                {
                      _systemMessage,
                     new { role = "user", content = userPrompt }
                },
                format = "json", // Forces Ollama to output strict JSON with no markdown fences
                stream = false,
                options = new { temperature = 0 } // Makes SQL generation deterministic
            };

            try
            {
                var response1Sql = await _httpClient.PostAsJsonAsync("api/chat", initialRequest);
                response1Sql.EnsureSuccessStatusCode();
            

                var response1Json = await response1Sql.Content.ReadAsStringAsync();

                // 2. Parse Ollama's response
                using var doc1 = JsonDocument.Parse(response1Json);
                string generatedContent = doc1.RootElement
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "";

                // Clean any accidental markdown fences just in case
                var generatedJsonClean = generatedContent
                    .Replace("```json", "")
                    .Replace("```sql", "")
                    .Replace("```", "")
                    .Trim();

                Console.WriteLine($"AI SQL Payload: {generatedJsonClean}");

                using var sqlDoc = JsonDocument.Parse(generatedJsonClean);

                // Check if the AI returned an {"error": "..."} object for an unanswerable question
                if (sqlDoc.RootElement.TryGetProperty("error", out JsonElement errorElement))
                {
                    return errorElement.GetString() ?? "The requested information cannot be retrieved.";
                }

                if (!sqlDoc.RootElement.TryGetProperty("sql", out JsonElement sqlElement))
                {
                    return "Could not generate a valid query for your request.";
                }

                string sql = sqlElement.GetString()!;
                Console.WriteLine($"Extracted SQL: {sql}");

                // Basic C# guard against non-SELECT queries before hitting the DB
                if (!sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Security violation: Only SELECT queries are permitted.");
                }

                // 3. Execute the SQL against the database
                string sqlResult = await _chatBotRepo.ExecuteSql(sql);
                Console.WriteLine("=========================");
                Console.WriteLine($"Database Result: {sqlResult}");

                // 4. Send the database result back to the AI to normalize into a human-friendly response
                var followupRequest = new
                {
                    model = _modelName,
                    messages = new object[]
                    {
                new { role = "system", content = _normalizerSystemPrompt },
                new
                {
                    role = "user",
                    content = $"User Question: {userPrompt}\nRaw Database Result: {sqlResult}\n\nPlease provide the final answer to the user:"
                }
                    },
                    stream = false,
                    options = new { temperature = 0.2 }
                };

                var response2Final = await _httpClient.PostAsJsonAsync("api/chat", followupRequest);
                response2Final.EnsureSuccessStatusCode();

                var response2Json = await response2Final.Content.ReadAsStringAsync();
                using var doc2 = JsonDocument.Parse(response2Json);
                string finalAnswer = doc2.RootElement
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "";

                // 5. Return the final natural-language answer to the client
                return finalAnswer.Trim();

                }
            catch (HttpRequestException ex)
            {
                return "Chatbot is not working right now try again later.";
            }
        }

        public async Task<string> ProcessChatAsync(string userPrompt)
        {

            // "who are the students which have not paid the rent in 9 month in 2026"


            // Define the system context so the AI knows its role and permissions


            // 1. Tool definition
            var tools = new object[] {
                new {
                    type = "function",
                    function = new {
                        name = "get_unpaid_students",
                        description = "Get unpaid students for a specific month and year",
                        parameters = new {
                            type = "object",
                            properties = new {
                                Month = new { type = "integer" },
                                Year = new { type = "integer" }
                            },
                            required = new[] { "Month", "Year" }
                        }
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "get_total_students",
                        description = "Get total students",
                        parameters = new {
                            type = "object",
                            properties = new { },
                            required = Array.Empty<string>()
                        }
                    }
                },
                new {
                    type = "function",
                    function = new {
                        name = "get_total_employees",
                        description = "Get unpaid employees for a specific month and year",
                        parameters = new {
                            type = "object",
                            properties = new {

                            },
                            required = Array.Empty<string>()
                        }
                    }
                },
                 new {
                    type = "function",
                    function = new {
                        name = "Get_Unsigned_Room_Students",
                        description = "Get unsigend room students",
                        parameters = new {
                            type = "object",
                            properties = new {

                            },
                            required = Array.Empty<string>()
                        }
                    },

                },
                  new {
                    type = "function",
                    function = new {
                        name = "Get_Availabe_Rooms",
                        description = "Get Availabe Rooms",
                        parameters = new {
                            type = "object",
                            properties = new {

                            },
                            required = Array.Empty<string>()
                        }
                    }
                  }

            };

            // Step 2: Send prompt and tools to http://localhost:11434/api/chat



            var initialRequest = new
            {
                model = _modelName, // llama3.2 or whatever model you use
                messages = new object[]
                    {
                        _systemMessageFunction,
                        new { role = "user", content = userPrompt }
                    },
                tools = tools,
                stream = false
            };


            var response1 = await _httpClient.PostAsJsonAsync("api/chat", initialRequest);
            response1.EnsureSuccessStatusCode();

            Console.WriteLine(response1);

            // ✅ CORRECT: Converts the JSON stream directly into your C# object
            // Read the stream ONCE into your DTO
            var aiResponse1 = await response1.Content.ReadFromJsonAsync<OllamaResponseDto>();

            Console.WriteLine("============================");
            Console.WriteLine("============================");
            Console.WriteLine(aiResponse1.Message);





            // 3. Check if AI requested a Tool Call
            if (aiResponse1?.Message?.ToolCalls != null && aiResponse1.Message.ToolCalls.Any())
            {
                var toolCall = aiResponse1.Message.ToolCalls.First();

                if (toolCall.Function.Name == "get_unpaid_students")
                {

                    var jsonOptions = new JsonSerializerOptions
                    {
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };
                    var args = JsonSerializer.Deserialize<GetUnpaidStudentsArgs>(toolCall.Function.Arguments.ToString(), jsonOptions);

                    // Execute DB query with await
                    var studentIds = await _studentRepo.GetUnpaidStudents(args.Month, args.Year);

                    // Give context to the output so Llama understands what the numbers mean
                    string toolContent;
                    if (studentIds == null || !studentIds.Any())
                    {
                        toolContent = "No unpaid students were found for this period.";
                    }
                    else
                    {
                        // Formats [2] as structured objects e.g., [{"StudentId": 2, "PaymentStatus": "Unpaid"}]
                        var structuredResult = studentIds.Select(id => new { StudentId = id, PaymentStatus = "Unpaid" });
                        toolContent = JsonSerializer.Serialize(structuredResult);
                    }

                    // Step 2: Followup Request (Include system message)
                    var followupRequest = new
                    {
                        model = _modelName,
                        messages = new object[]
                        {
                            _systemMessageFunction,
                            new { role = "user", content = userPrompt },
        
                            // CRITICAL: Include the assistant's message from Step 1 that triggered the tool
                            new {
                                role = "assistant",
                                content = "",
                                tool_calls = aiResponse1.Message.ToolCalls // Pass the tool_calls array from your first response
                            },
        
                            // Now pass the result of the tool execution
                            new {
                                role = "tool",
                                name = toolCall.Function.Name, // Supported in newer Ollama versions, safe to keep
                                content = toolContent // Ensure this is a JSON string or plain text string
                            }
                        },
                        stream = false
                    };

                    var response2 = await _httpClient.PostAsJsonAsync("api/chat", followupRequest);
                    response2.EnsureSuccessStatusCode();

                    var aiResponse2 = await response2.Content.ReadFromJsonAsync<OllamaResponseDto>();
                    return aiResponse2?.Message?.Content ?? "No final response generated.";
                }
                else if (toolCall.Function.Name == "get_total_students")
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };
                    var args = JsonSerializer.Deserialize<GetUnpaidStudentsArgs>(toolCall.Function.Arguments.ToString(), jsonOptions);

                    // Execute DB query with await
                    var totalStudents = await _studentRepo.GetTotalStudent();


                    // Give context to the output so Llama understands what the numbers mean
                    string toolContent;
                    if (totalStudents == null)
                    {
                        toolContent = "No students were found for this period.";
                    }
                    else
                    {
                        // Returns JSON: {"TotalStudents": 42}
                        var structuredResult = new { TotalStudents = totalStudents };
                        toolContent = JsonSerializer.Serialize(structuredResult);
                    }


                    // Step 2: Followup Request (Include system message)
                    var followupRequest = new
                    {
                        model = _modelName,
                        messages = new object[]
                        {
                            _systemMessageFunction,
                            new { role = "user", content = userPrompt },
        
                            // CRITICAL: Include the assistant's message from Step 1 that triggered the tool
                            new {
                                role = "assistant",
                                content = "",
                                tool_calls = aiResponse1.Message.ToolCalls // Pass the tool_calls array from your first response
                            },
        
                            // Now pass the result of the tool execution
                            new {
                                role = "tool",
                                name = toolCall.Function.Name, // Supported in newer Ollama versions, safe to keep
                                content = toolContent // Ensure this is a JSON string or plain text string
                            }
                        },
                        stream = false
                    };

                    var response2 = await _httpClient.PostAsJsonAsync("api/chat", followupRequest);
                    response2.EnsureSuccessStatusCode();

                    var aiResponse2 = await response2.Content.ReadFromJsonAsync<OllamaResponseDto>();
                    return aiResponse2?.Message?.Content ?? "No final response generated.";


                }
                else if (toolCall.Function.Name == "get_total_employees")
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };
                    var args = JsonSerializer.Deserialize<GetUnpaidStudentsArgs>(toolCall.Function.Arguments.ToString(), jsonOptions);

                    // Execute DB query with await
                    var totalEmployees = await _employeeRepo.GetTotalEmployees();


                    // Give context to the output so Llama understands what the numbers mean
                    string toolContent;
                    if (totalEmployees == null)
                    {
                        toolContent = "No employees were found.";
                    }
                    else
                    {
                        // Returns JSON: {"TotalStudents": 42}
                        var structuredResult = new { totalEmployees = totalEmployees };
                        toolContent = JsonSerializer.Serialize(structuredResult);
                    }


                    // Step 2: Followup Request (Include system message)
                    var followupRequest = new
                    {
                        model = _modelName,
                        messages = new object[]
                        {
                            _systemMessageFunction,
                            new { role = "user", content = userPrompt },
        
                            // CRITICAL: Include the assistant's message from Step 1 that triggered the tool
                            new {
                                role = "assistant",
                                content = "",
                                tool_calls = aiResponse1.Message.ToolCalls // Pass the tool_calls array from your first response
                            },
        
                            // Now pass the result of the tool execution
                            new {
                                role = "tool",
                                name = toolCall.Function.Name, // Supported in newer Ollama versions, safe to keep
                                content = toolContent // Ensure this is a JSON string or plain text string
                            }
                        },
                        stream = false
                    };

                    var response2 = await _httpClient.PostAsJsonAsync("api/chat", followupRequest);
                    response2.EnsureSuccessStatusCode();

                    var aiResponse2 = await response2.Content.ReadFromJsonAsync<OllamaResponseDto>();
                    return aiResponse2?.Message?.Content ?? "No final response generated.";


                }
                else if (toolCall.Function.Name == "Get_Unsigned_Room_Students")
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };
                    var args = JsonSerializer.Deserialize<GetUnpaidStudentsArgs>(toolCall.Function.Arguments.ToString(), jsonOptions);

                    // Execute DB query with await
                    var unsignedStudents = await _studentRepo.GetUnsignedRoomStudents();


                    // Give context to the output so Llama understands what the numbers mean
                    string toolContent;
                    if (unsignedStudents == null)
                    {
                        toolContent = "No unsigned students were found.";
                    }
                    else
                    {
                        // Returns JSON: {"TotalStudents": 42}
                        var structuredResult = new { unsignedStudents = unsignedStudents };
                        toolContent = JsonSerializer.Serialize(structuredResult);
                    }


                    // Step 2: Followup Request (Include system message)
                    var followupRequest = new
                    {
                        model = _modelName,
                        messages = new object[]
                        {
                            _systemMessageFunction,
                            new { role = "user", content = userPrompt },
        
                            // CRITICAL: Include the assistant's message from Step 1 that triggered the tool
                            new {
                                role = "assistant",
                                content = "",
                                tool_calls = aiResponse1.Message.ToolCalls // Pass the tool_calls array from your first response
                            },
        
                            // Now pass the result of the tool execution
                            new {
                                role = "tool",
                                name = toolCall.Function.Name, // Supported in newer Ollama versions, safe to keep
                                content = toolContent // Ensure this is a JSON string or plain text string
                            }
                        },
                        stream = false
                    };

                    var response2 = await _httpClient.PostAsJsonAsync("api/chat", followupRequest);
                    response2.EnsureSuccessStatusCode();

                    var aiResponse2 = await response2.Content.ReadFromJsonAsync<OllamaResponseDto>();
                    return aiResponse2?.Message?.Content ?? "No final response generated.";


                }
                else if (toolCall.Function.Name == "Get_Availabe_Rooms")
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };
                    var args = JsonSerializer.Deserialize<GetUnpaidStudentsArgs>(toolCall.Function.Arguments.ToString(), jsonOptions);

                    // Execute DB query with await
                    var availabeRooms = await _chatBotRepo.GetAvailabeRooms();


                    // Give context to the output so Llama understands what the numbers mean
                    string toolContent;
                    if (availabeRooms == null)
                    {
                        toolContent = "No Availabe Rooms were found.";
                    }
                    else
                    {
                        // Returns JSON: {"TotalStudents": 42}
                        var structuredResult = new { availabeRooms = availabeRooms };
                        toolContent = JsonSerializer.Serialize(structuredResult);
                    }


                    // Step 2: Followup Request (Include system message)
                    var followupRequest = new
                    {
                        model = _modelName,
                        messages = new object[]
                        {
                            _systemMessageFunction,
                            new { role = "user", content = userPrompt },
        
                            // CRITICAL: Include the assistant's message from Step 1 that triggered the tool
                            new {
                                role = "assistant",
                                content = "",
                                tool_calls = aiResponse1.Message.ToolCalls // Pass the tool_calls array from your first response
                            },
        
                            // Now pass the result of the tool execution
                            new {
                                role = "tool",
                                name = toolCall.Function.Name, // Supported in newer Ollama versions, safe to keep
                                content = toolContent // Ensure this is a JSON string or plain text string
                            }
                        },
                        stream = false
                    };

                    var response2 = await _httpClient.PostAsJsonAsync("api/chat", followupRequest);
                    response2.EnsureSuccessStatusCode();

                    var aiResponse2 = await response2.Content.ReadFromJsonAsync<OllamaResponseDto>();
                    return aiResponse2?.Message?.Content ?? "No final response generated.";


                }
            }



            return aiResponse1?.Message?.Content ?? "No response"; ;
        }

        
    }


    
}
