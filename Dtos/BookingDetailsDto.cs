namespace RentApi.Dtos
{
    public class BookingDetailsDto
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string PropertyTitle { get; set; }
        public string PropertyLocation { get; set; }

        public string RenterFullName { get; set; }
    }
}
