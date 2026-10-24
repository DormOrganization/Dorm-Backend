using System.Runtime.InteropServices;
using System.Text.Json;
using DormAPI.Database.Ado.Common;
using DormAPI.DTOs;
using DormAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;




namespace DormAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DormController : ControllerBase
    {
        private readonly clsExternalLayer _clsExternalLayerRepo;
        public DormController(clsExternalLayer clsExternalLayerRepo)
        {
            _clsExternalLayerRepo = clsExternalLayerRepo;
        }

        // ================================
        //         Contract Section
        // ================================

        [HttpPost("contract/create")]
        public IActionResult CreateContract([FromBody] ContractDTO contract)
        {
            if(contract == null || contract.studentId <= 0 || string.IsNullOrEmpty(contract.contractName))
            {
                return BadRequest("Invalid contract data.");
            }

            bool isCreated = _clsExternalLayerRepo.CreateContract(contract);

            if(isCreated)
            {
                return Ok("Contract created successfully.");
            }
            else
            {
                return StatusCode(500, "An error occurred while creating the contract.");
            }





        }
        [HttpPut("contract/payment/pay")]
        public IActionResult PayRent([FromBody] PaymentDTO payment)
        {
            if(payment == null || payment.paymentMethodId <= 0 || payment.paymentMethodId <= 0)
            {
                return BadRequest("Invalid payment data.");
            }

            bool isPaid = _clsExternalLayerRepo.PayRent(payment);
            if(isPaid)
            {
                return Ok("Payment processed successfully.");
            }
            else
            {
                return StatusCode(500, "An error occurred while processing the payment.");
            }
        }

        [HttpGet("payment/methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var paymentMethods = await _clsExternalLayerRepo.GetAllPaymentMethodAsync();
            if(paymentMethods == null || paymentMethods.Count == 0)
            {
                return NotFound("No payment methods found.");
            }
            return Ok(paymentMethods);
        }

        [HttpGet("contract/payments/history/{studentId}")]
        public IActionResult GetPaymentHistory (int studentId)
        {
           if(studentId <= 0)
            {
                return BadRequest("Invalid student id.");
            }
            var paymentHistory = _clsExternalLayerRepo.GetPaymentHistory(studentId);
            if(paymentHistory == null )
            {
                return NotFound("No payment history found for the given student.");
            }
            return Ok(paymentHistory);
        }

        [HttpGet("student/info/all/{studentId}")]
        public async Task<IActionResult>  GetAllStudentInfoOnce(int studentId) 
        {
            if(studentId <= 0)
            {
                return BadRequest("Invalid student id.");
            }
            // Contracts       => StudentId
            // Installments    => StudentId
            // payment history => StudentId 

            // Start all three operations at the same time
            var contractsTask = 
            _clsExternalLayerRepo.GetStudentContracts(studentId);

            var installmentsTask = 
                    _clsExternalLayerRepo.GetStudentInstallments(studentId);

            var paymentHistoryTask = 
                    _clsExternalLayerRepo.GetPaymentHistory(studentId);

            var accomadationInfoTask =
                    _clsExternalLayerRepo.GetAccommodationInfoByStudentID(studentId);

            // Wait for all three to finish
            await Task.WhenAll(
                contractsTask,
                installmentsTask,
                paymentHistoryTask,
                accomadationInfoTask
            );

            var studentContracts = await contractsTask;
            var studentInstallments = await installmentsTask;
            var studentPaymentHistory = await paymentHistoryTask;
            var studentAccomadationInfo = await accomadationInfoTask;

            if (studentContracts == null &&
                studentInstallments == null &&
                studentPaymentHistory == null &&
                studentAccomadationInfo == null )
            {
                return NotFound("No data found for the given student.");
            }

            var response = new
            {
                Contracts = studentContracts,
                Installments = studentInstallments,
                PaymentHistory = studentPaymentHistory,
                AccomadationInfo = studentAccomadationInfo
            };

            return Ok(response);


        }



        // ================================
        //          Files Section
        // ================================

        // get all student files/documents
        [HttpGet("documents/get/{studentId}")]
        public IActionResult GetAllStudentId(int studentId)
        {
            if(studentId <= 0)
            {
                return BadRequest("student id is not correct");
            }

            var data = _clsExternalLayerRepo.GetAllStudentDocuments(studentId);

            if(data == null)
            {
                return NoContent();
            }

            return Ok(data);
        }

        [HttpGet("document/get")]
        public async Task<IActionResult> GetDocumentByPath([FromQuery] string documentPath)
        {
            if (string.IsNullOrEmpty(documentPath))
            {
                return BadRequest("Document path is not correct");
            }

            var threeTupleData = await _clsExternalLayerRepo.GetDocumentByPathAsync(documentPath);

            if (threeTupleData == (null, null, null))
            {
                return NotFound("Document not found");
            }



            return Ok(new FileContentResult(threeTupleData.Item1, threeTupleData.Item2));
        }

        // uploading file
        [HttpPost("upload/file")]
        public async Task<IActionResult> UploadFileForAutoFill(IFormFile file)
        {
            if (file == null)
            {

                return BadRequest("File is not uploadded");
            }

            else
            {
                using Stream fileStream = file.OpenReadStream();
                PdfLoadedDocument uploadedDocument = new PdfLoadedDocument(fileStream);
                PdfPageBase page = uploadedDocument.Pages[0];

                string extractedText = page.ExtractText(true);
                fileStream.Close();


                var client = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };

                var body = new
                {
                    model = "qwen2.5:7b",

                    prompt = """
You are a strict Turkish document data extraction engine.

Your ONLY task is to extract explicitly written values from the DOCUMENT TEXT.

RETURN ONLY VALID JSON.
NO markdown.
NO ```json.
NO explanations.
NO comments.

CRITICAL EXTRACTION RULES:

1. NEVER guess a value.
2. NEVER infer a value from another field.
3. NEVER calculate a value.
4. If a value is not explicitly present, return null.
5. A FIELD LABEL IS NEVER A VALUE.
6. If you find a field label but cannot identify its actual value, return null.
7. Do not copy Turkish field names into the JSON values.
8. Preserve the exact value from the document whenever possible.
9. Ignore OCR errors when identifying field labels, but NEVER invent missing values.
10. Do not confuse similar fields.

FIELD LABEL -> VALUE MAPPING:

StudentNumber:
Look for:
- Öğrenci No
- Öğrenci Numarası
- Student No
- Student Number

Return ONLY the value next to the field.

StartedDate:
Look ONLY for:
- Kayıt Tarihi
- Kayıt tarihi
- Registration Date
- Enrollment Date

This means the student's registration/start date.

IMPORTANT:
Do NOT use:
- Belge Tarihi
- Düzenleme Tarihi
- Document Date
- Issue Date

If Kayıt Tarihi is not explicitly present with a value, return null.

EndDate:
Look ONLY for an explicit:
- Mezuniyet Tarihi
- Eğitim Bitiş Tarihi
- Program Bitiş Tarihi
- Graduation Date
- End Date

Do NOT calculate an end date.
Do NOT use the expected duration of study.
If no explicit date exists, return null.

YearOfStudy:
Look for:
- Sınıf
- Sınıfı
- Class
- Year of Study

Examples:
"2. SINIF" -> "2"
"2" -> "2"

Do not calculate the year.

UniversityName:
Extract the FULL university name explicitly appearing in the document.

IMPORTANT:
Do NOT return:
- faculty name
- department name
- program name
- abbreviation
- partial OCR text

If the university name is:
"OSTİM Teknik Üniversitesi"

return:
"OSTİM Teknik Üniversitesi"

Do not shorten it to "OSTIM TEKNIKÜ".

EducationLevel:
Return only if explicitly stated.

Examples:
- Lisans -> Lisans
- Yüksek Lisans -> Yüksek Lisans
- Doktora -> Doktora
- Ön Lisans -> Ön Lisans

Do NOT infer education level from department or program.

FirstName:
Look at:
- Adı
- Ad
- Adı / Soyadı

Extract the first given name.

SecondName:
If there is a second given name explicitly written, return it.
Otherwise null.

ThirdName:
If there is a third given name explicitly written, return it.
Otherwise null.

LastName:
Look at:
- Soyadı
- Adı / Soyadı

Extract the surname.

PassportNumber:
Look ONLY for:
- Pasaport No
- Pasaport Numarası
- Passport No
- Passport Number

CRITICAL:
Never use:
- T.C. Kimlik No
- Kimlik No
- Öğrenci No

as PassportNumber.

If the document does not explicitly identify a number as a passport number, return null.

DateOfBirth:
Look ONLY for:
- Doğum Tarihi
- Doğum tarihi
- Date of Birth

Return the value associated with that field.

Gender:
Look ONLY for an explicit gender field such as:
- Cinsiyet
- Gender

Possible values may include:
- Erkek
- Kadın
- Male
- Female

If no explicit gender field exists, return null.

Nationality:
Look ONLY for:
- Uyruğu
- Uyruk
- Nationality

Return the actual value.
NEVER return "Uyruğu" itself.

IdentityNumber:
Look ONLY for:
- T.C. Kimlik No
- T.C. Kimlik Numarası
- Kimlik No
- Turkish ID Number

CRITICAL:
The value must be the actual number AFTER the label.

For example:

"T.C. Kimlik No: 12345678901"

must produce:

"IdentityNumber": "12345678901"

NEVER return:
"T.C. Kimlik No"
"Kimlik No"
or any other field label.

If only the label exists and no number is visible, return null.

OUTPUT:

Return EXACTLY this JSON:

{
  "StudentNumber": null,
  "StartedDate": null,
  "EndDate": null,
  "YearOfStudy": null,
  "UniversityName": null,
  "EducationLevel": null,
  "FirstName": null,
  "SecondName": null,
  "ThirdName": null,
  "LastName": null,
  "PassportNumber": null,
  "DateOfBirth": null,
  "Gender": null,
  "Nationality": null,
  "IdentityNumber": null
}

FINAL VALIDATION BEFORE ANSWERING:

For every field:

- Is this an actual value from the document?
- Is it next to or clearly associated with the correct field?
- Is it NOT a field label?
- Did I accidentally use a value belonging to another field?
- Did I invent or calculate anything?

If any answer is uncertain, return null.

DOCUMENT TEXT:

""" + extractedText,

                    stream = false
                };
                
                var response = await client.PostAsJsonAsync(
                        "http://localhost:11434/api/generate",

                        body

                    );

                var responseString = await response.Content.ReadAsStringAsync();

                // 2. Parse the outer JSON wrapper from Ollama
                using JsonDocument doc = JsonDocument.Parse(responseString);
                string extractedJsonString = doc.RootElement.GetProperty("response").GetString();


                return Ok(extractedJsonString);
            }
        }


        // uploading document
        [HttpPost("upload/document/{studentId}/{fileName}")]
        public async Task<IActionResult>  UploadDocument(IFormFile file, int studentId, string fileName )
        {

            if (file == null)
            {

                return BadRequest("File is not uploadded");
            }

            else
            {
                Console.WriteLine($"File name: {file.FileName}");
                Console.WriteLine($"File size: {file.Length}");
                Console.WriteLine($"Content type: {file.ContentType}");

                // First, Store the document in the folder 
                var rootPath = @"C:\Users\kalum\Desktop\Dorm Graduation Project\DormDocuments";
                string studentFolder = Path.Combine(
                    rootPath,
                    "Students",
                    studentId.ToString()
                    
                );

                Directory.CreateDirectory(studentFolder);


                


                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var storedFileName = $"{Guid.NewGuid()}{extension}";

                var filePath = Path.Combine(
                    studentFolder,
                    storedFileName
                );

                await using var stream = new FileStream(
                    filePath,
                    FileMode.Create,
                    FileAccess.Write
                );


                await file.CopyToAsync(stream);

                // Second, Store the path in the Database 
                bool isPathStored = _clsExternalLayerRepo.UploadFilePath(studentId, filePath, fileName, extension);




                return Ok();
            }
        }


        // ==================================
        //          Room Assignement
        // ==================================

        [HttpPost("room/assign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AssignStudentToRoom([FromBody] DTOs.AssignStudentToRoomDTO dto)
        {
            if (dto == null || dto.StudentID <= 0 || dto.RoomID <= 0 || dto.RoomBedID <= 0)
            {
                return BadRequest("Invalid assignment payload.");
            }

            var success = _clsExternalLayerRepo.AssignStudentToRoom(dto, out var errorMessage);
            if (success) return Ok("Student assigned successfully.");

            if (!string.IsNullOrEmpty(errorMessage))
            {
                if (errorMessage.Contains("already assigned")) return Conflict(errorMessage);
                if (errorMessage.Contains("no available beds")) return Conflict(errorMessage);
            }

            return StatusCode(500, errorMessage ?? "An error occurred while assigning the student.");
        }
        [HttpDelete("room/remove/{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult RemoveStudentFromRoom(int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest("Invalid studentId.");
            }

            var success = _clsExternalLayerRepo.RemoveStudentFromRoom(studentId, out var errorMessage);
            if (success) return Ok("Student removed successfully.");

            if (!string.IsNullOrEmpty(errorMessage))
            {
                if (errorMessage.Contains("Student does not")) return Conflict(errorMessage);
                if (errorMessage.Contains("Student is not assigned")) return Conflict(errorMessage);
                if (errorMessage.Contains("Room allocation does not exist")) return Conflict(errorMessage);
            }

            return StatusCode(500, errorMessage ?? "An error occurred while removing the student.");
        }


        // =========================================
        //          STUDENT SECTION
        // =========================================
        [HttpGet("students")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetStudents(int page = 1, int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0) return BadRequest("Invalid paging parameters.");

            var items = _clsExternalLayerRepo.GetAllStudents(page, pageSize);
            var totalCount = _clsExternalLayerRepo.GetStudentsCount();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var response = new Dictionary<string, object>
            {
                { "items", items },
                { "page", page },
                { "pageSize", pageSize },
                { "totalCount", totalCount },
                { "totalPages", totalPages }
            };

            return Ok(response);
        }

        [HttpPost("student/add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddStudent([FromBody] DTOs.AddFullStudentDTO dto)
        {
            if ( string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.PhoneNumber) || string.IsNullOrEmpty(dto.FirstName) || string.IsNullOrEmpty(dto.LastName) || string.IsNullOrEmpty(dto.StudentNumber))
            {
                return BadRequest("Invalid student payload.");
            }

            var result = _clsExternalLayerRepo.AddFullStudent(dto);
            if (!result.Success)
            {
                return StatusCode(500, "An error occurred while adding the student.");
            }

            var response = new {
                StudentId = result.NewStudentId,
                PersonId = result.NewPersonId,
                ContactId = result.NewContactId
            };

            return CreatedAtAction(nameof(GetStudents), new { id = result.NewStudentId }, response);
        }
        [HttpPut("student/update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateStudent([FromBody] DTOs.UpdateFullStudentDTO dto)
        {
            if (dto == null || dto.StudentID <= 0)
            {
                return BadRequest("Invalid student payload.");
            }

            var success = _clsExternalLayerRepo.UpdateFullStudent(dto);
            if (!success) return StatusCode(500, "An error occurred while updating the student.");
            return Ok("Student updated successfully.");
        }

        [HttpDelete("student/delete")]
        public void DeleteStudent()
        {
        }



        // Building Section 
        [HttpGet("buildings/statistics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetBuildingsStatistics()
        {

            var allBuildings = _clsExternalLayerRepo.GetAllBuildingsStatistics();

            if (allBuildings == null)
            {
                return NotFound("Data is empty");
            }

            return Ok(allBuildings);

        }
        [HttpGet("buildings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetBuildings()
        {

            var allBuildings = _clsExternalLayerRepo.GetAllBuildings();

            if(allBuildings == null)
            {
                return NotFound("Data is empty");
            }

            return Ok(allBuildings);

        }

        [HttpPost("building/add")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult AddBuilding([FromBody] BuildingDTO building)
        {
            if(building == null || string.IsNullOrEmpty(building.buildingName))
            {
                // Handle invalid input
                 // Bad Request
                return BadRequest();
            }

            Building newBuilding = new Building
            {
                buildingName = building.buildingName
            };

            // Here you would typically add the newBuilding to your database
            var isAddedSuccessfully = _clsExternalLayerRepo.AddBuilding(newBuilding); // Simulate database operation result

            if(isAddedSuccessfully)
            {
               return CreatedAtAction(nameof(AddBuilding), new { id = newBuilding.buildingId == null ? 0 : newBuilding.buildingId }, newBuilding); // Created
            }
            else
            {
                return StatusCode(500); // Internal Server Error
            }
        }

        [HttpPut("building/update")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult UpdateBuilding([FromBody] Building updatedBuilding)
        {
            if(updatedBuilding == null) { 
                return BadRequest();
            }

            bool isBuildingUpdated = _clsExternalLayerRepo.UpdateBuilding(updatedBuilding);

            if (isBuildingUpdated)
            {
                return CreatedAtAction(nameof(UpdateBuilding), new {id = updatedBuilding.buildingId == null ? 0 : updatedBuilding.buildingId }, updatedBuilding);
            }else
            {
                return NoContent ();
            }

            

        }

        [HttpDelete("building/delete/{buildingId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteBuilding(int buildingId)
        {
            if(buildingId <= 0)
            {
                return BadRequest("Building Id is not correct.");
            }

            bool isBuildingDeleted = _clsExternalLayerRepo.DeleteBuilding(buildingId);

            if (isBuildingDeleted)
            {
                return Ok("Building is deleted successfully.");
            } else
            {
                return StatusCode(500);
            }

            
        }

        // Floor Section
        [HttpGet("floors/{buildingId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetFloors(int buildingId)
        {
            var data = _clsExternalLayerRepo.GetAllFloors(buildingId);

            if(data == null)
            {
                return NotFound();
            }

            return Ok(data);

        }


        [HttpPost("floor/add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        public IActionResult AddFloor([FromBody] FloorDTO floorDto)
        {
            if(floorDto == null || floorDto.floorNumber <= 0 || floorDto.buildingId <= 0)
            {
                return BadRequest("Invalid floor data.");
            }

            Floor newFloor = new Floor
            {
                floorNumber = floorDto.floorNumber,
                buildingId = floorDto.buildingId

            };

            bool isFloorAdded = _clsExternalLayerRepo.AddFloor(newFloor);
            if (isFloorAdded)

            {
                return CreatedAtAction(nameof(AddFloor), new { id = newFloor.floorId == null ? 0 : newFloor.floorId }, newFloor);
            }
            else
            {
                return Conflict();
            }
        }

       

        [HttpPut("floor/update")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult UpdateFloor([FromBody] DormAPI.DTOs.UpdateFloorDTO floorDto)
        {
            if (floorDto == null || floorDto.floorId <= 0 || floorDto.floorNumber <= 0 || floorDto.buildingId <= 0)
            {
                return BadRequest("Invalid floor data.");
            }

            var floorToUpdate = new Floor
            {
                floorId = floorDto.floorId,
                floorNumber = floorDto.floorNumber,
                buildingId = floorDto.buildingId
            };

            bool isFloorUpdated = _clsExternalLayerRepo.UpdateFloor(floorToUpdate);

            if (isFloorUpdated)
            {
                return Ok("Floor updated successfully.");
            }
            else
            {
                return StatusCode(500, "An error occurred while updating the floor.");
            }
        }

        [HttpDelete("floor/delete")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteFloor([FromBody] DormAPI.DTOs.DeleteFloorDTO floorDto)
        {
            if (floorDto == null || floorDto.floorId <= 0 || floorDto.buildingId <= 0)
            {
                return BadRequest("Invalid floor identifier.");
            }

            var floorToDelete = new Floor
            {
                floorId = floorDto.floorId
            };

            bool isFloorDeleted = _clsExternalLayerRepo.DeleteFloor(floorToDelete);

            if (isFloorDeleted)
            {
                return Ok("Floor deleted successfully.");
            }
            else
            {
                return Conflict();
            }
        }

        
        [HttpGet("rooms/status/list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetRoomStatusList()
        {
            

            var data = _clsExternalLayerRepo.GetRoomStatus();

            if (data == null || data.Count == 0) return NotFound();

            return Ok(data);
        }

        [HttpGet("education/level")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetEducationLevels()
        {
            var data = _clsExternalLayerRepo.GetEducationLevels();
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("nationalities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetNationalities()
        {
            var data = _clsExternalLayerRepo.GetNationalities();
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("students/unassigned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetStudentsWithoutAllocation()
        {
            var data = _clsExternalLayerRepo.GetStudentsWithoutAllocation();
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("relationship/types")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetRelationshipTypes()
        {
            var data = _clsExternalLayerRepo.GetRelationshipTypes();
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("universities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetUniversities()
        {
            var data = _clsExternalLayerRepo.GetUniversities();
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("faculties")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetFaculties()
        {
            var data = _clsExternalLayerRepo.GetFaculties();
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("departments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetDepartments()
        {
            var data = _clsExternalLayerRepo.GetDepartments();
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("building/details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetBuildingDetails()
        {
            var data = _clsExternalLayerRepo.GetBuildingDetails();
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("building/statistics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetBuildingStatistics(int? buildingId = null)
        {
            var data = _clsExternalLayerRepo.GetBuildingStatistics(buildingId);
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("students/overview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetStudentCountsOverview()
        {
            try
            {
                var data = _clsExternalLayerRepo.GetStudentCountsOverview();
                if (data == null || data.Count == 0) return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ================================
        //        Dashboard Section
        // ================================
        [HttpGet("dashboard/overview")]
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(DashboardOverviewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetDashboardOverview()
        {
            try
            {
                var data = _clsExternalLayerRepo.GetDashboardOverview();
                if (data == null) return NotFound("Dashboard data not found.");
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ===================================
        //          Room Section 
        // ===================================

        [HttpGet("floors-with-rooms/{buildingId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetFloorsWithRooms(int buildingId)
        {
            if (buildingId <= 0) return BadRequest("Invalid building id.");

            var data = _clsExternalLayerRepo.GetFloorsWithRooms(buildingId);
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("room/details/{floorID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetRoomDetailsById(int floorID)
        {
            if (floorID <= 0) return BadRequest("Invalid floor id.");

            var data = _clsExternalLayerRepo.GetRoomDetailsById(floorID);
            if (data == null || data.Count == 0) return NotFound();
            return Ok(data);
        }

        [HttpGet("rooms/{floorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetRooms(int floorId)
        {
            if (floorId <= 0) return BadRequest("Invalid floor id.");

            var data = _clsExternalLayerRepo.GetAllRooms(floorId);

            if (data == null || data.Count == 0) return NotFound();

            return Ok(data);
        }

        [HttpPost("room/add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddRoom([FromBody] DormAPI.DTOs.RoomAddDTO roomDto)
        {
            if (roomDto == null || roomDto.roomNumber == string.Empty || roomDto.floorId <= 0 || roomDto.capacity <= 0 )
            {
                return BadRequest("Invalid room data.");
            }

            var newRoom = new Room
            {
                roomNumber = roomDto.roomNumber,
                floorId = roomDto.floorId,
                capacity = roomDto.capacity,
                roomStatusId = roomDto.roomStatusId
            };

           

            bool isAdded = _clsExternalLayerRepo.AddRoom(newRoom);
            if (isAdded)
            {
                return CreatedAtAction(nameof(AddRoom), new { id = newRoom.roomId }, newRoom);
            }
            return StatusCode(500, "An error occurred while adding the room.");
        }

        [HttpPut("room/update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateRoom([FromBody] DormAPI.DTOs.RoomUpdateDTO roomDto)
        {
            if (roomDto == null || roomDto.roomId <= 0 || roomDto.roomNumber == string.Empty || roomDto.floorId <= 0 || roomDto.capacity <= 0 )
            {
                return BadRequest("Invalid room data.");
            }

            var roomToUpdate = new Room
            {
                roomId = roomDto.roomId,
                roomNumber = roomDto.roomNumber,
                floorId = roomDto.floorId,
                capacity = roomDto.capacity,
                roomStatusId = roomDto.roomStatusId
                
            };

            bool isUpdated = _clsExternalLayerRepo.UpdateRoom(roomToUpdate);
            if (isUpdated) return Ok("Room updated successfully.");
            return StatusCode(500, "An error occurred while updating the room.");
        }

        [HttpDelete("room/delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteRoom([FromBody] DormAPI.DTOs.RoomDeleteDTO roomDto)
        {
            if (roomDto == null || roomDto.roomId <= 0 || roomDto.floorId <= 0)
            {
                return BadRequest("Invalid room identifier.");
            }

            var roomToDelete = new Room
            {
                roomId = roomDto.roomId,
                floorId = roomDto.floorId
            };

            bool isDeleted = _clsExternalLayerRepo.DeleteRoom(roomToDelete);
            if (isDeleted) return Ok("Room deleted successfully.");
            return StatusCode(500, "An error occurred while deleting the room.");
        }


        [HttpGet("room/available")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableRooms()
        {


            var availabe = await _clsExternalLayerRepo.GetAvailableRooms();

            if (availabe == null)
                return NotFound("Rooms are not found.");

            return Ok(availabe);
        }
        




        // ================================
        //         Financial / Income Section
        // ================================

        [HttpGet("income/all")]
        public async Task<IActionResult> GetIncomesOverview()
        {
            var list = await _clsExternalLayerRepo.GetIncomesOverview();
            return Ok(list);
        }

        [HttpGet("income/categories")]
        public async Task<IActionResult> GetIncomeCategories()
        {
            var list = await _clsExternalLayerRepo.GetIncomeCategories();
            return Ok(list);
        }

        [HttpPost("income/add")]
        public async Task<IActionResult> AddIncome([FromBody] AddIncomeDTO dto)
        {
            if (dto == null || dto.IncomeCategoryID <= 0 || dto.IncomeAmount <= 0)
            {
                return BadRequest("Invalid income data. Category and positive amount are required.");
            }

            if (dto.IncomeDate == default)
            {
                dto.IncomeDate = DateTime.Now;
            }

            bool success = await _clsExternalLayerRepo.AddIncome(dto);
            if (success)
            {
                return Ok(new { message = "Income added successfully." });
            }

            return StatusCode(500, "An error occurred while adding the income.");
        }

        [HttpGet("income/statistics")]
        public async Task<IActionResult> GetIncomeStatistics()
        {
            var stats = await _clsExternalLayerRepo.GetIncomeStatisticsAsync();
            return Ok(stats);
        }

        [HttpGet("financial/overview-bundle")]
        public async Task<IActionResult> GetFinancialOverviewBundle()
        {
            var bundle = await _clsExternalLayerRepo.GetFinancialOverviewBundleAsync();
            return Ok(bundle);
        }

        // ================================
        //         Financial / Expense Section
        // ================================

        [HttpGet("expense/all")]
        public async Task<IActionResult> GetExpensesOverview()
        {
            var list = await _clsExternalLayerRepo.GetExpensesOverview();
            return Ok(list);
        }

        [HttpGet("expense/categories")]
        public async Task<IActionResult> GetExpenseCategories()
        {
            var list = await _clsExternalLayerRepo.GetExpenseCategories();
            return Ok(list);
        }

        [HttpPost("expense/add")]
        public async Task<IActionResult> AddExpense([FromBody] AddExpenseDTO dto)
        {
            if (dto == null || dto.ExpenseGategoryID <= 0 || dto.ExpenseAmount <= 0)
            {
                return BadRequest("Invalid expense data. Category and positive amount are required.");
            }

            if (dto.ExpenseDate == default)
            {
                dto.ExpenseDate = DateTime.Now;
            }

            bool success = await _clsExternalLayerRepo.AddExpense(dto);
            if (success)
            {
                return Ok(new { message = "Expense added successfully." });
            }

            return StatusCode(500, "An error occurred while adding the expense.");
        }

        // ================================
        //      Student Deposit Section
        // ================================

        [HttpPost("student/deposit/add")]
        public async Task<IActionResult> AddStudentDeposit([FromBody] AddStudentDepositDTO dto)
        {
            if (dto == null || dto.StudentID <= 0 || dto.DepositAmount <= 0)
            {
                return BadRequest("Invalid deposit data. StudentID and positive DepositAmount are required.");
            }

            if (dto.DepositDate == default)
            {
                dto.DepositDate = DateTime.Now;
            }

            bool success = await _clsExternalLayerRepo.AddStudentDepositAsync(dto);
            if (success)
            {
                return Ok(new { message = "Deposit added successfully." });
            }

            return StatusCode(500, "An error occurred while adding the deposit.");
        }

        [HttpGet("student/deposit/{studentId}")]
        public async Task<IActionResult> GetStudentDeposits(int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest("Invalid student ID.");
            }

            var list = await _clsExternalLayerRepo.GetStudentDepositsAsync(studentId);
            return Ok(list);
        }

        [HttpPut("student/deposit/refund/{depositId}")]
        public async Task<IActionResult> RefundStudentDeposit(int depositId)
        {
            if (depositId <= 0)
            {
                return BadRequest("Invalid deposit ID.");
            }

            bool success = await _clsExternalLayerRepo.RefundStudentDepositAsync(depositId);
            if (success)
            {
                return Ok(new { message = "Deposit refunded successfully." });
            }

            return StatusCode(500, "An error occurred while refunding the deposit.");
        }

    }
}


