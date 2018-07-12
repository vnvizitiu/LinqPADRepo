<Query Kind="Program" />

void Main()
{
	var constantRun = true; // this is a flag that keeps the clock running to see what time it is for everyone at the same time
	var dumpContainer = new DumpContainer().Dump(); // this is a LinqPad specific class that allows for updates without refreshing the whole output
	do // we use a do-while approach because if the *constantRun* flag is set to false, we still get the output once.
	{
		var currentDateTimeOffset = DateTimeOffset.Now; // this is the time that will be used to compute the clock in different regions
		dumpContainer.Content = new [] { // hItere we will create an array that holds all the locations we want to keep track of time wise.
				AssociatedTime.Create("Friend 1", currentDateTimeOffset, "Pacific Standard Time"),
				AssociatedTime.Create("Friend 1", currentDateTimeOffset, "Pacific Standard Time"),
				AssociatedTime.Create("Client 1", currentDateTimeOffset, "Eastern Standard Time"),
				AssociatedTime.Create("Friend 3", currentDateTimeOffset, "Eastern Standard Time"),
				AssociatedTime.Create("Client 2", currentDateTimeOffset, "GMT Standard Time"),
				AssociatedTime.Create("Client 3", currentDateTimeOffset, "Romance Standard Time"),
				AssociatedTime.Create("Local Time", currentDateTimeOffset, "GTB Standard Time"),
		};
		Thread.Sleep(TimeSpan.FromSeconds(1)); //  this sleep is used to throttle the loop so that we don't take up more resources than needed
	} while (constantRun); 
}

class AssociatedTime // this is a container class that will just hold the name of a location we want to track, its time and if it's in daylight savings.
{
	public string Name { get; } // the name of the location, or friend
	public DateTimeOffset Time { get; } // the current time at that location
	public bool IsDaylightSaving { get; } // if that location is in daylight savings

	private AssociatedTime(string name, DateTimeOffset currentDateTimeOffset, string timezoneId) // this constructor was made private so that I can use a static factory method which I consider cleaner
	{
		Name = name;
		TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId); // here we will get the TimeZoneInfo based of the Id we have it
		Time = TimeZoneInfo.ConvertTime(currentDateTimeOffset, timezone); // here we convert the current passed-in time to the specific timezone
		IsDaylightSaving = timezone.IsDaylightSavingTime(Time); // here we interogare if the current time is in daylight savings
	}

	public static AssociatedTime Create(string name, DateTimeOffset currentDateTimeOffset, string timeZoneId) // the factory method for creating a location
	{
		return new AssociatedTime(name, currentDateTimeOffset, timeZoneId);
	}
}