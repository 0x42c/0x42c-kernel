# 0x42c Boot to Disk

The default boot mechanism for 0x42c-kernel is to boot to the supplied disk, using one of
the default disk drivers. The disk format is as follows:

    0x0000: type_id
    0x0001: base_image_size (max 4096, or 8 sectors)
    0x0003: [...]

0x42c-kernel will load the first sector into garbage memory and determine the size of memory
required to load the boot image based on the `base_image_size`. It will allocate this amount
of memory and load the remainder of the disk into the allocated memory. Then, based on the
`type_id`, different actions may happen.

## Acceptable Type IDs

    1: Bootable Image
    2: Full Memory Image
    3: Filesystem

### Bootable Image

A bootable image has a relocation table at the beginning of the image, after `base_image_size`.
It is relocated and then the first word after the end of the relocation table is launched as
a thread. You are given 128 words of stack.

### Full Memory Image

The kernel will copy the first 0x8000 words from the disk to memory, starting at 0x8000 and
extending to 0xFFFF. Then, the kernel will execute SET PC, 0x8000 with context switching
disabled.

### Filesystem

A filesystem image is the most extensible image type available. The format is as follows:

    0x0003: Driver size
    0x0004: Pointer to filesystem driver
    0x0005: Boot file (ASCII, zero-terminated string, usually "/bin/init")
    0xNNNN: Filesystem driver

The driver itself will be relocated and placed into allocated memory. The driver follows the
format described in the filesystem driver documentation.