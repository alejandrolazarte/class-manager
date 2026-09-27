using System.Text;

namespace ClassManager.ImportExport.Tabular.Csv;

internal sealed class CsvRecordParser(string text, char delimiter)
{
    private const char Quote = '"';
    private const char CarriageReturn = '\r';
    private const char LineFeed = '\n';
    private const int FirstLineNumber = 1;

    private readonly List<TabularRow> _records = [];
    private readonly List<string> _cells = [];
    private readonly StringBuilder _cell = new();
    private int _position;
    private int _lineNumber = FirstLineNumber;
    private int _recordLineNumber = FirstLineNumber;
    private bool _isInsideQuotes;
    private bool _isCellQuoted;

    public IReadOnlyList<TabularRow>? Parse()
    {
        while (_position < text.Length)
        {
            if (_isInsideQuotes)
            {
                ReadQuotedCharacter();
            }
            else
            {
                ReadCharacter();
            }
        }

        if (_isInsideQuotes)
        {
            return null;
        }

        if (_cell.Length > 0 || _cells.Count > 0 || _isCellQuoted)
        {
            EndRecord();
        }

        return _records;
    }

    private void ReadQuotedCharacter()
    {
        var character = text[_position];
        if (character == Quote)
        {
            if (Peek() == Quote)
            {
                _cell.Append(Quote);
                _position += 2;
                return;
            }

            _isInsideQuotes = false;
            _position++;
            return;
        }

        if (character == LineFeed || (character == CarriageReturn && Peek() != LineFeed))
        {
            _lineNumber++;
        }

        _cell.Append(character);
        _position++;
    }

    private void ReadCharacter()
    {
        var character = text[_position];
        _position++;

        if (character == Quote && _cell.Length == 0 && !_isCellQuoted)
        {
            _isInsideQuotes = true;
            _isCellQuoted = true;
        }
        else if (character == delimiter)
        {
            EndCell();
        }
        else if (character is CarriageReturn or LineFeed)
        {
            if (character == CarriageReturn && Peek(offset: 0) == LineFeed)
            {
                _position++;
            }

            EndRecord();
            _lineNumber++;
            _recordLineNumber = _lineNumber;
        }
        else
        {
            _cell.Append(character);
        }
    }

    private void EndCell()
    {
        _cells.Add(_cell.ToString());
        _cell.Clear();
        _isCellQuoted = false;
    }

    private void EndRecord()
    {
        EndCell();
        _records.Add(new TabularRow(_recordLineNumber, [.. _cells]));
        _cells.Clear();
    }

    private char? Peek(int offset = 1) =>
        _position + offset < text.Length ? text[_position + offset] : null;
}
