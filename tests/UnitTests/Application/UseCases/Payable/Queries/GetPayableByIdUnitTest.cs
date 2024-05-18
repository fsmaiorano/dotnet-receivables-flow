using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UseCases.Payable.Queries;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Payable.Queries;

[TestFixture]
public class GetPayableByIdHandlerTests
{
    [Test]
    public async Task Handle_ValidRequest_ReturnsPayable()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GetPayableByIdHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new GetPayableByIdHandler(loggerMock.Object, contextMock.Object);
        var query = new GetPayableByIdQuery { Id = Guid.NewGuid() };

        contextMock.Setup(c => c.Payables.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PayableEntity
            {
                Value = 0,
                EmissionDate = default,
                AssignorId = default,
                Assignor = null!
            });

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result.Payable);
    }

    [Test]
    public void Handle_PayableNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GetPayableByIdHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new GetPayableByIdHandler(loggerMock.Object, contextMock.Object);
        var query = new GetPayableByIdQuery { Id = Guid.NewGuid() };

        contextMock.Setup(c => c.Payables.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PayableEntity)null!);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(query, CancellationToken.None));
    }
}