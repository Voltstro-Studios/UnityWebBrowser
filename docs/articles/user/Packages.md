# Packages

UWB is separated into four main different types of packages.

|Package Type   |Required?  |Description                                                                                          |
|-------------- |---------- |---------------------------------------------------------------------------------------------------- |
|Core           |✔         |Core UWB package, provides the heart of UWB                                                          |
|Engines        |✔         |Provides an Engine                                                                                   |
|Engines Natives|✔ (System)|Provides the native binaries used by an Engine. What ones you install depend on your platform targets|
|Helper         |✖         |Provides additional helper/extensions to UWB                                                         |

Most developers will probably only need the 'Core' package, and a single Engine with the Native Windows binaries, like so:

![Packages](~/assets/images/articles/user/packages/StandardPackages.webp)

If you plan on targeting Linux (or are developing on Linux), install the Engine's Linux binaries.
