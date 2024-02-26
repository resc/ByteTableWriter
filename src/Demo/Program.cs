
Console.OpenStandardOutput();
Console.Out.WriteLine("# HexTools ");
Console.Out.WriteLine();


Console.Out.WriteLine("## TableWriter ");
Console.Out.WriteLine();
var asciiTable = Enumerable.Range(0, 256).Select(b => (byte)b).ToArray();
var bytes = asciiTable.AsSpan()[110..150];

Console.Out.WriteLine("### Default Settings");

Console.Out.WriteLine();
Console.Out.WriteLine("#### Printing all bytes");
Console.Out.WriteLine();
Console.Out.WriteLine("""
```csharp
using (var writer = new HexTools.TableWriter(Console.Out))
{
    writer.Write(asciiTable);
}
```
Output:
""");
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out))
{
    writer.Write(asciiTable);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### Printing a table where the last row isn't full");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      using (var writer = new HexTools.TableWriter(Console.Out))
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      """);
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out))
{
    writer.Write(bytes);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### Change layout to 3 columns of 3 bytes");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          ColumnCount = 3,
                          ColumnWidth = 3,
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      """);
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out)
{
    ColumnCount = 3,
    ColumnWidth = 3,
})
{
    writer.Write(bytes);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### remove the offset column");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          ShowOffsetColumn = false,
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      """);
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out)
{
    ShowOffsetColumn = false,
})
{
    writer.Write(bytes);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### Remove the ascii column");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          ShowAsciiColumn = false,
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      """);
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out)
{
    ShowAsciiColumn = false,
})
{
    writer.Write(bytes);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### change hex to lower case");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          ByteFormat = "{0:x2}"
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      """);
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out)
{
    ByteFormat = "{0:x2}"
})
{
    writer.Write(bytes);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### print bytes as decimal");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          ByteFormat = "{0,4}" // no hex conversion, but write byte with a fixed width to 4
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      """);
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out)
{
    ByteFormat = "{0,4}" // no hex conversion, but write byte with a fixed width to 4
})
{
    writer.Write(bytes);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### print bytes as binary (.NET 8+)");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          ByteFormat = "{0:B8}",
                          ColumnWidth = 1,
                          ColumnCount = 4,
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      """);
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out)
{
    ByteFormat = "{0:B8}",
    ColumnWidth = 1,
    ColumnCount = 4,
    
})
{
    writer.Write(bytes);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### change column separator");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          ColumnSeparator = " | ",
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      """);
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out)
{
    ColumnSeparator = " | ",
})
{
    writer.Write(bytes);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### Prefix every row with >>>");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          RowStart = ">>> ",
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      """);
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out)
{
    RowStart = ">>> ",
})
{
    writer.Write(bytes);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### End every row with <<<");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          RowEnd = " <<<",
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      """);
Console.Out.WriteLine("```");
using (var writer = new HexTools.TableWriter(Console.Out)
{
    RowEnd = " <<<",
})
{
    writer.Write(bytes);
}
Console.Out.WriteLine("```");

Console.Out.WriteLine();
Console.Out.WriteLine("#### Make it a markdown table");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      
                      Console.Out.WriteLine("| offset | byte 0-1 | byte 2-3 | byte 4-5 | byte 6-7 | byte 8-9 | byte 10-11 | byte 12-13 | byte 14-15 | ASCII |");
                      Console.Out.WriteLine("|--------|----------|----------|----------|----------|----------|------------|------------|------------|-------|");
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          ColumnSeparator = " | ",
                          RowStart = "| ",
                          RowEnd = " |",
                          AsciiFormat = "`{0}`",
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      ```
                      Output:
                      
                      """);

Console.Out.WriteLine();
Console.Out.WriteLine("| offset | byte 0-1 | byte 2-3 | byte 4-5 | byte 6-7 | byte 8-9 | byte 10-11 | byte 12-13 | byte 14-15 | ASCII |");
Console.Out.WriteLine("|--------|----------|----------|----------|----------|----------|------------|------------|------------|-------|");
using (var writer = new HexTools.TableWriter(Console.Out)
{
    ColumnSeparator = " | ",
    RowStart = "| ",
    RowEnd = " |",
    AsciiFormat = "`{0}`",
})
{
    writer.Write(bytes);
}

Console.Out.WriteLine();
Console.Out.WriteLine("#### Generate some code ");
Console.Out.WriteLine();
Console.Out.WriteLine("""
                      ```csharp
                      Console.Out.WriteLine("public static readonly byte[] bytes = {");
                      using (var writer = new HexTools.TableWriter(Console.Out)
                      {
                          RowStart = "    ",
                          OffsetFormat = "/* {0,4} */",
                          ColumnSeparator = ", ",
                          ColumnWidth = 1,
                          ByteFormat = "0x{0:X2}",
                          AsciiFormat = "// ascii bytes: {0}",
                          OmitMissingByteColumns = true,
                      })
                      {
                          writer.Write(asciiTable);
                      }
                      Console.Out.WriteLine("};");
                      ```
                      Output:
                      """);

Console.Out.WriteLine("```csharp");
Console.Out.WriteLine("public static readonly byte[] bytes = {");
using (var writer = new HexTools.TableWriter(Console.Out)
       {
           RowStart = "    ",
           OffsetFormat = "/* {0,4} */",
           ColumnSeparator = ", ",
           ColumnWidth = 1,
           ByteFormat = "0x{0:X2}",
           AsciiFormat = "// ascii bytes: {0}",
           OmitMissingByteColumns = true,
       })
{
    writer.Write(bytes[..^2]);
}
Console.Out.WriteLine("};");
Console.Out.WriteLine("```");