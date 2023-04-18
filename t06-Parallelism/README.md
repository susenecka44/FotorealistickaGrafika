# Topic06 - Parallelism
* Use **parallelism** in your ray-tracer
  * simple way - use of `Parallel.For`
  * more advanced - using the `System.Threading.Tasks.Task` class
* **Screen-space parallelism** is recommended
  * workers share scene definition (read-only at the moment of image computation)
  * use of a ray/texture coherence for optimal use of the memory cache system
  * **master/manager thread** is responsible for work-unit distribution and result image assembly

## Notes
* All pre-processing (e.g. acceleration, texture-precomputation...) has to be done
  **before the actual image rendering**. Can use its own parallelism though
* Shared ray-tracing-scene data should be **read-only** to avoid **locking**
* **Working-unit** can be a *tile* (e.g. 16x16 pix example) or the whole
  *horizontal image line* (worse cache utilization)
* If possible, use *optional parallelism*, it is good for debugging (`Parallel.For`)

## References
* [Parallel.For](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.for?view=net-7.0)
* [Parallel programming in .NET](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/)
* [Task Parallel Library (TPL)](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl)

# Your Documentation
Use the [separate file DOC.md](DOC.md) to avoid merge conflicts.
