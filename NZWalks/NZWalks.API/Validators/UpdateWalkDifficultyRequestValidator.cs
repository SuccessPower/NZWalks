using FluentValidation;

namespace NZWalks.API.Validators
{
    public class UpdateWalkDifficultyRequestValidator : AbstractValidator<Models.DTO.WalkDifficulty>
    {
        public UpdateWalkDifficultyRequestValidator()
        {
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}
