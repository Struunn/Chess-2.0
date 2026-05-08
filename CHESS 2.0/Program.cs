
using System.ComponentModel;

bool[,] canBeCaptured = new bool[8, 8];
bool[,] canBeMovedTo = new bool[8, 8];
bool[,] kingCheckSquares = new bool[8, 8];

/// Resets the canBeCaptured and canBeMovedTo arrays
void ResetMoveData()
{
    for (int i = 0; i < 8; i++)
    {
        for (int j = 0; j < 8; j++)
        {
            canBeMovedTo[i, j] = false;
            canBeCaptured[i, j] = false;
            kingCheckSquares[i, j] = false;
        }
    }
}

/// Places the pieces in the starting setup
Piece[,] board = new Piece[,] { 
        { Piece._Rook__, Piece.Knight_, Piece.Bishop_, Piece._Queen_, Piece._King__, Piece.Bishop_, Piece.Knight_, Piece._Rook__ },
        { Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__, Piece._Pawn__ },
        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
        { Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______, Piece._______ },
        { Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_, Piece._Pawni_ },
        { Piece._Rooki_, Piece.Knighti, Piece.Bishopi, Piece.Queeni_, Piece._Kingi_, Piece.Bishopi, Piece.Knighti, Piece._Rooki_ }};

/// Checks if the square is occupied by a white piece
bool isWhitePiece(Piece? piece)
{
    return
        piece == Piece._Pawni_ ||
        piece == Piece._Rooki_ ||
        piece == Piece.Knighti ||
        piece == Piece.Bishopi ||
        piece == Piece.Queeni_ ||
        piece == Piece._Kingi_;
}

/// Checks if the square is occupied by a black piece
bool isBlackPiece(Piece? piece)
{
    return
        piece == Piece._Pawn__ ||
        piece == Piece._Rook__ ||
        piece == Piece.Knight_ ||
        piece == Piece.Bishop_ ||
        piece == Piece._Queen_ ||
        piece == Piece._King__;
}

// Keeps track of the position of the selected square
int selectedRow = 4;
int selectedColumn = 4;
int y = 0;
int x = 0;

/// Draws a square
void DrawSquare(int row, int col, bool canBeCaptured, bool canBeMovedTo, bool kingInCheck) // Draws the square
{
    bool isSelected = (row == selectedRow && col == selectedColumn);

    bool isWhiteSquare;
    if (((col + row) & 1) == 0) // Checks if the square is white or not
    {
        isWhiteSquare = true;
    }
    else
    {
        isWhiteSquare = false;
    }
    Console.BackgroundColor = isSelected? ConsoleColor.Green :
                              kingInCheck? ConsoleColor.Blue :
                              canBeCaptured? ConsoleColor.Red :
                              canBeMovedTo? ConsoleColor.DarkYellow :
                              isWhiteSquare? ConsoleColor.White : ConsoleColor.DarkGray;

    if (board[row, col] == Piece._Rooki_ || // Checks if piece belongs to White, changes its color accordingly
        board[row, col] == Piece.Knighti ||
        board[row, col] == Piece.Bishopi ||
        board[row, col] == Piece.Queeni_ ||
        board[row, col] == Piece._Kingi_ ||
        board[row, col] == Piece._Pawni_)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
    }
    else if (board[row, col] == Piece._______)
    {
        Console.ForegroundColor = isSelected? ConsoleColor.Green : isWhiteSquare? ConsoleColor.White : ConsoleColor.DarkGray;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
    }

    // Draws the whole square
    if (row == 0) // I need this, otherwise it breaks
    {
        Console.SetCursorPosition(col * 7, row);
    }
    else
    {
        Console.SetCursorPosition(col * 7, row * 3 - 1);
    }
    //Console.Write("       ");
    //Console.Write($" {x}, {y}  ");
    Console.Write($" {kingCheckSquares[row, col]} ");
    Console.SetCursorPosition(col * 7, row * 3);
    Console.Write(board[row, col]);
    Console.SetCursorPosition(col * 7, 1 + row * 3);
    //Console.Write("       ");
    Console.Write($"{row},{col}:{y},{x}");
    Console.SetCursorPosition(col * 7, row * 3);
}


// ----------------------------- METHODS FOR PIECE MOVES -----------------------------

int whiteKingRowPos = 7;
int whiteKingColPos = 4;
int blackKingRowPos = 0;
int blackKingColPos = 4;

/// Sets the pieces position to the new selected position
void SetPiecePos(Piece piece, int tempSelectedRow, int tempSelectedColumn, int selectedRowOffset, int selectedColOffset)
{
    board[tempSelectedRow, tempSelectedColumn] = Piece._______; // Sets old place to free
    board[selectedRow + selectedRowOffset, selectedColumn + selectedColOffset] = piece; // Sets new place to piece
    DrawBoard(); // Updates every square
}

/// Sets the square to moveable to
void CanBeMovedToPos(int rowOffset, int colOffset, int tempSelectedRow, int tempSelectedColumn)
{
    int row = tempSelectedRow + rowOffset;
    int col = tempSelectedColumn + colOffset;
    canBeMovedTo[row, col] = true;
    DrawSquare(row, col, false, true, false); // Updates square to be able to move to
}

/// Sets the square to captureable
void CanBeCapturedPos(int rowOffset, int colOffset, int tempSelectedRow, int tempSelectedColumn)
{
    int row = tempSelectedRow + rowOffset;
    int col = tempSelectedColumn + colOffset;
    if (row > 7)
    {
        row = 7;
    }
    else if (row < 0)
    {
        row = 0;
    }
    if (col >= 0 && col <= 7 && // Checks if it's within bounds
        row >= 0 && row <= 7)
    {
        canBeCaptured[row, col] = true;
        DrawSquare(row, col, true, false, false); // Updates square to be able to capture to
    }
}

/// Gets the position and the piece that is there
Piece? GetPosition(int rowOffset, int colOffset, int tempSelectedRow, int tempSelectedColumn)
{
    Piece? position;
    int row = tempSelectedRow + rowOffset;
    int col = tempSelectedColumn + colOffset;

    if (row >= 0 && row < 8 && col >= 0 && col < 8) // Checks if it's within bounds
    {
        return position = board[row, col]; // Returns the piece on the requested square location
    }
    else
    {
        return null; // If it's out of bounds it returns null
    }
}

/// Checks if the Piece on the square is white or not
bool IsWhite(Piece piece, int tempSelectedRow, int tempSelectedColumn)
{
    bool isWhite;
    if (GetPosition(0, 0, tempSelectedRow, tempSelectedColumn) == piece) // Determines which side the pawn belongs to
    {
        return isWhite = true;
    }
    else
    {
        return isWhite = false;
    }
}

/// Checks directions until it gets blocked / can capture
void CanMoveUntilBlocked(int rowOffset, int colOffset, bool isWhite, int tempSelectedRow, int tempSelectedColumn, bool checkKingCheck)
{
    for (int i = 1; i < 8; i++)
    {
        int row = tempSelectedRow + rowOffset * i;
        int col = tempSelectedColumn + colOffset * i;

        if (row < 0 || row > 7 || col < 0 || col > 7) // Stop if out of bounds
        {
            break;
        }

        Piece target = board[row, col]; // The square it is checking
        if (target == Piece._______ && !checkKingCheck) // Checks if the square is empty
        {
            CanBeMovedToPos(rowOffset * i, colOffset * i, tempSelectedRow, tempSelectedColumn);
        }
        else
        {
            if (isWhite? !isWhitePiece(target) : !isBlackPiece(target)) // Checks if it can capture on the square
            {
                if (checkKingCheck)
                {
                    kingCheckSquares[tempSelectedRow + rowOffset * i, tempSelectedColumn + colOffset * i] = true;
                    //CanMoveUntilBlocked(rowOffset, colOffset, isWhite, tempSelectedRow, tempSelectedColumn, true);
                }
                else
                {
                    CanBeCapturedPos(rowOffset * i, colOffset * i, tempSelectedRow, tempSelectedColumn);
                }
            }
            break;
        }
    }
}

/// Checks if the king is (or will be) in check
void CheckKingCheck(bool isWhite)
{
    void CheckPieceThatChecks(int rowDirection, int colDirection)
    {
        CanMoveUntilBlocked(rowDirection, colDirection, isWhite, isWhite? whiteKingRowPos : blackKingRowPos,
                            isWhite? whiteKingColPos : blackKingColPos, true);

        for (int i = 0; i < 7; i++)
        {
            x = i;

            for (int j = 0; j < 7; j++)
            {
                y = j;
                
                if (kingCheckSquares[j, i])
                {
                    if (rowDirection == 0 || colDirection == 0)
                    {
                        //CheckPieceThatChecks(1, 0);
                        DrawSquare(whiteKingRowPos,whiteKingColPos, false, false, true);
                    }
                }
            }
        }
    }

    CheckPieceThatChecks(1, 0);
    CheckPieceThatChecks(-1, 0);
    CheckPieceThatChecks(0, 1);
    CheckPieceThatChecks(0, -1);
    CheckPieceThatChecks(1, 1);
    CheckPieceThatChecks(-1, -1);
    CheckPieceThatChecks(-1, 1);
    CheckPieceThatChecks(1, -1);
}


// ----------------------------- PIECE MOVES -----------------------------

// Keeps track of Rook and King first moves for castling purposes
bool whiteRook1HasMoved = false;
bool blackRook1HasMoved = false;
bool whiteRook2HasMoved = false;
bool blackRook2HasMoved = false;
bool whiteKingHasMoved = false;
bool blackKingHasMoved = false;

/// Controls the movement of the Pawn
void PawnMove(int tempSelectedRow, int tempSelectedColumn) // ----- PAWN -----
{
    bool isWhite = IsWhite(Piece._Pawni_, tempSelectedRow, tempSelectedColumn);
    int rowOffset;
    int colOffset;

    /// Gets the
    Piece? TargetSquare(int RowOffset, int ColOffset)
    {
        rowOffset = RowOffset;
        colOffset = ColOffset;
        if (tempSelectedRow + rowOffset > 7 || tempSelectedRow + rowOffset < 0 ||
            tempSelectedColumn + colOffset > 7 || tempSelectedColumn + colOffset < 0)
        {
            return null;
        }
        Piece targetSquare = board[tempSelectedRow + RowOffset, tempSelectedColumn + ColOffset];
        return targetSquare;
    }

    while (true)
    {
        if (TargetSquare(isWhite? -1 : 1, 0) == Piece._______) // Checks if the square right in front of the pawn is free
        {
            CanBeMovedToPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
            
            if (TargetSquare(isWhite? -2 : 2, 0) == Piece._______ && isWhite ? tempSelectedRow == 6 : tempSelectedRow == 1) // Checks if the square 2 in front of the pawn is free and if it's on its starting row
            {
                CanBeMovedToPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
            }
        }
        if (TargetSquare(isWhite? -1 : 1, 1) != Piece._______) // Checks if it can capture the square on the right
        {
            if (isWhite? !isWhitePiece(TargetSquare(isWhite ? -1 : 1, 1)) :
            !isBlackPiece(TargetSquare(isWhite ? -1 : 1, 1)))
            {
                CanBeCapturedPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
            }
        }
        if (TargetSquare(isWhite? -1 : 1, -1) != Piece._______) // Checks if it can capture the square on the left
        {
            if (isWhite ? !isWhitePiece(TargetSquare(isWhite ? -1 : 1, -1)) :
            !isBlackPiece(TargetSquare(isWhite ? -1 : 1, -1)))
            {
                CanBeCapturedPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
            }
        }

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                SetPiecePos(isWhite ? Piece._Pawni_ : Piece._Pawn__, tempSelectedRow, tempSelectedColumn, 0, 0);
                Promotion();
                CheckKingCheck(isWhite);
                break;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                SetPiecePos(isWhite ? Piece._Pawni_ : Piece._Pawn__, tempSelectedRow, tempSelectedColumn, 0, 0);
                Promotion();
                CheckKingCheck(isWhite);
                break;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard();
                break;
            }
        }

        // ----- Promotion -----
        void Promotion()
        {
            void ReplacePiece(Piece whitePiece, Piece blackPiece, int i)
            {
                board[isWhite ? 0 : 7, i] = isWhite ? whitePiece : blackPiece;
                SetPiecePos(isWhite ? whitePiece : blackPiece, tempSelectedRow, tempSelectedColumn, 0, 0);
            }
            string[] options = {$"{(isWhite? Piece.Queeni_ : Piece._Queen_)}",
                                $"{(isWhite ? Piece._Rooki_ : Piece._Rook__)}",
                                $"{(isWhite? Piece.Bishopi : Piece.Bishop_)}",
                                $"{(isWhite? Piece.Knighti : Piece.Knight_)}"};
            int selected = 0;

            for (int i = 0; i < 7; i++)
            {
                if (board[0, i] == Piece._Pawni_ || board[7, i] == Piece._Pawn__)
                {
                    Console.ResetColor();

                    while (board[0, i] == Piece._Pawni_ || board[7, i] == Piece._Pawn__)
                    {
                        if (selected < 0)
                        {
                            selected = 3;
                        }
                        else if (selected > 3)
                        {
                            selected = 0;
                        }

                        Console.SetCursorPosition(0, 25);
                        for (int j = 0; j < options.Length; j++)
                        {
                            if (j == selected)
                            {
                                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                                Console.WriteLine($"> {options[j]}");
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.Black;
                                Console.WriteLine($"  {options[j]}");
                            }
                        }

                        ConsoleKey Choice = Console.ReadKey(true).Key;
                        switch (Choice)
                        {
                            case ConsoleKey.DownArrow:
                                selected++;
                                break;

                            case ConsoleKey.UpArrow:
                                selected--;
                                break;

                            case ConsoleKey.Enter:
                                switch (selected)
                                {
                                    case 0:
                                        ReplacePiece(Piece.Queeni_, Piece._Queen_, i);
                                        break;

                                    case 1:
                                        ReplacePiece(Piece._Rooki_, Piece._Rook__, i);
                                        break;

                                    case 2:
                                        ReplacePiece(Piece.Bishopi, Piece.Bishop_, i);
                                        break;

                                    case 3:
                                        ReplacePiece(Piece.Knighti, Piece.Knight_, i);
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}

/// Controls the movement of the Rook
void RookMove(int tempSelectedRow, int tempSelectedColumn) // ----- ROOK -----
{
    bool isWhite = IsWhite(Piece._Rooki_, tempSelectedRow, tempSelectedColumn);

    void FirsRookMove()
    {
        if (tempSelectedRow == 7 && tempSelectedColumn == 0)
        {
            whiteRook1HasMoved = true;
        }
        if (tempSelectedRow == 7 && tempSelectedColumn == 7)
        {
            whiteRook2HasMoved = true;
        }
        if (tempSelectedRow == 0 && tempSelectedColumn == 0)
        {
            blackRook1HasMoved = true;
        }
        if (tempSelectedRow == 0 && tempSelectedColumn == 7)
        {
            blackRook2HasMoved = true;
        }
    }

    while (true)
    {
        CanMoveUntilBlocked(1, 0, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(-1, 0, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(0, 1, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(0, -1, isWhite, tempSelectedRow, tempSelectedColumn, false);

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                FirsRookMove();
                SetPiecePos(isWhite? Piece._Rooki_ : Piece._Rook__, tempSelectedRow, tempSelectedColumn, 0, 0);
                CheckKingCheck(isWhite);
                break;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                FirsRookMove();
                SetPiecePos(isWhite? Piece._Rooki_ : Piece._Rook__, tempSelectedRow, tempSelectedColumn, 0, 0);
                CheckKingCheck(isWhite);
                break;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard();
                break;
            }
        }
    }
}

/// Controls the movement of the Bishop
void BishopMove(int tempSelectedRow, int tempSelectedColumn) // ----- BISHOP -----
{
    bool isWhite = IsWhite(Piece.Bishopi, tempSelectedRow, tempSelectedColumn);
    while (true)
    {
        CanMoveUntilBlocked(1, 1, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(-1, -1, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(-1, 1, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(1, -1, isWhite, tempSelectedRow, tempSelectedColumn, false);

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                SetPiecePos(isWhite ? Piece.Bishopi : Piece.Bishop_, tempSelectedRow, tempSelectedColumn, 0, 0);
                CheckKingCheck(isWhite);
                break;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                SetPiecePos(isWhite ? Piece.Bishopi : Piece.Bishop_, tempSelectedRow, tempSelectedColumn, 0, 0);
                CheckKingCheck(isWhite);
                break;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard();
                break;
            }
        }
    }
}

/// Controls the movement of the Queen
void QueenMove(int tempSelectedRow, int tempSelectedColumn) // ----- QUEEN -----
{
    bool isWhite = IsWhite(Piece.Queeni_, tempSelectedRow, tempSelectedColumn);
    while (true)
    {
        CanMoveUntilBlocked(1, 0, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(-1, 0, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(0, 1, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(0, -1, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(1, 1, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(-1, -1, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(-1, 1, isWhite, tempSelectedRow, tempSelectedColumn, false);
        CanMoveUntilBlocked(1, -1, isWhite, tempSelectedRow, tempSelectedColumn, false);

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                SetPiecePos(isWhite ? Piece.Queeni_ : Piece._Queen_, tempSelectedRow, tempSelectedColumn, 0, 0);
                CheckKingCheck(isWhite);
                break;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                SetPiecePos(isWhite ? Piece.Queeni_ : Piece._Queen_, tempSelectedRow, tempSelectedColumn, 0, 0);
                CheckKingCheck(isWhite);
                break;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard();
                break;
            }
        }
    }
}

/// Controls the movement of the Knight
void KnightMove(int tempSelectedRow, int tempSelectedColumn)// ----- KNIGHT -----
{
    bool isWhite = IsWhite(Piece.Knighti, tempSelectedRow, tempSelectedColumn);

    /// Checks where the Knight can go
    void Move(int rowOffset, int colOffset)
    {
        bool stop = false;

        int row = tempSelectedRow + rowOffset;
        int col = tempSelectedColumn + colOffset;

        if (row < 0 || row > 7 || col < 0 || col > 7)
        {
            stop = true;
        }
        
        if (!stop)
        {
            Piece target = board[tempSelectedRow + rowOffset, tempSelectedColumn + colOffset];
            if (target == Piece._______)
            {
                CanBeMovedToPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
            }
            else
            {
                if (isWhite? !isWhitePiece(target) : !isBlackPiece(target))
                {
                    CanBeCapturedPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
                }
            }
        }
    }

    while (true)
    {
        Move(2, 1);
        Move(2, -1);
        Move(-2, 1);
        Move(-2, -1);
        Move(1, 2);
        Move(1, -2);
        Move(-1, 2);
        Move(-1, -2);

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                SetPiecePos(isWhite ? Piece.Knighti : Piece.Knight_, tempSelectedRow, tempSelectedColumn, 0, 0);
                CheckKingCheck(isWhite);
                break;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                SetPiecePos(isWhite ? Piece.Knighti : Piece.Knight_, tempSelectedRow, tempSelectedColumn, 0, 0);
                CheckKingCheck(isWhite);
                break;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard();
                break;
            }
        }
    }
}

/// Controls the movement of the King
void KingMove(int tempSelectedRow, int tempSelectedColumn) // ----- KING -----
{
    bool isWhite = IsWhite(Piece._Kingi_, tempSelectedRow, tempSelectedColumn);

    void Move(int rowOffset, int colOffset)
    {
        int row = tempSelectedRow + rowOffset;
        int col = tempSelectedColumn + colOffset;        

        bool stop = false;
        if (row < 0 || row > 7 || col < 0 || col > 7)
        {
            stop = true;
        }

        if (!stop)
        {
            Piece target = board[tempSelectedRow + rowOffset, tempSelectedColumn + colOffset];
            if (target == Piece._______)
            {
                CanBeMovedToPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
            }
            else if (isWhite ? !isWhitePiece(target) : !isBlackPiece(target))
            {
                CanBeCapturedPos(rowOffset, colOffset, tempSelectedRow, tempSelectedColumn);
            }
        }
    }

    void KingHasMoved()
    {
        if (isWhite)
        {
            whiteKingHasMoved = true;
        }
        else
        {
            blackKingHasMoved = true;
        }
    }

    while (true)
    {
        Move(1, 0);
        Move(1, 1);
        Move(-1, 0);
        Move(-1, -1);
        Move(0, 1);
        Move(0, -1);
        Move(1, -1);
        Move(-1, 1);

        if (isWhite? !whiteKingHasMoved : !blackKingHasMoved)
        {
            if (isWhite? !whiteRook1HasMoved : !blackRook1HasMoved)
            {
                if (canBeMovedTo[tempSelectedRow, tempSelectedColumn - 1] &&
                    board[tempSelectedRow, tempSelectedColumn - 2] == Piece._______ &&
                    board[tempSelectedRow, tempSelectedColumn - 3] == Piece._______ &&
                    isWhite? board[7, 0] == Piece._Rooki_ : board[0, 0] == Piece._Rook__)
                {
                    Move(0, -2);
                }
            }
            if (isWhite? !whiteRook2HasMoved : !blackRook2HasMoved)
            {
                if (canBeMovedTo[tempSelectedRow, tempSelectedColumn + 1] &&
                    board[tempSelectedRow, tempSelectedColumn + 2] == Piece._______ &&
                    isWhite ? board[7, 7] == Piece._Rooki_ : board[0, 7] == Piece._Rook__)
                {
                    Move(0, 2);
                }
            }
        }

        // ----- Move Piece -----
        if (MoveSelectedSquare() == ConsoleKey.Enter) // Moves the piece if possible
        {
            if (canBeMovedTo[selectedRow, selectedColumn]) // Move
            {
                if (selectedColumn == tempSelectedColumn + 2)
                {
                    SetPiecePos(isWhite? Piece._Rooki_ : Piece._Rook__, isWhite? 7 : 0, 7, 0, -1);
                }
                if (selectedColumn == tempSelectedColumn - 2)
                {
                    SetPiecePos(isWhite? Piece._Rooki_ : Piece._Rook__, isWhite ? 7 : 0, 0, 0, 1);
                }

                KingHasMoved();
                SetPiecePos(isWhite? Piece._Kingi_ : Piece._King__, tempSelectedRow, tempSelectedColumn, 0, 0);
                CheckKingCheck(isWhite);

                if (isWhite)
                {
                    whiteKingRowPos = selectedRow;
                    whiteKingColPos = selectedColumn;
                }
                else
                {
                    blackKingRowPos = selectedRow;
                    blackKingColPos= selectedColumn;
                }
                break;
            }
            if (canBeCaptured[selectedRow, selectedColumn]) // Capture
            {
                KingHasMoved();
                SetPiecePos(isWhite? Piece._Kingi_ : Piece._King__, tempSelectedRow, tempSelectedColumn, 0, 0);
                CheckKingCheck(isWhite);

                if (isWhite)
                {
                    whiteKingRowPos = selectedRow;
                    whiteKingColPos = selectedColumn;
                }
                else
                {
                    blackKingRowPos = selectedRow;
                    blackKingColPos = selectedColumn;
                }
                break;
            }
            else if (selectedRow == tempSelectedRow && selectedColumn == tempSelectedColumn) // Selecting original pos to deselect
            {
                DrawBoard();
                break;
            }
        }
    }
}

// ----------------------------- -----------------------------

/// Draws the board
void DrawBoard()
{
    ResetMoveData();
    Console.ResetColor();
    Console.Clear();
    for (int length = 0; length < 8; length++)
    {
        for (int height = 0; height < 8; height++)
        {
            Console.SetCursorPosition(3 + length * 10, 2 + height * 3);
            
            DrawSquare(height, length, canBeCaptured[height, length], canBeMovedTo[height, length], kingCheckSquares[height, length]);
        }
    }
}

/// Updates the square to be redrawn with the current data
void UpdateSquare(int offsetRow, int offsetCol)
{
    DrawSquare(selectedRow + offsetRow, selectedColumn + offsetCol, false, false, false);
}

/// Moves the selected square around
ConsoleKey MoveSelectedSquare() // Navigate the board
{
    ConsoleKey Navigate = Console.ReadKey(true).Key;
    switch (Navigate)
    {
        case ConsoleKey.UpArrow: // Moves up
            if (selectedRow > 0)
            {
                selectedRow--;
                UpdateSquare(1, 0);
                UpdateSquare(0, 0);
            }
            break;

        case ConsoleKey.DownArrow: // Moves down
            if (selectedRow < 7)
            {
                selectedRow++;
                UpdateSquare(-1, 0);
                UpdateSquare(0, 0);
            }
            break;

        case ConsoleKey.LeftArrow: // Moves left
            if (selectedColumn > 0)
            {
                selectedColumn--;
                UpdateSquare(0, 1);
                UpdateSquare(0, 0);
            }
            break;

        case ConsoleKey.RightArrow: // Moves right
            if (selectedColumn < 7)
            {
                selectedColumn++;
                UpdateSquare(0, -1);
                UpdateSquare(0, 0);
            }
            break;
    }
    return Navigate;
}

DrawBoard(); // Draws the board at the start

// Start of game loop
while (true)
{
    if (MoveSelectedSquare() == ConsoleKey.Enter)
    {
        Piece selectedSquare = board[selectedRow, selectedColumn];

        if (selectedSquare == Piece._Pawni_ ||
            selectedSquare == Piece._Pawn__)
        {
            PawnMove(selectedRow, selectedColumn);
        }

        if (selectedSquare == Piece._Rooki_ ||
            selectedSquare == Piece._Rook__)
        {
            RookMove(selectedRow, selectedColumn);
        }

        if (selectedSquare == Piece.Bishopi ||
            selectedSquare == Piece.Bishop_)
        {
            BishopMove(selectedRow, selectedColumn);
        }

        if (selectedSquare == Piece.Queeni_ ||
            selectedSquare == Piece._Queen_)
        {
            QueenMove(selectedRow, selectedColumn);
        }

        if (selectedSquare == Piece.Knighti ||
            selectedSquare == Piece.Knight_)
        {
            KnightMove(selectedRow, selectedColumn);
        }

        if (selectedSquare == Piece._Kingi_ ||
            selectedSquare == Piece._King__)
        {
            KingMove(selectedRow, selectedColumn);
        }
    }
    else if (MoveSelectedSquare() == ConsoleKey.E)
    {
        SetPiecePos(Piece._______, selectedRow, selectedColumn, 0, 0);
    }
}

enum Piece // List of possible pieces
{
    _Rooki_, _Rook__, Knighti, Knight_, Bishopi, Bishop_, _Kingi_, _King__, Queeni_, _Queen_, _Pawni_, _Pawn__, _______
}