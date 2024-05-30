# Step02 - Command line program
* The simple command-line project is already available - see the
  [src directory](../src). It uses C# 10.0 and `.NET 6.0`.
* Copy the pilot project in your directory [/solution](../solution).
  Detailed instructions are in the section "Your solution" of
  the [global README.md](../README.md).
* Setup a simple command-line parameter and config-file parsing system
  – any simple text based (or XML-based) format is possible
* You can use the `CommandLineParser` (via the NuGet system) for
  command line parsing. It is always a good idea to be able to
  reference your config file from a command line argument
  (this way you can have multiple config files and easily switch
  between them)
* Put all your **config files** under GIT control, it helps other
  people (me) to test your project. Fill the text box
  `Debug/rt004 Debug Properties/Command line arguments` with reasonable
  values, as this item is also versioned
* Use `$(ProjectDir)` as the start directory for your project -
  it is easier to reference input/config files in this case
* Print the input argument (`Console.WriteLine()`) to see if everything works
* Some simple logging/debug system could be useful but you can postpone it...
* I need to be able to **compile your projects** easily. Please keep your solution
  (`.sln`) and project files (`.csproj`) working all the time
