@echo off
msbuild.exe ../tools/disk-img/diskimg.sln
organic.exe ../src/base.dasm ../bin/kernel.bin --include ../inc/ --listing ../bin/kernel.lst --working-directory ../src/ --json ../bin/kernel.json
@echo on