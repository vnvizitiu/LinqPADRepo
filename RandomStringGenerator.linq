<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.EnterpriseServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.RegularExpressions.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Design.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.ApplicationServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.ComponentModel.DataAnnotations.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.Protocols.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.ServiceProcess.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.Services.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.Build.Utilities.v4.0.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.Build.Framework.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.Build.Tasks.v4.0.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Caching.dll</Reference>
  <Namespace>System.Web.Security</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	// setting up the rules we want to be applied to our random string generator
	StringGenerationRule[] rules =
	{
		new DigitStringGenerationRule(2),
		new LowerCaseStringGenerationRule(1),
		new UpperCaseStringGenerationRule(1),
		new SymbolStringGenerationRule(1),
	
		// because each rule needs to be applied the number of time specified this will make it run a bit slower but it showcases what the custom string strategy can enforce
		new CustomStringGenerationRule("¶", 1),
	
		// added a second custom rule to validate that the check for duplicates is done via pattern instead of type
		new CustomStringGenerationRule("↨", 1)
	};

	new RandomStringGenerator(rules).Generate().Dump(); // generating the string
}

/// <summary>
/// Generates a random string based on the rules provided tot it
/// </summary>
class RandomStringGenerator
{
	private readonly IEnumerable<StringGenerationRule> _stringGenerationRules;

	/// <summary>
	/// Creates an instance of the generator
	/// </summary>
	/// <param name="rules">The rules that the generator needs to follow when creating a string</param>
	public RandomStringGenerator(IEnumerable<StringGenerationRule> rules)
	{
		var groupedRules = rules.GroupBy(a => a.StringPattern); // we group all the rules so that we can verify their validity

		// we check if there are any duplicate rules
		if (groupedRules.Any(a => a.Count() > 1))
		{
			var duplicatePatterns = groupedRules.Where(grouping => grouping.Count() > 1).Select(grouping => grouping.Key); // extract the duplicate rules

			// throw an exception letting us know that we have duplicate rules and what are those rules.
			throw new InvalidOperationException($"The rules need to be distinct, found duplicate rules at [{string.Join(",", duplicatePatterns)}]");
		}

		_stringGenerationRules = rules;
	}

	/// <summary>
	/// Generates the string
	/// </summary>
	/// <returns>A string that follows the length and rules</returns>
	public string Generate()
	{
		string str = string.Empty; // this will be the output string

		Random random = new Random();
		int limit = _stringGenerationRules.Sum(s => s.NumberOfCharacters); // here we calculate the length of the string that needs to be generated based on how many rules needs to be applied

		// while the length has not reached the limit, keep on appending to the string
		while (str.Length < limit)
		{
			char character; // the character that will be appended tot the string
			do
			{
				character = (char)random.Next(char.MaxValue); // get a random character out of the whole set of characters in the system
			}
			while (!_stringGenerationRules.Any(s => s.CanExecute(character, str))); // while the character doesn't fulfill any of the rules, keep choosing characters at random 

			StringGenerationRule validRule =
				_stringGenerationRules.First(s => s.CanExecute(character, str)); // get the first rule that applies

			str = validRule.Execute(character, str); // apply the rule and update the string
		}

		return str;
	}
}

/// <summary>
/// This is the base class for all the rules
/// </summary>
abstract class StringGenerationRule
{
	private readonly Regex Pattern;

	/// <summary>
	/// Represents the number of characters a rule has to apply
	/// </summary>
	public readonly int NumberOfCharacters;

	/// <summary>
	/// Sets up the requirements for when a concrete class is instantiated
	/// </summary>
	/// <param name="numberOfCharacters">The number of characters that will be applied by the rule</param>
	/// <param name="pattern">The pattern that the rule needs to follow</param>
	public StringGenerationRule(int numberOfCharacters, string pattern)
	{
		NumberOfCharacters = numberOfCharacters;
		Pattern = new Regex(pattern, RegexOptions.Compiled); // Here we set the pattern to Compiled so that is a bit more efficient
	}

	/// <summary>
	/// The pattern of the rule
	/// </summary>
	public string StringPattern => Pattern.ToString();

	/// <summary>
	/// Verifies if the rule can be applied to the current string
	/// </summary>
	/// <param name="character">The character that will be checked</param>
	/// <param name="currentString">The generated string so far</param>
	/// <returns>True if the character is valid and can be added to the string</returns>
	public bool CanExecute(char character, string currentString)
	{
		string stringOfchar = character.ToString(); // we cast the character tot a string so that we can verify it

		// if the character string is null or empty then the rule cannot be applied so we return false
		if (string.IsNullOrEmpty(stringOfchar))
		{
			return false;
		}

		bool isValidCharacter = Pattern.IsMatch(stringOfchar); // we validate the char based on our rule
		bool isRoomForMoreCharacters = Pattern.Matches(currentString).Count < NumberOfCharacters; // we check if we reached the intended number of characters in that string
		return isValidCharacter && isRoomForMoreCharacters; // if it's valid and we can add characters then we will return true
	}

	/// <summary>
	/// Executes the rule by concatenating the validated character to the current string
	/// </summary>
	/// <param name="character">The character to append</param>
	/// <param name="currentString">The current string before execution</param>
	/// <returns>The new string with the appended character</returns>
	public string Execute(char character, string currentString)
	{
		// Added this check in case someone forgets to check if a rule can be applied.
		if (!CanExecute(character, currentString))
		{
			return currentString;
		}

		return string.Concat(currentString, character);
	}
}

/// <summary>
/// Represents a rule for digits
/// </summary>
class DigitStringGenerationRule : StringGenerationRule
{
	public DigitStringGenerationRule(int numberOfCharacters)
		: base(numberOfCharacters, @"[0-9]")
	{
	}
}

/// <summary>
/// Represents a rule for lower case characters
/// </summary>
class LowerCaseStringGenerationRule : StringGenerationRule
{
	public LowerCaseStringGenerationRule(int numberOfCharacters)
		: base(numberOfCharacters, @"[a-z]")
	{
	}
}

/// <summary>
/// Represents a rule for upper case characters
/// </summary>
class UpperCaseStringGenerationRule : StringGenerationRule
{
	public UpperCaseStringGenerationRule(int numberOfCharacters)
		: base(numberOfCharacters, @"[A-Z]")
	{
	}
}

/// <summary>
/// Represents a rule for commonly used symbols
/// </summary>
class SymbolStringGenerationRule : StringGenerationRule
{
	public SymbolStringGenerationRule(int numberOfCharacters)
		: base(numberOfCharacters, @"[!@$%&*-]")
	{
	}
}

/// <summary>
/// Represents a rule that can be given a custom pattern
/// </summary>
class CustomStringGenerationRule : StringGenerationRule
{
	public CustomStringGenerationRule(string pattern, int numberOfCharacters)
		: base(numberOfCharacters, pattern)
	{
	}
}