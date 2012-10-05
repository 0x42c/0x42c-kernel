@echo off
mkdir ..\bin
organic.exe ../src/base.dasm ../bin/kernel.bin --include ../inc/ --listing ../bin/kernel.lst --working-directory ../src/ --debug-mode
@echo on