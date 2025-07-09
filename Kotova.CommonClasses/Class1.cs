using BCrypt.Net;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

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
        [Required]
        public DateTime begin_date { get; set; }
        [Required]
        public DateTime end_date { get; set; }
        [Required]
        public string? path_to_instruction { get; set; }
        [Required]
        public string cause_of_instruction { get; set; }
        [Required]
        public Byte type_of_instruction { get; set; }
        public bool is_passed_by_everyone { get; set; }
        public bool is_assigned_to_people { get; set; }
        public bool is_passed_by_chief_unplanned_instr { get; set; }
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
        public Instruction Instruction { get; set; }
        public List<string> Paths { get; set; }

        public FullCustomInstruction(Instruction instruction, List<string> paths)
        {
            Instruction = instruction;
            Paths = paths;
        }
    }

    public class EmployeeInfo
    {
        public string FullName { get; set; }
        public string BirthDate { get; set; }
        public string Role { get; set; }
    }

    public class NormativeInstructionInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsUnplannedInstruction { get; set; } // Add this property
    }


    public class UnplannedInstructionAssignmentPackage
    {
        [Required]
        public string InstructionCause { get; set; }

        [Required]
        public int InstructionId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one employee must be selected")]
        public List<SelectedEmployeeDto> SelectedEmployees { get; set; }

        public List<int> NormativeInstructionNameIds { get; set; } = new List<int>();
    }

    public class SelectedEmployeeDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string BirthDate { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class EmployeeComparisonDto
    {
        public string PersonnelNumber { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string Email { get; set; }
        public bool HasDifferences { get; set; }
        public List<string> DifferenceFields { get; set; } = new List<string>();

        // Data from LynksDataBase
        public TelpEmployeeDto LynksData { get; set; }

        // Data from Трансэлектропроект DataBase
        public TransElectroEmployeeDto TransElectroData { get; set; }

        // NEW: Color coding properties
        /// <summary>
        /// Color for the row display in UI
        /// Values: "Red", "Green", "Blue", "Yellow"
        /// </summary>
        public string RowColor { get; set; }

        /// <summary>
        /// Human-readable status message explaining the color
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Helper property to get differences as a readable string
        /// </summary>
        public string DifferenceFieldsString
        {
            get
            {
                if (DifferenceFields == null || !DifferenceFields.Any())
                    return string.Empty;
                return string.Join(", ", DifferenceFields);
            }
        }

        /// <summary>
        /// Helper property to determine the color based on the logic:
        /// - Red: Has differences between databases
        /// - Green: No differences - data is identical
        /// - Blue: Exists only in Lynks database
        /// - Yellow: Exists only in TransElectro database (shouldn't happen)
        /// </summary>
        public string GetColorCode()
        {
            if (!string.IsNullOrEmpty(RowColor))
                return RowColor;

            // Fallback logic if RowColor is not set
            if (LynksData != null && TransElectroData != null)
            {
                // Both exist
                return HasDifferences ? "Red" : "Green";
            }
            else if (LynksData != null && TransElectroData == null)
            {
                // Only in Lynks
                return "Blue";
            }
            else if (LynksData == null && TransElectroData != null)
            {
                // Only in TransElectro
                return "Yellow";
            }

            return "Gray"; // Default/unknown state
        }

        /// <summary>
        /// Helper property to get status message based on color
        /// </summary>
        public string GetStatusMessage()
        {
            if (!string.IsNullOrEmpty(StatusMessage))
                return StatusMessage;

            // Fallback logic if StatusMessage is not set
            return GetColorCode() switch
            {
                "Red" => "Есть различия в данных",
                "Green" => "Данные идентичны",
                "Blue" => "Есть только в базе Lynks",
                "Yellow" => "Так не должно было быть - есть только в базе TransElectro",
                _ => "Неизвестный статус"
            };
        }
    }

    public class TransElectroEmployeeDto
    {
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string Email { get; set; }
        public string PersonnelNumber { get; set; }
    }


    public class ChiefInfo
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string JobPosition { get; set; }
    }

    // Keep the existing models for backwards compatibility
    public class UnplannedInstructionForChiefsPackage
    {
        [Required]
        public InstructionCreateDto Instruction { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one chief must be selected")]
        public List<int> SelectedChiefIds { get; set; }

        public List<string> FilePaths { get; set; }

        public List<int> NormativeInstructionIds { get; set; }

        // Add this property for normative base text processing
        public string NormativeBaseText { get; set; }

        // Add this property to indicate if created normative instructions should be marked as unplanned
        public bool MarkNormativeAsUnplanned { get; set; } = true;
    }

    public class NormativeInstructionFilterDto
    {
        public bool? IsUnplannedInstruction { get; set; }
        public string SearchTerm { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }

    public class InstructionCreateDto
    {
        [Required]
        [StringLength(500)]
        public string CauseOfInstruction { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0, 5)]
        public byte TypeOfInstruction { get; set; }
    }


    public class NormativeInstructionCreateDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsUnplannedInstruction { get; set; } = false;
    }

    public class NormativeInstructionUpdateDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsUnplannedInstruction { get; set; } = false;
    }

    public class NormativeInstructionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsUnplannedInstruction { get; set; } // Add this property
    }

    public class UnplannedInstructionStatusDto
    {
        public int InstructionId { get; set; }
        public string CauseOfInstruction { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TypeName { get; set; }
        public int TotalAssigned { get; set; }
        public int TotalPassed { get; set; }
        public List<ChiefStatusDto> ChiefStatuses { get; set; } = new List<ChiefStatusDto>();
    }

    public class ChiefStatusDto
    {
        public string ChiefName { get; set; }
        public string DepartmentName { get; set; }
        public string JobPosition { get; set; }
        public bool IsPassed { get; set; }
        public DateTime? DatePassed { get; set; }
        public DateTime? DateAssigned { get; set; }
    }

    #region DTOs for Data Download

    /// <summary>
    /// DTO for department data download
    /// </summary>
    public class DepartmentDownloadDto
    {
        [Required]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(255)]
        public string DepartmentName { get; set; } = string.Empty;

        public bool IsChiefOnline { get; set; }

        public DateTime? LastOnlineSetUTC { get; set; }

        public byte CodeNumberTelpDb { get; set; }
    }

    /// <summary>
    /// DTO for role data download
    /// </summary>
    public class RoleDownloadDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string RoleNameRussian { get; set; } = string.Empty;
    }

    #endregion

    #region Response DTOs

    /// <summary>
    /// Response model for departments and roles combined endpoint
    /// </summary>
    public class DepartmentsAndRolesResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("departments")]
        public DepartmentsData? Departments { get; set; }

        [JsonProperty("roles")]
        public RolesData? Roles { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Container for departments data in combined response
    /// </summary>
    public class DepartmentsData
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("data")]
        public List<DepartmentDownloadDto>? Data { get; set; }
    }

    /// <summary>
    /// Container for roles data in combined response
    /// </summary>
    public class RolesData
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("data")]
        public List<RoleDownloadDto>? Data { get; set; }
    }

    #endregion

    // Add these new DTOs to the existing CommonClasses/Class1.cs file
    // These should be added at the end of the file, before the closing namespace brace

    #region Coordinator Window DTOs

    /// <summary>
    /// Data transfer object for initial instruction people in coordinator interface
    /// </summary>
    public class InitialInstructionPersonDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for new employee creation with property change notifications
    /// </summary>
    public class NewEmployeeDto : INotifyPropertyChanged
    {
        private string _fullName = string.Empty;
        private string _position = string.Empty;
        private string _personnelNumber = string.Empty;
        private string _workplaceNumber = string.Empty;
        private DateTime? _birthDate;
        private string _department = string.Empty;
        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _role = string.Empty;
        private bool _addInitialInstruction;

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        public string Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }

        public string PersonnelNumber
        {
            get => _personnelNumber;
            set { _personnelNumber = value; OnPropertyChanged(); }
        }

        public string WorkplaceNumber
        {
            get => _workplaceNumber;
            set { _workplaceNumber = value; OnPropertyChanged(); }
        }

        public DateTime? BirthDate
        {
            get => _birthDate;
            set { _birthDate = value; OnPropertyChanged(); }
        }

        public string Department
        {
            get => _department;
            set { _department = value; OnPropertyChanged(); }
        }

        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }

        public bool AddInitialInstruction
        {
            get => _addInitialInstruction;
            set { _addInitialInstruction = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Data transfer object for user instructions in coordinator interface
    /// </summary>
    public class UserInstructionDto
    {
        public string CauseOfInstruction { get; set; } = string.Empty;
        public List<InstructionFileDto> Files { get; set; } = new List<InstructionFileDto>();
    }

    /// <summary>
    /// Data transfer object for instruction files
    /// </summary>
    public class InstructionFileDto
    {
        public string FileName { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
    }

    /// <summary>
    /// Enhanced normative instruction DTO with property change notifications for UI binding
    /// </summary>
    public class NormativeInstructionDtoWithNotification : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _url = string.Empty;
        private DateTime _createdAt;
        private bool _isUnplannedInstruction;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Url
        {
            get => _url;
            set { _url = value; OnPropertyChanged(); }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(); }
        }

        public bool IsUnplannedInstruction
        {
            get => _isUnplannedInstruction;
            set { _isUnplannedInstruction = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Convert from existing NormativeInstructionDto to the notification version
        /// </summary>
        public static NormativeInstructionDtoWithNotification FromNormativeInstructionDto(NormativeInstructionDto dto)
        {
            return new NormativeInstructionDtoWithNotification
            {
                Id = dto.Id,
                Name = dto.Name,
                Url = dto.Url,
                CreatedAt = dto.CreatedAt,
                IsUnplannedInstruction = dto.IsUnplannedInstruction
            };
        }

        /// <summary>
        /// Convert to existing NormativeInstructionDto
        /// </summary>
        public NormativeInstructionDto ToNormativeInstructionDto()
        {
            return new NormativeInstructionDto
            {
                Id = this.Id,
                Name = this.Name,
                Url = this.Url,
                CreatedAt = this.CreatedAt,
                IsUnplannedInstruction = this.IsUnplannedInstruction
            };
        }
    }

    /// <summary>
    /// Data transfer object for coordinator reports
    /// </summary>
    public class CoordinatorReportDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? InstructionType { get; set; }
        public string? Department { get; set; }
        public string? AdditionalFilter { get; set; }
    }

    /// <summary>
    /// Data transfer object for coordinator dashboard statistics
    /// </summary>
    public class CoordinatorDashboardDto
    {
        public int TotalEmployees { get; set; }
        public int PendingInitialInstructions { get; set; }
        public int CompletedInstructionsThisMonth { get; set; }
        public int OverdueInstructions { get; set; }
        public List<DepartmentStatsDto> DepartmentStats { get; set; } = new List<DepartmentStatsDto>();
    }

    /// <summary>
    /// Data transfer object for department statistics
    /// </summary>
    public class DepartmentStatsDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public int CompletedInstructions { get; set; }
        public int PendingInstructions { get; set; }
        public double CompletionPercentage { get; set; }
    }

    #endregion

    // Add the necessary using statements at the top of the Class1.cs file:
    // using System.ComponentModel;
    // using System.Runtime.CompilerServices;


    /// <summary>
    /// Data transfer object for an instruction with employee status information for chiefs
    /// </summary>
    public class InstructionForChiefDto
    {
        public int InstructionId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CauseOfInstruction { get; set; }
        public string TypeOfInstruction { get; set; }
        public bool IsPassedByEveryone { get; set; }
        public double PassedPercentage { get; set; }
        public List<PersonStatusDto> Persons { get; set; } = new List<PersonStatusDto>();

        // Helper property to get total count of people
        public int TotalPeopleCount => Persons?.Count ?? 0;

        // Helper property to get count of people who have passed
        public int PassedPeopleCount => Persons?.Count(p => p.Passed) ?? 0;

        // Helper property to get completion percentage
        public int CompletionPercentage => TotalPeopleCount > 0
            ? (int)((PassedPeopleCount / (double)TotalPeopleCount) * 100)
            : 0;
    }


    public class InstructionReportItem
    {
        public int InstructionId { get; set; }
        public string CauseOfInstruction { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte TypeOfInstruction { get; set; }
        public string TypeName { get; set; }
        public bool IsAssignedToPeople { get; set; }
        public bool IsPassedByEveryone { get; set; }
        public List<EmployeeComplianceItem> EmployeeData { get; set; } = new List<EmployeeComplianceItem>();
    }

    public class EmployeeComplianceItem
    {
        public string FullName { get; set; }
        public string Position { get; set; }
        public DateTime BirthDate { get; set; }
        public bool HasPassed { get; set; }
        public DateTime? DatePassed { get; set; }
        public DateTime? DateAssigned { get; set; }
        public string AssignedBy { get; set; }
        public List<string> NormativeDocuments { get; set; } = new List<string>();
    }

    public class PersonStatusDto
    {
        public string PersonnelNumber { get; set; }
        public string PersonName { get; set; }
        public bool Passed { get; set; }
        public DateTime? DatePassed { get; set; }

        public override string ToString()
        {
            return PersonName;
        }
    }

    // Add these DTOs to the bottom of your controller or create a separate DTOs file

    public class DepartmentWithChiefsDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public List<ChiefDto> Chiefs { get; set; } = new List<ChiefDto>();
    }

    public class ChiefDto
    {
        public int UserId { get; set; }
        public int PersonnelId { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string JobPosition { get; set; }
    }


    public class DepartmentDto
    {
        public int department_id { get; set; }
        public string department_name { get; set; }

        // Property aliases for the Dictionary conversion
        public int Id => department_id;
        public string Name => department_name;
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

        public InstructionExportInstance() { }
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

    public class EmployeeCreationResponse
    {
        public string Message { get; set; }
        public int PersonnelId { get; set; }
        public string PersonnelNumber { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public bool InitialInstructionCreated { get; set; }
        public UserCredentialsResult UserCredentials { get; set; }
    }

    public class UserCredentialsResult
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
        public string DeskNumber { get; set; }
        public bool Success { get; set; }
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
    /*public class InstructionDto
    {
        public int InstructionId { get; set; }
        public string TenDigitNumber { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }*/

    // Should it be here in terms of safety? TODO: Check it - if it safe.
    // ANd create safechecker(Обработка пустых/нулевых значений for this class)
    public class TelpEmployeeDto
    {
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string Email { get; set; }
        public string PersonnelNumber { get; set; }
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
        public bool? is_working_in_department { get; set; }
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
        public byte code_number_TELP_DB { get; set; }
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

        public List<int>? NormativeInstructionNameIds { get; set; }

        public InstructionPackage() { }

        public InstructionPackage(List<Tuple<string, string>>? namesAndBirthDates, string instruction, List<int>? normativeInstructionNameIds = null)
        {
            NamesAndBirthDates = namesAndBirthDates;
            InstructionCause = instruction;
            NormativeInstructionNameIds = normativeInstructionNameIds;
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

    public class UnplannedInstructionForDepartmentsPackage
    {
        [Required]
        public InstructionCreateDto Instruction { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one department must be selected")]
        public List<int> SelectedDepartmentIds { get; set; }

        public List<string> FilePaths { get; set; }

        public List<int> NormativeInstructionIds { get; set; }

        // Add this property for the processed normative base text
        public string NormativeBaseText { get; set; }

        // Add this property to indicate if created normative instructions should be marked as unplanned
        public bool MarkNormativeAsUnplanned { get; set; } = true;
    }


    /// <summary>
    /// Data transfer object for updating an existing instruction
    /// </summary>
    public class InstructionUpdateDto
    {
        /// <summary>
        /// The cause or reason for this instruction
        /// </summary>
        public string CauseOfInstruction { get; set; }

        /// <summary>
        /// The end date of the instruction - when it expires
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The type of instruction (0=Introductory, 1=Unplanned, 2=Primary, etc.)
        /// </summary>
        public byte TypeOfInstruction { get; set; }
    }

    public class EmployeeCreationDto
    {
        public string PersonnelNumber { get; set; }
        public string FullName { get; set; }
        public string JobPosition { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public string? Email { get; set; }
        public string? WorkplaceNumber { get; set; }
        public string? Group { get; set; }
        public DateTime BirthDate { get; set; }
        public byte? Gender { get; set; } = 3;
        public bool IsDriver { get; set; } = false;
        public bool? IsWorkingInDepartment { get; set; } = true;
    }

    public class EmployeeCreationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? PersonnelId { get; set; }
        public string PersonnelNumber { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public bool InitialInstructionCreated { get; set; }
        public UserCredentialsInfo? UserCredentials { get; set; }
    }

    public class UserCredentialsInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string DeskNumber { get; set; }
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
