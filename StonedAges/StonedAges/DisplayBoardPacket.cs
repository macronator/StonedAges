using System.Collections.Generic;
using System.Linq;

namespace StonedAges;

public class DisplayBoardPacket : PacketStructure
{
    private DisplayBoardS _board;

    public DisplayBoardS Board
    {
        get
        {
            DisplayBoardS result = default(DisplayBoardS);
            int num = 4;
            result.BoardType = ReadByte(num);
            num++;
            if (result.BoardType == 1)
            {
                result.BoardList = new List<BoardS>();
                byte b = ReadByte(num);
                num++;
                for (int i = 0; i < b; i++)
                {
                    BoardS item = default(BoardS);
                    item.BoardID = ReadUShort(num);
                    num += 2;
                    byte b2 = ReadByte(num);
                    num++;
                    item.BoardName = ReadString(num, b2);
                    num += b2;
                    result.BoardList.Add(item);
                }
            }
            else if (result.BoardType == 2 || result.BoardType == 4)
            {
                BoardS board = default(BoardS);
                board.Permission = ReadByte(num);
                num++;
                board.BoardID = ReadUShort(num);
                num += 2;
                byte b3 = ReadByte(num);
                num++;
                board.BoardName = ReadString(num, b3);
                num += b3;
                board.PostList = new List<PostS>();
                byte b4 = ReadByte(num);
                num++;
                for (int j = 0; j < b4; j++)
                {
                    PostS item2 = default(PostS);
                    item2.Highlight = ReadBool(num);
                    num++;
                    item2.PostNumber = ReadUShort(num);
                    num += 2;
                    b3 = ReadByte(num);
                    num++;
                    item2.PosterName = ReadString(num, b3);
                    num += b3;
                    item2.Day = ReadByte(num);
                    num++;
                    item2.Month = ReadByte(num);
                    num++;
                    b3 = ReadByte(num);
                    num++;
                    item2.PostTitle = ReadString(num, b3);
                    num += b3;
                    board.PostList.Add(item2);
                }
                result.Board = board;
            }
            else if (result.BoardType == 3 || result.BoardType == 5)
            {
                PostS post = default(PostS);
                post.Permission = ReadByte(num);
                num++;
                post.Highlight = ReadBool(num);
                num++;
                post.PostNumber = ReadUShort(num);
                num += 2;
                byte b5 = ReadByte(num);
                num++;
                post.PosterName = ReadString(num, b5);
                num += b5;
                post.Day = ReadByte(num);
                num++;
                post.Month = ReadByte(num);
                num++;
                b5 = ReadByte(num);
                num++;
                post.PostTitle = ReadString(num, b5);
                num += b5;
                ushort num2 = ReadUShort(num);
                num += 2;
                post.PostBody = ReadString(num, num2);
                num += num2;
                result.Post = post;
            }
            else if (result.BoardType == 6 || result.BoardType == 7)
            {
                BoardPromptS prompt = default(BoardPromptS);
                prompt.CloseBoard = ReadBool(num);
                num++;
                byte b6 = ReadByte(num);
                num++;
                prompt.PromptText = ReadString(num, b6);
                num += b6;
                result.Prompt = prompt;
            }
            return result;
        }
        set
        {
            _board = value;
            int num = 4;
            WriteByte(_board.BoardType, num);
            num++;
            if (_board.BoardType == 1)
            {
                WriteByte((byte)_board.BoardList.Count(), num);
                num++;
                {
                    foreach (BoardS board in _board.BoardList)
                    {
                        WriteUShort(board.BoardID, num);
                        num += 2;
                        WriteByte((byte)board.BoardName.Length, num);
                        num++;
                        WriteString(board.BoardName, num);
                        num += board.BoardName.Length;
                    }
                    return;
                }
            }
            if (_board.BoardType == 2 || _board.BoardType == 4)
            {
                WriteByte(_board.Board.Permission, num);
                num++;
                WriteUShort(_board.Board.BoardID, num);
                num += 2;
                WriteByte((byte)_board.Board.BoardName.Length, num);
                num++;
                WriteString(_board.Board.BoardName, num);
                num += _board.Board.BoardName.Length;
                WriteByte((byte)_board.Board.PostList.Count(), num);
                num++;
                {
                    foreach (PostS post in _board.Board.PostList)
                    {
                        WriteBool(post.Highlight, num);
                        num++;
                        WriteUShort(post.PostNumber, num);
                        num += 2;
                        WriteByte((byte)post.PosterName.Length, num);
                        num++;
                        WriteString(post.PosterName, num);
                        num += post.PosterName.Length;
                        WriteByte(post.Day, num);
                        num++;
                        WriteByte(post.Month, num);
                        num++;
                        WriteByte((byte)post.PostTitle.Length, num);
                        num++;
                        WriteString(post.PostTitle, num);
                        num += post.PostTitle.Length;
                    }
                    return;
                }
            }
            if (_board.BoardType == 3 || _board.BoardType == 5)
            {
                WriteByte(_board.Post.Permission, num);
                num++;
                WriteBool(_board.Post.Highlight, num);
                num++;
                WriteUShort(_board.Post.PostNumber, num);
                num += 2;
                WriteByte((byte)_board.Post.PosterName.Length, num);
                num++;
                WriteString(_board.Post.PosterName, num);
                num += _board.Post.PosterName.Length;
                WriteByte(_board.Post.Day, num);
                num++;
                WriteByte(_board.Post.Month, num);
                num++;
                WriteByte((byte)_board.Post.PostTitle.Length, num);
                num++;
                WriteString(_board.Post.PostTitle, num);
                num += _board.Post.PostTitle.Length;
                WriteUShort((ushort)_board.Post.PostBody.Length, num);
                num += 2;
                WriteString(_board.Post.PostBody, num);
                num += _board.Post.PostBody.Length;
            }
            else if (_board.BoardType == 6 || _board.BoardType == 7)
            {
                WriteBool(_board.Prompt.CloseBoard, num);
                num++;
                WriteByte((byte)_board.Prompt.PromptText.Length, num);
                num++;
                WriteString(_board.Prompt.PromptText, num);
                num += _board.Prompt.PromptText.Length;
            }
        }
    }

    public DisplayBoardPacket(DisplayBoardS board, ushort length)
        : base((ushort)(4 + length), 39)
    {
        Board = board;
    }

    public DisplayBoardPacket(byte[] packet)
        : base(packet)
    {
    }
}
