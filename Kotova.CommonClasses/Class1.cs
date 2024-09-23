using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;

namespace Kotova.CommonClasses
{
    public static class DataBaseNames
    {
        public const string testDB_USER_instructionId = "instruction_id"; // Заполни остальное

        public const string tableName_sql_index = "index";
        public const string tableName_sql_names = "names";
        public const string tableName_sql_jobPosition = "job_position";
        public const string tableName_sql_isDriver = "is_driver";
        public const string tableName_sql_BirthDate = "birth_date";
        public const string tableName_sql_gender = "gender";
        public const string tableName_sql_PN = "personnel_number";
        public const string tableName_sql_department = "department";
        public const string tableName_sql_group = "group";

        public const string tableName_sql_MainName = "dbo.TableTest";
        public const string tableName_Instructions_sql = "dbo.Instructions";
        public const string connectionString_server = "localhost";
        public const string connectionString_database = "TestDB";

        public const string tableName_pos_users = "users";
        public const string columnName_sql_pos_users_username = "username";
        public const string columnName_sql_pos_users_PN = "current_personnel_number";

        public const string tableName_sql_USER_instruction_id = "instruction_id";
        public const string tableName_sql_USER_is_instruction_passed = "is_instruction_passed";
        public const string tableName_sql_USER_datePassed = "date_when_passed";
        public const string tableName_sql_INSTRUCTIONS_cause = "cause_of_instruction";
        public const string tableName_sql_USER_whenWasSendByHeadOfDepartment = "when_was_send_to_user";
        public const string tableName_sql_USER_whenWasSendByHeadOfDepartment_UTCTime = "when_was_send_to_user_UTC_Time";
        public const string tableName_sql_pathToInstruction = "path_to_instruction";
    }

    public class Instruction
    {
        [Key]
        public int instruction_id { get; set; }
        public DateTime begin_date { get; set; }
        public DateTime end_date { get; set; }
        public string? path_to_instruction { get; set; }
        public string cause_of_instruction { get; set; }
        public Byte type_of_instruction { get; set; }
        public bool is_passed_by_everyone { get; set; }
        public bool is_assigned_to_people { get; set; }
        [JsonIgnore]
        public virtual ICollection<FilePath> FilePaths { get; set; } = new List<FilePath>();

        public Instruction() { }
        public Instruction(string CauseOfInstruction_)
        {
            cause_of_instruction = CauseOfInstruction_;
        }
        public Instruction(string CauseOfInstruction_, DateTime BeginDate_, DateTime EndDate_, string? PathToInstruction_, Byte TypeOfInstruction_)
        {
            cause_of_instruction = CauseOfInstruction_;
            begin_date = BeginDate_;
            end_date = EndDate_;
            path_to_instruction = PathToInstruction_;
            type_of_instruction = TypeOfInstruction_;
            is_passed_by_everyone = false;
            is_assigned_to_people = false;
            
        }
    }
    public class FullCustomInstruction
    {
        public Instruction _instruction { get; set; }
        public List<string?> _paths { get; set; }
        public FullCustomInstruction(Instruction instruction, List<string?> paths)
        {
            _instruction = instruction;
            _paths = paths;
        }
        public FullCustomInstruction()
        {

        }
    }

    public class DynamicEmployeeInstruction
    {
        public int instruction_id { get; set; }
        public bool? is_instruction_passed { get; set; }
        public DateTime? date_when_passed { get; set; }
        public DateTime? date_when_passed_UTC_Time { get; set; }
        public DateTime? when_was_send_to_user { get; set; }
        public DateTime? when_was_send_to_user_UTC_Time { get; set; }
        public string was_signed_by_PN { get; set; }
    }

    [Table("Tasks", Schema = "UsersSchema")]
    public class TaskForUser
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public int? UserRole { get; set; }

        [MaxLength(10)]
        public string? AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? DueDate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Не назначено";

        public bool IsDeleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }
    }

    public class InstructionExportRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Byte> InstructionTypes { get; set; }

        public InstructionExportRequest(DateTime startDate, DateTime endDate, List<Byte> instructionTypes)
        {
            StartDate = startDate;
            EndDate = endDate;
            InstructionTypes = instructionTypes;
            Validate();
        }

        private void Validate()
        {
            if (StartDate > DateTime.Today)
                throw new ArgumentException("Начальная дата не может быть больше чем сегодня");

            if (EndDate > DateTime.Today)
                throw new ArgumentException("Конечная дата не может быть больше чем сегодня");

            if (InstructionTypes == null || InstructionTypes.Count == 0)
                throw new ArgumentException("InstructionTypes должна содержать хотя бы один тип инструктажей");
        }
    }

    public class InstructionExportResponse
    {
        public DateTime ExportDate { get; set; }
        public List<string> InstructionTypes { get; set; }
        public List<InstructionExportInstance> ListOfInstructions { get; set; }
        public InstructionExportResponse(DateTime exportDate, List<string> instructionTypes, List<InstructionExportInstance> listOfInstructions)
        {
            ExportDate = exportDate;
            InstructionTypes = instructionTypes;
            ListOfInstructions = listOfInstructions;
        }
    }

    [NotMapped]
    public class InstructionExportInstance
    {
        public int InstructionId { get; set; }
        public DateTime DateWhenPassedByEmployee { get; set; }
        public string FullNameOfEmployee { get; set; }
        public string PositionOfEmployee { get; set; }
        public DateTime BirthDateOfEmployee { get; set; }
        public Byte InstructionType { get; set; }
        public string CauseOfInstruction { get; set; }
        public string FullNameOfEmployeeWhoConductedInstruction { get; set; }
        [NotMapped]
        public List<string?>? FileNamesOfInstruction { get; set; }
        [NotMapped]
        public string? FileNamesOfInstructionInOneString { get; set; }

        InstructionExportInstance() { }
    }

    public class CustomTask
    {
        public string Description { get; set; }
        public int DepartmentId { get; set; }
        public int UserRole { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public CustomTask(string description, int departmentId, int userRole, DateTime dueDate, string? assignedTo = null, string status = "Не назначено")
        {
            Description = description;
            DepartmentId = departmentId;
            UserRole = userRole;
            DueDate = dueDate;
            AssignedTo = assignedTo;
            Status = status;
        }

    }

    public class TaskDto
    {
        public int TaskId { get; set; }
        public string Description { get; set; }
    }

    public class FilePath
    {
        [Key]
        public int path_id { get; set; }
        public int instruction_id { get; set; }
        public string file_path { get; set; }
        [JsonIgnore]
        public virtual Instruction Instruction { get; set; }
    }

    public class QueryResult
    {
        public List<Dictionary<string, object>> Result1 { get; set; }
        public List<Dictionary<string, object>> Result2 { get; set; }
    }
    public class InstructionDto
    {
        public int InstructionId { get; set; }
        public string TenDigitNumber { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class Employee
    {
        [Key]
        public string personnel_number { get; set; }
        public string full_name { get; set; }
        public string job_position { get; set; }
        public string department { get; set; }
        public string? group { get; set; }
        public DateTime birth_date { get; set; }
        public Byte? gender { get; set; }
        public bool is_driver { get; set; }

    }

    public class Department
    {
        [Key]
        public int department_id { get; set; }
        public string department_name { get; set; }
        public string department_schema {  get; set; }
        public string department_DB_name { get; set; }
        public bool is_chief_online { get; set; }
        public DateTime? last_online_set_UTC { get; set; }
    }
    public class Dept
    {
        public string DepartmentId { get; set; }
        public string Name { get; set; }
        public List<Employee> Employees { get; set; }
    }
    public class Role
    {
        [Key]
        public int roleid { get; set; }
        public string roletype { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }

    public class UserCredentials
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
    }

    public class InstructionPackage
    {
        [Required(ErrorMessage = "Names list cannot be empty.")]
        [MinLength(1, ErrorMessage = "At least one name with birthday is required.")]
        public List<Tuple<string, string>>? NamesAndBirthDates { get; set; }

        [Required(ErrorMessage = "Instruction name is required.")]
        public string? InstructionCause { get; set; }

        public InstructionPackage() { }
        public InstructionPackage(List<Tuple<string,string>>? namesAndBirthDates, string instruction)
        {
            NamesAndBirthDates = namesAndBirthDates;
            InstructionCause = instruction;
        }

    }

    public class UnplannedInstructionPackage
    {
        [Required(ErrorMessage = "Department list cannot be empty.")]
        [MinLength(1, ErrorMessage = "At least one department name is required.")]
        public List<string>? DepartmentNames { get; set; }

        public FullCustomInstruction? FullInstruction { get; set; }

        public UnplannedInstructionPackage() { }
        public UnplannedInstructionPackage(List<string>? departmentNames, FullCustomInstruction fullInstruction)
        {
            DepartmentNames = departmentNames;
            FullInstruction = fullInstruction;
        }
    }

    #region Encryption
    public class Encryption_Kotova
    {
        public static string HashPassword(string password)
        {
            // Hash the password using BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Verify the password against the hashed password
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        // Business logic methods here
        public static string EncryptString(string clearText) // use AES or something! encrypt and transfer over https.
        {
            return clearText;
        }
        public static string DecryptString(string clearText) // use AES or something! encrypt and transfer over https.
        {
            return clearText;
        }
        public static List<string> EncryptListOfStrings(List<string> clearList) // use json serealize list of strings into one strings or something.
        {
            List<string> encryptedList = new List<string>();
            foreach (string str in clearList)
            {
                encryptedList.Add(EncryptString(str));
            }
            return encryptedList;
        }
        public static string EncryptDictionary(Dictionary<string, string> dictionary) // use json serealize list of strings into one strings or something.
        {
            string serializedDictionary = SerializeDictionaryToJson(dictionary);

            return EncryptString(serializedDictionary);
        }
        public static string EncryptListOfTuples(List<Tuple<string, string>> listOfTuples) // use json serealize list of strings into one strings or something.
        {
            string serializedDictionary = JsonConvert.SerializeObject(listOfTuples);

            return EncryptString(serializedDictionary);
        }
        public static string SerializeDictionaryToJson(Dictionary<string, string> dictionary)
        {
            return JsonConvert.SerializeObject(dictionary);
        }
    }
    public class InstructionForChief
    {
        public int InstructionId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CauseOfInstruction { get; set; }
        public string TypeOfInstruction { get; set; }
        public List<PersonStatus> Persons { get; set; } = new List<PersonStatus>();

        public class PersonStatus
        {
            public string PersonnelNumber { get; set; }
            public string PersonName { get; set; }
            public bool Passed { get; set; }
            public override string ToString()
            {
                return PersonName;
            }
        }
        
    }

    #endregion

}
