using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ComputingEPOS.Backend.Attributes;

public class CompareToAttribute : CompareToBaseAttribute {
    public CompareToAttribute(string propertyToCompare, ComparisonOperator comparisonOperator) {
        PropertyToCompare = propertyToCompare;
        ComparisonOperator = comparisonOperator;
	}

    public string PropertyToCompare { get; }
    public override Func<object?, ValidationContext, (object, bool)> GetOther => (value, validationContext) => {
		try {
			object other = validationContext.ObjectType
				.GetProperty(PropertyToCompare, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
				.GetValue(validationContext.ObjectInstance)!;

			return (other, true);
		} catch {
			return ($"Failed to find property \"{PropertyToCompare}\"", false);
		}
	};
}