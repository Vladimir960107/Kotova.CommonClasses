using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kotova.CommonClasses
{
    public class Notification
    {
        public string? NotificationID { get; set; }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsForDrivers { get; set; }
        public string? PathToInstruction { get; set; }
        public string? NameOfInstruction { get; set; }

        public Notification() { }
        public Notification(string NameOfInstruction_)
        {
            NameOfInstruction = NameOfInstruction_;
        }
    }


    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
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
    #region Encryption
    public class Encryption_Kotova
    {
        // Business logic methods here
        public static string EncryptString(string clearText) // use AES or something! encrypt and transfer over https.
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
    #endregion
}
