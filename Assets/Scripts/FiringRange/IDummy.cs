namespace FiringRange
{
	public interface IDummy
	{
		public bool IsActive { get; }
		public void Initialize(FiringRangeController _controller);
		public void Activate();
		public void Deactivate();
		public void OnHit();
	}
}