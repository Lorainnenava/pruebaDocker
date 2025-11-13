using System.ComponentModel.DataAnnotations;

namespace MyApp.Domain.Entities
{
    public class IdentificationTypesEntity
    {
        [Key]
        public int IdentificationTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSystemDefined { get; set; } = true;
    }
}
