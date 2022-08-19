namespace AddressRegistry.Api.BackOffice.Validators
{
    using Abstractions;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.GrAr.Edit.Contracts;
    using Be.Vlaanderen.Basisregisters.GrAr.Edit.Validators;
    using FluentValidation;
    using Projections.Syndication;

    public class AddressChangePositionRequestValidator : AbstractValidator<AddressChangePositionRequest>
    {
        public AddressChangePositionRequestValidator(SyndicationContext syndicationContext)
        {
            RuleFor(x => x.PositieSpecificatie)
                .NotEmpty()
                .When(x => x.PositieGeometrieMethode == PositieGeometrieMethode.AangeduidDoorBeheerder)
                .WithMessage(ValidationErrorMessages.Address.PositionSpecificationRequired)
                .WithErrorCode(ValidationErrors.Address.PositionSpecificationRequired);

            RuleFor(x => x.PositieSpecificatie)
                .Must(x =>
                    x == PositieSpecificatie.Ingang ||
                    x == PositieSpecificatie.Perceel ||
                    x == PositieSpecificatie.Lot ||
                    x == PositieSpecificatie.Standplaats ||
                    x == PositieSpecificatie.Ligplaats)
                .When(x => x.PositieGeometrieMethode == PositieGeometrieMethode.AangeduidDoorBeheerder)
                .WithMessage(ValidationErrorMessages.Address.PositionSpecificationInvalid)
                .WithErrorCode(ValidationErrors.Address.PositionSpecificationInvalid);

            RuleFor(x => x.PositieSpecificatie)
                .Must(x =>
                    x is null ||
                    x == PositieSpecificatie.Gemeente ||
                    x == PositieSpecificatie.Wegsegment)
                .When(x => x.PositieGeometrieMethode == PositieGeometrieMethode.AfgeleidVanObject)
                .WithMessage(ValidationErrorMessages.Address.PositionSpecificationInvalid)
                .WithErrorCode(ValidationErrors.Address.PositionSpecificationInvalid);

            RuleFor(x => x.Positie)
                .NotEmpty()
                .When(x => x.PositieGeometrieMethode == PositieGeometrieMethode.AangeduidDoorBeheerder)
                .WithErrorCode(ValidationErrors.Address.PositionRequired)
                .WithMessage(ValidationErrorMessages.Address.PositionRequired);

            RuleFor(x => x.Positie)
                .Must(gml => GmlPointValidator.IsValid(gml, GmlHelpers.CreateGmlReader()))
                .When(x => !string.IsNullOrEmpty(x.Positie))
                .WithErrorCode(ValidationErrors.Address.PositionInvalidFormat)
                .WithMessage(ValidationErrorMessages.Address.PositionInvalidFormat);
        }
    }
}
