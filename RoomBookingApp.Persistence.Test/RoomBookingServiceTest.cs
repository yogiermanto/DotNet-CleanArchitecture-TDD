using Microsoft.EntityFrameworkCore;
using RoomBookingApp.Domain;
using RoomBookingApp.Persistence.Repositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomBookingApp.Persistence.Test
{
    public class RoomBookingServiceTest
    {
        [Fact]
        public void Should_Return_Available_Rooms()
        {
            //Arrange 
            var date = new DateTime(2023, 02, 19);

            var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase("AvailableRoomTest")
                .Options;

            using var context = new RoomBookingAppDbContext(dbOptions);
            context.Add(new Room { Id = 1, Name = "Room 1" });
            context.Add(new Room { Id = 2, Name = "Room 2" });
            context.Add(new Room { Id = 3, Name = "Room 3" });

            context.Add(new RoomBooking { RoomId = 1, Date = date });
            context.Add(new RoomBooking { RoomId = 2, Date = date.AddDays(-1) });

            context.SaveChanges();

            var roomBookingService = new RoomBookingService(context);

            //Act
            var availableRooms = roomBookingService.GetAvailableRooms(date);

            //Assert
            availableRooms.Count().ShouldBeEquivalentTo(2);
            availableRooms.ShouldContain(x => x.Id == 2);
            availableRooms.ShouldContain(x => x.Id == 3);
            availableRooms.ShouldNotContain(x => x.Id == 1);
        }

        [Fact]
        public void Should_Save_Room_Booking()
        {
            // Arrange
            var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase("ShouldSaveTest")
                .Options;

            var roomBooking = new RoomBooking { Id = 1, RoomId = 1, Date = new DateTime(2023, 02, 19) };

            // Act
            using var context = new RoomBookingAppDbContext(dbOptions);
            var roomBookingService = new RoomBookingService(context);
            roomBookingService.Save(roomBooking);

            var bookings = context.RoomBookings.ToList();

            // Assert
            RoomBooking booking = bookings.ShouldHaveSingleItem();
            booking.Id.ShouldBeEquivalentTo(roomBooking.Id);
            booking.RoomId.ShouldBeEquivalentTo(roomBooking.RoomId);
            booking.Date.ShouldBeEquivalentTo(roomBooking.Date);
        }
    }
}
