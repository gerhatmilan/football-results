using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class MinLengthIfNotEmptyAttribute : ValidationAttribute
{
    private readonly int _minLength;

    public MinLengthIfNotEmptyAttribute(int minLength)
    {
        _minLength = minLength;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null && !string.IsNullOrWhiteSpace(value.ToString()) && value.ToString().Length < _minLength)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}