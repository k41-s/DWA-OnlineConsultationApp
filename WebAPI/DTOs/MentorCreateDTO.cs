namespace WebAPI.DTOs
{
    public class MentorCreateDTO
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int TypeOfWorkId { get; set; }
        public List<int> AreaIds { get; set; }
    }
}
