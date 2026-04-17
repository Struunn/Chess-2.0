using System.Security.AccessControl;

// Places the pieces in the starting setup
Piece[,] board = new Piece[,] { 
                        { Piece.Rook, Piece.Knight, Piece.Bishop, Piece.King, Piece.Queen, Piece.Bishop, Piece.Knight, Piece.Rook },
                        { Piece.Pawn___, Piece.Pawn___, Piece.Pawn___, Piece.Pawn___, Piece.Pawn___, Piece.Pawn___, Piece.Pawn___, Piece.Pawn___ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
                        { Piece.Pawni__, Piece.Pawni__, Piece.Pawni__, Piece.Pawni__, Piece.Pawni__, Piece.Pawni__, Piece.Pawni__, Piece.Pawni__ },
                        { Piece.Rooki__, Piece.Knighti, Piece.Bishopi, Piece.Kingi__, Piece.Queeni_, Piece.Bishopi, Piece.Knighti, Piece.Rooki__ }};

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
    Console.ResetColor();
}

void PawnMove(int tempSelectedRow, int tempSelectedColumn)
{
    Piece position = board[tempSelectedRow, tempSelectedColumn];
    bool isWhite = true;

    if (position == Piece.Pawni__) // Determines which side the pawn belongs to
    {
        isWhite = true;
    }
    else if (position == Piece.Pawn___)
    {
        isWhite = false;
    }

    while (true)
    {
        if (isWhite)
        {
            DrawSquare(tempSelectedRow - 1, tempSelectedColumn, false, true);
            if (tempSelectedRow == 6)
            {
                DrawSquare(tempSelectedRow - 2, tempSelectedColumn, false, true);
            }
        }
        MoveSelectedSquare();
        if (MoveSelectedSquare() == ConsoleKey.Enter)
        {
            if (board[selectedRow, selectedColumn] == Piece._______)
            {
                board[tempSelectedRow, tempSelectedColumn] = Piece._______;
                board[selectedRow, selectedColumn] = Piece.Pawni__;
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
            if (board[height, length] == Piece.Rooki__ ||     // Checks if piece belongs to White, changes its color accordingly
                board[height, length] == Piece.Knighti ||
                board[height, length] == Piece.Bishopi ||
                board[height, length] == Piece.Queeni_ ||
                board[height, length] == Piece.Kingi__ ||
                board[height, length] == Piece.Pawni__)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (board[height, length] == Piece._______)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            }
            DrawSquare(height, length, false, false);
        }
    }
}
DrawBoard();
ConsoleKey MoveSelectedSquare()
{
    ConsoleKey Navigate = Console.ReadKey(true).Key;
    switch (Navigate)
    {
        case ConsoleKey.UpArrow:
            selectedRow--;
            DrawSquare(selectedRow + 1, selectedColumn, false, false);
            DrawSquare(selectedRow, selectedColumn, false, false);
            break;

        case ConsoleKey.DownArrow:
            selectedRow++;
            DrawSquare(selectedRow - 1, selectedColumn, false, false);
            DrawSquare(selectedRow, selectedColumn, false, false);
            break;

        case ConsoleKey.LeftArrow:
            selectedColumn--;
            DrawSquare(selectedRow, selectedColumn + 1, false, false);
            DrawSquare(selectedRow, selectedColumn, false, false);
            break;

        case ConsoleKey.RightArrow:
            selectedColumn++;
            DrawSquare(selectedRow, selectedColumn - 1, false, false);
            DrawSquare(selectedRow, selectedColumn, false, false);
            break;
    }
    return Navigate;
}
// Start of game loop
while (true)
{
    MoveSelectedSquare();
    if (MoveSelectedSquare() == ConsoleKey.Enter)
    {
        if (board[selectedRow, selectedColumn] == Piece.Pawni__ ||
            board[selectedRow, selectedColumn] == Piece.Pawn___)
        {
            PawnMove(selectedRow, selectedColumn);
        }
    }
}
enum Piece // List of possible pieces
{
    Rooki__, Rook, Knighti, Knight, Bishopi, Bishop, Kingi__, King, Queeni_, Queen, Pawni__, Pawn___, _______
}