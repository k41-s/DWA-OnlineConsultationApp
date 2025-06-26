namespace OnlineConsultationApp.core.DTOs
{
    public class MentorDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;

        // TypeOfWork details
        public int TypeOfWorkId { get; set; }
        public string TypeOfWorkName { get; set; } = null!;
        public string? ImagePath { get; set; }

        // List of Areas (ids and names)
        public List<int> AreaIds { get; set; } = new();
        public List<string> AreaNames { get; set; } = new();
    }
}
