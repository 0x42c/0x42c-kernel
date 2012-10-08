# Table structure

0x42c-kernel has several tables in memory for storing information in, including thread
information, libraries, signals, and more. The structure of these tables is defined in
this document.

## Thread List

The thread list starts at [thread_table] and is 0x100 words long. Each entry is 8 words
long, allowing for a total of 32 concurrent threads. Each entry is structured as such:

    0000: Thread ID
    0001: Address of thread
    0002: Stack pointer
    0003: Flags (bit mask)
        00: May be suspended
        01: Is suspended
        02-07: Reserved for future use
        08-15: User flags
    0004: Pointer to file path (or 0xFFFF for threads with no related file)
    0005: Parent thread
    0006-0007: Reserved for future use

User flags are available for any use, and may be delegated as each thread and its caller
see fit. When the parent thread is terminated, all child threads are terminated as well.

# Library List

The library list starts at [library_table] and is 0x100 words long. Each entry is 4 words
long, allowing for a total of 64 concurrently loaded libraries. Each entry is structured
as such:

    0000: Library ID
    0001: Address of library
    0002: Current usage
    0003: Pointer to library file path

"Current usage" refers to the total number of processes that depend on this library. A
process is defined as any thread with no parent.

# Signal List

Signals are simple mechanisms for moving data between active threads. The signal list
starts at [signal_table] and is 0x100 words long. Each entry is 4 words long, allowing
for a total of 64 pending signals. Ideally, this cap will never be reached, as
developers are encouraged to consume pending signals quickly. Each entry is structured
as such:

    0000: Recipient thread ID
    0001: Originating thread ID
    0002: Reserved for future use
    0003: Signal

When more complex data than a single word can represent must be send between threads,
developers are encouraged to send a pointer to more data in the signal.

# File List

The file list is a list of active file handles. The file list starts at [file_table] and
is 0x100 words long. Each entry is 16 words long, allowing for a maximum of 16 active file
handles. This table is more ambiguously structured, to allow for greater flexibility with
various storage devices.

    0000: File handle ID
    0001: Owning thread ID
    0002: File driver address
    0003: Flags (bit mask)
        00: Readable
        01: Writable
        02: May seek
        03-15: Reserved for driver use
    0004-000F: Reserved for driver use

The file driver address points to a file driver object in memory. The structure of this
is as such:

    0000: Address of read function
    0001: Address of write function
    0002: Address of seek function
    0003: Address of length function

The signatures of these methods is as such:

    void read(uint16 fileHandleId, uint16* destination, uint16 length);
    void write(uint16 fileHandleId, uint16* data, uint16 length);
    void seek(uint16 fileHandleId, uint32 address);
    uint32 length(uint16 fileHandleId);

0x42c-kernel ships with a driver for the M35FD floppy drive. The structure of a file
handle when being managed by the M35FD driver is as such:

    0000: File handle ID
    0001: Owning thread ID
    0002: File driver address
    0003: Flags (bit mask)
        00: Readable
        01: Writable
        02: May seek
        03-15: Reserved for future use
    0004: Disk drive hardware ID
    0005: Current sector
    0006: Current address
    0007: Sector of metadata entry
    0008: Address of metadata entry
    0009: Address of data entry
    000A: Length of file LSB
    000B: Length of file MSB
    000C-000F: Reserved for future use

The M35FD driver is designed to read files from a 42c file system (FTCFS).