# RT004
Support for the ***NPGR004 (Photorealistic graphics)*** lecture

## Project plan
Every `sNN-*` directory refers to one item of the lab plan
(see [row 1 of this table](https://docs.google.com/spreadsheets/d/1rOb0dhPapRahL4XWfD5Ar2pz9qGD7f0cMBM8gY_RkEE/edit?usp=sharing))

More details will be provided in this repository. Checkpoint
information will be included in the associated step directories
(e.g. `s03-ch1-HDRimage` contains definition of the `Step 03` and `Checkpoint 1` as well).

Every `tNN-*` directory refers to one optional topic. You
should pick at least two topics for your individual ray-tracer
extensions.

## src
The src directory contains support files from the lecturer. The default
version uses `.NET 6.0`, you can use the `.NET 7.0` variant if necessary
(update the project file yourself in this case).

Select `.NET 6.0 (Long Term Support)` if creating a new project,
it works well on Windows, Linux and macOS.

We use `Visual Studio 2022`, the [Community](https://visualstudio.microsoft.com/vs/community/)
(free) version is good enough for all tasks.

## Point table
See [this shared table](https://docs.google.com/spreadsheets/d/1rOb0dhPapRahL4XWfD5Ar2pz9qGD7f0cMBM8gY_RkEE/edit?usp=sharing)
for current points. Check the dates of individual Checkpoints...

Contact me <pepca@cgg.mff.cuni.cz> for any suggestions, comments or
complaints.

## Your solution
Please place all your files in separate [solution](solution/README.md)
directory in the repository.

The naming conventions in the standard Visual Studio project creation procedures
allow you to simply copy a [pilot task project](src/rt004) to the `solution` directory.
Rewrite the `README.md` which will be your default file for documentation!

Files and directories that should be copied from `src` to the `solution` directory:
```
rt004/
rt004/rt004.csproj
rt004/rt004.sln
rt004/Program.cs
rt004/Properties/
rt004/Properties/launchSettings.json
rt004/shared/
rt004/shared/placeholder
shared/
shared/FloatImage.cs
```

Template for your documentation is [here](solution/README.md). Please
edit all the sections, you could include individual file sections for
your finished checkpoints and extensions.

## AI assistants
The use of **AI assistants (based on Large Language Models)** is not prohibited,
on the contrary! We welcome you to use them under two conditions:
1. you must **acknowledge** for each checkpoint that the AI assistant significantly
   helped you.
2. you must **document your use of the assistant**. For example, if you use
   ChatGPT, record the entire conversation and post a link to it.
   For more "built-in" assistants, you should write a verbal report of
   what the help looked like, how often (and how hard) you had to
   correct the machine-generated code - and if you used comments in
   the code, leave them in!

## GIT instructions
You will work in your own **private repositories**.
You could start from our shared repository and add your own code and
documentation as you solve individual checkpoints.

We recommend using one of the following platforms - there are more
detailed instructions for each of them. The only bigger difference
is in the 3rd step (granting permissions to a lab supervisor).

### GitHub
1. You have to set up a new **private repository** yourself.
2. Connect it to
our shared GIT using `git remote`. The command might look like this
```bash
$ git remote add origin https://github.com/pepcape/RT004.git
```
3. Finally, you have to give me permissions to access your private
repository, this is done using the **"Collaborator"** role.
Please invite me - https://github.com/pepcape.
4. If your GitHub username is a **nickname**, please email us with
your real name.

### GitLab (MFF UK server)
1. You have to set up a new **private repository** yourself.
2. Connect it to
our shared GIT using `git remote`. The command might look like this
```bash
$ git remote add origin https://github.com/pepcape/RT004.git
```
3. Finally, you have to give me permissions to access your private
repository, this is done using the **"Reporter"** role.
Please invite me - https://gitlab.mff.cuni.cz/pelikan.

## Notes
* If anything doesn't work well in your **Linux/macOS environment**,
  you should write me (<pepca@cgg.mff.cuni.cz>) as soon as possible.
  Of course you could report positive experience in Linux/macOS as well.
* You can work in your repositories without major restrictions.
  The recommended place for your solutions is the [solution](solution)
  directory.
* You can tag your GIT history (e.g. `Chk 1` etc.) for archiving your
  progress at the checkpoints. It is always a good idea to write me an email! 
* It seems like the `System.Numerics` library doesn't support `double`
  types (yet?), so I'm going to use the lightweighted **`OpenTK.Mathematics`**
  library instead, distributed in [NuGet form](https://www.nuget.org/packages/OpenTK.Mathematics/5.0.0-pre.8)
* **Visual Studio 2022** supports direct **MarkDown editing** (with live
  result preview) starting from the 17.5 update
