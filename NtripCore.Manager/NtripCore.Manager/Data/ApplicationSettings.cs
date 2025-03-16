using System.ComponentModel.DataAnnotations;

namespace NtripCore.Manager.Data
{
    public class ApplicationSettings
    {
        [Key]
        public string Id { get; set; }

        public string Value { get; set; }
    }
}
