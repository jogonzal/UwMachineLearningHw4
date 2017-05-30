namespace Problem1BiasAndVariance.DecisionTreeClasses
{
	public enum InequalityOperation
	{
		GreaterThan,
		LessThan,
		GreaterThanOrEqualTo,
		LessThanOrEqualTo
	}

	public class Inequality
	{
		public Inequality(int amount, InequalityOperation operation)
		{
			Amount = amount;
			Operation = operation;
		}

		public int Amount { get; set; }
		public InequalityOperation Operation { get; set; }
	}
}
