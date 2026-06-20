namespace StonedAges;

public class RequestBoardPacket : PacketStructure
{
    private RequestBoardS _board;

    public RequestBoardS Board
    {
        get
        {
            RequestBoardS result = default(RequestBoardS);
            int num = 4;
            result.Type = ReadByte(num);
            num++;
            if (result.Type == 2)
            {
                result.BoardID = ReadUShort(num);
                num += 2;
                result.LastPostNumber = ReadUShort(num);
                num += 2;
            }
            else if (result.Type == 3)
            {
                result.BoardID = ReadUShort(num);
                num += 2;
                result.PostNumber = ReadUShort(num);
                num += 2;
                result.Navigation = ReadByte(num);
                num++;
            }
            else if (result.Type == 5)
            {
                result.BoardID = ReadUShort(num);
                num += 2;
                result.PostNumber = ReadUShort(num);
                num += 2;
            }
            else if (result.Type == 6)
            {
                result.BoardID = ReadUShort(num);
                num += 2;
                byte b = ReadByte(num);
                num++;
                result.PosterName = ReadString(num, b);
                num += b;
                b = ReadByte(num);
                num++;
                result.PostTitle = ReadString(num, b);
                num += b;
                ushort num2 = ReadUShort(num);
                num += 2;
                result.PostBody = ReadString(num, num2);
                num += num2;
            }
            return result;
        }
        set
        {
            _board = value;
            int num = 4;
            WriteByte(_board.Type, num);
            num++;
            if (_board.Type == 2)
            {
                WriteUShort(_board.BoardID, num);
                num += 2;
                WriteUShort(_board.LastPostNumber, num);
                num += 2;
            }
            else if (_board.Type == 3)
            {
                WriteUShort(_board.BoardID, num);
                num += 2;
                WriteUShort(_board.PostNumber, num);
                num += 2;
                WriteByte(_board.Navigation, num);
                num++;
            }
            else if (_board.Type == 5)
            {
                WriteUShort(_board.BoardID, num);
                num += 2;
                WriteUShort(_board.PostNumber, num);
                num += 2;
            }
            else if (_board.Type == 6)
            {
                WriteUShort(_board.BoardID, num);
                num += 2;
                WriteByte((byte)_board.PosterName.Length, num);
                num++;
                WriteString(_board.PosterName, num);
                num += _board.PosterName.Length;
                WriteByte((byte)_board.PostTitle.Length, num);
                num++;
                WriteString(_board.PostTitle, num);
                num += _board.PostTitle.Length;
                WriteUShort((ushort)_board.PostBody.Length, num);
                num += 2;
                WriteString(_board.PostBody, num);
                num += _board.PostBody.Length;
            }
        }
    }

    public RequestBoardPacket(RequestBoardS board, ushort length)
        : base((ushort)(4 + length), 38)
    {
        Board = board;
    }

    public RequestBoardPacket(byte[] packet)
        : base(packet)
    {
    }
}
