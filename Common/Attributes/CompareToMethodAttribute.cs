using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ComputingEPOS.Common.Attributes;

public class CompareToMethodAttribute : CompareToBaseAttribute {
    public CompareToMethodAttribute(Func<object?, ValidationContext, (object, bool)> getOther, ComparisonOperator comparisonOperator) {
        this.getOther = getOther;
        ComparisonOperator = comparisonOperator;
	}

	private Func<object?, ValidationContext, (object, bool)> getOther;
    public override Func<object?, ValidationContext, (object, bool)> GetOther => getOther;
}