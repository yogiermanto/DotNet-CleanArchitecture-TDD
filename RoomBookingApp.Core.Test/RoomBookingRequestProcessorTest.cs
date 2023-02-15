using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Shouldly;

namespace RoomBookingApp.Core.Test
{
    public class RoomBookingRequestProcessorTest
    {
        private readonly RoomBookingRequestProcessor _processor;

        public RoomBookingRequestProcessorTest()
        {
            _processor = new RoomBookingRequestProcessor();
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

            RoomBookingRequestProcessor processor = new();

            //Act
            RoomBookingResult result = processor.BookRoom(request);

            //Assert
            result.ShouldNotBeNull();
            result.FullName.ShouldBe(request.FullName);
            result.Email.ShouldBe(request.Email);
            result.Date.ShouldBe(request.Date);
        }

        [Fact]
        public void Should_Throw_Exception_For_Null_Request()
        {
            RoomBookingRequestProcessor processor = new();

            var exception = Should.Throw<ArgumentNullException>(() => processor.BookRoom(null));
            exception.ParamName.ShouldBe("bookingRequest");
        }
    }
}
