@echo off
call build.bat
start lib\cassini.exe "%cd%\Sample" 8112
start http://localhost:8112/boo/index.aspx