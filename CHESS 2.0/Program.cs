
// Places the pieces in the starting setup
Piece[,] board = new Piece[,] { 
                        { Piece._Rook__, Piece.Knight_, Piece.Bishop_, Piece._King__, Piece._Queen_, Piece.Bishop_, Piece.Knight_, Piece._Rook__ },
                        { Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_ },
                        { Piece._Rooki_, Piece.Knighti, Piece.Bishopi, Piece._Kingi_, Piece.Queeni_, Piece.Bishopi, Piece.Knighti, Piece._Rooki_ }};

// Keeps track of the position of the selected square
int selectedRow = 3;
int selectedColumn = 3;
void DrawSquare(int Row, int Col, bool canBeCaptured, bool canBeMovedTo) // Draws the square
{
    bool isSelected = (Row == selectedRow && Col == selectedColumn);
    bool isWhite;
    if ((Col + Row) % 2 == 0)
    {
        isWhite = true;
    }
    else
    {
        isWhite = false;
    }
    Console.BackgroundColor = isSelected ? ConsoleColor.Green : 
        canBeCaptured? ConsoleColor.Red : 
        canBeMovedTo? ConsoleColor.DarkYellow : 
        isWhite ? ConsoleColor.White : ConsoleColor.DarkGray;

    if (board[Row, Col] == Piece._Rooki_ ||     // Checks if piece belongs to White, changes its color accordingly
                board[Row, Col] == Piece.Knighti ||
                board[Row, Col] == Piece.Bishopi ||
                board[Row, Col] == Piece.Queeni_ ||
                board[Row, Col] == Piece._Kingi_ ||
                board[Row, Col] == Piece._Pawni_)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
    }
    else if (board[Row, Col] == Piece._______)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
    }

    // Draws the whole square
    if (Row == 0)
    {
        Row = 1;
        Console.SetCursorPosition(Col * 7, Row - 1);
        Row = 0;
    }
    else
    {
        Console.SetCursorPosition(Col * 7, Row * 3 - 1);
    }
    Console.Write("       ");
    Console.SetCursorPosition(Col * 7, Row * 3);
    Console.Write(board[Row, Col]);
    Console.SetCursorPosition(Col * 7, 1 + Row * 3);
    Console.Write("       ");
    Console.SetCursorPosition(Col * 7, Row * 3);
}

void PawnMove(int tempSelectedRow, int tempSelectedColumn)
{
    Piece position = board[tempSelectedRow, tempSelectedColumn];
    bool isWhite = true;

    if (position == Piece._Pawni_) // Determines which side the pawn belongs to
    {
        isWhite = true;
    }
    else if (position == Piece._Pawn__)
    {
        isWhite = false;
    }

    while (true)
    {
        if (isWhite)
        {
            if (board[tempSelectedRow - 1, tempSelectedColumn] == Piece._______) // Shows the squares the pawn can move to, if any
            {
                DrawSquare(tempSelectedRow - 1, tempSelectedColumn, false, true);
                if (tempSelectedRow == 6 && board[tempSelectedRow - 2, tempSelectedColumn] == Piece._______)
                {
                    DrawSquare(tempSelectedRow - 2, tempSelectedColumn, false, true);
                }
            }
            else
            {
                break;
            }
            if (MoveSelectedSquare() == ConsoleKey.Enter && board[selectedRow, selectedColumn] == Piece._______) // Moves the pawn if possible
            {
                board[tempSelectedRow, tempSelectedColumn] = Piece._______;
                board[selectedRow, selectedColumn] = Piece._Pawni_;
                DrawBoard();
                break;
            }
        }
    }
}

void DrawBoard()
{
    Console.Clear();
    for (int length = 0; length < 8; length++) // Draws the board
    {
        for (int height = 0; height < 8; height++)
        {
            Console.SetCursorPosition(3 + length * 10, 2 + height * 3);
            
            DrawSquare(height, length, false, false);
        }
    }
}
DrawBoard();
void UpdateSquare(int offsetRow, int offsetCol)
{
    DrawSquare(selectedRow + offsetRow, selectedColumn + offsetCol, false, false);
}
ConsoleKey MoveSelectedSquare() // Navigate the board
{
    ConsoleKey Navigate = Console.ReadKey(true).Key;
    switch (Navigate)
    {
        case ConsoleKey.UpArrow:
            selectedRow--;
            UpdateSquare(1, 0);
            UpdateSquare(0, 0);
            break;

        case ConsoleKey.DownArrow:
            selectedRow++;
            UpdateSquare(-1, 0);
            UpdateSquare(0, 0);
            break;

        case ConsoleKey.LeftArrow:
            selectedColumn--;
            UpdateSquare(0, 1);
            UpdateSquare(0, 0);
            break;

        case ConsoleKey.RightArrow:
            selectedColumn++;
            UpdateSquare(0, -1);
            UpdateSquare(0, 0);
            break;
    }
    return Navigate;
}

// Start of game loop
while (true)
{
    if (MoveSelectedSquare() == ConsoleKey.Enter)
    {
        if (board[selectedRow, selectedColumn] == Piece._Pawni_ ||
            board[selectedRow, selectedColumn] == Piece._Pawn__)
        {
            PawnMove(selectedRow, selectedColumn);
        }
    }
}
enum Piece // List of possible pieces
{
    _Rooki_, _Rook__, Knighti, Knight_, Bishopi, Bishop_, _Kingi_, _King__, Queeni_, _Queen_, _Pawni_, _Pawn__, _______
}