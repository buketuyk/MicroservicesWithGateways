using CodeFirstMicroservice.Validations;
using CodeFirstMicroservice.Models.Dtos;
using FluentValidation.TestHelper;

namespace CodeFirstMicroservice.Tests
{
    public class StatusDtoValidatorTests
    {
        private readonly StatusDtoValidator _validator; // field
        public StatusDtoValidatorTests()
        {
            _validator = new StatusDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Null()
        {
            var model = new StatusDto { Name = null };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var model = new StatusDto { Name = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Whitespace()
        {
            var model = new StatusDto { Name = "   " };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Short()
        {
            var model = new StatusDto { Name = "A" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Long()
        {
            var model = new StatusDto { Name = new string('A', 51) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Valid()
        {
            var model = new StatusDto { Name = "Active" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}
