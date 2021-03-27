Chromium Embedded Framework (CEF) Minimal Binary Distribution for Windows
-------------------------------------------------------------------------------

Date:             March 23, 2021

CEF Version:      90.0.4+gdc12a89+chromium-90.0.4430.30
CEF URL:          https://bitbucket.org/chromiumembedded/cef.git
                  @dc12a89f28f628cca699acbfe3cb0bf4f9612957

Chromium Version: 90.0.4430.30
Chromium URL:     https://chromium.googlesource.com/chromium/src.git
                  @5e981e45e3d9b89ca83194d856877df448a3074c

This distribution contains the minimial components necessary to build and
distribute an application using CEF on the Windows platform. Please see
the LICENSING section of this document for licensing terms and conditions.


CONTENTS
--------

cmake       Contains CMake configuration files shared by all targets.

include     Contains all required CEF header files.

libcef_dll  Contains the source code for the libcef_dll_wrapper static library
            that all applications using the CEF C++ API must link against.

Release     Contains libcef.dll, libcef.lib and other components required to
            build and run the release version of CEF-based applications. By
            default these files should be placed in the same directory as the
            executable.

Resources   Contains resources required by libcef.dll. By default these files
            should be placed in the same directory as libcef.dll.


USAGE
-----

Building using CMake:
  CMake can be used to generate project files in many different formats. See
  usage instructions at the top of the CMakeLists.txt file.

Please visit the CEF Website for additional usage information.

https://bitbucket.org/chromiumembedded/cef/


REDISTRIBUTION
--------------

This binary distribution contains the below components.

Required components:

The following components are required. CEF will not function without them.

* CEF core library.
  * libcef.dll

* Crash reporting library.
  * chrome_elf.dll

* Unicode support data.
  * icudtl.dat

* V8 snapshot data.
  * snapshot_blob.bin
  * v8_context_snapshot.bin

Optional components:

The following components are optional. If they are missing CEF will continue to
run but any related functionality may become broken or disabled.

* Localized resources.
  Locale file loading can be disabled completely using
  CefSettings.pack_loading_disabled. The locales directory path can be
  customized using CefSettings.locales_dir_path. 
 
  * locales/
    Directory containing localized resources used by CEF, Chromium and Blink. A
    .pak file is loaded from this directory based on the CefSettings.locale
    value. Only configured locales need to be distributed. If no locale is
    configured the default locale of "en-US" will be used. Without these files
    arbitrary Web components may display incorrectly.

* Other resources.
  Pack file loading can be disabled completely using
  CefSettings.pack_loading_disabled. The resources directory path can be
  customized using CefSettings.resources_dir_path.

  * chrome_100_percent.pak
  * chrome_200_percent.pak
  * resources.pak
    These files contain non-localized resources used by CEF, Chromium and Blink.
    Without these files arbitrary Web components may display incorrectly.

* Angle and Direct3D support.
  * d3dcompiler_47.dll (required for Windows Vista and newer)
  * libEGL.dll
  * libGLESv2.dll
  Without these files HTML5 accelerated content like 2D canvas, 3D CSS and WebGL
  will not function.

* SwiftShader support.
  * swiftshader/libEGL.dll
  * swiftshader/libGLESv2.dll
  Without these files WebGL will not function in software-only mode when the GPU
  is not available or disabled.


LICENSING
---------

The CEF project is BSD licensed. Please read the LICENSE.txt file included with
this binary distribution for licensing terms and conditions. Other software
included in this distribution is provided under other licenses. Please visit
"about:credits" in a CEF-based application for complete Chromium and third-party
licensing information.
