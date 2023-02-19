using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Domain;

namespace RoomBookingApp.Persistence.Repositories
{
    public class RoomBookingService : IRoomBookingService
    {
        private readonly RoomBookingAppDbContext _context;

        public RoomBookingService(RoomBookingAppDbContext context)
        {
            _context = context;
        }

        public List<Room> GetAvailableRooms(DateTime dateTime)
        {
            var availableRooms = _context.Rooms.Where(x => !x.RoomBookings!.Any(y => y.Date == dateTime)).ToList();

            return availableRooms;
        }

        public void Save(RoomBooking roomBooking)
        {
            throw new NotImplementedException();
        }
    }
}
