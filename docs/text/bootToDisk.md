# 0x42c Boot to Disk

The default boot mechanism for 0x42c is to boot to the supplied disk, using one of
the default disk drivers. The kernel does the following:

1. Allocate 512 words of memory and copy the first sector of the disk to memory.
2. Check the header to determine the image type.

The header of this disk includes one word to identify the type. These are:
0x0000: Directly executable boot sector
0x0001: Full memory image
0x0002: Filesystem disk

## Directly executable boot sector

For this type of disk, 0x42c-kernel will relocate the sector. The sector should
have a relocation table directly following the type identifier. Then, it will
pass control to the boot sector with interrupts disabled.

## Full memory image

For this type of disk, 0x42c-kernel will copy the contents of the disk from 0x0000-
0x8000 to DCPU memory, starting at 0x8000. It will then pass control to the image
with interrupts disabled. Note that memory management routines are not available to
full memory images.

## Filesystem disk

For this type of disk, the header format is as follows:

    0000: Type ID (0x0002 for filesystem disk)
    0001: Zero-delimited string of first file to execute (usually /bin/init)
    nnnn: Relocation table for following 0x42c-kernel file stream driver
    nnnn: File stream driver

0x42c-kernel will copy the driver to memory and relocate it, then it will launch the
specified file as a program and pass execution to it with interrupts and context
switching enabled.