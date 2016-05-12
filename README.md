# BufferedDataReaderDotNet

A wrapper around `DbDataReader` that buffers and compresses your data.

## License

*BufferedDataReaderDotNet* is licensed under [*The MIT License (MIT)*](LICENSE.md).

## Use Cases

loading data, slow source but fast destinatino == locking

or you want to send data remotely

* benchmark 2016
* time to read with, without
* total compressed, uncompressed

## Getting Started

install from nuget TODO publish nuget

### Buffering Data

`GetBufferedDataAsync` buffers and compresses data asynchronously.

```csharp
// See "Sequential Access" below for more information about this flag.
const CommandBehavior commandBehavior = CommandBehavior.SequentialAccess;

using (var dataReader = await command.ExecuteReaderAsync(commandBehavior, cancellationToken))
using (var bufferedData = await dataReader.GetBufferedDataAsync(cancellationToken))
{
    // continues below...
}
```

`GetBufferedDataAsync` will consume the entire data reader and close it.

By default a `MemoryStream` is used for every column's compressed data. Use [`BufferedDataOptions`](#BufferedDataOptions) to supply your own `Stream` factory function.

### Reading Buffered Data

```csharp
// ...continues above

using (var bufferedDataReader = bufferedData.GetDataReader())
{
    do
    {
        if (await bufferedDataReader.ReadAsync(cancellationToken))
        {
            // "RecordCount" is known in advance (from the buffer).
            // You can use it for things like displaying progress.
            Console.WriteLine("Reading {0} records", bufferedDataReader.RecordCount);

            do
            {
                // ...
            } while (await bufferedDataReader.ReadAsync(cancellationToken))
        }
    } while (await bufferedDataReader.NextResultAsync(cancellationToken));
}
```

automatically yields if it has to in case streams are synchronous. Use `BufferedDataOptions` to control the yield interval.

### BufferedDataOptions

```csharp
asdf
```

## Considerations for Concurrency

`BufferedData.GetDataReader` returns a `BufferedDataReader` that reads data from the compressed streams.

The streams are re-used between calls to `GetDataReader`. They have their `Position` reset back to the beginning of the stream.

This behaviour means that `GetDataReader` can be called multiple times, at the expense of invalidating any prior `BufferedDataReader`.

## Considerations for Serialization

*BufferedDataReaderDotNet* does not make any promises about forwards or backwards compatibility of its serialized representation. Use the same version of the library for (de)serializing a `BufferedDataReader`.

## Considerations for SqlConnection

If you're using `SqlConnection` (i.e. querying Microsoft SQL Server) then consider the following points for maximum compatibility and performance.

### [Sequential Access](https://msdn.microsoft.com/en-us/library/system.data.commandbehavior.aspx)

For performance: use `CommandBehavior.SequentialAccess` when calling `ExecuteReader` or `ExecuteReaderAsync`. *BufferedDataReaderDotNet* reads data sequentially.

### [Packet Size](https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnectionstringbuilder.packetsize.aspx)

For performance: define the connection string's `Packet Size` greater than the default. (A `Packet Size` of `32768` might be a good starting point.)

The possible values for `Packet Size` are [documented on MSDN](https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring.aspx).

### [Type System Version](https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnectionstringbuilder.typesystemversion.aspx)

For compatibility: define the connection string's `Type System Version` in advance. For example, `SQL Server 2012`. This will ensure the expected version of `Microsoft.SqlServer.Types` is loaded.

Note that when you know the expected version of `Microsoft.SqlServer.Types` in advance you can ship that version side-by-side your application. This is done easily with the `Microsoft.SqlServer.Types` [package from NuGet](https://www.nuget.org/packages/Microsoft.SqlServer.Types/).

The possible values for `Type System Version` are [documented on MSDN](https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring.aspx).
