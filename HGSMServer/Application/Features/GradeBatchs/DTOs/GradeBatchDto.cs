namespace Application.Features.GradeBatchs.DTOs
{
    public class GradeBatchDto
    {
        public int BatchID { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int SemesterId { get; set; }
    }
}
