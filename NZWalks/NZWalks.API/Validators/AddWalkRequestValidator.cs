using FluentValidation;

namespace NZWalks.API.Validators
{
    public class AddWalkRequestValidator : AbstractValidator<Models.DTO.UpdateWalkRequest>
    {
        public AddWalkRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Length).GreaterThan(0);
        }
    }
}
