using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Chess1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D _square,
            _whitePawn, _blackPawn,
            _whiteRook, _blackRook,
            _whiteKnight, _blackKnight,
            _whiteBishop, _blackBishop,
            _whiteKing, _blackKing,
            _whiteQueen, _blackQueen;

        Texture2D _buttonImage,_messageBoxImage;

        SoundEffect _pickUp, _putDown, _wrongMove;
        Song _openingFanFare, _checkMateSong;


        SpriteFont _mainFont,_titleFont;
        
        SpriteChessPiece _chessPiece, _kingPiece;
        SpriteChessBoard _originalChessBoard;

        MessageBox _messageBox;
        string _mainTitle = "Chess!";
      
        List<Sprite> _sprites = new List<Sprite>();
        List<SpriteButton> _buttons = new List<SpriteButton>();
        List<SpriteChessBoard> _potetialBoards = new List<SpriteChessBoard>();
        
        List<List<Point>> _potentialMoves;
        List<List<SpriteChessPiece>> _deadPieces = new List<List<SpriteChessPiece>>();

        GameState _gameState = GameState.Start;

        string _whiteMoveMessage;
        Point _currentPoint;
        Vector2  
                _center;
        bool _mouseLeftButtonPressed,
            _isEscapePressed,
            _cleared,
            _dragged,
            _isSoundPlayed,
            _messageAdded,
            _isS_pressed,
            _whiteTurn = true;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _mainFont = Content.Load<SpriteFont>("MainFont");
            _titleFont = Content.Load<SpriteFont>("titleFont");

            switch (_gameState)
            {
                case GameState.Start:
                    _buttonImage = Content.Load<Texture2D>("buttonImage");
                    _openingFanFare = Content.Load<Song>("fanFare");
                    _buttons.Add(new SpriteButton(_buttonImage, 
                        new Vector2(
                        _graphics.PreferredBackBufferWidth/2 - _buttonImage.Width/2, 
                        (_graphics.PreferredBackBufferHeight/4)*3 
                        ),ButtonType.Start,_mainFont));


                    break;
                case GameState.Game:
                    _wrongMove = Content.Load<SoundEffect>("wrong_move");
                    _pickUp = Content.Load<SoundEffect>("A_note_mute");
                    _putDown = Content.Load<SoundEffect>("F_note_mute");
                    _square = Content.Load<Texture2D>("whiteSquare");
                    _whitePawn = Content.Load<Texture2D>("WhitePawn");
                    _whiteRook = Content.Load<Texture2D>("WhiteRook");
                    _whiteKnight = Content.Load<Texture2D>("WhiteKnight");
                    _whiteBishop = Content.Load<Texture2D>("WhiteBishop");
                    _whiteKing = Content.Load<Texture2D>("WhiteKing");
                    _whiteQueen = Content.Load<Texture2D>("WhiteQueen");
                    _blackPawn = Content.Load<Texture2D>("BlackPawn");
                    _blackRook = Content.Load<Texture2D>("BlackRook");
                    _blackKnight = Content.Load<Texture2D>("BlackKnight");
                    _blackBishop = Content.Load<Texture2D>("BlackBishop");
                    _blackKing = Content.Load<Texture2D>("BlackKing");
                    _blackQueen = Content.Load<Texture2D>("BlackQueen");
                    _checkMateSong = Content.Load<Song>("checkMate");
                    _messageBoxImage = Content.Load<Texture2D>("MessageBox");
                    break;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            
            if (Keyboard.GetState().IsKeyUp(Keys.Escape) && _isEscapePressed)
            {
                _isEscapePressed = false;
            }
            if (!_dragged)
            {
                _chessPiece = _sprites.OfType<SpriteChessPiece>().Where(
                    piece => piece._bounds.Contains(Mouse.GetState().Position) && !piece._isDead).FirstOrDefault();
            }
            foreach(SpriteButton button in _buttons)
            {
                button.Update(_sprites);
            }
            switch (_gameState)
            {
                case GameState.Start:
                    StartScreen();
                    break;
                case GameState.Game:
                    RunGame();
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        _sprites.Clear();
                        _messageBox.ClearMessage();
                        _isEscapePressed = true;
                        _gameState= GameState.Start;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.S)&& !_isS_pressed)
                    {
                        _isS_pressed = true;
                        JsonFileHandler jsonFileHandler = new JsonFileHandler();

                        List<SpriteChessPiece> pieces = _sprites.OfType<SpriteChessPiece>().ToList();


                        jsonFileHandler.SaveChessPiece(pieces);
                    }

                    break;

                case GameState.GameOver:
                    
                    _messageBox.ClearMessage();

                    string winner = _whiteTurn ?  "Black":"White" ;
                    _messageBox.AddMessage($"CheckMate the winner is {winner}, Press ESC to Quit or Press R to Reset");
                    if (Keyboard.GetState().IsKeyDown(Keys.R))
                    {
                        _messageBox.ClearMessage();
                        _gameState = GameState.Game;
                        // Reset Chess board
                        SetChessBoard();
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        _sprites.Clear();
                        _messageBox.ClearMessage();
                        _isEscapePressed = true;
                        _gameState = GameState.Start;
                    }
                    break;
                default:
                    break;
            }
            // updates every Sprite in _sprites
            foreach (Sprite sprite in _sprites)
            {
                sprite.Update(_sprites);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            Vector2 mainMessageLenth = _titleFont.MeasureString(_mainTitle);
            switch (_gameState)
            {
                case GameState.Start:
                    foreach(SpriteButton button in _buttons) 
                    {
                        button.Draw(_spriteBatch);
                    }
                    _spriteBatch.DrawString(_titleFont, _mainTitle, new Vector2(
                        _graphics.PreferredBackBufferWidth/2 - mainMessageLenth.X /2, 
                        _graphics.PreferredBackBufferHeight/2 - mainMessageLenth.Y /2 ), 
                        Color.Black);
                    break;
                
            }
            foreach (Sprite sprite in _sprites)
            {
                sprite.Draw(_spriteBatch);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void StartScreen()
        {
            if (MediaPlayer.State != MediaState.Playing && !_isSoundPlayed)
            {
                MediaPlayer.Play(_openingFanFare);
                _isSoundPlayed = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !_isEscapePressed)
                Exit();
           
            foreach (SpriteButton button in _buttons) 
            { 
                if (button._isButtonPressed)
                {
                    switch (button._buttonType)
                    {
                        case ButtonType.Start:
                            _isSoundPlayed = false;
                            _gameState = GameState.Game;
                            LoadContent();
                            SetChessBoard();
                            break;
                    }
                }
            }
        }
        public void RunGame()
        {
            _whiteMoveMessage = _whiteTurn ? "It's whites turn" : "It's blacks turn";
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                // Reset Chess board
                SetChessBoard();
            }
            // organise sprite list
            List<SpriteChessPiece> pieces = _sprites.OfType<SpriteChessPiece>().ToList();
            List<SpriteChessBoard> Boards = _sprites.OfType<SpriteChessBoard>().ToList();
            // sets the current chess piece as the one thats bounds contains the mouse position 
            if (_chessPiece != null)
            {
                // when chess piece is not being dragged update the potential moves
                if (!_chessPiece._dragged)
                {
                    _potentialMoves = PotentialMoves(_chessPiece);
                }
                MouseConrols();
                // here  -------------------------------------------------
                if (_chessPiece._selected)
                {
                    if (_chessPiece._dragged && !_cleared)
                    {
                        _potetialBoards.Clear();
                        _cleared = true;
                    }
                    else if (!_chessPiece._dragged && _cleared)
                    {
                        _cleared = false;
                    }
                    FindPotentialMoves();
                }
            }
            foreach (SpriteChessBoard board in Boards)
            {
                if (_potetialBoards.Contains(board))
                {
                    board._color = Color.Chartreuse;
                    if (board._enemy)
                    {
                        board._color = Color.Red;
                    }
                }
                else if (_originalChessBoard != null && board._point == _originalChessBoard._point)
                {
                    board._color = Color.Blue;
                }
                else
                {
                    board._color = board._tileColor;
                }
            }
            foreach (SpriteChessPiece piece in pieces)
            {
                if (piece != _chessPiece)
                {
                    piece._selected = false;
                }
            }
           
        }
        public void MouseConrols()
        {
            // when left button on mouse is pressed the chess piece bounds that contains the mouse cursor position is selected
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !_mouseLeftButtonPressed)
            {
                if (_chessPiece != null)
                    _originalChessBoard = _sprites.OfType<SpriteChessBoard>().Where(board => board._point == _chessPiece._piecePoint).First();
                _center = new Vector2(
                               _originalChessBoard._position.X + (_originalChessBoard._image.Width / 2 - _chessPiece._image.Width / 2),
                               _originalChessBoard._position.Y + (_originalChessBoard._image.Height / 2 - _chessPiece._image.Height / 2));
                // checks turn and displays message if not that colors turn otherwise it selects the piece
                if (_whiteTurn)
                {
                    if (_chessPiece._white)
                        _chessPiece._selected = true;
                    else _messageBox.AddMessage("please select a white piece");
                }
                else
                {
                    if (!_chessPiece._white)
                        _chessPiece._selected = true;
                    else _messageBox.AddMessage("please select a black piece");
                }
                _mouseLeftButtonPressed = true;
                // saves current board point and position in new variable 
                _currentPoint = _chessPiece._piecePoint;
            }
            // if the button is continues to be held down the piece can be dragged around 
            else if (Mouse.GetState().LeftButton == ButtonState.Pressed && _mouseLeftButtonPressed)
            {
                if ((_whiteTurn && _chessPiece._white) || (!_whiteTurn && !_chessPiece._white))
                {
                    if (!_isSoundPlayed)
                    {
                        _pickUp.Play();
                        _isSoundPlayed = true;
                    }
                    _chessPiece._position = new Vector2(
                        Mouse.GetState().Position.X - _chessPiece._image.Width / 2,
                        Mouse.GetState().Position.Y - _chessPiece._image.Height / 2);
                }
                else
                {
                    if (!_messageAdded)
                    {
                        _messageBox.AddMessage($"{_whiteMoveMessage}");
                        _messageAdded = true;
                    }
                }
                _chessPiece._dragged = true;
                _dragged = true;
            }
            // when mouse is released
            else if (Mouse.GetState().LeftButton != ButtonState.Pressed && _mouseLeftButtonPressed)
            {
                _messageAdded = false;
                _mouseLeftButtonPressed = false;
                _chessPiece._dragged = false;
                _dragged = false;
                _isSoundPlayed = false;
                // checks if Piece has moved from its last position
                if (_chessPiece._piecePoint != _currentPoint)
                {
                    CheckLegalMove();
                }
                else _chessPiece._position = _center;
            }
        }
        public void CheckLegalMove()
        {
            bool legalMove = false;
            foreach (SpriteChessBoard board in _potetialBoards)
            {
                // checks if new position is in potetential boards and therefor a legal move
                if (board._point == _chessPiece._piecePoint)
                {
                    bool kingInCheck = false;
                    legalMove = true;
                    _kingPiece = _sprites.OfType<SpriteChessPiece>().Where
                    (piece => piece._piece == ChessPiece.King && piece._pieceColor != _chessPiece._pieceColor).FirstOrDefault();
                    // check enemy king in check
                    kingInCheck = IsKingInCheck(_kingPiece, _sprites);
                    
                    if (kingInCheck)
                    {

                        IsCheckMate();


                        _messageBox.AddMessage($"{_kingPiece._pieceColor} is in check");
                    }
                    // check self
                    _kingPiece = _sprites.OfType<SpriteChessPiece>().Where
                        (piece => piece._piece == ChessPiece.King && piece._pieceColor == _chessPiece._pieceColor).FirstOrDefault();
                    //moves enemy off board
                    if (!IsKingInCheck(_kingPiece, _sprites) && board._enemy)
                    {
                        SpriteChessPiece enemy = _sprites.OfType<SpriteChessPiece>().Where
                            (enemy => enemy._bounds.Contains(_chessPiece._center) && enemy != _chessPiece && !enemy._isDead).FirstOrDefault();
                        enemy._isDead = true;
                        if (enemy._pieceColor == PieceColor.White)
                        {
                            _deadPieces[0].Add(enemy);
                            if (_deadPieces[0].Count < 9)
                            {
                                enemy._position = new Vector2(0, 50 + (50 * _deadPieces[0].Count));
                            }
                            else
                            {
                                enemy._position = new Vector2(50, 50 + (50 * (_deadPieces[0].Count -8)));

                            }
                        }
                        else
                        {
                            _deadPieces[1].Add(enemy);
                            if (_deadPieces[1].Count < 9)
                            {
                                enemy._position = new Vector2(
                                    _graphics.PreferredBackBufferWidth -50, 50 + (50 * _deadPieces[1].Count));
                            }
                            else
                            {
                                enemy._position = new Vector2(_graphics.PreferredBackBufferWidth - 100, 
                                    50 + (50 * (_deadPieces[1].Count - 8)));
                            }
                            
                        }
                        enemy._piecePoint = new Point(10, 10);
                    }
                    else if(IsKingInCheck(_kingPiece, _sprites) && board._enemy)
                    {
                        SpriteChessPiece enemy = _sprites.OfType<SpriteChessPiece>().Where
                            (enemy => enemy._bounds.Contains(_chessPiece._center) && enemy != _chessPiece && !enemy._isDead).FirstOrDefault();
                        enemy._isDead = true;
                        // Todo sort out position of dead sprites
                        if (enemy._pieceColor == PieceColor.White)
                        {
                            _deadPieces[0].Add(enemy);
                            enemy._position = new Vector2(0, 0 + (50 * _deadPieces[0].Count));
                        }
                        else
                        {
                            _deadPieces[1].Add(enemy);
                            enemy._position = new Vector2(
                                _graphics.PreferredBackBufferWidth - enemy._image.Width,
                                0 + (50 * _deadPieces[1].Count));
                        }
                        enemy._piecePoint = new Point(10, 10);
                        if (IsKingInCheck(_kingPiece, _sprites))
                        {
                            legalMove = false;
                            enemy._piecePoint = board._point;
                            enemy._isDead = false;
                            enemy._position = board._position;
                            break;
                        }
                        else
                        {

                        }

                    }

                    if (IsKingInCheck(_kingPiece, _sprites))
                    {
                        legalMove = false;
                        break;
                    }
                    _whiteTurn = !_whiteTurn;
                    _whiteMoveMessage = _whiteTurn ? "It's whites Turn" : "It's blacks turn";
                    _messageBox.AddMessage(_whiteMoveMessage);
                    _chessPiece._selected = false;
                    _potetialBoards.Clear();
                    _originalChessBoard = null;
                    // snap to center
                    _center = new Vector2(
                        board._position.X + (board._image.Width / 2 - _chessPiece._image.Width / 2),
                        board._position.Y + +(board._image.Height / 2 - _chessPiece._image.Height / 2));
                    _chessPiece._position = _center;
                    break;
                }
                else
                    legalMove = false;
            }
            if (!legalMove)
            {
                _wrongMove.Play();
                _messageBox.AddMessage("Illegal Move Please Try again!!");
                _chessPiece._position = _center;
                _chessPiece._selected = false;
                _potetialBoards.Clear();
            }
            else
            {
                _chessPiece._moved = true;
                _putDown.Play();
                //promote pawn
                if(_chessPiece._piece == ChessPiece.Pawn)
                {
                    if(_chessPiece._pieceColor == PieceColor.Black && _chessPiece._piecePoint.Y == 7)
                    {
                        _messageBox.AddMessage("The Black Pawn has been promoted to a Queen!!");

                        _chessPiece._piece = ChessPiece.Queen;
                        _chessPiece._image = _blackQueen;




                    }
                    else if (_chessPiece._pieceColor == PieceColor.White && _chessPiece._piecePoint.Y == 0)
                    {
                        _messageBox.AddMessage("The White Pawn has been promoted to a Queen!!");
                        _chessPiece._piece = ChessPiece.Queen;
                        _chessPiece._image = _whiteQueen;
                    }

                    SpriteChessBoard board = _sprites.OfType<SpriteChessBoard>().Where
                        (board => board._point == _chessPiece._piecePoint).FirstOrDefault(); 

                    _center = new Vector2(
                        board._position.X + (board._image.Width / 2 - _chessPiece._image.Width / 2),
                        board._position.Y + +(board._image.Height / 2 - _chessPiece._image.Height / 2));
                    _chessPiece._position = _center;
                }
            }
            Debug.WriteLine(legalMove.ToString());
        }
        public void FindPotentialMoves()
        {
            foreach (List<Point> pointList in _potentialMoves)
            {
                foreach (Point point in pointList)
                {
                    SpriteChessBoard Potentialboard = _sprites.OfType<SpriteChessBoard>().Where(board => board._point == point).FirstOrDefault();

                    if (_chessPiece._piece == ChessPiece.Pawn && pointList != _potentialMoves[0])
                    {
                        if (Potentialboard != null &&
                            _chessPiece._pieceColor != Potentialboard._boardPieceColor &&
                            Potentialboard._boardPieceColor != PieceColor.Empty)
                        {
                            Potentialboard._enemy = true;
                            if (!_potetialBoards.Contains(Potentialboard))
                                _potetialBoards.Add(Potentialboard);
                            break;
                        }
                    }
                    else
                    {
                        if (Potentialboard != null && Potentialboard._boardPieceColor == PieceColor.Empty)
                        {
                            Potentialboard._enemy = false;
                            if (!_potetialBoards.Contains(Potentialboard))
                                _potetialBoards.Add(Potentialboard);
                        }
                        else if (Potentialboard != null && _chessPiece._pieceColor != Potentialboard._boardPieceColor)
                        {
                            if (_chessPiece._piece != ChessPiece.Pawn)
                            {
                                Potentialboard._enemy = true;
                                if (!_potetialBoards.Contains(Potentialboard))
                                    _potetialBoards.Add(Potentialboard);
                            }
                            break;
                        }
                        else break;
                    }
                }
            }
        }

        public void IsCheckMate()
        {
            /*
                CheckMateFuntion()
                When in check
                Kings potential moves
                Check each one and see if new position puts king in check
                If there is a move that doesn’t put king in check then CheckMate is false.
                If there are no potential moves check if offending piece can be taken.
                If it can be taken and king will no longer be in check once done then false.
                if piece cannot be taken without putting king in check 
                see if it can be blocked without putting the king in check.
                if it cant be blocked or if blocking puts king in check

                then CheckMate!!!!!!!!!!!!!!!!!
             */

            List<List<Point>> pieceMoves = PotentialMoves(_kingPiece);
            bool checkMate = true;
            Point placeholder = _kingPiece._piecePoint;
            PieceColor placeholderColor = _kingPiece._pieceColor;
            SpriteChessBoard kingBoard = _sprites.OfType<SpriteChessBoard>().Where(board => board._point == placeholder).FirstOrDefault();
            kingBoard._boardPieceColor = PieceColor.Empty;
            foreach (List<Point> moves in pieceMoves)
            {
                foreach (Point point in moves)
                {
                    SpriteChessBoard potentialMove = _sprites.OfType<SpriteChessBoard>().Where(board => board._point == point).FirstOrDefault();
                    if (potentialMove != null && potentialMove._boardPieceColor != _kingPiece._pieceColor)
                    {
                        _kingPiece._piecePoint = point;
                        if (!IsKingInCheck(_kingPiece, _sprites))
                        {
                            _messageBox.AddMessage("King can still move");
                            checkMate = false;
                            break;
                        }
                    }
                }
            }
            _kingPiece._piecePoint = placeholder;
            kingBoard._boardPieceColor = placeholderColor;
            // checks if offending piece can be taken
            List<SpriteChessPiece> enemyPieces = _sprites.OfType<SpriteChessPiece>().Where(piece => piece._pieceColor ==_kingPiece._pieceColor).ToList();
            if (checkMate)
            {
                foreach (SpriteChessPiece piece in enemyPieces)
                {
                    pieceMoves = PotentialMoves(piece);
                    foreach (List<Point> moves in pieceMoves)
                    {
                        if(piece._piece == ChessPiece.Pawn && moves == pieceMoves[0])
                        {
                            continue;
                        }
                        foreach (Point point in moves)
                        {
                            SpriteChessPiece chessPiece = _sprites.OfType<SpriteChessPiece>().Where(piece => piece._piecePoint == point).FirstOrDefault();
                            if (chessPiece != null)
                            {
                                Point placeHolder = piece._piecePoint;
                                if(piece._piece == ChessPiece.King)
                                {
                                    piece._piecePoint = point;
                                    if (IsKingInCheck(piece, _sprites))
                                    {
                                        piece._piecePoint = placeHolder;
                                        break;
                                    }
                                    piece._piecePoint = placeHolder;

                                }
                                if (point != _chessPiece._piecePoint)
                                {
                                    break;
                                }
                                else
                                {
                                    _messageBox.AddMessage("The King cannot move but offender can be taken!");
                                    checkMate = false;
                                }
                            }
                        }
                    }
                }
            }
            // creates list of points between the king and the offender
            List<Point> blocking = new List<Point>();
            if (checkMate)
            {
                Point point = _kingPiece._piecePoint - _chessPiece._piecePoint;
                // checking diagonals
                if(_chessPiece._piece == ChessPiece.Bishop || 
                   _chessPiece._piece == ChessPiece.Queen &&
                   Math.Abs(point.X) == Math.Abs(point.Y))
                {
                    if (point.X < 0)
                    {
                        if (point.Y < 0)
                        {
                            //down right
                            for(int i =1; i < Math.Abs(point.X); i++)
                            {
                                blocking.Add(_kingPiece._piecePoint + new Point(i,i));
                            }
                        }
                        else
                        {
                            //up right
                            for (int i = 0; i < Math.Abs(point.X); i++)
                            {
                                blocking.Add(_kingPiece._piecePoint + new Point(i, -i));
                            }
                        }
                    }
                    else if (point.Y < 0)
                    {
                        //down left
                        for (int i = 0; i < point.X; i++)
                        {
                            blocking.Add(_kingPiece._piecePoint + new Point(-i, i));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < point.X; i++)
                        {
                            blocking.Add(_kingPiece._piecePoint + new Point(-i,-i));
                        }
                        // up left
                    }
                }
                else if (_chessPiece._piece == ChessPiece.Rook || _chessPiece._piece == ChessPiece.Queen)
                {
                    if(_kingPiece._piecePoint.X == _chessPiece._piecePoint.X)
                    {
                        // up and down
                        if (_kingPiece._piecePoint.Y > _chessPiece._piecePoint.Y)
                        {
                            for (int i = _kingPiece._piecePoint.Y; i > point.Y; i--)
                            {
                                // offender is up
                                blocking.Add( new Point(_kingPiece._piecePoint.X, i));
                            } 

                        }
                        else {
                            for (int i = _kingPiece._piecePoint.Y; i < -point.Y; i++)
                            {
                                //offender is down
                                blocking.Add( new Point(_kingPiece._piecePoint.X, i));
                            }
                        }
                    }
                    else if (_kingPiece._piecePoint.Y == _chessPiece._piecePoint.Y)
                    {
                        //left and right
                        if (_kingPiece._piecePoint.X < _chessPiece._piecePoint.X)
                        // offender is to right
                        {
                            for (int i = _kingPiece._piecePoint.X; i < point.X; i++) 
                            { 
                                blocking.Add(new Point(_kingPiece._piecePoint.X, i));
                            }
                        }
                        else 
                        //offender is to left
                        { 
                            for (int i = _kingPiece._piecePoint.X; i > point.X; i--) 
                            {
                                blocking.Add(new Point(_kingPiece._piecePoint.X, i));
                            }
                        }
                    }
                }
            }
            // checks list and chess pieces to see if it can move into one of the places the blocks the offender
            foreach (SpriteChessPiece piece in enemyPieces)
            {
                pieceMoves = PotentialMoves(piece);
                foreach (List<Point> moves in pieceMoves)
                {
                    if (piece._piece == ChessPiece.Pawn && moves != pieceMoves[0])
                    {
                        continue;
                    }
                    if (piece == _kingPiece)
                    {
                        continue;
                    }
                    foreach (Point point in moves)
                    {
                        SpriteChessBoard board = _sprites.OfType<SpriteChessBoard>().Where(board => board._point == point).FirstOrDefault();
                        if (board != null && board._boardPieceColor == PieceColor.Empty)
                        {
                            if (blocking.Contains(point))
                            {
                                _messageBox.AddMessage("The offender can be blocked!");
                                checkMate = false;
                            }
                        }
                    }
                }
            }
            if (checkMate)
            {
                _gameState = GameState.GameOver;
                MediaPlayer.Play(_checkMateSong);
            }
        }
        public void SetChessBoard()
        {
            _deadPieces.Clear();
            _deadPieces.Add(new List<SpriteChessPiece>());
            _deadPieces.Add(new List<SpriteChessPiece>());

            _sprites.Clear();
            _sprites.Add(new MessageBox(_messageBoxImage, new Vector2(0,_graphics.PreferredBackBufferHeight - _messageBoxImage.Height), _mainFont));
            
            _messageBox = _sprites.OfType<MessageBox>().FirstOrDefault();
            
            _whiteTurn = true;
            _messageBox.AddMessage("New Game, White moves first!");
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    Color color = Color.White;
                    if (row % 2 == 0)
                    {
                        if (col % 2 == 0)
                        {
                            color = Color.Yellow;
                        }
                        else
                        {
                            color = Color.Brown;
                        }
                    }
                    else
                    {
                        if (col % 2 != 0)
                        {
                            color = Color.Yellow;
                        }
                        else
                        {
                            color = Color.Brown;
                        }
                    }
                    _sprites.Add(new SpriteChessBoard(_square,
                        new Vector2(
                            (_square.Width * row) + _square.Width * 2,
                            (_square.Height * col) + _square.Width * 2),
                        new Point(row, col),color));
                }
            }
            for (int i = 0; i < 8; i++)
            {
                _sprites.Add(new SpriteChessPiece(_whitePawn, new Vector2(100 + (50 * i), 400), true, ChessPiece.Pawn));
                _sprites.Add(new SpriteChessPiece(_blackPawn, new Vector2(100 + (50 * i), 150), false, ChessPiece.Pawn));
            }

            _sprites.Add(new SpriteChessPiece(_whiteRook, new Vector2(100, 450), true, ChessPiece.Rook));
            _sprites.Add(new SpriteChessPiece(_whiteRook, new Vector2(450, 450), true, ChessPiece.Rook));
            _sprites.Add(new SpriteChessPiece(_whiteKnight, new Vector2(150, 450), true, ChessPiece.Knight));
            _sprites.Add(new SpriteChessPiece(_whiteKnight, new Vector2(400, 450), true, ChessPiece.Knight));
            _sprites.Add(new SpriteChessPiece(_whiteBishop, new Vector2(200, 450), true, ChessPiece.Bishop));
            _sprites.Add(new SpriteChessPiece(_whiteBishop, new Vector2(350, 450), true, ChessPiece.Bishop));
            _sprites.Add(new SpriteChessPiece(_whiteKing, new Vector2(300, 450), true, ChessPiece.King));
            _sprites.Add(new SpriteChessPiece(_whiteQueen, new Vector2(250, 450), true, ChessPiece.Queen));

            _sprites.Add(new SpriteChessPiece(_blackRook, new Vector2(100, 100), false, ChessPiece.Rook));
            _sprites.Add(new SpriteChessPiece(_blackRook, new Vector2(450, 100), false, ChessPiece.Rook));
            _sprites.Add(new SpriteChessPiece(_blackKnight, new Vector2(150, 100), false, ChessPiece.Knight));
            _sprites.Add(new SpriteChessPiece(_blackKnight, new Vector2(400, 100), false, ChessPiece.Knight));
            _sprites.Add(new SpriteChessPiece(_blackBishop, new Vector2(200, 100), false, ChessPiece.Bishop));
            _sprites.Add(new SpriteChessPiece(_blackBishop, new Vector2(350, 100), false, ChessPiece.Bishop));
            _sprites.Add(new SpriteChessPiece(_blackKing, new Vector2(300, 100), false, ChessPiece.King));
            _sprites.Add(new SpriteChessPiece(_blackQueen, new Vector2(250, 100), false, ChessPiece.Queen));

            List<SpriteChessBoard> boards = _sprites.OfType<SpriteChessBoard>().ToList();
            List<SpriteChessPiece> pieces = _sprites.OfType<SpriteChessPiece>().ToList();

            foreach (SpriteChessPiece piece in pieces)
            {
                foreach (SpriteChessBoard board in boards)
                {
                    if (board._bounds.Contains(piece._center))
                    {

                        piece._piecePoint = board._point;
                        piece._position = new Vector2(
                                    board._position.X + (board._image.Width / 2 - piece._image.Width / 2),
                                    board._position.Y + +(board._image.Height / 2 - piece._image.Height / 2));
                    }
                }
            }
        }
        static List<List<Point>> PotentialMoves(SpriteChessPiece chessPiece)
        {
            List<List<Point>> potentialMoves = new List<List<Point>>();

            switch (chessPiece._piece)
            {
                case ChessPiece.Pawn:
                    {
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        int moves = 1;

                        if (chessPiece._white)
                        {
                            moves *= -1;
                        }

                        potentialMoves[0].Add(new Point(chessPiece._piecePoint.X, chessPiece._piecePoint.Y + moves));
                        if (!chessPiece._moved)
                        {
                            potentialMoves[0].Add(new Point(chessPiece._piecePoint.X, chessPiece._piecePoint.Y + moves * 2));
                        }


                        potentialMoves[1].Add(new Point(chessPiece._piecePoint.X + 1, chessPiece._piecePoint.Y + moves));
                        potentialMoves[2].Add(new Point(chessPiece._piecePoint.X - 1, chessPiece._piecePoint.Y + moves));

                        return potentialMoves;
                    }
                case ChessPiece.Rook:
                    {
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());

                        for (int i = 1; i < 8; i++)
                        {
                            potentialMoves[0].Add(new Point(chessPiece._piecePoint.X, chessPiece._piecePoint.Y + i));
                            potentialMoves[1].Add(new Point(chessPiece._piecePoint.X, chessPiece._piecePoint.Y - i));
                            potentialMoves[2].Add(new Point(chessPiece._piecePoint.X + i, chessPiece._piecePoint.Y));
                            potentialMoves[3].Add(new Point(chessPiece._piecePoint.X - i, chessPiece._piecePoint.Y));

                        }
                        return potentialMoves;
                    }
                case ChessPiece.Knight:
                    {
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());


                        potentialMoves[0].Add(new Point(chessPiece._piecePoint.X + 2, chessPiece._piecePoint.Y + 1));
                        potentialMoves[1].Add(new Point(chessPiece._piecePoint.X + 2, chessPiece._piecePoint.Y - 1));
                        potentialMoves[2].Add(new Point(chessPiece._piecePoint.X - 2, chessPiece._piecePoint.Y + 1));
                        potentialMoves[3].Add(new Point(chessPiece._piecePoint.X - 2, chessPiece._piecePoint.Y - 1));
                        potentialMoves[4].Add(new Point(chessPiece._piecePoint.X + 1, chessPiece._piecePoint.Y + 2));
                        potentialMoves[5].Add(new Point(chessPiece._piecePoint.X - 1, chessPiece._piecePoint.Y + 2));
                        potentialMoves[6].Add(new Point(chessPiece._piecePoint.X + 1, chessPiece._piecePoint.Y - 2));
                        potentialMoves[7].Add(new Point(chessPiece._piecePoint.X - 1, chessPiece._piecePoint.Y - 2));


                        return potentialMoves;
                    }
                case ChessPiece.Bishop:
                    {
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());

                        for (int i = 1; i < 8; i++)
                        {
                            potentialMoves[0].Add(new Point(chessPiece._piecePoint.X + i, chessPiece._piecePoint.Y + i));
                            potentialMoves[1].Add(new Point(chessPiece._piecePoint.X + i, chessPiece._piecePoint.Y - i));
                            potentialMoves[2].Add(new Point(chessPiece._piecePoint.X - i, chessPiece._piecePoint.Y + i));
                            potentialMoves[3].Add(new Point(chessPiece._piecePoint.X - i, chessPiece._piecePoint.Y - i));

                        }
                        return potentialMoves;
                    }
                case ChessPiece.King:
                    {
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());

                        potentialMoves[0].Add(new Point(chessPiece._piecePoint.X + 1, chessPiece._piecePoint.Y + 1));
                        potentialMoves[1].Add(new Point(chessPiece._piecePoint.X + 1, chessPiece._piecePoint.Y));
                        potentialMoves[2].Add(new Point(chessPiece._piecePoint.X + 1, chessPiece._piecePoint.Y - 1));
                        potentialMoves[3].Add(new Point(chessPiece._piecePoint.X - 1, chessPiece._piecePoint.Y + 1));
                        potentialMoves[4].Add(new Point(chessPiece._piecePoint.X - 1, chessPiece._piecePoint.Y));
                        potentialMoves[5].Add(new Point(chessPiece._piecePoint.X - 1, chessPiece._piecePoint.Y - 1));
                        potentialMoves[6].Add(new Point(chessPiece._piecePoint.X, chessPiece._piecePoint.Y + 1));
                        potentialMoves[7].Add(new Point(chessPiece._piecePoint.X, chessPiece._piecePoint.Y - 1));

                        return potentialMoves;
                    }
                case ChessPiece.Queen:
                    {
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());
                        potentialMoves.Add(new List<Point>());

                        for (int i = 1; i < 8; i++)
                        {
                            potentialMoves[0].Add(new Point(chessPiece._piecePoint.X, chessPiece._piecePoint.Y + i));
                            potentialMoves[1].Add(new Point(chessPiece._piecePoint.X, chessPiece._piecePoint.Y - i));
                            potentialMoves[2].Add(new Point(chessPiece._piecePoint.X + i, chessPiece._piecePoint.Y));
                            potentialMoves[3].Add(new Point(chessPiece._piecePoint.X - i, chessPiece._piecePoint.Y));
                            potentialMoves[4].Add(new Point(chessPiece._piecePoint.X + i, chessPiece._piecePoint.Y + i));
                            potentialMoves[5].Add(new Point(chessPiece._piecePoint.X + i, chessPiece._piecePoint.Y - i));
                            potentialMoves[6].Add(new Point(chessPiece._piecePoint.X - i, chessPiece._piecePoint.Y + i));
                            potentialMoves[7].Add(new Point(chessPiece._piecePoint.X - i, chessPiece._piecePoint.Y - i));
                        }
                        return potentialMoves;
                    }
                default:
                    {
                        return potentialMoves;
                    }
            }
        }

        static bool IsKingInCheck(SpriteChessPiece king, List<Sprite> sprites)
        {
            bool isKingInCheck = false;

            List<List<Point>> kingCheckList = new List<List<Point>>();

            kingCheckList.Add(new List<Point>());//down         [0]
            kingCheckList.Add(new List<Point>());//up           [1]
            kingCheckList.Add(new List<Point>());//right        [2]
            kingCheckList.Add(new List<Point>());//left         [3]
            kingCheckList.Add(new List<Point>());//down right   [4]
            kingCheckList.Add(new List<Point>());//down left    [5]
            kingCheckList.Add(new List<Point>());//up right     [6]
            kingCheckList.Add(new List<Point>());//up left      [7]

            kingCheckList.Add(new List<Point>());//knight moves [8]


            for (int i = 1; i < 8; i++)
            {
                kingCheckList[0].Add(new Point(king._piecePoint.X, king._piecePoint.Y + i));
                kingCheckList[1].Add(new Point(king._piecePoint.X, king._piecePoint.Y - i));
                kingCheckList[2].Add(new Point(king._piecePoint.X + i, king._piecePoint.Y));
                kingCheckList[3].Add(new Point(king._piecePoint.X - i, king._piecePoint.Y));
                kingCheckList[4].Add(new Point(king._piecePoint.X + i, king._piecePoint.Y + i));
                kingCheckList[5].Add(new Point(king._piecePoint.X + i, king._piecePoint.Y - i));
                kingCheckList[6].Add(new Point(king._piecePoint.X - i, king._piecePoint.Y + i));
                kingCheckList[7].Add(new Point(king._piecePoint.X - i, king._piecePoint.Y - i));
            }

            kingCheckList[8].Add(new Point(king._piecePoint.X + 2, king._piecePoint.Y + 1));
            kingCheckList[8].Add(new Point(king._piecePoint.X + 2, king._piecePoint.Y - 1));
            kingCheckList[8].Add(new Point(king._piecePoint.X - 2, king._piecePoint.Y + 1));
            kingCheckList[8].Add(new Point(king._piecePoint.X - 2, king._piecePoint.Y - 1));
            kingCheckList[8].Add(new Point(king._piecePoint.X + 1, king._piecePoint.Y + 2));
            kingCheckList[8].Add(new Point(king._piecePoint.X - 1, king._piecePoint.Y + 2));
            kingCheckList[8].Add(new Point(king._piecePoint.X + 1, king._piecePoint.Y - 2));
            kingCheckList[8].Add(new Point(king._piecePoint.X - 1, king._piecePoint.Y - 2));


            foreach (List<Point> list in kingCheckList)
            {
                foreach (Point point in list)
                {
                    SpriteChessPiece enemy = sprites.OfType<SpriteChessPiece>().Where(
                        enemy => enemy._piecePoint == point && enemy._pieceColor != king._pieceColor).FirstOrDefault();

                    SpriteChessBoard board = sprites.OfType<SpriteChessBoard>().Where(
                        board => board._point == point).FirstOrDefault();

                    if (board != null && board._boardPieceColor == king._pieceColor && list != kingCheckList[8])
                    {
                        if (enemy == null)
                        {
                            break;
                        }
                    }

                    if (enemy != null)
                    {
                        // check pawns

                        if (king._pieceColor == PieceColor.Black && (list == kingCheckList[4] || list == kingCheckList[6]))
                        {
                            if (point == list[0])
                            {
                                if (enemy._piece == ChessPiece.Pawn)
                                {
                                    isKingInCheck = true;
                                    break;
                                }
                            }
                        }
                        else if (king._pieceColor == PieceColor.White && (list == kingCheckList[5] || list == kingCheckList[7]))
                        {
                            if (point == list[0])
                            {
                                if (enemy._piece == ChessPiece.Pawn)
                                {
                                    isKingInCheck = true;

                                    break;
                                }
                            }
                        }
                        // check straits
                        if (
                            list == kingCheckList[0] ||
                            list == kingCheckList[1] ||
                            list == kingCheckList[2] ||
                            list == kingCheckList[3])
                        {
                            if ((enemy._piece == ChessPiece.Queen || enemy._piece == ChessPiece.Rook))
                            {
                                isKingInCheck = true;
                                break;
                            }
                            else break;
                        }
                        // check diagonals
                        else if (
                            list == kingCheckList[4] ||
                            list == kingCheckList[5] ||
                            list == kingCheckList[6] ||
                            list == kingCheckList[7])
                        {
                            if ((enemy._piece == ChessPiece.Queen || enemy._piece == ChessPiece.Bishop))
                            {
                                isKingInCheck = true;

                                break;
                            }
                            else break;


                        }
                        //check knights
                        else
                        {
                            if (enemy._piece == ChessPiece.Knight)
                            {
                                isKingInCheck = true;

                                break;
                            }
                        }
                    }
                }
                if (isKingInCheck) break;
            }
            return isKingInCheck;
        }
    }
    enum GameState 
    {
        Start,
        Game,
        GameOver,
        Options,
        Credits
    }
}