using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UseCases.Payable.Commands.DeletePayable;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Payable.Commands;

[TestFixture]
public class DeletePayableHandlerTests
{
    [Test]
    public async Task Handle_ValidRequest_DeletesPayable()
    {
        var loggerMock = new Mock<ILogger<DeletePayableHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new DeletePayableHandler(loggerMock.Object, contextMock.Object);
        var command = new DeletePayableCommand { Id = Guid.NewGuid() };

        contextMock.Setup(c => c.Payables.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PayableEntity
            {
                Value = 0,
                EmissionDate = default,
                AssignorId = default
            });

        var result = await handler.Handle(command, CancellationToken.None);

        contextMock.Verify(c => c.Payables.Remove(It.IsAny<PayableEntity>()), Times.Once);
        contextMock.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Test]
    public void Handle_PayableNotFound_ThrowsNotFoundException()
    {
        var loggerMock = new Mock<ILogger<DeletePayableHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new DeletePayableHandler(loggerMock.Object, contextMock.Object);
        var command = new DeletePayableCommand { Id = Guid.NewGuid() };

        contextMock.Setup(c => c.Payables.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PayableEntity)null!);

        Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}