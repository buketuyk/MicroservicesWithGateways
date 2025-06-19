using CodeFirstMicroservice.Models;
using FluentValidation;

namespace CodeFirstMicroservice.Validations
{
    public class TaskValidator : AbstractValidator<TaskItem>
    {
        public TaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title must not be empty.")
                .MaximumLength(20).WithMessage("Title must be at most 20 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description must not be empty.")
                .MaximumLength(200).WithMessage("Description must be at most 200 characters.");

            RuleFor(x => x.DueDate)
                .Must(d => d > DateTime.UtcNow)
                .WithMessage("Due date must be in the future (UTC).");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category must be selected.");
        }
    }
}
