@echo off
organic.exe ../src/base.dasm ../bin/kernel.bin --include ../inc/ --listing ../bin/kernel.lst --working-directory ../src/ --json ../bin/kernel.json
@echo on