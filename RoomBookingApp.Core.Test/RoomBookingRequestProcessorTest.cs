using System.Runtime.InteropServices;
using Moq;
using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Core.Domain;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Exceptions;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Shouldly;

namespace RoomBookingApp.Core.Test
{
public class RoomBookingRequestProcessorTest
{
    private readonly RoomBookingRequestProcessor _processor;
    private readonly Mock<IRoomBookingService> _roomBookingServiceMock;
    private readonly RoomBookingRequest _request;
    private readonly List<Room> _availableRooms;

    public RoomBookingRequestProcessorTest()
    {
        _request = new RoomBookingRequest
        {
            FullName = "Test Name",
            Email = "test@request.com",
            Date = DateTime.Now,
        };

        _availableRooms = new List<Room>()
        {
            new() {Id = 1}
        };

        _roomBookingServiceMock = new Mock<IRoomBookingService>();
        _roomBookingServiceMock.Setup(x => x.GetAvailableRooms(_request.Date))
            .Returns(_availableRooms);
        _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);
    }

    [Fact]
    public void Should_Return_Room_Booking_Response_With_Request_Value()
    {
        //Arrange
        RoomBooking? savedBooking = null;
        _roomBookingServiceMock.Setup(x => x.Save(It.IsAny<RoomBooking>()))
            .Callback<RoomBooking>(x => { savedBooking = x; });

        // Act
        RoomBookingResult result = _processor.BookRoom(_request);

        // Assert
        _roomBookingServiceMock.Verify(x => x.Save(It.IsAny<RoomBooking>()), Times.Once);
        result.ShouldNotBeNull();
        result.FullName.ShouldBe(_request.FullName);
        result.Email.ShouldBe(_request.Email);
        result.Date.ShouldBe(_request.Date);
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
        // Arrange
        RoomBooking savedBooking = null;
        _roomBookingServiceMock.Setup(x => x.Save(It.IsAny<RoomBooking>()))
            .Callback<RoomBooking>(x => savedBooking = x);

        // Act
        _processor.BookRoom(_request);

        // Assert
        _roomBookingServiceMock.Verify(x => x.Save(It.IsAny<RoomBooking>()), Times.Once);
        savedBooking.ShouldNotBeNull();
        savedBooking.FullName.ShouldBe(_request.FullName);
        savedBooking.Email.ShouldBe(_request.Email);
        savedBooking.Date.ShouldBe(_request.Date);
        savedBooking.RoomId.ShouldBe(_availableRooms.First().Id);
    }

    [Fact]
    public void Should_Not_Save_Room_Booking_Request_If_None_Available()
    {
        // Arrange
        _availableRooms.Clear();

        // Act
        _processor.BookRoom(_request);

        // Assert
        _roomBookingServiceMock.Verify(x => x.Save(It.IsAny<RoomBooking>()), Times.Never);
    }

    [Theory]
    [InlineData(BookingResultFlag.Failure, false)]
    [InlineData(BookingResultFlag.Success, true)]
    public void Should_Return_SuccessOrFailure_Flag_In_Result(BookingResultFlag flag, bool isAvailable)
    {
        // Arrange
        if (!isAvailable)
        {
            _availableRooms.Clear();
        }

        // Act
        var result = _processor.BookRoom(_request);

        // Assert
        flag.ShouldBe(result.Flag);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(null, false)]
    public void Should_Return_RoomBookingId_In_Result(int? roomBookingId, bool isAvailable)
    {
        // Arrange
        if (!isAvailable)
        {
            _availableRooms.Clear();
        }
        else
        {
            _roomBookingServiceMock.Setup(x => x.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(x => x.Id = roomBookingId!.Value);
        }

        // Act
        var result = _processor.BookRoom(_request);

        // Assert
        result.RoomBookingId.ShouldBe(roomBookingId);
    }
}

}
