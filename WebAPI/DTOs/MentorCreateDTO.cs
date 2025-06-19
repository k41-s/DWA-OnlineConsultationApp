namespace WebAPI.DTOs
{
    public class MentorCreateDTO
    {
        public int TypeOfWorkId { get; set; }
        public int UserId { get; set; }
        public List<int> AreaIds { get; set; }
    }
}
