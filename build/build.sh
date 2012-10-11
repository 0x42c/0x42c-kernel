#!/bin/sh
mkdir ../bin/
mono organic.exe ../src/base.dasm ../bin/kernel.bin --include ../inc/ --listing ../bin/kernel.lst --working-directory ../src/