<Query Kind="Program" />

void Main()
{
	var constantRun = true;
	var dumpContainer = new DumpContainer().Dump();
	do
	{
		var currentDateTimeOffset = DateTimeOffset.Now;
		dumpContainer.Content = new [] {
				AssociatedTime.Create("Friend 1", currentDateTimeOffset, "Pacific Standard Time"),
				AssociatedTime.Create("Friend 1", currentDateTimeOffset, "Pacific Standard Time"),
				AssociatedTime.Create("Client 1", currentDateTimeOffset, "Eastern Standard Time"),
				AssociatedTime.Create("Friend 3", currentDateTimeOffset, "Eastern Standard Time"),
				AssociatedTime.Create("Client 2", currentDateTimeOffset, "GMT Standard Time"),
				AssociatedTime.Create("Client 3", currentDateTimeOffset, "Romance Standard Time"),
				AssociatedTime.Create("Local Time", currentDateTimeOffset, "GTB Standard Time"),
		};
		Thread.Sleep(TimeSpan.FromSeconds(1));
	} while (constantRun);
}

class AssociatedTime
{
	public string Name { get; }
	public DateTimeOffset Time { get; }
	public bool IsDaylightSaving { get; }

	private AssociatedTime(string name, DateTimeOffset currentDateTimeOffset, string timezoneId)
	{
		Name = name;
		TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
		Time = TimeZoneInfo.ConvertTime(currentDateTimeOffset, timezone);
		IsDaylightSaving = timezone.IsDaylightSavingTime(Time);
	}

	public static AssociatedTime Create(string name, DateTimeOffset currentDateTimeOffset, string timeZoneId)
	{
		return new AssociatedTime(name, currentDateTimeOffset, timeZoneId);
	}
}