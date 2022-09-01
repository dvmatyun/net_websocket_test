@echo off
@echo.
@echo Building "websocket_io.dart" to "websocket_io.exe"
@echo.
@call dart2native websocket_io.dart -k exe -o websocket_io.exe
@timeout 5