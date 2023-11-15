using System.ComponentModel.DataAnnotations;

namespace ComputingEPOS.Backend.Attributes;

public class DateNotInFutureAttribute : CompareToBaseAttribute {
    public override ComparisonOperator ComparisonOperator => ComparisonOperator.LessThanOrEqual;

    public override Func<object?, ValidationContext, (object, bool)> GetOther
		=> (value, validationContext)
		=> (DateTime.Now, true);
}