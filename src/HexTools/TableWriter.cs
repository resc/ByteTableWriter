namespace HexTools;

/// <summary>
/// Writes bytes in a table format to the output. <br/>
/// The table can be written incrementally <br/>
/// By default the first column contains the byte offset,
/// then 8 columns of 2 hex-formatted bytes, and then a column
/// containing the ASCII representation of the bytes.<br/>
/// The output is customizable, see the properties of this class.
/// <code>
///   0 0001 0203 0405 0607 0809 0A0B 0C0D 0E0F ................
///  16 1011 1213 1415 1617 1819 1A1B 1C1D 1E1F ................
///  32 2021 2223 2425 2627 2829 2A2B 2C2D 2E2F .!"#$%&amp;'()*+,-./
///  48 3031 3233 3435 3637 3839 3A3B 3C3D 3E3F 0123456789:;&lt;=&gt;?
///  64 4041 4243 4445 4647 4849 4A4B 4C4D 4E4F @ABCDEFGHIJKLMNO
///  80 5051 5253 5455 5657 5859 5A5B 5C5D 5E5F PQRSTUVWXYZ[\]^_
///  96 6061 6263 6465 6667 6869 6A6B 6C6D 6E6F `abcdefghijklmno
/// 112 7071 7273 7475 7677 7879 7A7B 7C7D 7E7F pqrstuvwxyz{|}~.
/// 128 8081 8283 8485 8687 8889 8A8B 8C8D 8E8F ................
/// </code>
/// </summary>
public class TableWriter : IDisposable
{
    private const int DefaultColumnWidth = 2;
    private const int DefaultColumnCount = 8;

    /// <summary>
    /// This lookup table converts all byte values to a printable char.
    /// space is also converted to a dot. This class uses a space in the
    /// ascii column to indicate a missing byte.
    /// </summary>
    private static readonly char[] _printableCharMap =
    {
        /*   0 */ '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
        /*  16 */ '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
        /*  32 */ '.', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/',
        /*  48 */ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?',
        /*  64 */ '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
        /*  80 */ 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_',
        /*  96 */ '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
        /* 112 */ 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~', '.',
        /* 128 */ '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
        /* 144 */ '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
        /* 160 */ '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
        /* 176 */ '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
        /* 192 */ '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
        /* 208 */ '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
        /* 224 */ '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
        /* 240 */ '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.',
    };

    private bool _isDisposed;
    private int _offset;
    private int _bytesInColumn;
    private int _bytesInRow;
    private bool _completingRow;

    private readonly int _columnCount = DefaultColumnCount;
    private readonly int _columnWidth = DefaultColumnWidth;
    private readonly char[] _asciiColumnBuffer = NewAsciiColumnBuffer(DefaultColumnCount, DefaultColumnWidth);
    private readonly TextWriter _writer;
    private readonly bool _ownsWriter;

    /// <summary>
    /// Creates a <see cref="TableWriter"/> with an underlying <see cref="StringWriter"/>.
    /// Use <see cref="ToString"/> to get the text.
    /// </summary>
    public TableWriter() : this(new StringWriter(), true)
    {
    }

    /// <summary>
    /// Creates a new <see cref="TableWriter"/> with the
    /// <paramref name="writer"/> as the underlying <see cref="TextWriter"/>
    /// if <paramref name="ownsWriter"/> is <c>true</c> the underlying <see cref="TextWriter"/> is disposed
    /// when this instance is disposed, otherwise it is not.
    /// </summary>
    public TableWriter(TextWriter writer, bool ownsWriter = false)
    {
        _writer = writer;
        _ownsWriter = ownsWriter;
    }

    /// <summary> The underlying <see cref="System.IO.TextWriter"/> </summary>
    public TextWriter TextWriter => _writer;

    /// <summary> Prefix every row with this value </summary>
    public string RowStart { get; init; } = "";

    /// <summary> Suffix every row with this value </summary>
    public string RowEnd { get; init; } = "";

    /// <summary> Separator between columns, a single space byt default </summary>
    public string ColumnSeparator { get; init; } = " ";

    /// <summary> Number of bytes shown in a column </summary>
    public int ColumnWidth
    {
        get => _columnWidth;
        init
        {
            _columnWidth = value;
            _asciiColumnBuffer = NewAsciiColumnBuffer(_columnCount, value);
        }
    }

    /// <summary> Number of columns showing bytes (excludes the offset and ascii columns) </summary>
    public int ColumnCount
    {
        get => _columnCount;
        init
        {
            _columnCount = value;
            _asciiColumnBuffer = NewAsciiColumnBuffer(value, _columnWidth);
        }
    }

    private static char[] NewAsciiColumnBuffer(int columnCount, int columnWidth)
    {
        var count = columnWidth * columnCount;
        return count > 0 ? new char[count] : Array.Empty<char>();
    }

    /// <summary> Whether or not to show the column with the byte offset. </summary>
    public bool ShowOffsetColumn { get; init; } = true;

    /// <summary> Whether or not to the column with the ascii representation of the bytes. </summary>
    public bool ShowAsciiColumn { get; init; } = true;

    /// <summary> Format string for the byte offset. Make sure this is a fixed width for all byte offsets </summary>
    public string OffsetFormat { get; init; } = "{0,4}";

    /// <summary> Format string for the byte value. <c>{0:X2}</c> by default.
    /// Make sure this is a fixed with for all values of a byte </summary>
    public string ByteFormat { get; init; } = "{0:X2}";

    /// <summary> Number of bytes per output line. </summary>
    public int BytesPerLine => int.Clamp(ColumnCount * ColumnWidth, 0, int.MaxValue);

    /// <summary> Format string for the Ascii column, gets the entire ascii string as a format parameter </summary>
    public string AsciiFormat { get; set; } = "";

    /// <summary> Don't write out the byte columns for the missing bytes in a row. </summary>
    public bool OmitMissingByteColumns { get; set; }

    /// <summary>
    /// Resets the byte offset to zero, to start writing a new byte table.
    /// You might want to call <c>WriteLine()</c> on the <see cref="TextWriter"/> before starting a new table.
    /// </summary>
    public void Reset()
    {
        _offset = 0;
        ResetLineCounters();
    }

    /// <summary> Writes the bytes from the array, using the <paramref name="offset"/> and <paramref name="count"/></summary>
    public void Write(byte[] bytes, int offset, int count)
        => Write(bytes.AsSpan(offset, count));

    /// <summary> Writes all the bytes in the span</summary>
    public void Write(ReadOnlySpan<byte> bytes)
    {
        foreach (var b in bytes)
            Write(b);
    }

    /// <summary> Writes a single byte </summary>
    public void Write(byte b)
    {
        WriteRowStart();
        TryWriteOffsetColumn();
        WriteByte(b);
        if (ShouldStartNewLine())
        {
            ResetLineCounters();
            TryWriteAsciiColumn();
            WriteRowEnd();
        }
        else
        {
            TryWriteByteColumnSeparator();
        }
    }

    private void WriteRowEnd()
    {
        if (RowEnd.Length > 0)
            _writer.Write(RowEnd);
        _writer.WriteLine();
    }

    private void WriteRowStart()
    {
        if (_bytesInRow == 0 && RowStart.Length > 0)
            _writer.Write(RowStart);
    }

    /// <summary>
    /// Writes the remaining byte columns and ascii column. Prints spaces instead of the missing bytes.
    /// </summary>
    public void CompleteRow()
    {
        try
        {
            _completingRow = true;
            while (_bytesInRow != 0)
                Write(0);
        }
        finally
        {

            _completingRow = false;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (_isDisposed)
            return;
        _isDisposed = true;
        CompleteRow();
        if (_ownsWriter)
            _writer.Dispose();
    }

    /// <inheritdoc cref="object.ToString()"/>
    public override string? ToString()
    {
        if (_writer is StringWriter sw)
            return sw.ToString();

        return base.ToString();
    }

    private void WriteByte(byte b)
    {
        if (_completingRow)
        {
            WriteAsciiColumnBuffer(' ');
            if (!OmitMissingByteColumns)
                WriteSpaces();
        }
        else
        {
            WriteAsciiColumnBuffer(GetChar(b));
            _writer.Write(ByteFormat, b);
        }
        _offset++;
        _bytesInRow++;
    }

    private void WriteSpaces()
    {
        // because the ByteFormat is user-settable
        // we need to compute the length of this format
        var l = string.Format(ByteFormat, (byte)0).Length;
        while (l > 0)
        {
            _writer.Write(' ');
            l--;
        }
    }

    private void WriteAsciiColumnBuffer(char c)
    {
        if (0 <= _bytesInRow && _bytesInRow < _asciiColumnBuffer.Length)
            _asciiColumnBuffer[_bytesInRow] = c;
    }

    private void TryWriteOffsetColumn()
    {
        if (ShowOffsetColumn && _bytesInRow == 0)
        {
            _writer.Write(OffsetFormat, _offset);
            _writer.Write(ColumnSeparator);
        }
    }

    private bool ShouldStartNewLine()
    {
        var max = BytesPerLine;
        if (max <= 0)
            return false;

        return _bytesInRow >= max;
    }

    private void ResetLineCounters()
    {
        _bytesInRow = 0;
        _bytesInColumn = 0;
    }

    private void TryWriteAsciiColumn()
    {
        if (!ShowAsciiColumn) return;


        if (_completingRow && OmitMissingByteColumns)
        {
            // omit the column separator, it's already been written
        }
        else
        {
            _writer.Write(ColumnSeparator);
        }

        if (AsciiFormat.Length > 0)
        {
            var s = new string(_asciiColumnBuffer);
            s = s.TrimEnd();
            _writer.Write(AsciiFormat, s);
            for (int i = s.Length; i < _asciiColumnBuffer.Length; i++)
                _writer.Write(' ');
        }
        else
        {
            foreach (var b in _asciiColumnBuffer)
                _writer.Write(b);
        }

        Array.Clear(_asciiColumnBuffer);
    }

    private static char GetChar(byte b) => _printableCharMap[b];

    private void TryWriteByteColumnSeparator()
    {
        if (ColumnWidth <= 0)
            return;

        _bytesInColumn++;
        if (_bytesInColumn < ColumnWidth)
            return;

        _bytesInColumn = 0;

        if (_completingRow && OmitMissingByteColumns)
            return;

        _writer.Write(ColumnSeparator);
    }
}