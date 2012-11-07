# disk-tool

This is a tool for turning a directory of files into a valid f2c-fs disk image.

## Usage

*Requires mono on Linux and Mac.*

    diskimage.exe --driver path/to/driver.bin path/to/filesystem/ output.img

The driver is copied to the start of the image, at the boot sector. All values
are properly adjusted to compensate for multi-sector drivers.

The specified `path/to/filesystem/` becomes / in the disk image.