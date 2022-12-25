
call cmake ^
-G "Visual Studio 16 2019" -A x64 ^
-B "glfw-bin" ^
-S "glfw-src" ^
-DCMAKE_INSTALL_PREFIX="glfw-install" ^
-DBUILD_SHARED_LIBS=ON ^
-DCMAKE_CONFIGURATION_TYPES=Release ^
-DRUNTIME_OUTPUT_DIRECTORY="..\runtime_test"