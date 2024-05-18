using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UseCases.Assignor.Queries;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.UseCases.Assignor.Queries;

[TestFixture]
public class GetAssignorByIdHandlerTests
{
    [Test]
    public async Task Handle_ValidRequest_ReturnsAssignor()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GetAssignorByIdHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new GetAssignorByIdHandler(loggerMock.Object, contextMock.Object);
        var query = new GetAssignorByIdQuery { Id = Guid.NewGuid() };

        contextMock.Setup(c => c.Assignors.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AssignorEntity
            {
                Name = "Test", Document = "123", Phone = "1234567890", Email = "test@test.com"
            });

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result.Assignor);
    }

    [Test]
    public void Handle_AssignorNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GetAssignorByIdHandler>>();
        var contextMock = new Mock<IDataContext>();
        var handler = new GetAssignorByIdHandler(loggerMock.Object, contextMock.Object);
        var query = new GetAssignorByIdQuery { Id = Guid.NewGuid() };

        contextMock.Setup(c => c.Assignors.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AssignorEntity)null!);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(query, CancellationToken.None));
    }
}