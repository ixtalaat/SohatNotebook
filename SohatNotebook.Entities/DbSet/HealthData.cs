namespace SohatNotebook.Entities.DbSet;

public class HealthData : BaseEntity
{
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public string BloodType { get; set; } = string.Empty; // TODO: make this information based on Enum
    public string Race { get; set; } = string.Empty;
    public bool UseGlasses { get; set; }
}
