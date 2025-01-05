namespace TechnicalAssessment.Model
{
    public class Request
    {
        public string Originator { get; set; }
        public string FileName { get; set; }
        public List<string> FileContentLines { get; set; }
    }
}
