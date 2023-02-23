# RT004
Support for ***NPGR004 (Photorealistic graphics)*** lecture

## Project plan
Every `sNN-*` directory refers to one item of the lab plan
(see [row 1 of this table](https://docs.google.com/spreadsheets/d/1jnkLW1R7_FYD6QcWP1X_a3_xoVFAU-LzF6ErXKNgqVs/edit?usp=sharing))

More details will be provided in this repository. Checkpoint
information will be included in the associated step directories
(e.g. `s03-ch1-HDRimage` contains definition of the `Step 03` and `Checkpoint 1` as well).

## src
The src directory contains support files from the lecturer. Default
version uses `.NET 7.0`, you can use the `.NET 6.0` variant if necessary.

## Point table
See [this shared table](https://docs.google.com/spreadsheets/d/1jnkLW1R7_FYD6QcWP1X_a3_xoVFAU-LzF6ErXKNgqVs/edit?usp=sharing)
for current points. Check the dates of individual Checkpoints...

Contact me <pepca@cgg.mff.cuni.cz> for any suggestions, comments or
complaints.

## Notes
* If anything doesn't work well in your **Linux/macOS environment**,
  you should write me (<pepca@cgg.mff.cuni.cz>) as soon as possible.
  Of course you could report positive experience in Linux/macOS as well.
* After some thinking I've come to a recommendation for you - use
  the `git fork` command at the beginning of the semester and
  use special branches (e.g. `Checkpoint 1` etc.) for archiving your
  progress at the checkpoints.
* It seems like the `System.Numerics` library doesn't support `double`
  types (yet?), so I'm going to use the lightweighted **`OpenTK.Mathematics`**
  library instead, distributed in [NuGet form](https://www.nuget.org/packages/OpenTK.Mathematics/5.0.0-pre.8)
* Visual Studio 2022 supports direct **MarkDown editing** (with live
  result preview) starting from the 17.5 update

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
