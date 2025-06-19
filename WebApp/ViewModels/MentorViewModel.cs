namespace WebApp.ViewModels
{
    public class MentorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int TypeOfWorkId { get; set; }
        public string TypeOfWorkName { get; set; } = string.Empty;
        public List<int> AreaIds { get; set; } = new();
        public List<string> AreaNames { get; set; } = new();
    }
}
