using NodaTime;

namespace ExampleAssembly
{
	/// <summary>
	///		Test class that depends on an external NuGet library.
	/// </summary>
	public class TestExternalDependency
	{
		/// <summary>
		///		Get an Instant from the Nodatime library.
		/// </summary>
		/// <returns></returns>
		public Instant GetNow()
		{
			return SystemClock.Instance.GetCurrentInstant();
		}
	}
}
