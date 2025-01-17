namespace AddressRegistry.Tests.BackOffice.Api.WhenProposingAddress
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Address;
    using AddressRegistry.Api.BackOffice.Abstractions.Requests;
    using FluentAssertions;
    using FluentValidation;
    using FluentValidation.Results;
    using Moq;
    using StreetName;
    using StreetName.Exceptions;
    using Xunit;
    using Xunit.Abstractions;
    using AddressController = AddressRegistry.Api.BackOffice.AddressController;
    using HouseNumber = StreetName.HouseNumber;

    public class GivenNoParentExistsForHouseNumber : BackOfficeApiTest
    {
        private readonly AddressController _controller;

        public GivenNoParentExistsForHouseNumber(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _controller = CreateApiBusControllerWithUser<AddressController>();
        }

        [Fact]
        public void ThenThrowsValidationException()
        {
            string houseNumber = "11";
            string boxNumber = "1A";
            var streetNamePersistentId = new StreetNamePersistentLocalId(123);
            var postInfoId = new PersistentLocalId(456);

            var mockRequestValidator = new Mock<IValidator<AddressProposeRequest>>();
            mockRequestValidator.Setup(x => x.ValidateAsync(It.IsAny<AddressProposeRequest>(), CancellationToken.None))
                .Returns(Task.FromResult(new ValidationResult()));

            MockMediator.Setup(x => x.Send(It.IsAny<AddressProposeRequest>(), CancellationToken.None))
                .Throws(new ParentAddressNotFoundException(streetNamePersistentId, new HouseNumber(houseNumber)));

            var streetNamePuri = $"https://data.vlaanderen.be/id/straatnaam/{streetNamePersistentId}";
            var postInfoPuri = $"https://data.vlaanderen.be/id/postinfo/{postInfoId}";
            var body = new AddressProposeRequest
            {
                StraatNaamId = streetNamePuri,
                PostInfoId = postInfoPuri,
                Huisnummer = houseNumber,
                Busnummer = boxNumber
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(
                ResponseOptions,
                mockRequestValidator.Object,
                body);

            // Assert
            var d = act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result;
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x =>
                    x.Errors.Any(
                        failure => failure.ErrorCode == "AdresActiefHuisNummerNietGekendValidatie"
                                   && failure.ErrorMessage == $"Er bestaat geen actief adres zonder busnummer voor straatnaam '{streetNamePuri}' en huisnummer '{houseNumber}'."
                                   && failure.PropertyName == nameof(body.Huisnummer)));
        }
    }
}
