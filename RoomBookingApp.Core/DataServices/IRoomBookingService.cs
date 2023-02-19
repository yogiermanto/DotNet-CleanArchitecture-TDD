using RoomBookingApp.Domain;

namespace RoomBookingApp.Core.DataServices
{
    public interface IRoomBookingService
    {
        void Save(RoomBooking roomBooking);

        List<Room> GetAvailableRooms(DateTime dateTime);
    }
}
