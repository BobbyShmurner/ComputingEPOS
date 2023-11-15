using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ComputingEPOS.Backend.Attributes;

public enum ComparisonOperator {
	Equal,
	NotEqual,
	LessThan,
	GreaterThan,
	LessThanOrEqual,
	GreaterThanOrEqual
}

public abstract class CompareToBaseAttribute : ValidationAttribute {
    public abstract Func<object?, ValidationContext, (object, bool)> GetOther { get; }
    public virtual ComparisonOperator ComparisonOperator { get; protected set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
		dynamic thisVal = value!;
		(object other, bool success) = GetOther(value, validationContext);

		if (!success) {
			return new((string)other);
		}

		switch (ComparisonOperator) {
			case ComparisonOperator.Equal:
				if (!(thisVal.CompareTo(other) == 0)) return new($"{value} must be equal to {other}");
				break;
			case ComparisonOperator.NotEqual:
				if (!(thisVal.CompareTo(other) != 0)) return new($"{value} must not be equal to {other}");
				break;
			case ComparisonOperator.LessThan:
				if (!(thisVal.CompareTo(other) < 0)) return new($"{value} must be less than to {other}");
				break;
			case ComparisonOperator.GreaterThan:
				if (!(thisVal.CompareTo(other) > 0)) return new($"{value} must be greater than to {other}");
				break;
			case ComparisonOperator.LessThanOrEqual:
				if (!(thisVal.CompareTo(other) <= 0)) return new($"{value} must be less than or equal to {other}");
				break;
			case ComparisonOperator.GreaterThanOrEqual:
				if (!(thisVal.CompareTo(other) >= 0)) return new($"{value} must be greater than or equal to {other}");
				break;
		}

        return ValidationResult.Success;
    }
}