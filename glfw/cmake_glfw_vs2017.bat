
call cmake ^
-G "Visual Studio 15 2017" -A x64 ^
-B "glfw-bin" ^
-S "glfw-src" ^
-DCMAKE_INSTALL_PREFIX="glfw-install" ^
-DBUILD_SHARED_LIBS=ON ^
-DCMAKE_CONFIGURATION_TYPES=Release ^
-DRUNTIME_OUTPUT_DIRECTORY="..\runtime_test"