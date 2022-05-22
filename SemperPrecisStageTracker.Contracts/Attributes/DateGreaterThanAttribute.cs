using System;
using System.ComponentModel.DataAnnotations;

namespace SemperPrecisStageTracker.Contracts;

[AttributeUsage(AttributeTargets.Property)]
public class DateGreaterThanAttribute : ValidationAttribute
{
    public DateGreaterThanAttribute(string dateToCompareToFieldName)
    {
        DateToCompareToFieldName = dateToCompareToFieldName;
    }

    private string DateToCompareToFieldName { get; set; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        DateTime laterDate = (DateTime)value;

        DateTime earlierDate = (DateTime)validationContext.ObjectType.GetProperty(DateToCompareToFieldName).GetValue(validationContext.ObjectInstance, null);

        if (laterDate.Day >= earlierDate.Day)
        {
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult("Date is not later");
        }
    }
}