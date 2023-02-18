using Moq;
using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Shouldly;

namespace RoomBookingApp.Core.Test
{
    public class RoomBookingRequestProcessorTest
    {
        private readonly RoomBookingRequestProcessor _processor;
        private readonly Mock<IRoomBookingService> _roomBookingServiceMock;
        private RoomBookingRequest _request;
        

        public RoomBookingRequestProcessorTest()
        {
            _request = new RoomBookingRequest
            {
                FullName = "Test Name",
                Email = "test@request.com",
                Date = DateTime.Now,
            };

            _roomBookingServiceMock = new Mock<IRoomBookingService>();
            _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);
        }

        [Fact]
        public void Should_Return_Room_Booking_Response_With_Request_Value()
        {
            //Arrange
            RoomBookingRequest request = new()
            {
                FullName = "Test Name",
                Email = "test@request.com",
                Date = DateTime.Now,
            };

            //Act
            RoomBookingResult result = _processor.BookRoom(request);

            //Assert
            result.ShouldNotBeNull();
            result.FullName.ShouldBe(request.FullName);
            result.Email.ShouldBe(request.Email);
            result.Date.ShouldBe(request.Date);
        }

        [Fact]
        public void Should_Throw_Exception_For_Null_Request()
        {
            var exception = Should.Throw<ArgumentNullException>(() => _processor.BookRoom(null));
            exception.ParamName.ShouldBe("bookingRequest");
        }

        [Fact]
        public void Should_Save_Room_Booking_Request()
        {
            RoomBooking? savedBooking = null;
            _roomBookingServiceMock.Setup(x => x.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(x =>
                {
                    savedBooking = x;
                });

            _processor.BookRoom(_request);
            
            _roomBookingServiceMock.Verify(x => x.Save(It.IsAny<RoomBooking>()), Times.Once);
        }
        
    }
}
