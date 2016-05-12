using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using BufferedDataReaderDotNet.Infrastructure;
using JetBrains.Annotations;

namespace BufferedDataReaderDotNet
{
    public sealed class BufferedDataReader : DbDataReader
    {
        private readonly Queue<BufferedResult> _bufferedResults;

        [CanBeNull] private BufferedResultReader _bufferedResultReader;

        internal BufferedDataReader(Queue<BufferedResult> bufferedResults)
        {
            _bufferedResults = bufferedResults;

            NextResult();
        }

        public override int FieldCount => _bufferedResultReader?.FieldCount ?? -1;

        public override object this[int ordinal]
        {
            get { throw new NotImplementedException(); }
        }

        public override object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public override bool HasRows => _bufferedResultReader?.HasRows ?? false;

        public override bool IsClosed => _bufferedResultReader?.IsClosed ?? true;

        public int RecordCount => _bufferedResultReader.RecordCount;

        // TODO: Close our data reader
        /// <remarks>"The RecordsAffected property is not set until all rows are read and you close the DataReader."</remarks>
        /// <see>https://msdn.microsoft.com/en-us/library/system.data.common.dbdatareader.recordsaffected(v=vs.110).aspx</see>
        public override int RecordsAffected { get; }

        public override int Depth => _bufferedResultReader.Depth;

        public override string GetName(int ordinal) => _bufferedResultReader?.GetName(ordinal);

        public override int GetValues(object[] values) => _bufferedResultReader?.GetValues(values) ?? 0;

        public override bool IsDBNull(int ordinal) => _bufferedResultReader.IsDBNull(ordinal);

        public override void Close() => _bufferedResultReader?.Close();

        public override DataTable GetSchemaTable() => _bufferedResultReader?.GetSchemaTable();

        public override bool NextResult()
        {
            if (_bufferedResults.Count > 0)
            {
                _bufferedResultReader = new BufferedResultReader(_bufferedResults.Dequeue());

                return true;
            }

            _bufferedResultReader = null;

            return false;
        }

        public override async Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();

            return NextResult();
        }

        public override bool Read() => _bufferedResultReader.Read();

        public override Task<bool> ReadAsync(CancellationToken cancellationToken) =>
            _bufferedResultReader.ReadAsync(cancellationToken);

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetInt32(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}