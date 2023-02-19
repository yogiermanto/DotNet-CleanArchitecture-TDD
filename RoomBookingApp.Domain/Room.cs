namespace RoomBookingApp.Domain;

public class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<RoomBooking>? RoomBookings { get; set; }
}