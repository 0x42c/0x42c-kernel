# Forty-two-c Filesystem

The forty-two-c filesystem (f2c-fs) is the reccomended filesystem for operating systems
built on 0x42c-kernel. Tools for using it are distributed along with 0x42c itself.

## Disk Structure

The first sector (subject to change) of the disk is the metadata, as described in the
boot-to-disk documentation. The f2c-fs driver may be found in `src/f2c-fs/`, though you
are welcome to provide your own.

The second sector of the disk is the start of the data section. The data section
contains the contents of files stored in the file system.

Finally, beginning on the last sector of the disk is the allocation table. This table
grows backwards from the last word on the disk towards the start of the disk, and
describes the filesystem hierarchy and layout.

## Data Section

The data section begins at address 0x200 in the disk, and grows forwards. It consists
of a series of data blobs, which are simply structured as follows:

    0x0000: Blob size
    0x0001: Flags (bit mask)
        00: Is allocated (1 for allocated)
        01-15: [Reserved for future use]
    0x0002: Data...

The blob size and flags are used when allocating space for new files.

## Allocation Table

*All formatting information described in this section is backwards.*

The allocation table starts at the final word of the disk, and moves towards 0. It
consists of several entries, which are structured as follows:

    0x0000: Entry type
    0x0001: Entry length
    0x0002: Entry...

There are several entry types. Valid types include:

    -1: End of table
    00: [Reserved for future use]
    01: Directory
    02: File
    03: Symbolic link
    04: Deleted entry

### End of table

This signals the end of the table. There is always exactly one end_of_table entry in
the allocation table, and it progresses backwards as the filesystem grows.

This entry has no further data.

### Directory

Directory entries represent directories and subdirectories within the filesystem.

*NOTE: The parent of all directories is /, or root, which does not have an explicitly
defined directory entry in the table. The directory_id of / is 0.*

**Entry**:

    0x0000: Directory ID
    0x0001: Parent ID
    0x0002: Flags (bit mask)
        00-15: [Reserved for future use]
    0x0003: Length of name
    0x0004: Name (ASCII, no delimiter)

### File

File entries represent links to data in the data section, as well as file metadata.

**Entry**:

    0x0000: Parent directory ID
    0x0001: Flags (bit mask)
        00-15: [Reserved for future use]
    0x0003: Pointer to data section entry
    0x0004: 32-bit file length
    0x0006: Length of name
    0x0007: Name (ASCII, no delimiter)

### Symbolic link

Symbolic links (or symlinks) are a simple link that leads to a new path when referenced.

**Entry**:

    0x0000: Parent directory ID
    0x0001: Flags (bit mask)
        00-15: [Reserved for future use]
    0x0002: Length of name
    0x0003: Name (ASCII, no delimiter)
    0xNNNN: Length of path
    0xNNNN: Path (ASCII, no delimiter)

The `Path` is the destination. To link /bin/foo to /bin/bar, the `Name` would be `foo` and
the `Path` would be `/bin/bar/`.

Symbolic links may use either relative or absolute paths. Relative paths are relative to the
directory they reside in.

### Deleted entry

Deleted entries are created when the entry type of an existing entry is changed to 4. These
entries are cleaned out on the next defragmentation, or occasionally when a new entry may fit
into the available space.

## Usage

The reccomended use for this filesystem is with unix-style paths and naming. To clarify:

* To access the "example" file in the "etc" folder of the root folder, use `/etc/example`.
* There are two special directory names:
  * `.` refers to the current directory. `/bin/./` refers to `/bin/`.
  * `..` refers to the parent of the current directory. `/etc/example/../` refers to `/etc/`.
* All paths are relative. In a given context, `example/` may refer to `/etc/example/`, or 
  `/bin/example`, and so on. To "root" a path, prepend a forward slash (`/`).
* The following regular expression matches all valid file and directive names:
  `[A-Za-z0-9_ .()-]`.
* All file and directory names are limited to 256 characters.

## Defragmenting

Each time either section crosses a 0x1000 word boundary, a mandatory defragmentation should
be triggered. In other words, with `last_address % 0x1000 > current_address % 1000`, trigger
a defragment. This may be optionally suspended in certain scenarios, but the filesystem cannot
ignore defragmentation eternally. At the maximum, every 0x2000 word boundary should trigger a
degragmentation.

A defragmentation is simple - the process should remove all deleted entries from the allocation
table, and all unused data section entries, and update the corresponding file entries.