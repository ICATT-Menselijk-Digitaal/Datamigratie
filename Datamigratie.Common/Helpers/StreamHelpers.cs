using System.Buffers;

namespace Datamigratie.Common.Helpers;


public static class StreamHelpers
{
    /// <summary>
    /// Asynchronously reads the bytes from the source stream and writes them to another stream, untill the number of specified bytes has been reached. Both streams positions are advanced by the number of bytes copied.
    /// </summary>
    /// <param name="source">The source stream</param>
    /// <param name="destination">The stream to which the contents of the source stream will be copied.</param>
    /// <param name="maxBytes">The maximum amount of bytes to copy</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns></returns>
    public static async Task CopyBytesToAsync(this Stream source, Stream destination, long maxBytes, CancellationToken cancellationToken)
    {
        // Keep track of how many bytes remain to be read.
        var bytesToRead = maxBytes;

        // Continue reading until the requested maxBytes is reached or the source runs out of data.
        while (bytesToRead > 0)
        {
            // Rent a buffer from the shared memory pool to reduce allocations and GC pressure.
            // MemoryPool<byte>.Shared.Rent() returns a disposable owner that must be disposed
            // to return the buffer to the pool.
            using var memoryOwner = MemoryPool<byte>.Shared.Rent();

            // Use only as much memory as needed for this iteration (don't exceed remaining bytes).
            var memory = memoryOwner.Memory;
            var currentLength = (int)Math.Min(memory.Length, bytesToRead);
            memory = memory[..currentLength];

            // Read from the source stream asynchronously.
            var readBytes = await source.ReadAsync(memory, cancellationToken);

            // If no bytes were read, the source has reached the end — stop copying.
            if (readBytes == 0)
            {
                break;
            }

            // Only write the portion of the buffer that was actually filled.
            memory = memory[..readBytes];
            await destination.WriteAsync(memory, cancellationToken);

            // Decrease the number of remaining bytes to copy.
            bytesToRead -= readBytes;
        }

        // When the loop exits:
        // - Either we've copied 'maxBytes' bytes
        // - Or the source ran out of data before reaching 'maxBytes'
        // In both cases, stream positions reflect how many bytes were actually transferred.
    }
}
