namespace RentApi.Dtos
{
    public class BookingUpdateDto
    {
        public int? PropertyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
