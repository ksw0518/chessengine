using System.Collections.Generic;

using System;
using System.Linq;
using System.Threading;
using System.Collections;

public class ChessCore
{
    
    //public Canvas canvas;
    //public GameObject Promotion;
    public List<int> Locations = new();
    public List<int> MoveIndexLists = new();
    //public GameObject[] Pieces;
    //public List<GameObject> PieceList;
    //BitBoard 정의
    //ulong[] pieceBitboards = new ulong[14];
    //List<int> pieceHashList =new();
    const int BoardSize = 64;
    public int[] IndexToX = new int[64];
    public int[] IndexToY = new int[64];
    public int selectedSquare;
    //public Transform dragging = null;
    //private Vector3 offset;
    int RealStartX;
    int RealStartY;
    public float startX;
    public float startY;
    public float endX;
    public float endY;
    public List<float> xs;
    public List<float> ys;
    //public BoardSetComponent board;

    List<Move> PossibleMovements = new();
    List<List<int>> KnightPreCalc = new();
    //ulong not_a_file = 18374403900871474942UL;
    //ulong not_h_file =  9187201950435737471UL;
    //ulong not_hg_file = 4557430888798830399UL;
    //ulong not_ab_file = 18229723555195321596UL;
    public int[] chessBoard = new int[64];
    public List<int> chessBoardHash = new();
    public const string StartFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";//rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1;
    public bool IsPromoting;
    public int PawnPromoLocation;
    const int knightPromo = MoveType.promotion;
    const int BishopPromo = MoveType.promotion | MoveType.Special00;
    const int RookPromo = MoveType.promotion | MoveType.Special01;
    const int QueenPromo = MoveType.promotion | MoveType.Special00 | MoveType.Special01;
    Move lastMove;

    //public int 
    int[] SlidingPiece = new int[3] { Piece.Bishop, Piece.Rook, Piece.Queen };
    Dictionary<UInt64, int> ThreeFoldRep = new Dictionary<UInt64, int>();
    TranspositionValue DefaultTransposition = null;
    int[] PawnSquare = new int[64] {  0,  0,  0,  0,  0,  0,  0,  0,
50, 50, 50, 50, 50, 50, 50, 50,
10, 10, 20, 30, 30, 20, 10, 10,
 5,  5, 10, 25, 25, 10,  5,  5,
 0,  0,  0, 20, 20,  0,  0,  0,
 5, -5,-10,  0,  0,-10, -5,  5,
 5, 10, 10,-20,-20, 10, 10,  5,
 0,  0,  0,  0,  0,  0,  0,  0};
    int[] PawnSquareEnd = new int[64] { 0, 0, 0, 0, 0, 0, 0, 0 ,
    50, 50, 50, 50, 50, 50, 50, 50 ,
    20, 30, 40, 40, 40, 40, 30, 20 ,
    5,  5, 10, 25, 25, 10,  5,  5 ,
    0,  0,  0, 20, 20,  0,  0,  0 ,
    5, -5,-10,  0,  0,-10, -5,  5,
    5, 10, 10,-20,-20, 10, 10,  5 ,
    0,  0,  0,  0,  0,  0,  0,  0 };
    int[] BishopSquare = new int[64] {-20,-10,-10,-10,-10,-10,-10,-20,
-10,  0,  0,  0,  0,  0,  0,-10,
-10,  0,  5, 10, 10,  5,  0,-10,
-10,  5,  5, 10, 10,  5,  5,-10,
-10,  0, 10, 10, 10, 10,  0,-10,
-10, 10, 10, 10, 10, 10, 10,-10,
-10,  10,  0,  0,  0,  0,  10,-10,
-20,-10,-10,-10,-10,-10,-10,-20,};
    int[] KnightSquare = new int[64] {-50,-40,-30,-30,-30,-30,-40,-50,
-40,-20,  0,  0,  0,  0,-20,-40,
-30,  0, 10, 15, 15, 10,  0,-30,
-30,  5, 15, 20, 20, 15,  5,-30,
-30,  0, 15, 20, 20, 15,  0,-30,
-30,  5, 10, 15, 15, 10,  5,-30,
-40,-20,  0,  5,  5,  0,-20,-40,
-50,-40,-30,-30,-30,-30,-40,-50};
    int[] RookSquare = new int[64] {  0,  0,  0,  0,  0,  0,  0,  0,
  5, 10, 10, 10, 10, 10, 10,  5,
 -5,  0,  0,  0,  0,  0,  0, -5,
 -5,  0,  0,  0,  0,  0,  0, -5,
 -5,  0,  0,  0,  0,  0,  0, -5,
 -5,  0,  0,  0,  0,  0,  0, -5,
 -5,  0,  0,  0,  0,  0,  0, -5,
  0,  0,  0,  5,  5,  0,  0,  0};
    int[] QueenSquare = new int[64] {-20,-10,-10, -5, -5,-10,-10,-20,
-10,  0,  0,  0,  0,  0,  0,-10,
-10,  0,  5,  5,  5,  5,  0,-10,
 -5,  0,  5,  5,  5,  5,  0, -5,
  0,  0,  5,  5,  5,  5,  0, -5,
-10,  5,  0,  5,  5,  0,  0,-10,
-10,  0,  5,  0,  0,  0,  0,-10,
-20,-10,-10, -5, -5,-10,-10,-20};
    int[] KingSquare = new int[64] {-30,-40,-40,-50,-50,-40,-40,-30,
-30,-40,-40,-50,-50,-40,-40,-30,
-30,-40,-40,-50,-50,-40,-40,-30,
-30,-40,-40,-50,-50,-40,-40,-30,
-20,-30,-30,-40,-40,-30,-30,-20,
-10,-20,-20,-20,-20,-20,-20,-10,
 20, 20,  0,  0,  0,  0, 20, 20,
 20, 30, 10,  0,  0, 10, 30, 20};
    int[] KingSquareEnd = new int[64] {-50,-40,-30,-20,-20,-30,-40,-50,
-30,-20,-10,  0,  0,-10,-20,-30,
-30,-10, 20, 30, 30, 20,-10,-30,
-30,-10, 30, 40, 40, 30,-10,-30,
-30,-10, 30, 40, 40, 30,-10,-30,
-30,-10, 20, 30, 30, 20,-10,-30,
-30,-30,  0,  0,  0,  0,-30,-30,
-50,-30,-30,-30,-30,-30,-30,-50 };


    int[] Distance_FromCenter = new int[64] {5, 4, 3, 3, 3, 3, 4, 5,
4, 3, 2, 2, 2, 2, 3, 4,
3, 2, 1, 1, 1, 1, 2, 3,
3, 2, 1, 0, 0, 1, 2, 3,
3, 2, 1, 0, 0, 1, 2, 3,
3, 2, 1, 1, 1, 1, 2, 3,
4, 3, 2, 2, 2, 2, 3, 4,
5, 4, 3, 3, 3, 3, 4, 5 };
    public bool WhiteKingCastle = true;
    public bool WhiteQueenCastle = true;
    public bool BlackKingCastle = true;
    public bool BlackQueenCastle = true;
    Dictionary<int, int> pieceValues = new Dictionary<int, int>
        {
            { Piece.Pawn, 100 },
            { Piece.Knight, 320 },
            { Piece.Bishop, 330 },
            { Piece.Rook, 500 },
            { Piece.Queen, 900 },
            { Piece.King, 0}
        };

    public int MAX_DEPTH;
    public int Q_DEPTH;
    //public int BASE_DEPTH;
    public int TIME_LIMIT;
    //public int OPENING_MAXDEPTH = 3;
    int perftDepth;
    public int moveCount;
    public int QmoveCount;
    public int SavedByTransposition;
    public int cutoff;
    public System.Diagnostics.Stopwatch AITime = new();
    public int[,] MVVLVA_T = new int[6, 6];
    public Dictionary<int, int> MVVLVA_PieceInd = new Dictionary<int, int> { { Piece.Pawn, 0 }, { Piece.Knight, 1 }, { Piece.Bishop, 2 }, { Piece.Rook, 3 }, { Piece.Queen, 4 }, { Piece.King, 5 } };
    System.Diagnostics.Stopwatch TimeControl = new();
    UInt64 RandomSeed = 1804289383;

    UInt64[,] piece_Keys = new UInt64[12, 64];
    UInt64[] en_Keys = new UInt64[64];
    UInt64[] castle_Keys = new UInt64[16];
    UInt64 side_Keys;

    public UInt64 HashKey;

    const int hashSize = 10000000;//10000000
    TranspositionValue[] TranspositionTable = new TranspositionValue[hashSize];
    //bool[] TurnArrayForDebug = new bool[hashSize];


    //[SerializeField]
    PVLine EngineLine;
    int CurrDepth;
    bool isQuiescence = true;
    bool TranspositionEnable = false;
    bool isMoveOrdering = true;
    //public bool WhiteTurn;
    // Start is called before the first frame update

    Move OppMove;
    //public Slider EvalBar;
    public static void Main(string[] args)
    {
        
        ChessCore Engine = new ChessCore();
        initializeLists();
        
        Engine.WhiteKingCastle = true;
        Engine.WhiteQueenCastle = true;
        Engine.BlackKingCastle = true;
        Engine.BlackQueenCastle = true;
        //there must be error with checking transposition table. check it today.
        Engine.init_rand_keys();
        Engine.MAX_DEPTH = 99;
        Engine.Q_DEPTH = 10;
        //BASE_DEPTH = 5;
        Engine.TIME_LIMIT = 10000;
        //OPENING_MAXDEPTH = 4;

        //IsPromoting = false;
        //PawnPromoLocation = 0;
        Engine.chessBoardHash.Clear();

        Engine.PreCalculateKnightMovement();



        //printBitboard(pieceBitboards[(int)PieceType.WhiteKnight]);
        //printBitboard(4UL);
        Engine.chessBoard = Engine.FEN_ToList(StartFEN);
        //RepresentBoard();
        Engine.HashKey = Engine.Generate_HashKey();
        //AI();
        //PossibleMovements = GenerateNotSliding(pieceBitboards[(int)PieceType.WhiteKnight], PieceType.WhiteKnight);
        (Engine.PossibleMovements, _, _) = Engine.GenerateLegalMoves(true, true, true, true, Engine.chessBoard, Engine.chessBoardHash, Piece.White, false, false, null);


        Engine.lastMove = new Move(0, 0, 0);
        //(Move move, int score) = Engine.AI(Piece.White);
        //Console.WriteLine(move + "," + score);

        void initializeLists()
        {
            int k = 0;
            for(int i = 0; i < 64; i++)
            {
                Engine.IndexToX[i] = k;
                
                k++;
                if(k == 8)
                {
                    k = 0;
                }
            }
            k = 0;
            for (int i = 0; i < 8; i++)
            {
                
                for(int t = 0; t < 8; t++)
                {
                    Engine.IndexToY[i * 8 + t] = i;
                    //Console.WriteLine(i);
                }
            }
        }
        if(Console.ReadLine() == "uci")
        {
            Engine.StartUCIProtocol();
        }
        //StartCoroutine(SimulateAiGame(Piece.White));
        //isQuiescence = true;
        //AI(Piece.Black);

        //UInt64 hash_key = 0;
        //hash_key ^= piece_Keys[Get_HashIndex(Piece.Pawn, Piece.Black), 0];
        //Debug.Log(hash_key.ToString("X11"));
        //hash_key ^= piece_Keys[Get_HashIndex(Piece.Pawn, Piece.Black), 0];
        //Debug.Log(hash_key.ToString("X11"));


        //List<int> Locations = new();
        ////Debug.Log(PossibleMovements.Count);
        //for (int j = 0; j < PossibleMovements.Count; j++)
        //{
        //    Locations.Add(PossibleMovements[j].To);
        //    Debug.Log(PossibleMovements[j].From + "to" +PossibleMovements[j].To);
        //}
        //board.ChangeColor(Locations.ToArray());


        Engine.perftDepth = 6;
        for (int i = 1; i < Engine.perftDepth + 1; i++)
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            sw.Start();

            

            int Nodes = Engine.PerfTest(i, Engine.chessBoard, Engine.chessBoardHash, true, true, true, true, new Move(0, 0, 0), Piece.White, Engine.HashKey, Engine.lastMove);
            //Debug.Log();
            sw.Stop();
            Console.WriteLine("Depth:" + i + "Nodes:" + Nodes + "Time:" + sw.ElapsedMilliseconds + " MS (" + sw.ElapsedMilliseconds / 1000f + " S)" + Nodes / (sw.ElapsedMilliseconds / 1000f) + "NPS");
            //eld return 0;

            //Thread.Sleep(100);



        }
        //board.VisualizePieceSquare(PawnSquare);
        //(int score, bool isreal) = Engine.quiescenceMax(99, Engine.chessBoard, Engine.chessBoardHash, Engine.WhiteKingCastle, Engine.WhiteQueenCastle, Engine.BlackKingCastle, Engine.BlackQueenCastle, Engine.lastMove, int.MinValue, int.MaxValue);
        //Debug.Log(QmoveCount + "," + score);

        //Debug.Log(StaticExchangeEvaluation(chessBoard, chessBoardHash, new Move(21, 45, MoveType.capture)));
        //Debug.Log(EvaluateBoard(chessBoard, chessBoardHash, WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackKingCastle));
    }

    // Update is called once per frame
    //IEnumerator SimulateAiGame(int turn)
    //{

    //    board.ResetBoard();
    //    RepresentBoard();
    //    //Q_DEPTH = 10;
    //    isQuiescence = false;
    //    if (turn == Piece.Black) isQuiescence = true;

    //    AI(turn);
    //    yield return new WaitForSeconds(2f);
    //    (List<Move> Move, _, _) = GenerateLegalMoves(WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, chessBoard, chessBoardHash, OppColor(turn), false, false);
    //    if (Move.Count != 0)
    //    {

    //        StartCoroutine(SimulateAiGame(OppColor(turn)));


    //    }
    //    //yield return null;
    //}

    void StartUCIProtocol()
    {
        Console.WriteLine(StringToNumber("e4"));
        Console.WriteLine("id name " + "Avalanche");
        Console.WriteLine("id author " + "ksw0518");
        Console.WriteLine("uciok");
        //Console.WriteLine("")
        if (Console.ReadLine() == "ucinewgame")
        {
            string input = Console.ReadLine();
            string moveinput = input.Substring(input.Length - 5);
            string move = "";
            for(int i = 0; i < moveinput.Length; i++)
            {
                if(moveinput[i] == ' ')
                {
                    continue;
                }
                move += moveinput[i].ToString();

            }
            //Console.WriteLine(move);
            Move m = convertStringToRealMove(move);
            MakeMove(m, chessBoard, chessBoardHash, ref HashKey);

            string readline = Console.ReadLine();
            if (readline.Contains("go"))
            {
                //input = Console.ReadLine();
                //moveinput = input.Substring(input.Length - 5);
                //=move = "";
                //for (int i = 0; i < moveinput.Length; i++)
                //{
                //    if (moveinput[i] == ' ')
                //    {
                //        continue;
                //    }
                //    move += moveinput[i].ToString();

                //}
                //Console.WriteLine(move);
                //Move m = convertStringToRealMove(move);
                //MakeMove(m, chessBoard, chessBoardHash, ref HashKey);


            }
        }

        Move convertStringToRealMove(string move)
        {
            string whatPiecePromote = "";
            List<Move> moves;
            moves = GenerateLegalMoves(WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, chessBoard, chessBoardHash, Piece.White, false, false, lastMove).Item1;
            int from = StringToNumber(move.Substring(0, 2));
            int to = StringToNumber(move.Substring(2, 2));
            if (move.Count() == 5)//promotion
            {
                whatPiecePromote = move[4].ToString();

            }
            Move whatmove = new Move(0, 0, 0);
            //Console.WriteLine(moves.Count());
            for(int i = 0; i < moves.Count(); i++)
            {
                Move m = moves[i];
                //Console.WriteLine(m.From + "," + from);
                if (m.From == from && m.To == to)
                {
                    
                    if(whatPiecePromote != "")//promoting move
                    {
                        if(whatPiecePromote == "q")
                        {
                            if(m.Type == (MoveType.promotion | MoveType.Special01 | MoveType.Special00) || m.Type == (MoveType.promotion|MoveType.capture| MoveType.Special01 | MoveType.Special00))
                            {
                                whatmove = m;
                                break;
                            }
                        }
                        if(whatPiecePromote == "n")
                        {
                            if (m.Type == (MoveType.promotion) || m.Type == (MoveType.capture| MoveType.promotion))
                            {
                                whatmove = m;
                                break;
                            }
                        }
                        if (whatPiecePromote == "r")
                        {
                            if (m.Type == (MoveType.promotion | MoveType.Special01) || m.Type == (MoveType.promotion | MoveType.capture | MoveType.Special01))
                            {
                                whatmove = m;
                                break;
                            }
                        }
                        if (whatPiecePromote == "b")
                        {
                            if (m.Type == (MoveType.promotion | MoveType.Special00) || m.Type == (MoveType.promotion | MoveType.capture | MoveType.Special00))
                            {
                                whatmove = m;
                                break;
                            }
                        }
                    }
                    else
                    {
                        whatmove = m;
                        break;
                    }
                }
                    
            }
            return whatmove;
            //Console.WriteLine(whatmove.From);
            
        }
    }
    void AIInvoke()
    {
        AI(Piece.Black);

    }

    void DoHashChangeThings()
    {
        if (lastMove.Type == MoveType.Special00)//hash enpassent
        {
            HashKey ^= en_Keys[lastMove.From + 8];
        }

    }
    //bool DetectThreeFoldRepetition(List<UInt64> MoveRec, UInt64 CurrPos)
    //{

    //}
    void init_rand_keys()
    {
        RandomSeed = 1804289383;
        for (int piece = 0; piece < 12; piece++)
        {
            for (int square = 0; square < 64; square++)
            {
                piece_Keys[piece, square] = XorRandom();
                //Debug.Log(piece_Keys[piece, square].ToString("X11"));
            }
        }
        for (int square = 0; square < 64; square++)
        {
            en_Keys[square] = XorRandom();
        }
        side_Keys = XorRandom();
        for (int i = 0; i < 16; i++)
        {
            castle_Keys[i] = XorRandom();
        }
    }
    UInt64 XorRandom()
    {
        UInt64 number = RandomSeed;

        number ^= number << 13;
        number ^= number >> 7;
        number ^= number << 17;
        RandomSeed = number;

        return number;
    }
    int Get_HashIndex(int Piece)
    {
        //Debug.Log(Piece);
        if (ValueToPiece(Piece) == 0)
        {
            //Debug.Log(Piece);
            //Debug.Log("asdf");
        }
        return MVVLVA_PieceInd[ValueToPiece(Piece)] + ((ValueToCol(Piece) - 8) / 8) * 6;
    }
    int Get_MVVLVA_Value(Move move, int[] cb)
    {
        int from = move.From;
        int to = move.To;
        if (move.Type == (MoveType.capture | MoveType.Special00)) //ep capture
        {
            return MVVLVA_T[MVVLVA_PieceInd[Piece.Pawn], MVVLVA_PieceInd[Piece.Pawn]];
        }
        //Debug.Log(ValueToPiece(cb[from]) + "," + ValueToPiece(cb[to]));
        return MVVLVA_T[MVVLVA_PieceInd[ValueToPiece(cb[from])], MVVLVA_PieceInd[ValueToPiece(cb[to])]];
    }
    string NumberToString(int Square)
    {
        string[] File = new string[8] { "a", "b", "c", "d", "e", "f", "g", "h" };
        return File[IndexToX[Square]] + (8 - IndexToY[Square]).ToString();
    }
    int StringToNumber(string square)
    {
        string[] File = new string[8] { "a", "b", "c", "d", "e", "f", "g", "h" };

        char fileChar = square[0];
        int rank = int.Parse(square[1].ToString());

        int fileIndex = Array.IndexOf(File, fileChar.ToString());
        int rankIndex = 8 - rank;

        return rankIndex * 8 + fileIndex;
    }

    bool IsThreeFold(Dictionary<UInt64, int> MoveList, UInt64 CurrentPos, int HowMuch)
    {
        if (MoveList.ContainsKey(CurrentPos))
        {
            if (MoveList[CurrentPos] >= HowMuch)
            {
                return true;
            }
        }

        return false;
    }
    int StaticExchangeEvaluation(int[] CB, List<int> HashList, Move move)
    {

        int From = move.From;
        int to = move.To;
        int type = move.Type;
        //Debug.Log(to + "important");

        //int[] CBCalc = (int[])CB.Clone();
        //List<int> HashCalc = new(HashList);
        UInt64 a = 0;
        List<int> w_Attackers = new();
        List<int> w_AttackersLocation = new();
        List<int> b_Attackers = new();
        List<int> b_AttackersLocation = new();
        if (!IsCapture(move))
        {
            Console.WriteLine("Static Exchange Evaluation cannot be executed: the move provided is not a capture move.");
        }
        bool isEnpassent = (type == (MoveType.capture | MoveType.Special00));
        int Col = ValueToCol(CB[From]);

        int posX = IndexToX[to];
        int posY = IndexToY[to];
        int HowMuchLeft = posX;
        int HowMuchRight = 7 - posX;
        int HowMuchUp = posY;
        int HowMuchDown = 7 - posY;

        int HowMuchLeftTop = Math.Min(HowMuchLeft, HowMuchUp);
        int HowMuchRightTop = Math.Min(HowMuchRight, HowMuchUp);
        int HowMuchLeftDown = Math.Min(HowMuchLeft, HowMuchDown);
        int HowMuchRightDown = Math.Min(HowMuchRight, HowMuchDown);
        int XLoc = 0;
        int YLoc = 0;
        //MakeMove(move, CBCalc, HashCalc, ref a);

        //find attackers
        if (IndexToY[to] > 0)
        {
            if (IndexToX[to] > 0)
            {
                //Debug.Log(to - 9);
                if (CB[to - 9] == (Piece.White | Piece.Pawn))
                {
                    w_Attackers.Add(Piece.Pawn);
                    w_AttackersLocation.Add(to - 9);
                }



            }
            if (IndexToX[to] < 7)
            {
                if (CB[to - 7] == (Piece.White | Piece.Pawn))
                {
                    w_Attackers.Add(Piece.Pawn);
                    w_AttackersLocation.Add(to - 7);
                }

            }
        }

        XLoc = posX;
        YLoc = posY - 1;
        for (int i = 1; i < HowMuchUp - 1; i++)//up
        {
            int Pos = XLoc + 8 * YLoc;
            //Debug.Log(XLoc + "," + YLoc);
            //Debug.Log(Pos);
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.White && ValueToCol(CB[Pos]) != 0) break; //if there is other colored piece in between, break.
            if (piece == Piece.Rook || piece == Piece.Queen)
            {
                w_Attackers.Add(piece);
                w_AttackersLocation.Add(Pos);
            }
            if (piece == Piece.King) break;
            YLoc--;
        }
        XLoc = posX;
        YLoc = posY + 1;
        for (int i = 1; i < HowMuchDown - 1; i++)//down
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.White && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Rook || piece == Piece.Queen)
            {
                w_Attackers.Add(piece);
                w_AttackersLocation.Add(Pos);
            }
            YLoc++;
        }
        XLoc = posX + 1;
        YLoc = posY;
        for (int i = 1; i < HowMuchRight - 1; i++)//right
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.White && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Rook || piece == Piece.Queen)
            {
                w_Attackers.Add(piece);
                w_AttackersLocation.Add(Pos);
            }
            XLoc++;
        }
        XLoc = posX - 1;
        YLoc = posY;
        for (int i = 1; i < HowMuchLeft - 1; i++)//left
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.White && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Rook || piece == Piece.Queen)
            {
                w_Attackers.Add(piece);
                w_AttackersLocation.Add(Pos);
            }
            XLoc--;
        }
        XLoc = posX + 1;
        YLoc = posY - 1;
        for (int i = 1; i < HowMuchRightTop - 1; i++)
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.White && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Bishop || piece == Piece.Queen)
            {
                w_Attackers.Add(piece);
                w_AttackersLocation.Add(Pos);
            }
            XLoc++;
            YLoc--;

        }
        XLoc = posX - 1;
        YLoc = posY - 1;
        for (int i = 1; i < HowMuchLeftTop - 1; i++)
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.White && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Bishop || piece == Piece.Queen)
            {
                w_Attackers.Add(piece);
                w_AttackersLocation.Add(Pos);
            }
            XLoc--;
            YLoc--;

        }
        XLoc = posX + 1;
        YLoc = posY + 1;
        for (int i = 1; i < HowMuchRightDown - 1; i++)
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.White && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Bishop || piece == Piece.Queen)
            {
                w_Attackers.Add(piece);
                w_AttackersLocation.Add(Pos);
            }
            XLoc++;
            YLoc++;

        }
        XLoc = posX - 1;
        YLoc = posY + 1;
        for (int i = 1; i < HowMuchLeftDown - 1; i++)
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.White && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Bishop || piece == Piece.Queen)
            {
                w_Attackers.Add(piece);
                w_AttackersLocation.Add(Pos);
            }
            XLoc--;
            YLoc++;

        }
        int Loc = 0;
        for (int i = 0; i < KnightPreCalc[to].Count(); i++)
        {

            Loc = KnightPreCalc[to][i];
            int piece = ValueToPiece(CB[Loc]);
            if (piece == Piece.Knight)
            {
                w_Attackers.Add(piece);
                w_AttackersLocation.Add(Loc);
            }
        }

        if (IndexToY[to] < 7)
        {
            if (IndexToX[to] > 0)
            {
                if (CB[to + 7] == (Piece.Black | Piece.Pawn))
                {
                    b_Attackers.Add(Piece.Pawn);
                    b_AttackersLocation.Add(to + 7);
                }


            }
            if (IndexToX[to] < 7)
            {
                if (CB[to + 9] == (Piece.Black | Piece.Pawn))
                {
                    b_Attackers.Add(Piece.Pawn);
                    b_AttackersLocation.Add(to + 9);
                }

            }
        }

        XLoc = posX;
        YLoc = posY - 1;
        //Debug.Log(HowMuchUp);
        for (int i = 1; i < HowMuchUp - 1; i++)//up
        {
            int Pos = XLoc + 8 * YLoc;
            //Debug.Log(Pos);
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.Black && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Rook || piece == Piece.Queen)
            {
                b_Attackers.Add(piece);
                b_AttackersLocation.Add(Pos);
            }
            YLoc--;
        }
        XLoc = posX;
        YLoc = posY + 1;
        for (int i = 1; i < HowMuchDown - 1; i++)//down
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.Black && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Rook || piece == Piece.Queen)
            {
                b_Attackers.Add(piece);
                b_AttackersLocation.Add(Pos);
            }
            YLoc++;
        }
        XLoc = posX + 1;
        YLoc = posY;
        for (int i = 1; i < HowMuchRight - 1; i++)//right
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.Black && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Rook || piece == Piece.Queen)
            {
                b_Attackers.Add(piece);
                b_AttackersLocation.Add(Pos);
            }
            XLoc++;
        }
        XLoc = posX - 1;
        YLoc = posY;
        for (int i = 1; i < HowMuchLeft - 1; i++)//left
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.Black && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Rook || piece == Piece.Queen)
            {
                b_Attackers.Add(piece);
                b_AttackersLocation.Add(Pos);
            }
            XLoc--;
        }
        XLoc = posX + 1;
        YLoc = posY - 1;
        for (int i = 1; i < HowMuchRightTop - 1; i++)
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.Black && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Bishop || piece == Piece.Queen)
            {
                b_Attackers.Add(piece);
                b_AttackersLocation.Add(Pos);
            }
            XLoc++;
            YLoc--;

        }
        XLoc = posX - 1;
        YLoc = posY - 1;
        for (int i = 1; i < HowMuchLeftTop - 1; i++)
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.Black && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Bishop || piece == Piece.Queen)
            {
                b_Attackers.Add(piece);
                b_AttackersLocation.Add(Pos);
            }
            XLoc--;
            YLoc--;

        }
        XLoc = posX + 1;
        YLoc = posY + 1;
        for (int i = 1; i < HowMuchRightDown - 1; i++)
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.Black && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Bishop || piece == Piece.Queen)
            {
                b_Attackers.Add(piece);
                b_AttackersLocation.Add(Pos);
            }
            XLoc++;
            YLoc++;

        }
        XLoc = posX - 1;
        YLoc = posY + 1;
        for (int i = 1; i < HowMuchLeftDown - 1; i++)
        {
            int Pos = XLoc + 8 * YLoc;
            int piece = ValueToPiece(CB[Pos]);
            if (ValueToCol(CB[Pos]) != Piece.Black && ValueToCol(CB[Pos]) != 0) break;
            if (piece == Piece.Bishop || piece == Piece.Queen)
            {
                b_Attackers.Add(piece);
                b_AttackersLocation.Add(Pos);
            }
            XLoc--;
            YLoc++;

        }
        Loc = 0;
        for (int i = 0; i < KnightPreCalc[to].Count(); i++)
        {

            Loc = KnightPreCalc[to][i];
            int piece = ValueToPiece(CB[Loc]);
            if (piece == Piece.Knight)
            {
                b_Attackers.Add(piece);
                b_AttackersLocation.Add(Loc);
            }
        }




        //simulate captures
        bool isWhiteTurn;
        int score = 0;
        //Debug.Log(CB[to]);
        //if (CB[to] == 0)
        //{
        //    for (int i = 0; i < CB.Length; i++)
        //    {
        //        int c  = CB[i];

        //        Debug.Log(i + ":"+ColToString(ValueToCol(c)) + PieceToString(ValueToPiece(c)));
        //    }
        //}
        //Debug.Log(ValidityCheck(CB, HashList));
        //Debug.Log(to);
        int VictimValue = 0;
        if (isEnpassent)
        {
            if (Col == Piece.Black)
            {
                VictimValue = pieceValues[ValueToPiece(CB[to - 8])];
            }
            else
            {
                VictimValue = pieceValues[ValueToPiece(CB[to + 8])];
            }
        }
        else
        {
            VictimValue = pieceValues[ValueToPiece(CB[to])];
        }

        //Debug.Log(CB[to]);

        int whiteCount = w_Attackers.Count();
        int blackCount = b_Attackers.Count();
        int w = 0;
        int b = 0;
        //Debug.Log(whiteCount + "," + blackCount);
        if (Col != Piece.White)
        {
            for (int i = 0; i < whiteCount + blackCount; i++)
            {
                //Debug.Log(i);
                //짝수면 백, 홀수면 흑차례임
                if (i % 2 != 0)//white turn
                {
                    if (w >= whiteCount) break;
                    score += VictimValue;

                    VictimValue = pieceValues[w_Attackers[w]];
                    w++;
                }
                else // black turn
                {
                    if (b >= blackCount) break;
                    score -= VictimValue;
                    VictimValue = pieceValues[b_Attackers[b]];
                    b++;
                }
            }

        }
        else
        {
            for (int i = 0; i < whiteCount + blackCount; i++)
            {
                //짝수면 흑, 홀수면 백차례임
                if (i % 2 != 0)//black turn
                {
                    if (b >= blackCount) break;
                    score -= VictimValue;
                    VictimValue = pieceValues[b_Attackers[b]];
                    b++;
                }
                else // white turn
                {
                    if (w >= whiteCount) break;
                    score += VictimValue;
                    VictimValue = pieceValues[w_Attackers[w]];
                    w++;
                }
            }
        }
        return score;

    }

    bool ValidityCheck(int[] CB, List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (CB[list[i]] == 0)
            {
                return false;
            }
        }
        return true;
    }
    int EvaluateBoard(int[] CB, List<int> HashList, bool WKC, bool WQC, bool BKC, bool BQC)
    {
        List<bool> isOpenFile = new List<bool> { true, true, true, true, true, true, true, true };
        List<bool> isWhiteHalfOpen = new List<bool> { true, true, true, true, true, true, true, true };
        List<bool> isBlackHalfOpen = new List<bool> { true, true, true, true, true, true, true, true };
        List<int> WhitePawnList = new();
        List<int> BlackPawnList = new();
        int WhiteLightSquarePawn = 0;
        int WhiteDarkSquarePawn = 0;
        int BlackLightSquarePawn = 0;
        int BlackDarkSquarePawn = 0;
        int PawnCount = 0;
        //int WhiteIsolatedPawn = 0;
        //int BlackIsolatedPawn = 0;
        bool isOpened = false;
        bool isEndGame = false;
        int CenterPawnCount = 0;
        int QueenCount = 0;
        int MinorCount = 0;

        int[] whitePawnFile = new int[8];
        int[] blackPawnFile = new int[8];
        int WhiteKing = 0;
        int BlackKing = 0;
        for (int i = 0; i < HashList.Count; i++)
        {

            int Location = HashList[i];
            int Value = CB[Location];
            int XLoc = IndexToX[Location];
            int YLoc = IndexToY[Location];

            if (ValueToPiece(Value) == Piece.Pawn)
            {
                if (XLoc == 3 || XLoc == 4)//centre pawn
                {
                    CenterPawnCount++;
                }
                PawnCount++;
                if (ValueToCol(Value) == Piece.White)
                {
                    WhitePawnList.Add(Location);
                    if ((XLoc + YLoc) % 2 == 0) //lightSquare
                    {
                        WhiteLightSquarePawn++;
                    }
                    else//darkSquare
                    {
                        WhiteDarkSquarePawn++;
                    }
                    whitePawnFile[IndexToX[Location]] += 1;
                    isOpenFile[IndexToX[Location]] = false;
                    isWhiteHalfOpen[IndexToX[Location]] = false;
                }
                else
                {
                    BlackPawnList.Add(Location);
                    if ((XLoc + YLoc) % 2 == 0) //lightSquare
                    {
                        BlackLightSquarePawn++;
                    }
                    else//darkSquare
                    {
                        BlackDarkSquarePawn++;
                    }
                    blackPawnFile[IndexToX[Location]] += 1;

                    isOpenFile[IndexToX[Location]] = false;
                    isBlackHalfOpen[IndexToX[Location]] = false;
                }
            }
            else if (ValueToPiece(Value) == Piece.Queen)
            {
                QueenCount++;
            }
            else if (ValueToPiece(Value) == Piece.Bishop || ValueToPiece(Value) == Piece.Knight)
            {
                MinorCount++;
            }
            else if (ValueToPiece(Value) == Piece.King)
            {
                if (ValueToCol(Value) == Piece.White)
                {
                    WhiteKing = Location;
                }
                else
                {
                    BlackKing = Location;
                }
            }
        }
        if (QueenCount == 0 || MinorCount <= 1)
        {
            isEndGame = true;
        }
        if (CenterPawnCount < 3)
        {
            isOpened = true;
        }
        //Debug.Log(WhiteLightSquarePawn + " " + WhiteDarkSquarePawn + " " + BlackLightSquarePawn +" " + BlackDarkSquarePawn);
        int Eval = 0;

        int WhiteCount = 0;
        bool isKingsideCastledWhite = (IndexToX[WhiteKing] >= 5);
        bool isQueensideCastledWhite = (IndexToX[WhiteKing] <= 2);
        bool isKingsideCastledBlack = (IndexToX[BlackKing] >= 5);
        bool isQueensideCastledBlack = (IndexToX[BlackKing] <= 2);
        //Debug.Log(HashList.Count);
        for (int i = 0; i < HashList.Count; i++)
        {


            int Location = HashList[i];
            int Value = CB[Location];
            int Col = ValueToCol(Value);
            int Shape = ValueToPiece(Value);
            int ColMult = (Col - 12) / -4;
            bool IsLightSquare = ((IndexToX[Location] + IndexToY[Location]) % 2 == 0);


            int BasePieceScore = pieceValues[Shape];
            int forward = 8;
            int file = IndexToX[Location];
            if (Col == Piece.White)
            {
                forward = -8;
                WhiteCount++;
            }

            int PieceBonus = 0;

            if (Shape == Piece.Pawn)
            {
                if (Col == Piece.White)
                {
                    //int file = IndexToX[Location];
                    bool isPassed = false;
                    if (blackPawnFile[file] == 0)
                    {
                        if (file > 0 && blackPawnFile[file - 1] == 0)
                        {
                            isPassed = true;
                        }
                        if (isPassed)
                        {
                            if (file < 7 && blackPawnFile[file + 1] == 0)
                            {
                                isPassed = true;
                            }
                            else
                            {
                                isPassed = false;
                            }
                        }

                    }
                    if (isEndGame)
                    {
                        PieceBonus += PawnSquareEnd[Location];
                        if (isPassed)
                        {
                            PieceBonus += 150;
                        }

                    }
                    else
                    {
                        PieceBonus += PawnSquare[Location];
                        if (isPassed)
                        {
                            PieceBonus += 100;
                        }
                    }
                    if (isKingsideCastledBlack && isQueensideCastledWhite)//white can pawn storm black in the kingside
                    {
                        if (IndexToX[Location] >= 5)
                        {
                            PieceBonus += (7 - IndexToY[Location]) * 10;
                        }

                    }
                    if (isQueensideCastledBlack && isKingsideCastledWhite)//white can pawn storm black in the queenside
                    {
                        if (IndexToX[Location] <= 2)
                        {
                            PieceBonus += (7 - IndexToY[Location]) * 10;
                        }
                    }
                    if (whitePawnFile[file] > 1)//double pawn penalty
                    {
                        PieceBonus -= (whitePawnFile[file] - 1) * 20;
                    }

                }
                else
                {
                    //int file = IndexToX[Location];
                    bool isPassed = false;
                    if (whitePawnFile[file] == 0)
                    {
                        if (file > 0 && whitePawnFile[file - 1] == 0)
                        {
                            isPassed = true;
                        }
                        if (isPassed)
                        {
                            if (file < 7 && whitePawnFile[file + 1] == 0)
                            {
                                isPassed = true;
                            }
                            else
                            {
                                isPassed = false;
                            }
                        }

                    }
                    if (isEndGame)
                    {
                        PieceBonus += PawnSquareEnd[63 - Location];
                        if (isPassed)
                        {
                            PieceBonus += 150;
                        }
                    }
                    else
                    {
                        PieceBonus += PawnSquare[63 - Location];
                        if (isPassed)
                        {
                            PieceBonus += 100;
                        }
                    }
                    if (isKingsideCastledBlack && isQueensideCastledWhite)//black can pawn storm white in the queenside
                    {
                        //if()
                        if (IndexToX[Location] <= 2)
                        {
                            PieceBonus += (7 - IndexToY[Location]) * 10;
                        }
                    }
                    if (isQueensideCastledBlack && isKingsideCastledWhite)//black can pawn storm white in the kingside
                    {
                        if (IndexToX[Location] >= 5)
                        {
                            PieceBonus += (7 - IndexToY[Location]) * 10;
                        }
                    }
                    if (blackPawnFile[file] > 1)//double pawn penalty
                    {
                        PieceBonus -= (blackPawnFile[file] - 1) * 20;
                    }
                }
            }
            else if (Shape == Piece.Bishop)
            {

                if (Col == Piece.White)
                {
                    PieceBonus += BishopSquare[Location];
                    if (IsLightSquare)
                    {
                        PieceBonus += (WhiteLightSquarePawn) * 3;
                    }
                    else
                    {
                        PieceBonus += (WhiteDarkSquarePawn) * 3;
                    }
                }
                else
                {
                    PieceBonus += BishopSquare[63 - Location];
                    if (IsLightSquare)
                    {
                        PieceBonus += (BlackLightSquarePawn) * 3;
                    }
                    else
                    {
                        PieceBonus += (BlackDarkSquarePawn) * 3;
                    }
                }
                PieceBonus += (16 - PawnCount) * 1;
                if (isOpened) PieceBonus += 1;
            }
            else if (Shape == Piece.Knight)
            {
                if (Col == Piece.White)
                {
                    PieceBonus += KnightSquare[Location];
                }
                else
                {

                    PieceBonus += KnightSquare[63 - Location];
                }

                if (IsLightSquare)
                {
                    PieceBonus += (4 - (WhiteDarkSquarePawn + BlackDarkSquarePawn)) * 1;
                }
                else
                {
                    PieceBonus += (4 - (WhiteLightSquarePawn + BlackLightSquarePawn)) * 1;
                }
                PieceBonus += (PawnCount) * 1;
                if (!isOpened) PieceBonus += 1;

                if (IndexToY[Location] > 0 && IndexToY[Location] < 7)
                {

                    if ((IndexToX[Location] < 7) && (CB[Location - forward + 1] == (Col | Piece.Pawn)))
                    {
                        PieceBonus += ColMult * 30;
                    }
                    else if ((IndexToX[Location] > 0) && (CB[Location - forward - 1] == (Col | Piece.Pawn)))
                    {
                        PieceBonus += ColMult * 30;
                    }

                }

            }
            else if (Shape == Piece.Rook)
            {
                if (Col == Piece.White)
                {
                    PieceBonus += RookSquare[Location];
                    if (isOpenFile[file])
                    {
                        PieceBonus += 20;
                    }
                    else if (isWhiteHalfOpen[file])
                    {
                        PieceBonus += 10;
                    }
                }
                else
                {
                    PieceBonus += RookSquare[63 - Location];
                    if (isOpenFile[file])
                    {
                        PieceBonus += 20;
                    }
                    else if (isBlackHalfOpen[file])
                    {
                        PieceBonus += 10;
                    }
                }


            }
            else if (Shape == Piece.Queen)
            {
                if (Col == Piece.White)
                {
                    PieceBonus += QueenSquare[Location];

                }
                else
                {
                    PieceBonus += QueenSquare[63 - Location];
                }

            }
            else if (Shape == Piece.King)
            {
                if (Col == Piece.White)
                {
                    //WhiteKing = Location;
                    if (isEndGame)
                    {
                        PieceBonus += KingSquareEnd[Location];
                    }
                    else
                    {
                        PieceBonus += KingSquare[Location];
                    }
                }
                else
                {
                    //BlackKing = Location;
                    if (isEndGame)
                    {
                        PieceBonus += KingSquareEnd[63 - Location];
                    }
                    else
                    {
                        PieceBonus += KingSquare[63 - Location];
                    }
                }
            }
            //int PieceValue = 
            Eval += (BasePieceScore + PieceBonus) * ColMult;
            //Debug.Log(BasePieceScore);
        }
        if (!isEndGame)
        {
            if (WhiteKing != 62 && WhiteKing != 63 && WhiteKing != 56 && WhiteKing != 57 && WhiteKing != 58)//white king not on safe square
            {
                if (WKC || WQC)
                {
                    Eval += 30;
                }
                else
                {
                    Eval -= 30;
                }
            }
            if (BlackKing != 6 && BlackKing != 7 && BlackKing != 0 && BlackKing != 1 && BlackKing != 2)
            {
                if (BKC || BQC)
                {
                    Eval -= 30;
                }
                else
                {
                    Eval += 30;
                }
            }

        }
        if (CB[11] == (Piece.Black | Piece.Pawn)) //penalty if blocks center pawn advance
        {
            if (ValueToCol(CB[19]) == Piece.Black)
            {
                Eval += 20;
            }
        }
        if (CB[12] == (Piece.Black | Piece.Pawn))
        {
            if (ValueToCol(CB[20]) == Piece.Black)
            {
                Eval += 20;
            }
        }

        if (CB[51] == (Piece.White | Piece.Pawn))
        {
            if (ValueToCol(CB[43]) == Piece.White)
            {
                Eval -= 20;
            }
        }
        if (CB[52] == (Piece.White | Piece.Pawn))
        {
            if (ValueToCol(CB[44]) == Piece.White)
            {
                Eval -= 20;
            }
        }
        if (HashList.Count <= 3)
        {
            int BlackCount = HashList.Count - WhiteCount;
            int AddScore = 0;
            if (WhiteCount < BlackCount) // black is winning
            {
                AddScore = Distance_FromCenter[WhiteKing] * -10;
            }
            else if (WhiteCount > BlackCount)
            {
                AddScore = Distance_FromCenter[BlackKing] * 10;
            }
            else if (WhiteCount == BlackCount)
            {
                AddScore = Distance_FromCenter[BlackKing] * 10 + Distance_FromCenter[WhiteKing] * -10;
            }
            Eval += AddScore;
        }
        return Eval;


    }

    int SimpleEvaluateBoard(int[] CB, List<int> HashList, bool WKC, bool WQC, bool BKC, bool BQC)
    {
        List<bool> isOpenFile = new List<bool> { true, true, true, true, true, true, true, true };
        List<bool> isWhiteHalfOpen = new List<bool> { true, true, true, true, true, true, true, true };
        List<bool> isBlackHalfOpen = new List<bool> { true, true, true, true, true, true, true, true };
        List<int> WhitePawnList = new();
        List<int> BlackPawnList = new();
        int WhiteLightSquarePawn = 0;
        int WhiteDarkSquarePawn = 0;
        int BlackLightSquarePawn = 0;
        int BlackDarkSquarePawn = 0;
        int PawnCount = 0;
        //int WhiteIsolatedPawn = 0;
        //int BlackIsolatedPawn = 0;
        bool isOpened = false;
        bool isEndGame = false;
        int CenterPawnCount = 0;
        int QueenCount = 0;
        int MinorCount = 0;

        int[] whitePawnFile = new int[8];
        int[] blackPawnFile = new int[8];
        int WhiteKing = 0;
        int BlackKing = 0;

        //Debug.Log(WhiteLightSquarePawn + " " + WhiteDarkSquarePawn + " " + BlackLightSquarePawn +" " + BlackDarkSquarePawn);
        int Eval = 0;

        int WhiteCount = 0;
        bool isKingsideCastledWhite = (IndexToX[WhiteKing] >= 5);
        bool isQueensideCastledWhite = (IndexToX[WhiteKing] <= 2);
        bool isKingsideCastledBlack = (IndexToX[BlackKing] >= 5);
        bool isQueensideCastledBlack = (IndexToX[BlackKing] <= 2);
        //Debug.Log(HashList.Count);
        for (int i = 0; i < HashList.Count; i++)
        {


            int Location = HashList[i];
            int Value = CB[Location];
            int Col = ValueToCol(Value);
            int Shape = ValueToPiece(Value);
            int ColMult = (Col - 12) / -4;
            bool IsLightSquare = ((IndexToX[Location] + IndexToY[Location]) % 2 == 0);


            int BasePieceScore = pieceValues[Shape];

            //int PieceValue = 
            Eval += (BasePieceScore) * ColMult;
            //Debug.Log(BasePieceScore);
        }

        return Eval;


    }
    (Move, int) AI(int col)
    {

        //col = Piece.White;
        //RepresentBoard();
        EngineLine = new PVLine(0, new Move[20]);
        //EngineLine.Clear();

        //(PossibleMovements, _) = GenerateLegalMoves(WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, chessBoard, chessBoardHash, ValueToCol(chessBoard[location]), false);

        //IamAI
        //return;
        OppMove = new Move(0, 0, 0);
        TimeControl.Reset();
        TimeControl.Start();
        Move lastOpp = new Move(0, 0, 0);
        //int CurrDepth;
        int Eval = 0;
        int LastEval = 0;
        if (IsThreeFold(ThreeFoldRep, HashKey, 3))
        {
            return(new Move(0,0,0), 0);
        }
        //HashKey ^= side_Keys;

        //bool isReal;
        for (CurrDepth = 1; CurrDepth < MAX_DEPTH; CurrDepth++)
        {
            SavedByTransposition = 0;
            moveCount = 0;
            QmoveCount = 0;
            cutoff = 0;
            EngineLine = new PVLine(0, new Move[20]);

            if (TimeControl.ElapsedMilliseconds >= TIME_LIMIT)
            {
                break;
            }
            AITime.Reset();
            AITime.Start();

            //int Eval;
            bool isReal;
            if (col == Piece.Black)
            {

                (Eval, OppMove, isReal) = AlphaBetaMin(CurrDepth, chessBoard, chessBoardHash, WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, lastMove, int.MinValue, int.MaxValue, HashKey, ThreeFoldRep, ref EngineLine);
            }
            else
            {
                (Eval, OppMove, isReal) = AlphaBetaMax(CurrDepth, chessBoard, chessBoardHash, WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, lastMove, int.MinValue, int.MaxValue, HashKey, ThreeFoldRep, ref EngineLine);
            }

            if (!isReal)
            {
                //Debug.Log(OppMove.From + "," + OppMove.To + "," + OppMove.Type);
                //Debug.Log(new Move(0, 0, 0).From + "," + new Move(0, 0, 0).To + "," + new Move(0, 0, 0).Type);


                //Debug.Log(new Move(0, 0, 0) == new Move(0, 0, 0));
                if (ValueEquals(OppMove, new Move(0, 0, 0)))
                {
                    //Debug.Log("enter");
                    OppMove = lastOpp;
                    Eval = LastEval;
                }

            }
            else
            {
                LastEval = Eval;

            }
            AITime.Stop();
            //Debug.Log(isReal);
            Console.WriteLine("info "+"depth " + CurrDepth + " cp "  + Eval + " time " + AITime.ElapsedMilliseconds +" nodes " + (moveCount + QmoveCount) +" nps " + (moveCount + QmoveCount) / (AITime.ElapsedMilliseconds / 1000f) + " pv " + GetEngineLine());
            //Debug.Log(EngineLine.ArgMove.Length);
            lastOpp = OppMove;

            //if (Eval <= -90000000)
            //{
            //    break;
            //}
        }
        string GetEngineLine()
        {
            if (EngineLine == null)
            {
                return "";
            }
            string line = "";
            for (int i = 0; i < EngineLine.ArgMove.Length; i++)
            {
                if (EngineLine.ArgMove[i] == null)
                {
                    //Debug.Log(i);
                    break;
                }
                line += NumberToString(EngineLine.ArgMove[i].From) + NumberToString(EngineLine.ArgMove[i].To) + " ";


            }
            return line;
        }
        //Debug.Log("----FINAL_RESULT----\n" + "Depth" + CurrDepth + "EvalValue:" + Eval + "bestmove: " + NumberToString(OppMove.From) + "to" + NumberToString(OppMove.To) + "Time:" + TimeControl.ElapsedMilliseconds + " MS (" + TimeControl.ElapsedMilliseconds / 1000f + " S)" + "NormalMoves:" + moveCount + "QMoves:" + QmoveCount);
        if (ValueEquals(OppMove, new Move(0, 0, 0)))
        {
            List<Move> moves;
            if (col == Piece.Black)
            {
                moves = GenerateLegalMoves(WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, chessBoard, chessBoardHash, Piece.Black, false, false, lastMove).Item1;

            }
            else
            {
                moves = GenerateLegalMoves(WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, chessBoard, chessBoardHash, Piece.White, false, false, lastMove).Item1;
            }
            OppMove = moves[0];
        }

        
        MakeMove(OppMove, chessBoard, chessBoardHash, ref HashKey);
        //board.ChangeColorPieceMovement(OppMove.From, OppMove.To);
        //chessBoard = data.Board;
        //chessBoardHash = data.Hash;

        if (OppMove.Type == MoveType.Special00)//hash enpassent
        {
            if (col == Piece.Black)
            {
                HashKey ^= en_Keys[OppMove.From - 8];
            }
            else
            {

                HashKey ^= en_Keys[OppMove.From + 8];
            }

        }
        HashKey ^= castle_Keys[CastlingIndex(WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle)];
        if (col == Piece.Black)
        {
            if (BlackKingCastle && (chessBoard[4] != (Piece.Black | Piece.King)) || (chessBoard[7] != (Piece.Black | Piece.Rook)))
            {

                BlackKingCastle = false;
            }

            if (BlackQueenCastle && (chessBoard[4] != (Piece.Black | Piece.King)) || (chessBoard[0] != (Piece.Black | Piece.Rook)))
            {
                BlackQueenCastle = false;
            }
        }
        else
        {
            if (WhiteKingCastle && (chessBoard[60] != (Piece.White | Piece.King)) || (chessBoard[63] != (Piece.White | Piece.Rook)))
            {

                WhiteKingCastle = false;
            }

            if (WhiteQueenCastle && (chessBoard[60] != (Piece.White | Piece.King)) || (chessBoard[56] != (Piece.White | Piece.Rook)))
            {
                WhiteQueenCastle = false;
            }


        }
        HashKey ^= castle_Keys[CastlingIndex(WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle)];
        HashKey ^= side_Keys;
        
        if (((OppMove.Type & MoveType.capture) != 0) || (ValueToPiece(chessBoard[OppMove.To]) == Piece.Pawn) || ((OppMove.Type == MoveType.Special01) || (OppMove.Type == (MoveType.Special01 | MoveType.Special00))))//resetMoveCount
        {
            ThreeFoldRep.Clear();
        }
        if (ThreeFoldRep.ContainsKey(HashKey))
        {
            ThreeFoldRep[HashKey] += 1;
        }
        else
        {
            ThreeFoldRep.Add(HashKey, 1);
        }


        moveCount++;
        lastMove = OppMove;


        //RepresentBoard();

        //EvalBar.value = LastEval;
        return (OppMove, Eval);

    }
    bool ValueEquals(Move move1, Move move2)
    {
        return (move1.From == move2.From && move1.To == move2.To && move1.Type == move2.Type);
    }
    //void PrintThreeFoldRep()
    //{
    //    Debug.Log("Key\t\tValue");
    //    foreach (var kvp in ThreeFoldRep)
    //    {
    //        Debug.Log($"{kvp.Key}\t{kvp.Value}");
    //    }
    //}
    (int, Move, bool) AlphaBetaMax(int depth, int[] CB, List<int> HashList, bool WKC, bool WQC, bool BKC, bool BQC, Move lMove, int alpha, int beta, UInt64 h, Dictionary<UInt64, int> ThreeFold, ref PVLine pline)
    {
        PVLine line = new PVLine(0, new Move[20]);
        int[] Board = (int[])CB.Clone();
        //Array.Copy()
        List<int> HL = new List<int>(HashList);
        List<Move> MoveList = new();
        bool Check;
        bool isCheckingMove;
        int Col = Piece.White;
        Move BestMove = new Move(0, 0, 0);
        UInt64 Hash = h;
        List<Move> Qpv = new();
        (MoveList, Check, isCheckingMove) = GenerateLegalMoves(WKC, WQC, BKC, BQC, CB, HashList, Col, false, true, lMove);

        Dictionary<UInt64, int> ThreeFoldRepCopy = new Dictionary<UInt64, int>(ThreeFold);
        //List<Move> nodeLineCopy = new List<Move>(NodeLine);
        if (MoveList.Count == 0)
        {
            if (Check)
            {
                //Hash ^= side_Keys;
                //TranspositionTable[GetTranspositionIndex(Hash, hashSize)] = new TranspositionValue(Hash, -(100000000 + CurrDepth - depth), 9999, null, 0);
                //pline.Cmove = 0;
                return (-100000000, lMove, true);

            }
            else
            {
                //Hash ^= side_Keys;
                //TranspositionTable[GetTranspositionIndex(Hash, hashSize)] = new TranspositionValue(Hash, 0, 9999, null, 0);
                //pline.Cmove = 0;
                return (0, lMove, true);
            }

        }
        if (depth == 0)
        {
            //pline.Cmove = 0;
            //if (((AITime.ElapsedMilliseconds / 1000f) < TIME_LIMIT) && ((lMove.Type | MoveType.capture) != 0 || Check || (lMove.Type | MoveType.promotion) != 0) && TotalDepth < MAX_DEPTH) //iterative deepening
            //{
            //    //Debug.Log("deep");
            //    depth++;
            //    TotalDepth++;
            //}
            //else
            //{


            //}
            if (isQuiescence && IsCapture(lMove))//capture move
            {
                //int Eval = EvaluateBoard(CB, HashList, WKC, WQC, BKC, BQC);

                //UpdateTranspositionTable(h, Eval, pline, 0, 0);
                //TranspositionTable[GetTranspositionIndex(h, hashSize)] = new TranspositionValue(h, 0, 9999, null);
                //return (Eval, lMove, true);
                //return (, lMove, true, null);
                int Val;
                bool isComplete;

                (Val, isComplete) = quiescenceMax(Q_DEPTH, CB, HashList, WKC, WQC, BKC, BQC, lMove, alpha, beta);

                return (Val, lMove, isComplete);
            }
            else
            {
                int Eval = EvaluateBoard(CB, HashList, WKC, WQC, BKC, BQC);
                UpdateTranspositionTable(h, Eval, 0, 0, true);

                return (Eval, lMove, true);
            }


        }
        if (HashList.Count == 2)
        {
            return (0, lMove, true);
        }
        if (IsThreeFold(ThreeFoldRepCopy, Hash, 3))
        {
            return (0, new Move(0, 0, 0), true);
        }

        if (depth != 1)
        {
            if (isMoveOrdering)
                MoveList.Sort(CompareMoves);
        }


        bool WhiteKingCastle = WKC;
        bool WhiteQueenCastle = WQC;
        bool BlackKingCastle = BKC;
        bool BlackQueenCastle = BQC;
        bool isChildSearchFinished;
        //List<Move> ChildLine = new();
        int Score = 0;
        // List<Move> bestMoveLine = new();
        moveCount++;
        for (int i = 0; i < MoveList.Count; i++)
        {
            Hash = h;
            WhiteKingCastle = WKC;
            WhiteQueenCastle = WQC;
            BlackKingCastle = BKC;
            BlackQueenCastle = BQC;
            ThreeFoldRepCopy = ThreeFoldRep.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            if (TimeControl.ElapsedMilliseconds >= TIME_LIMIT)
            {
                if (depth == CurrDepth)
                {
                    return (0, BestMove, false);
                }
                else
                {
                    return (0, new Move(0, 0, 0), false);
                }

            }
            Board = (int[])CB.Clone();
            //Array.Copy()
            HL = new List<int>(HashList);
            MakeMove(MoveList[i], Board, HL, ref Hash);
            if (lMove.Type == MoveType.Special00)//hash enpassent
            {
                Hash ^= en_Keys[lMove.From + 8];
            }

            Hash ^= castle_Keys[CastlingIndex(WKC, WQC, BKC, BQC)];
            if (WKC && ((Board[60] != (Piece.White | Piece.King)) || (Board[63] != (Piece.White | Piece.Rook))))
            {
                WhiteKingCastle = false;
            }
            if (WQC && ((Board[60] != (Piece.White | Piece.King)) || (Board[56] != (Piece.White | Piece.Rook))))
            {
                WhiteQueenCastle = false;
            }
            Hash ^= castle_Keys[CastlingIndex(WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle)];
            Hash ^= side_Keys;

            //nodeLineCopy.Add(MoveList[i]);

            if (((MoveList[i].Type & MoveType.capture) != 0) || (ValueToPiece(CB[MoveList[i].To]) == Piece.Pawn) || ((MoveList[i].Type == MoveType.Special01) || (MoveList[i].Type == (MoveType.Special01 | MoveType.Special00))))//resetMoveCount
            {
                ThreeFoldRepCopy.Clear();
            }
            if (ThreeFoldRepCopy.ContainsKey(Hash))
            {
                ThreeFoldRepCopy[Hash] += 1;
            }
            else
            {
                ThreeFoldRepCopy.Add(Hash, 1);
            }
            bool isTransposition = false;

            if (IsThreeFold(ThreeFoldRepCopy, Hash, 3))
            {
                Score = 0;
                isTransposition = true;
                //Debug.Log(true);
                //return (0, new Move(0, 0, 0), true);

            }
            else
            {
                //1234567890
                if (TranspositionEnable)
                {
                    TranspositionValue transValue = TranspositionTable[GetTranspositionIndex(Hash, hashSize)];
                    if (transValue != null && transValue.Zobrish == (Hash) && transValue.Depth >= depth - 1)//prevent collision problem
                    {
                        //Debug.Log(transValue.Depth + "." + (CurrDepth - (depth - 1)) + ","+CurrDepth + ","+depth);
                        //if (TurnArrayForDebug[GetTranspositionIndex(Hash, hashSize)] != true)
                        //{
                        //    Debug.Log("thisisfuckingpart");
                        //}
                        if (transValue.State == 0)//fully searched node
                        {
                            isTransposition = true;
                            Score = transValue.Eval;

                            //Array.Copy(transValue.EngineLine.ArgMove, line.ArgMove, line.Cmove);
                            //line.Cmove = transValue.EngineLine.Cmove;
                            //line = transValue.EngineLine;
                        }
                        else if (transValue.State == 2)
                        {
                            if (beta <= transValue.Eval)
                            {
                                isTransposition = true;
                                Score = beta;

                                //Array.Copy(transValue.EngineLine.ArgMove, line.ArgMove, line.Cmove);
                                //line.Cmove = transValue.EngineLine.Cmove;
                                //line = transValue.EngineLine;
                            }

                        }
                    }
                }

            }
            //isTransposition = true;
            //int TranspositionVal = 0;
            if (!isTransposition)
            {
                (Score, _, isChildSearchFinished) = AlphaBetaMin(depth - 1, Board, HL, WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, MoveList[i], alpha, beta, Hash, ThreeFoldRepCopy, ref line);
                if (!isChildSearchFinished)
                {
                    return (0, new Move(0, 0, 0), false);
                }
                if (Score < -90000000)
                {
                    //Score -= (CurrDepth - depth);
                    Score += 1;
                }


            }
            else
            {
                SavedByTransposition++;
            }

            //isTransposition = false;
            if (Score >= beta)
            {
                if (!isTransposition && TranspositionEnable)
                {
                    UpdateTranspositionTable(Hash, beta, depth - 1, 2, true);
                }
                cutoff++;
                return (beta, MoveList[i], true);
            }
            if (!isTransposition && TranspositionEnable)
            {
                if (Score < -90000000)
                {
                    UpdateTranspositionTable(Hash, Score, 9999, 0, true);
                }
                else
                {
                    UpdateTranspositionTable(Hash, Score, depth - 1, 0, true);
                }

            }
            if (Score > alpha)
            {
                alpha = Score;
                BestMove = MoveList[i];

                pline.ArgMove[0] = BestMove;
                Array.Copy(line.ArgMove, 0, pline.ArgMove, 1, line.Cmove);
                pline.Cmove = line.Cmove + 1;
                //Debug.Log(Qpv.Count());
                //bestMoveLine.Clear();
                //if (nodeLineCopy != null)
                //{
                //    bestMoveLine.AddRange(nodeLineCopy);
                //}

                //if (ChildLine != null)
                //{
                //    bestMoveLine.AddRange(ChildLine);
                //}
                //NodeLine.Clear();
                //NodeLine.Add(BestMove);
                ////Debug.Log(NodeLine.Count + "," + ChildLine.Count);
                //if (ChildLine != null)
                //{
                //    NodeLine.AddRange(ChildLine);
                //}
            }
            //nodeLineCopy.RemoveAt(nodeLineCopy.Count - 1);
            //Hash = h;
        }
        //if (BestMove != new Move(0, 0, 0))
        //{
        //    //nodeLineCopy.Add(BestMove);
        //    if (bestMoveLine != null)
        //    {
        //        nodeLineCopy.AddRange(bestMoveLine);

        //    }
        //}
        //bestMoveLine.Add(BestMove);
        return (alpha, BestMove, true);
        int CompareMoves(Move move1, Move move2)
        {
            //changecomparemoves
            //return 0;
            //if (depth == CurrDepth)
            //{

            //    if (move1 == OppMove)
            //    {
            //        return -1;
            //    }
            //    if (move2 == OppMove)
            //    {
            //        return 1;
            //    }
            //}
            if (move1.To == lMove.To)
            {
                if (move2.To == lMove.To)
                    return 0; // Both moves capture the lastly moved piece.
                else
                    return -1; // move1 captures the lastly moved piece.
            }
            else if (move2.To == lMove.To)
            {
                return 1; // move2 captures the lastly moved piece.
            }
            if (IsCapture(move1) && IsCapture(move2))
            {
                //Debug.Log(move1.From + ", " + move1.To + "," + move1.Type);
                //Debug.Log(move2.From + ", " + move2.To + "," + move2.Type);
                int value1 = Get_MVVLVA_Value(move1, CB);
                int value2 = Get_MVVLVA_Value(move2, CB);
                return value2.CompareTo(value1); // Sort in descending order
            }
            else if (IsCapture(move1))
            {
                return -1; // move1 is a capture, move2 is not
            }
            else if (IsCapture(move2))
            {
                return 1; // move2 is a capture, move1 is not
            }
            else
            {
                return 0; // Both moves are non-captures; maintain their order
            }
        }
    }

    (int, Move, bool) AlphaBetaMin(int depth, int[] CB, List<int> HashList, bool WKC, bool WQC, bool BKC, bool BQC, Move lMove, int alpha, int beta, UInt64 h, Dictionary<UInt64, int> ThreeFold, ref PVLine pline)
    {
        PVLine line = new PVLine(0, new Move[20]);
        int[] Board = (int[])CB.Clone();
        //Array.Copy()
        List<int> HL = new List<int>(HashList);
        List<Move> MoveList = new();
        bool Check;
        bool isCheckingMove;
        int Col = Piece.Black;
        UInt64 Hash = h;
        (MoveList, Check, isCheckingMove) = GenerateLegalMoves(WKC, WQC, BKC, BQC, CB, HashList, Col, false, true, lMove);
        //move ordering
        List<Move> Qpv = new();
        Dictionary<UInt64, int> ThreeFoldRepCopy = new Dictionary<UInt64, int>(ThreeFold);
        //List<Move> nodeLineCopy = new List<Move>(NodeLine);
        if (MoveList.Count == 0)
        {
            if (Check)
            {
                //Hash ^= side_Keys;
                //TranspositionTable[GetTranspositionIndex(Hash, hashSize)] = new TranspositionValue(Hash, 100000000 + CurrDepth - depth, 9999, null, 0);
                //pline.Cmove = 0;
                return (100000000, lMove, true);
            }
            else
            {
                //Hash ^= side_Keys;
                //TranspositionTable[GetTranspositionIndex(Hash, hashSize)] = new TranspositionValue(Hash, 0, 9999, null, 0);
                //pline.Cmove = 0;
                return (0, lMove, true);
            }

        }
        if (depth == 0)
        {
            //pline.Cmove = 0;

            if (isQuiescence && IsCapture(lMove))//capture move
            {
                //int Eval = EvaluateBoard(CB, HashList, WKC, WQC, BKC, BQC);

                //UpdateTranspositionTable(h, Eval, pline, 0, 0);
                //TranspositionTable[GetTranspositionIndex(h, hashSize)] = new TranspositionValue(h, 0, 9999, null);
                //return (Eval, lMove, true);
                //if(TranspositionTable[GetTranspositionIndex(h, hashSize)] )

                int Val;
                bool isComplete;
                //int[] Copy = new int[64];
                //Array.Copy(CB, Copy, CB.Length);
                (Val, isComplete) = quiescenceMin(Q_DEPTH, CB, HashList, WKC, WQC, BKC, BQC, lMove, alpha, beta);

                return (Val, lMove, isComplete);
                //return (quiescenceMin(Q_DEPTH, CB, HashList, WKC, WQC, BKC, BQC, lMove, alpha, beta, Hash), lMove, true);

            }
            else
            {
                int Eval = EvaluateBoard(CB, HashList, WKC, WQC, BKC, BQC);

                UpdateTranspositionTable(h, Eval, 0, 0, false);
                return (Eval, lMove, true);
            }
            //if (((AITime.ElapsedMilliseconds / 1000f) < TIME_LIMIT) && ((lMove.Type | MoveType.capture) != 0 || Check) && (TotalDepth < MAX_DEPTH)) //iterative deepening
            //{
            //    //Debug.Log(TotalDepth + " " + MAX_DEPTH);

            //    //Debug.Log("deep");
            //    depth++;
            //    TotalDepth++;
            //}
            //else
            //{


            //}



        }
        if (HashList.Count == 2)
        {
            return (0, lMove, true);
        }
        if (IsThreeFold(ThreeFoldRepCopy, Hash, 3))
        {
            return (0, new Move(0, 0, 0), true);

        }
        if (depth != 1)
        {
            if (isMoveOrdering)
                MoveList.Sort(CompareMoves);

        }



        bool WhiteKingCastle = WKC;
        bool WhiteQueenCastle = WQC;
        bool BlackKingCastle = BKC;
        bool BlackQueenCastle = BQC;

        Move BestMove = new Move(0, 0, 0);
        //List<Move> ChildLine = new();
        int Score = 0;
        bool isChildSearchFinished;
        moveCount++;
        //List<Move> bestMoveLine = new();
        for (int i = 0; i < MoveList.Count; i++)
        {
            Hash = h;
            WhiteKingCastle = WKC;
            WhiteQueenCastle = WQC;
            BlackKingCastle = BKC;
            BlackQueenCastle = BQC;
            ThreeFoldRepCopy = ThreeFoldRep.ToDictionary(entry => entry.Key,
                                      entry => entry.Value);

            if (TimeControl.ElapsedMilliseconds >= TIME_LIMIT)
            {
                if (depth == CurrDepth)
                {
                    return (0, BestMove, false);
                }
                else
                {
                    return (0, new Move(0, 0, 0), false);
                }

            }
            //if(AITime)
            Board = (int[])CB.Clone();
            //Array.Copy()
            HL = new List<int>(HashList);
            MakeMove(MoveList[i], Board, HL, ref Hash);
            if (lMove.Type == MoveType.Special00)//hash enpassent
            {
                Hash ^= en_Keys[lMove.From - 8];
            }

            Hash ^= castle_Keys[CastlingIndex(WKC, WQC, BKC, BQC)];
            if (BKC && (Board[4] != (Piece.Black | Piece.King)) || (Board[7] != (Piece.Black | Piece.Rook)))
            {
                BlackKingCastle = false;

            }

            if (BQC && (Board[4] != (Piece.Black | Piece.King)) || (Board[0] != (Piece.Black | Piece.Rook)))
            {
                BlackQueenCastle = false;
            }
            Hash ^= castle_Keys[CastlingIndex(WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle)];

            Hash ^= side_Keys;
            //nodeLineCopy.Add(MoveList[i]);


            if (((MoveList[i].Type & MoveType.capture) != 0) || (ValueToPiece(CB[MoveList[i].To]) == Piece.Pawn) || ((MoveList[i].Type == MoveType.Special01) || (MoveList[i].Type == (MoveType.Special01 | MoveType.Special00))))//resetMoveCount
            {
                ThreeFoldRepCopy.Clear();
            }
            if (ThreeFoldRepCopy.ContainsKey(Hash))
            {
                ThreeFoldRepCopy[Hash] += 1;
            }
            else
            {
                ThreeFoldRepCopy.Add(Hash, 1);
            }
            bool isTransposition = false;
            //Debug.Log(IsThreeFold(ThreeFoldRepCopy, Hash));
            if (IsThreeFold(ThreeFoldRepCopy, Hash, 3))
            {
                Score = 0;
                isTransposition = true;

                //Debug.Log(true);
                //return (0, new Move(0, 0, 0), true);
            }
            else
            {
                if (TranspositionEnable)
                {
                    TranspositionValue transValue = TranspositionTable[GetTranspositionIndex(Hash, hashSize)];
                    if (transValue != null && transValue.Zobrish == (Hash) && transValue.Depth >= (depth - 1))//prevent collision problem
                    {
                        //Debug.Log(transValue.Depth);
                        //if(TurnArrayForDebug[GetTranspositionIndex(Hash, hashSize)] != false)
                        //{
                        //    Debug.Log("thisisfuckingpart");
                        //}
                        if (transValue.State == 0)//fully searched node
                        {
                            isTransposition = true;
                            Score = transValue.Eval;
                            //line.ArgMove = transValue.EngineLine.ArgMove;
                            //Array.Copy(transValue.EngineLine.ArgMove, line.ArgMove, line.Cmove);
                            //line.Cmove = transValue.EngineLine.Cmove;
                        }
                        else if (transValue.State == 1)
                        {
                            if (alpha >= transValue.Eval)
                            {
                                isTransposition = true;
                                Score = alpha;
                                //Array.Copy(transValue.EngineLine.ArgMove, line.ArgMove, line.Cmove);
                                //line.Cmove = transValue.EngineLine.Cmove;
                                //line = transValue.EngineLine;
                            }

                        }
                    }
                }

            }
            //isTransposition = true;
            //int TranspositionVal = 0;
            if (!isTransposition)
            {

                (Score, _, isChildSearchFinished) = AlphaBetaMax(depth - 1, Board, HL, WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, MoveList[i], alpha, beta, Hash, ThreeFoldRepCopy, ref line);
                if (!isChildSearchFinished)
                {
                    return (0, new Move(0, 0, 0), false);
                }
                if (Score > 90000000)
                {
                    //Score += (CurrDepth - depth);
                    Score -= 1;
                }



            }
            else
            {
                SavedByTransposition++;
            }

            //isTransposition = false;


            if (Score <= alpha)
            {
                if (!isTransposition && TranspositionEnable)
                {
                    UpdateTranspositionTable(Hash, alpha, depth - 1, 1, false);
                }
                cutoff++;
                return (alpha, MoveList[i], true);
            }
            if (!isTransposition && TranspositionEnable)
            {
                if (Score > 90000000)
                {
                    UpdateTranspositionTable(Hash, Score, 9999, 0, false);
                }
                else
                {
                    UpdateTranspositionTable(Hash, Score, depth - 1, 0, false);
                }

            }

            if (Score < beta)
            {
                //EngineLine.Clear();
                beta = Score;
                BestMove = MoveList[i];

                pline.ArgMove[0] = BestMove;
                Array.Copy(line.ArgMove, 0, pline.ArgMove, 1, line.Cmove);
                pline.Cmove = line.Cmove + 1;
                //NodeLine.Clear();
                //if(depth == CurrDepth)

                //bestMoveLine.Clear();

                //if(nodeLineCopy != null)
                //{
                //    bestMoveLine.AddRange(nodeLineCopy);
                //}

                //if (ChildLine != null)
                //{
                //    bestMoveLine.AddRange(ChildLine);
                //}

                //NodeLine.Add(BestMove);
                ////Debug.Log(NodeLine.Count + "," + ChildLine.Count);
                //if(ChildLine != null)
                //{
                //    NodeLine.AddRange(ChildLine);
                //}

            }
            //nodeLineCopy.RemoveAt(nodeLineCopy.Count - 1);


        }
        //bestMoveLine.Add(BestMove);
        //if (BestMove != new Move(0, 0, 0))
        //{
        //    //nodeLineCopy.Add(BestMove);
        //    if (bestMoveLine != null)
        //    {
        //        nodeLineCopy.AddRange(bestMoveLine);
        //    }
        //}

        return (beta, BestMove, true);

        int CompareMoves(Move move1, Move move2)
        {
            //changecomparemoves
            //return 0;

            //if (depth == CurrDepth)
            //{
            //    //Debug.Log("bitch");
            //    if (move1 == OppMove)
            //    {
            //        return -1;
            //    }
            //    if (move2 == OppMove)
            //    {
            //        return 1;
            //    }

            //}

            if (move1.To == lMove.To)
            {
                if (move2.To == lMove.To)
                    return 0; // Both moves capture the lastly moved piece.
                else
                    return -1; // move1 captures the lastly moved piece.
            }
            else if (move2.To == lMove.To)
            {
                return 1; // move2 captures the lastly moved piece.
            }
            if (IsCapture(move1) && IsCapture(move2))
            {
                //Debug.Log(move1.From + ", " + move1.To + "," + move1.Type);
                //Debug.Log(move2.From + ", " + move2.To + "," + move2.Type);
                int value1 = Get_MVVLVA_Value(move1, CB);
                int value2 = Get_MVVLVA_Value(move2, CB);
                return value2.CompareTo(value1); // Sort in descending order
            }
            else if (IsCapture(move1))
            {
                return -1; // move1 is a capture, move2 is not
            }
            else if (IsCapture(move2))
            {
                return 1; // move2 is a capture, move1 is not
            }
            else
            {
                return 0; // Both moves are non-captures; maintain their order
            }
        }
    }


    void UpdateTranspositionTable(UInt64 Zobrish, int eval, int depth, int State, bool Col)
    {
        int index = GetTranspositionIndex(Zobrish, hashSize);
        TranspositionValue Transposition = TranspositionTable[index];
        //if(Transposition != null)
        //{
        //    Debug.Log(depth + Transposition.Depth);
        //}
        //Debug.Log(Transposition);

        //Debug.Log(Transposition == null);
        if (Transposition == null || depth > Transposition.Depth)// || depth >= Transposition.Depth
        {
            TranspositionTable[index] = new TranspositionValue(Zobrish, eval, depth, State);
            //TurnArrayForDebug[index] = Col;
        }
    }
    //bool isCheck(Move move, )
    bool IsCapture(Move move)
    {
        if ((move.Type & MoveType.capture) != 0)
        {
            return true;
        }
        return false;
    }
    int GetValueOfPiece(int piece)
    {
        int shape = ValueToPiece(piece);
        if (shape == Piece.Pawn)
        {
            return 1;
        }
        else if ((shape == Piece.Knight) || (shape == Piece.Bishop))
        {
            return 2;
        }
        else if (shape == Piece.Rook)
        {
            return 3;
        }
        else if (shape == Piece.Queen)
        {
            return 4;
        }
        else//king
        {
            return 5;
        }
    }

    (int, bool) quiescenceMax(int depth, int[] CB, List<int> HashList, bool WKC, bool WQC, bool BKC, bool BQC, Move lMove, int alpha, int beta)
    {
        UInt64 hash = 0;
        QmoveCount++;
        int eval = EvaluateBoard(CB, HashList, WKC, WQC, BKC, BQC);

        if (eval >= beta)
            return (beta, true);

        if (eval > alpha)
            alpha = eval;

        int[] Board = (int[])CB.Clone();
        //Array.Copy()
        List<int> HL = new List<int>(HashList);
        List<Move> MoveList = new();
        bool Check;
        bool isCheckingMove;
        int Col = Piece.White;
        Move BestMove = new Move(0, 0, 0);

        bool isChildSearchFinished;

        (MoveList, Check, isCheckingMove) = GenerateLegalMoves(WKC, WQC, BKC, BQC, CB, HashList, Col, false, true, lMove);
        if (depth != 1)
        {
            MoveList.Sort(CompareMoves);
        }
        int howmuch = 0;
        for (int i = 0; i < MoveList.Count; i++)
        {
            if (!IsCapture(MoveList[i])) continue;
            //Debug.Log(ValidityCheck(CB, HashList));
            if (StaticExchangeEvaluation(CB, HashList, MoveList[i]) < 0) continue;
            howmuch++;
            WhiteKingCastle = WKC;
            WhiteQueenCastle = WQC;
            BlackKingCastle = BKC;
            BlackQueenCastle = BQC;
            if (TimeControl.ElapsedMilliseconds >= TIME_LIMIT)
            {
                return (0, false);

            }
            Board = (int[])CB.Clone();
            //Array.Copy()
            HL = new List<int>(HashList);
            MakeMove(MoveList[i], Board, HL, ref hash);



            if (WKC && ((Board[60] != (Piece.White | Piece.King)) || (Board[63] != (Piece.White | Piece.Rook))))
            {
                WhiteKingCastle = false;
            }
            if (WQC && ((Board[60] != (Piece.White | Piece.King)) || (Board[56] != (Piece.White | Piece.Rook))))
            {
                WhiteQueenCastle = false;
            }
            EvaluateBoard(Board, HL, WKC, WQC, BKC, BQC);
            (eval, isChildSearchFinished) = quiescenceMin(depth - 1, Board, HL, WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, MoveList[i], alpha, beta);
            if (!isChildSearchFinished)
            {
                return (0, false);
            }
            if (eval < -90000000)
            {
                eval += 1;
            }
            if (eval >= beta)
            {

                return (beta, true);
            }

            if (eval > alpha)
            {
                alpha = eval;
                //BestMove = MoveList[i];
            }
        }
        if (howmuch == 0)
            return (eval, true);
        return (alpha, true);
        int CompareMoves(Move move1, Move move2)
        {
            if (move1.To == lMove.To)
            {
                if (move2.To == lMove.To)
                    return 0; // Both moves capture the lastly moved piece.
                else
                    return -1; // move1 captures the lastly moved piece.
            }
            else if (move2.To == lMove.To)
            {
                return 1; // move2 captures the lastly moved piece.
            }
            if (IsCapture(move1) && IsCapture(move2))
            {
                //Debug.Log(move1.From + ", " + move1.To + "," + move1.Type);
                //Debug.Log(move2.From + ", " + move2.To + "," + move2.Type);
                int value1 = Get_MVVLVA_Value(move1, CB);
                int value2 = Get_MVVLVA_Value(move2, CB);
                return value2.CompareTo(value1); // Sort in descending order
            }
            else if (IsCapture(move1))
            {
                return -1; // move1 is a capture, move2 is not
            }
            else if (IsCapture(move2))
            {
                return 1; // move2 is a capture, move1 is not
            }
            else
            {
                return 0; // Both moves are non-captures; maintain their order
            }
        }
    }
    (int, bool) quiescenceMin(int depth, int[] CB, List<int> HashList, bool WKC, bool WQC, bool BKC, bool BQC, Move lMove, int alpha, int beta)
    {
        UInt64 hash = 0;
        QmoveCount++;
        int eval = EvaluateBoard(CB, HashList, WKC, WQC, BKC, BQC);

        if (eval <= alpha)
            return (alpha, true);

        if (eval < beta)
            alpha = eval;

        int[] Board = (int[])CB.Clone();
        //Array.Copy()
        List<int> HL = new List<int>(HashList);
        List<Move> MoveList = new();
        bool Check;
        bool isCheckingMove;
        int Col = Piece.Black;
        Move BestMove = new Move(0, 0, 0);

        bool isChildSearchFinished;
        //int len = PVLine.Count();
        (MoveList, Check, isCheckingMove) = GenerateLegalMoves(WKC, WQC, BKC, BQC, CB, HashList, Col, false, false, lMove);
        if (depth != 1)
        {
            MoveList.Sort(CompareMoves);
        }
        int howmuch = 0;
        for (int i = 0; i < MoveList.Count; i++)
        {
            if (!IsCapture(MoveList[i])) continue;
            //Debug.Log(ValidityCheck(CB, HashList));
            //Debug.Log(MoveList[i].From + "," + MoveList[i].To + "," + MoveList[i].Type);
            //if (!ValidityCheck(CB, HashList)) Debug.Log("fcked up");
            if (StaticExchangeEvaluation(CB, HashList, MoveList[i]) > 0) continue;
            //Debug.Log(MoveList[i].From + "," + MoveList[i].To + "," + MoveList[i].Type);
            howmuch++;
            WhiteKingCastle = WKC;
            WhiteQueenCastle = WQC;
            BlackKingCastle = BKC;
            BlackQueenCastle = BQC;
            if (TimeControl.ElapsedMilliseconds >= TIME_LIMIT)
            {
                return (0, false);

            }
            Board = (int[])CB.Clone();
            //Array.Copy()
            HL = new List<int>(HashList);
            MakeMove(MoveList[i], Board, HL, ref hash);
            //EvaluateBoard(CB, HashList, WKC, WQC, BKC, BQC);

            //try
            //{
            //    EvaluateBoard(Board, HL, WKC, WQC, BKC, BQC);
            //}
            //catch(Exception ex)
            //{
            //    Debug.Log(ex+ "failed");

            //}


            if (BKC && (Board[4] != (Piece.Black | Piece.King)) || (Board[7] != (Piece.Black | Piece.Rook)))
            {
                BlackKingCastle = false;

            }

            if (BQC && (Board[4] != (Piece.Black | Piece.King)) || (Board[0] != (Piece.Black | Piece.Rook)))
            {
                BlackQueenCastle = false;
            }

            //EvaluateBoard(Board, HL, WKC, WQC, BKC, BQC);
            (eval, isChildSearchFinished) = quiescenceMax(depth - 1, Board, HL, WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, MoveList[i], alpha, beta);
            if (!isChildSearchFinished)
            {
                return (0, false);
            }
            if (eval > 90000000)
            {
                //Score += (CurrDepth - depth);
                eval -= 1;
            }
            if (eval <= alpha)
            {

                return (alpha, true);
            }


            if (eval < beta)
            {
                beta = eval;
                //BestMove = MoveList[i];
            }
        }
        if (howmuch == 0)
            return (eval, true);
        return (beta, true);
        int CompareMoves(Move move1, Move move2)
        {
            if (move1.To == lMove.To)
            {
                if (move2.To == lMove.To)
                    return 0; // Both moves capture the lastly moved piece.
                else
                    return -1; // move1 captures the lastly moved piece.
            }
            else if (move2.To == lMove.To)
            {
                return 1; // move2 captures the lastly moved piece.
            }
            if (IsCapture(move1) && IsCapture(move2))
            {
                //Debug.Log(move1.From + ", " + move1.To + "," + move1.Type);
                //Debug.Log(move2.From + ", " + move2.To + "," + move2.Type);
                int value1 = Get_MVVLVA_Value(move1, CB);
                int value2 = Get_MVVLVA_Value(move2, CB);
                return value2.CompareTo(value1); // Sort in descending order
            }
            else if (IsCapture(move1))
            {
                return -1; // move1 is a capture, move2 is not
            }
            else if (IsCapture(move2))
            {
                return 1; // move2 is a capture, move1 is not
            }
            else
            {
                return 0; // Both moves are non-captures; maintain their order
            }
        }
    }
    int PerfTest(int depth, int[] CB, List<int> HashList, bool WKC, bool WQC, bool BKC, bool BQC, Move lMove, int Col, UInt64 h, Move lmove)
    {
        int[] Board;
        List<int> HL;
        List<Move> MoveList = new();
        int nodes = 0;
        if (depth == 0)
            return 1;
        bool WhiteKingCastle = WKC;
        bool WhiteQueenCastle = WQC;
        bool BlackKingCastle = BKC;
        bool BlackQueenCastle = BQC;
        UInt64 Hash = h;
        //int[] CBClone = (int[])CB.Clone();
        //List<int> Hash = new List<int>(HashList);
        //Move LM = lMove;

        //int[] CBCloneTest = (int[])CB.Clone();
        //List<int> HashTest = new List<int>(HashList);
        //*************************이거봐라 이거 generatelegalmoves 넘기는거 다 복사해서 메모리 주소 분리해놓고 넘겨야함,
        (MoveList, _, _) = GenerateLegalMoves(WKC, WQC, BKC, BQC, CB, HashList, Col, false, false, lmove);
        if (MoveList.Count == 0)
        {
            return 0;
        }
        if (depth == 1) return MoveList.Count;
        //Debug.Log((CBClone.SequenceEqual(CBCloneTest)) +","+ Hash.SequenceEqual(HashTest));
        for (int i = 0; i < MoveList.Count; i++)
        {
            Board = (int[])CB.Clone();
            //Array.Copy()
            HL = new List<int>(HashList);
            //if (depth == 1)
            //{
            //    Debug.Log(lMove.From + "to" + lMove.To + ", " + MoveList[i].From + "to" + MoveList[i].To + "col" + ValueToCol(CB[MoveList[i].From]) + "ParaCol" + Col + "CB" + CB[MoveList[i].From]);
            //    for(int j = 0; j < CB.Length; j++)
            //    {
            //        Debug.Log(j + ":"+CB[j]);
            //    }
            //}



            //if (depth == 1) Debug.Log(lMove.From + "to" + lMove.To + ", " + MoveList[i].From + "to" + MoveList[i].To + "col" + ValueToCol(CB[MoveList[i].From]) + "ParaCol" + Col);
            //int[] TempCB = CB;
            MakeMove(MoveList[i], Board, HL, ref Hash);

            if (ValueToCol(Board[MoveList[i].From]) == Piece.White)
            {
                if (WKC && ((Board[60] != (Piece.White | Piece.King)) || (Board[63] != (Piece.White | Piece.Rook))))
                {
                    WhiteKingCastle = false;
                }

                if (WQC && ((Board[60] != (Piece.White | Piece.King)) || (Board[56] != (Piece.White | Piece.Rook))))
                {
                    WhiteQueenCastle = false;
                }

            }
            else
            {
                if (BKC && (Board[4] != (Piece.White | Piece.King)) || (Board[7] != (Piece.White | Piece.Rook)))
                {
                    BlackKingCastle = false;
                }

                if (BQC && (Board[4] != (Piece.White | Piece.King)) || (Board[0] != (Piece.White | Piece.Rook)))
                {
                    BlackQueenCastle = false;
                }

            }
            int ThisNode = PerfTest(depth - 1, Board, HL, WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle, MoveList[i], 24 - Col, Hash, MoveList[i]);
            //if (depth == perftDepth)
            //{
            //    Debug.Log(NumberToString(MoveList[i].From) + NumberToString(MoveList[i].To) + ":" + ThisNode);
            //}

            nodes += ThisNode;




        }
        return nodes;
    }
    //public void CancelPromotion()
    //{
    //    IsPromoting = false;
    //    Promotion.SetActive(false);
    //}
    //public void PromoteSet(int piece)
    //{
    //    //DoHashChangeThings();
    //    Debug.Log(piece);
    //    IsPromoting = false;
    //    Promotion.SetActive(false);
    //    int PromoMoveType = 0;
    //    if (piece == Piece.Queen)
    //    {
    //        PromoMoveType = QueenPromo;
    //    }
    //    else if (piece == Piece.Rook)
    //    {
    //        PromoMoveType = RookPromo;
    //    }
    //    else if (piece == Piece.Bishop)
    //    {
    //        PromoMoveType = BishopPromo;
    //    }
    //    else if (piece == Piece.Knight)
    //    {
    //        PromoMoveType = knightPromo;
    //    }
    //    if (chessBoard[PawnPromoLocation] != 0)
    //    {
    //        PromoMoveType |= MoveType.capture;
    //    }



    //    for (int j = 0; j < Locations.Count; j++)
    //    {
    //        if (Locations[j] == PawnPromoLocation)
    //        {
    //            if (PossibleMovements[MoveIndexLists[j]].Type == PromoMoveType)
    //            {
    //                //Debug.Log("promo");
    //                MakeMove(new Move(PossibleMovements[MoveIndexLists[j]].From, PossibleMovements[MoveIndexLists[j]].To, PromoMoveType), chessBoard, chessBoardHash, ref HashKey);
    //                RepresentBoard();
    //                //DoHashChangeThings();
    //                lastMove = new Move(PossibleMovements[MoveIndexLists[j]].From, PossibleMovements[MoveIndexLists[j]].To, PromoMoveType);
    //                HashKey ^= side_Keys;
    //                AI(Piece.Black);
    //                return;
    //            }
    //        }
    //    }

    //}
    void MakeMove(Move move, int[] cb, List<int> hs, ref UInt64 Hash)
    {

        //int[] AppliedBoard = (int[])cb.Clone();
        //List<int> AppliedHash = new List<int>(hs);

        int from = move.From;

        int to = move.To;
        int moveType = move.Type;

        int whatPiece = cb[from];
        int WhatColor = ValueToCol(whatPiece);

        //Debug.Log(from + "to" + to+ "movetype" + moveType);
        //for(int i= 0; i < AppliedHash.Count; i++)
        //{
        //    Debug.Log("hash" + AppliedHash[i]);
        //}
        //Debug.Log("moveType" + moveType);
        //Debug.Log("from" + from + "to" + to);
        if (moveType == 0 || moveType == MoveType.Special00) // quiet move & double pawn push
        {
            cb[from] = 0;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), from];

            cb[to] = whatPiece;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), to];

            hs[hs.IndexOf(from)] = to;


        }
        else if ((moveType & MoveType.promotion) != 0) // promotion
        {

            //Debug.Log("moveType" + moveType);
            cb[from] = 0;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), from];
            if ((moveType & MoveType.capture) != 0)//capturePromo
            {
                //Debug.Log("capturepromom");
                hs.RemoveAt(hs.IndexOf(from));
                Hash ^= piece_Keys[Get_HashIndex(cb[to]), to];


            }
            else
            {
                hs[hs.IndexOf(from)] = to;

            }


            if ((moveType & QueenPromo) == QueenPromo)
            {
                cb[to] = WhatColor | Piece.Queen;
                Hash ^= piece_Keys[Get_HashIndex(WhatColor | Piece.Queen), to];
                //Debug.Log(ValueToPiece(AppliedBoard[to]));
            }
            else if ((moveType & RookPromo) == RookPromo)
            {
                cb[to] = WhatColor | Piece.Rook;
                Hash ^= piece_Keys[Get_HashIndex(WhatColor | Piece.Rook), to];
            }
            else if ((moveType & BishopPromo) == BishopPromo)
            {
                cb[to] = WhatColor | Piece.Bishop;
                Hash ^= piece_Keys[Get_HashIndex(WhatColor | Piece.Bishop), to];
            }
            else if ((moveType & knightPromo) == knightPromo)
            {
                cb[to] = WhatColor | Piece.Knight;
                Hash ^= piece_Keys[Get_HashIndex(WhatColor | Piece.Knight), to];
            }


        }
        else if (moveType == MoveType.capture)
        {
            cb[from] = 0;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), from];
            cb[to] = whatPiece;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), to];

            if (hs.IndexOf(from) == -1)
            {
                //Debug.Log("fucked up");
            }
            hs.RemoveAt(hs.IndexOf(from));
            //Debug.Log("deleted" + AppliedHash.IndexOf(from));
        }
        else if (moveType == (MoveType.capture | MoveType.Special00))
        {
            int Forward;
            if (WhatColor == Piece.White)
            {
                Forward = 8;
            }
            else
            {
                Forward = -8;
            }

            cb[from] = 0;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), from];
            cb[to + Forward] = 0;
            Hash ^= piece_Keys[Get_HashIndex(WhatColor | Piece.Pawn), to + Forward];
            cb[to] = whatPiece;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), to];

            hs.RemoveAt(hs.IndexOf(from));
            //Debug.Log(to - Forward);
            hs.RemoveAt(hs.IndexOf(to + Forward));
            hs.Add(to);
        }
        else if (moveType == MoveType.Special01) // Kingside Castling
        {
            cb[from] = 0;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), from];

            cb[to] = whatPiece;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), to];

            hs[hs.IndexOf(from)] = to;


            cb[from + 3] = 0;
            Hash ^= piece_Keys[Get_HashIndex(WhatColor | Piece.Rook), from + 3];

            cb[from + 1] = WhatColor | Piece.Rook;
            Hash ^= piece_Keys[Get_HashIndex(WhatColor | Piece.Rook), from + 1];
            hs[hs.IndexOf(from + 3)] = from + 1;
        }
        else if (moveType == (MoveType.Special00 | MoveType.Special01))
        {
            cb[from] = 0;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), from];

            cb[to] = whatPiece;
            Hash ^= piece_Keys[Get_HashIndex(whatPiece), to];

            hs[hs.IndexOf(from)] = to;

            cb[from - 4] = 0;
            Hash ^= piece_Keys[Get_HashIndex(WhatColor | Piece.Rook), from - 4];
            cb[from - 1] = WhatColor | Piece.Rook;
            Hash ^= piece_Keys[Get_HashIndex(WhatColor | Piece.Rook), from - 1];

            hs[hs.IndexOf(from - 4)] = from - 1;
        }


        //lastMove = move;

        //return (AppliedBoard, AppliedHash);
    }

    //void RepresentBoard()
    //{
    //    for (int i = 0; i < PieceList.Count; i++)
    //    {
    //        GameObject temp = PieceList[i];

    //        Destroy(temp);
    //    }
    //    PieceList.Clear();
    //    //Debug.Log(chessBoardHash.Count);
    //    for (int i = 0; i < chessBoardHash.Count; i++)
    //    {
    //        int Xpos = (int)xs[chessBoardHash[i]];
    //        int Ypos = (int)ys[chessBoardHash[i]];
    //        int WhatPiece = chessBoard[chessBoardHash[i]];
    //        Dictionary<int, int> PieceDic = new Dictionary<int, int>() { { Piece.White | Piece.King, 10 }, { Piece.White | Piece.Pawn, 0 }, { Piece.White | Piece.Knight, 4 }, { Piece.White | Piece.Bishop, 6 }, { Piece.White | Piece.Rook, 2 }, { Piece.White | Piece.Queen, 8 }, { Piece.Black | Piece.King, 11 }, { Piece.Black | Piece.Pawn, 1 }, { Piece.Black | Piece.Knight, 5 }, { Piece.Black | Piece.Bishop, 7 }, { Piece.Black | Piece.Rook, 3 }, { Piece.Black | Piece.Queen, 9 } };
    //        PieceList.Add((GameObject)Instantiate(Pieces[PieceDic[WhatPiece]], new Vector3(Xpos, Ypos, -1), Quaternion.identity, this.transform));
    //    }


    //}
    //void RepresentBoard(int[] CB, List<int> PL)
    //{
    //    for (int i = 0; i < PieceList.Count; i++)
    //    {
    //        GameObject temp = PieceList[i];

    //        Destroy(temp);
    //    }
    //    PieceList.Clear();
    //    //Debug.Log(chessBoardHash.Count);
    //    for (int i = 0; i < PL.Count; i++)
    //    {
    //        int Xpos = (int)xs[PL[i]];
    //        int Ypos = (int)ys[PL[i]];
    //        int WhatPiece = CB[PL[i]];
    //        Dictionary<int, int> PieceDic = new Dictionary<int, int>() { { Piece.White | Piece.King, 10 }, { Piece.White | Piece.Pawn, 0 }, { Piece.White | Piece.Knight, 4 }, { Piece.White | Piece.Bishop, 6 }, { Piece.White | Piece.Rook, 2 }, { Piece.White | Piece.Queen, 8 }, { Piece.Black | Piece.King, 11 }, { Piece.Black | Piece.Pawn, 1 }, { Piece.Black | Piece.Knight, 5 }, { Piece.Black | Piece.Bishop, 7 }, { Piece.Black | Piece.Rook, 3 }, { Piece.Black | Piece.Queen, 9 } };
    //        PieceList.Add((GameObject)Instantiate(Pieces[PieceDic[WhatPiece]], new Vector3(Xpos, Ypos, -1), Quaternion.identity, this.transform));
    //    }


    //}


    //void AlignPiece(Transform transform)
    //{
    //    float[] distances = new float[64];
    //    int minimumIndex = 0;
    //    float minimumValue = Vector2.Distance(new Vector2(xs[0], ys[0]), transform.position);
    //    for (int i = 0; i < 64; i++)
    //    {

    //        distances[i] = Vector2.Distance(new Vector2(xs[i], ys[i]), transform.position);
    //        if (distances[i] < minimumValue)
    //        {
    //            minimumValue = distances[i];
    //            minimumIndex = i;
    //        }
    //    }
    //    transform.position = new Vector3(xs[minimumIndex], ys[minimumIndex], -1);

    //    //if(chessBoard[minimumIndex%8,minimumIndex/(minimumIndex-(minimumIndex%8))]!=null)
    //    //{
    //    //    chessBoard[minimumIndex % 8, minimumIndex / (minimumIndex - (minimumIndex % 8))] = BlackBishop;
    //    //    Pieces[minimumIndex] = ((GameObject)Instantiate(, new Vector3(x, y, -1), Quaternion.identity, this.transform));
    //    //}
    //}
    (List<Move>, bool, bool) GenerateLegalMoves(bool WhiteKC, bool WhiteQC, bool BlackKC, bool BlackQC, int[] CB, List<int> HashList, int Col, bool isCapture, bool CheckIfCheck, Move lmove)
    {
        Move lastmove = lmove;
        List<int> AttackedSquare;
        List<int> AttackingPiece;
        int kingLoc = Array.IndexOf(CB, Col | Piece.King);
        int OppkingLoc = Array.IndexOf(CB, OppColor(Col) | Piece.King);
        //Debug.Log(kingLoc);
        CB[kingLoc] = 0;
        (AttackedSquare, AttackingPiece) = CalculateAttackedSquare(24 - Col, HashList, CB);
        CB[kingLoc] = Col | Piece.King;

        bool isCheck = AttackedSquare.Contains(kingLoc);
        bool IsChecking = false;
        List<int> Attackers = new();
        List<int> CheckLine = new();

        for (int i = 0; i < AttackedSquare.Count; i++)
        {
            if (AttackedSquare[i] == kingLoc)
            {
                Attackers.Add(AttackingPiece[i]);
            }
        }
        int AttackerNum = Attackers.Count;
        List<List<int>> PinRays = new();
        List<int> pinPiece = new();
        List<int> pinDir = new();
        for (int i = 0; i < HashList.Count; i++)
        {
            int Loc = HashList[i];
            int piece = ValueToPiece(CB[Loc]);
            //Debug.Log("step-1");
            if (ValueToCol(CB[Loc]) == Col || Array.IndexOf(SlidingPiece, piece) == -1) continue; //skips if it's same colored piece since we're looking for "opponent piece" which is pinning my piece. also skips non-sliding piece like knight or pawn.
            int XDiff = IndexToX[Loc] - IndexToX[kingLoc];
            int YDiff = IndexToY[Loc] - IndexToY[kingLoc];
            //Debug.Log("step0");
            if (Math.Abs(XDiff) == 1 && Math.Abs(YDiff) == 1) continue; //skips because if there's no space between two pieces there can't be pinned piece.
            //Debug.Log("step1");
            if ((Math.Abs(XDiff) != Math.Abs(YDiff)) && (XDiff != 0 && YDiff != 0)) continue; //skips if it's not diagonally or horizontal/vertically placed from the king.
            int dir;
            if ((XDiff == 0 || YDiff == 0)) //수직방향
            {
                if (XDiff == 0)
                {

                    if (YDiff < 0) // king 위쪽
                    {
                        dir = 2;
                    }
                    else //king 아래쪽
                    {
                        dir = 3;
                    }

                }
                else
                {
                    if (XDiff > 0) // king 오른쪽
                    {
                        dir = 0;
                    }
                    else //king 왼쪽
                    {
                        dir = 1;
                    }

                }
                //dir = 1; //straight
            }
            else//대각선방향
            {
                if (XDiff > 0)//오른쪽
                {
                    if (YDiff < 0)//위쪽
                    {
                        dir = 4;
                    }
                    else
                    {
                        dir = 5;
                    }
                }
                else
                {
                    if (YDiff < 0)//위쪽
                    {
                        dir = 6;
                    }
                    else
                    {
                        dir = 7;
                    }
                }
                //dir = 0;//diagonal
            }
            List<int> StraightPiece = new List<int> { Piece.Rook, Piece.Queen };
            List<int> DiagonalPiece = new List<int> { Piece.Bishop, Piece.Queen };
            if (dir <= 3 && !StraightPiece.Contains(piece)) continue; // if it can't pin
            if (dir >= 4 && !DiagonalPiece.Contains(piece)) continue;

            //what's left is an opponent sliding piece which is placed diagonally, horizontally, or vertically. there can be potentionally pinned piece between those pieces and the king.
            //Debug.Log("possiblypinning");
            int Distance = Math.Max(Math.Abs(XDiff), Math.Abs(YDiff));
            int xIncrease = XDiff / Distance;
            int yIncrease = YDiff / Distance;
            List<int> thisPieceRay = new();
            int estPinned = 0;
            bool isPinned = false;
            for (int j = 1; j < Distance + 1; j++)
            {
                int pos = kingLoc + (xIncrease * j) + (yIncrease * 8 * j);
                if (CB[pos] != 0)
                {
                    if (!isPinned)
                    {
                        if (ValueToCol(CB[pos]) == Col)
                        {

                            //Debug.Log("pinadd");
                            estPinned = pos;
                            isPinned = true;
                        }
                        else
                        {
                            isPinned = false;
                            break;
                        }
                    }
                    else
                    {
                        if (j != Distance)//if j is equal to distance + 1, it is piece that is pinning the piece. doesn't have to delete
                        {
                            //Debug.Log("pinbreak" + j);
                            isPinned = false;
                            break;
                        }
                    }

                }

                //int y = kingLoc + yIncrease * j;
                thisPieceRay.Add(pos);
            }
            if (isPinned)
            {
                PinRays.Add(thisPieceRay);
                pinPiece.Add(estPinned);
                pinDir.Add(dir);
                //Debug.Log(pinDir[0]);
            }



        }

        //Debug.Log(pinPiece.Count + "pinned");
        for (int i = 0; i < Attackers.Count; i++)
        {
            int CurrentAttacker = Attackers[i];
            //CheckLine.Add(kingLoc);
            if (!isCapture && Array.IndexOf(SlidingPiece, ValueToPiece(CB[CurrentAttacker])) != -1) // slidingpiece
            {
                int XDiff = IndexToX[CurrentAttacker] - IndexToX[kingLoc];
                int YDiff = IndexToY[CurrentAttacker] - IndexToY[kingLoc];

                int Distance = Math.Max(Math.Abs(XDiff), Math.Abs(YDiff));
                int xIncrease = XDiff / Distance;
                int yIncrease = YDiff / Distance;
                for (int j = 1; j < Distance + 1; j++)
                {
                    int pos = kingLoc + (xIncrease * j) + (yIncrease * 8 * j);
                    //int y = kingLoc + yIncrease * j;
                    CheckLine.Add(pos);
                }
            }
            else
            {
                CheckLine.Add(Attackers[i]);
                //Debug.Log(Attackers[i]);
            }


        }
        //for(int i = 0; i < CheckLine.Count; i++)
        //{
        //    Debug.Log(CheckLine[i]);
        //}
        List<Move> possMoves_List = new();
        for (int i = 0; i < HashList.Count; i++)
        {
            int Color = ValueToCol(CB[HashList[i]]);

            if (Col != Color) continue;
            int OppColor = 24 - Color;
            int Type = ValueToPiece(CB[HashList[i]]);
            int from = HashList[i];


            int posX = IndexToX[from];
            int posY = IndexToY[from];
            int HowMuchLeft = posX;
            int HowMuchRight = 7 - posX;
            int HowMuchUp = posY;
            int HowMuchDown = 7 - posY;

            int HowMuchLeftTop = Math.Min(HowMuchLeft, HowMuchUp);
            int HowMuchRightTop = Math.Min(HowMuchRight, HowMuchUp);
            int HowMuchLeftDown = Math.Min(HowMuchLeft, HowMuchDown);
            int HowMuchRightDown = Math.Min(HowMuchRight, HowMuchDown);

            int pinIndex = pinPiece.IndexOf(HashList[i]);

            int pinnedPiece = 0;
            bool isPinned = false;
            List<int> pinRay = new();
            int pinDirection = 0;
            if (pinIndex != -1)//this piece is pinned
            {
                isPinned = true;
                pinnedPiece = pinPiece[pinIndex];
                pinRay = new List<int>(PinRays[pinIndex]);
                pinDirection = pinDir[pinIndex];
            }
            //Debug.Log(isPinned);
            if (Type == Piece.King)
            {
                List<int> kingMoves = new List<int>() { 8, -8, 1, -1, 7, 9, -7, -9 };
                List<int> Distance = new List<int>() { HowMuchDown, HowMuchUp, HowMuchRight, HowMuchLeft, HowMuchLeftDown, HowMuchRightDown, HowMuchRightTop, HowMuchLeftTop };
                for (int j = 0; j < kingMoves.Count; j++)
                {
                    if (Distance[j] > 0 && ValueToCol(CB[from + kingMoves[j]]) != Color)
                    {
                        if (AttackerNum > 0) // single or double check
                        {
                            if (!AttackedSquare.Contains(from + kingMoves[j]))
                            {
                                if (isCapture)
                                {
                                    if (ValueToCol(CB[from + kingMoves[j]]) != Color && CB[from + kingMoves[j]] != 0)
                                    {
                                        possMoves_List.Add(new Move(from, from + kingMoves[j], MoveType.capture));
                                    }
                                }
                                else
                                {
                                    if (ValueToCol(CB[from + kingMoves[j]]) != Color && CB[from + kingMoves[j]] != 0)
                                    {
                                        possMoves_List.Add(new Move(from, from + kingMoves[j], MoveType.capture));
                                    }
                                    else
                                    {
                                        possMoves_List.Add(new Move(from, from + kingMoves[j], 0));
                                    }
                                }

                            }

                        }
                        else
                        {
                            if (!AttackedSquare.Contains(from + kingMoves[j]))
                            {
                                if (isCapture)
                                {
                                    if (ValueToCol(CB[from + kingMoves[j]]) != Color && CB[from + kingMoves[j]] != 0)
                                    {
                                        possMoves_List.Add(new Move(from, from + kingMoves[j], MoveType.capture));
                                    }
                                }
                                else
                                {
                                    if (ValueToCol(CB[from + kingMoves[j]]) != Color && CB[from + kingMoves[j]] != 0)
                                    {
                                        possMoves_List.Add(new Move(from, from + kingMoves[j], MoveType.capture));
                                    }
                                    else
                                    {
                                        possMoves_List.Add(new Move(from, from + kingMoves[j], 0));
                                    }
                                }

                            }

                        }

                    }
                }
                // Add castling moves
                if (AttackerNum == 0)
                {
                    if (Color == Piece.White)
                    {
                        // White king-side castling
                        if (CB[60] == (Piece.White | Piece.King) && CB[63] == (Piece.White | Piece.Rook) && CB[61] == 0 && CB[62] == 0)
                        {
                            if (!AttackedSquare.Contains(61) && !AttackedSquare.Contains(62))
                            {
                                Move kingSideCastle = new Move(60, 62, MoveType.Special01);
                                possMoves_List.Add(kingSideCastle);
                            }
                        }
                        // White queen-side castling
                        if (CB[60] == (Piece.White | Piece.King) && CB[56] == (Piece.White | Piece.Rook) && CB[57] == 0 && CB[58] == 0 && CB[59] == 0)
                        {
                            if (!AttackedSquare.Contains(58) && !AttackedSquare.Contains(59))
                            {
                                Move queenSideCastle = new Move(60, 58, MoveType.Special00 | MoveType.Special01);
                                possMoves_List.Add(queenSideCastle);
                            }

                        }
                    }
                    else
                    {
                        // Black king-side castling
                        if (CB[4] == (Piece.Black | Piece.King) && CB[7] == (Piece.Black | Piece.Rook) && CB[5] == 0 && CB[6] == 0)
                        {
                            if (!AttackedSquare.Contains(5) && !AttackedSquare.Contains(6))
                            {
                                Move kingSideCastle = new Move(4, 6, MoveType.Special01);
                                possMoves_List.Add(kingSideCastle);
                            }

                        }
                        // Black queen-side castling
                        if (CB[4] == (Piece.Black | Piece.King) && CB[0] == (Piece.Black | Piece.Rook) && CB[1] == 0 && CB[2] == 0 && CB[3] == 0)
                        {
                            if (!AttackedSquare.Contains(2) && !AttackedSquare.Contains(3))
                            {
                                Move queenSideCastle = new Move(4, 2, MoveType.Special00 | MoveType.Special01);
                                possMoves_List.Add(queenSideCastle);
                            }

                        }
                    }
                }




            }
            if (AttackerNum != 2)
            {
                if (Type == Piece.Knight)
                {
                    //Debug.Log("n");
                    if (!isPinned)
                    {
                        for (int j = 0; j < KnightPreCalc[from].Count; j++)
                        {
                            Move move = new Move(from, KnightPreCalc[from][j], 0);
                            if (ValueToCol(CB[move.To]) != Color)// if target square is empty or can be captuerd --if it is empty valuetocol function returns 0--
                            {
                                if (AttackerNum == 1)//single check
                                {
                                    if (CheckLine.Contains(move.To))
                                    {
                                        if (CB[move.To] != 0)
                                        {
                                            move.Type = MoveType.capture;
                                        }
                                        possMoves_List.Add(move);
                                    }
                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    //Debug.Log("nocheck");
                                    if (isCapture)
                                    {
                                        if (CB[move.To] != 0)
                                        {
                                            move.Type = MoveType.capture;
                                            possMoves_List.Add(move);
                                        }

                                    }
                                    else
                                    {
                                        if (CB[move.To] != 0)
                                        {
                                            move.Type = MoveType.capture;
                                        }
                                        possMoves_List.Add(move);
                                    }

                                }
                            }

                        }
                    }


                }
                else if (Type == Piece.Pawn)
                {
                    //Debug.Log("p");
                    int forward;
                    int DoublePush;
                    int Promotion;
                    int LeftAttack;
                    int RightAttack;
                    int LeftFile = 0;
                    int RightFile = 7;
                    int enRank;
                    int Left = -1;
                    int Right = 1;
                    if (Color == Piece.White)
                    {
                        forward = -8;
                        DoublePush = 6;
                        Promotion = 0;
                        LeftAttack = -9;
                        RightAttack = -7;
                        enRank = 3;

                    }
                    else
                    {
                        forward = 8;
                        DoublePush = 1;
                        Promotion = 7;
                        LeftAttack = 7;
                        RightAttack = 9;
                        enRank = 4;

                    }




                    Move move;
                    //Debug.Log(isPinned);
                    if (!isCapture)
                    {
                        if (CB[from + forward] == 0)
                        {
                            if ((!isPinned) || (pinDirection <= 3 && pinRay.Contains(from + forward)))
                            {

                                if (IndexToY[from + forward] == Promotion) //add forward promotion movement
                                {
                                    if (AttackerNum == 1)//single check
                                    {
                                        if (CheckLine.Contains(from + forward))
                                        {
                                            possMoves_List.Add(new Move(from, from + forward, knightPromo)); // knight promo
                                            possMoves_List.Add(new Move(from, from + forward, BishopPromo)); // bishop promo
                                            possMoves_List.Add(new Move(from, from + forward, RookPromo)); // rook promo
                                            possMoves_List.Add(new Move(from, from + forward, QueenPromo)); // queen promo
                                        }
                                    }
                                    else if (AttackerNum == 0) // no check
                                    {
                                        possMoves_List.Add(new Move(from, from + forward, knightPromo)); // knight promo
                                        possMoves_List.Add(new Move(from, from + forward, BishopPromo)); // bishop promo
                                        possMoves_List.Add(new Move(from, from + forward, RookPromo)); // rook promo
                                        possMoves_List.Add(new Move(from, from + forward, QueenPromo)); // queen promo
                                    }

                                }
                                else //add basic one move forward
                                {
                                    if (AttackerNum == 1)//single check
                                    {
                                        if (CheckLine.Contains(from + forward))
                                        {
                                            //Debug.Log(from + "pawn");
                                            move = new Move(from, from + forward, 0);
                                            possMoves_List.Add(move);
                                        }
                                    }
                                    else if (AttackerNum == 0) // no check
                                    {
                                        //Debug.Log("nocheck");
                                        //Debug.Log(from + "pawn");
                                        move = new Move(from, from + forward, 0);
                                        possMoves_List.Add(move);
                                    }

                                }
                            }


                        }
                    }
                    //add double pawn push
                    if (!isCapture)
                    {
                        if (IndexToY[from] == DoublePush)
                        {
                            if ((!isPinned) || (pinDirection <= 3 && pinRay.Contains(from + forward * 2)))
                            {
                                if (CB[from + forward] == 0 && CB[from + forward * 2] == 0)
                                {
                                    if (AttackerNum == 1)//single check
                                    {
                                        if (CheckLine.Contains(from + forward * 2))
                                        {
                                            move = new Move(from, from + forward * 2, MoveType.Special00);
                                            possMoves_List.Add(move);
                                        }
                                    }
                                    else if (AttackerNum == 0) // no check
                                    {
                                        move = new Move(from, from + forward * 2, MoveType.Special00);
                                        possMoves_List.Add(move);
                                    }

                                }
                            }
                        }
                    }



                    if (IndexToX[from] != LeftFile)//add left attack
                    {
                        //Debug.Log(isPinned);
                        if ((!isPinned) || (isPinned && pinDirection >= 4 && pinRay.Contains(from + LeftAttack)))
                        {
                            if ((CB[from + LeftAttack] != 0) && (ValueToCol(CB[from + LeftAttack]) != Color))
                            {
                                if (IndexToY[from + LeftAttack] == Promotion) //add capture promotion movement
                                {
                                    if (AttackerNum == 1)//single check
                                    {
                                        if (CheckLine.Contains(from + LeftAttack))
                                        {
                                            possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture | knightPromo)); // knight promo
                                            possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture | BishopPromo)); // bishop promo
                                            possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture | RookPromo)); // rook promo
                                            possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture | QueenPromo)); // queen promo
                                        }
                                    }
                                    else if (AttackerNum == 0) // no check
                                    {
                                        possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture | knightPromo)); // knight promo
                                        possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture | BishopPromo)); // bishop promo
                                        possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture | RookPromo)); // rook promo
                                        possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture | QueenPromo)); // queen promo
                                    }

                                }
                                else
                                {
                                    if (AttackerNum == 1)//single check
                                    {
                                        if (CheckLine.Contains(from + LeftAttack))
                                        {
                                            possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture));
                                        }
                                    }
                                    else if (AttackerNum == 0) // no check
                                    {
                                        possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture));
                                    }

                                }

                            }
                            if (IndexToY[from] == enRank)//CalculateEnpassent
                            {
                                //Debug.Log("enpassent" + lastMove.From + "," + lastMove.To + "," + lastMove.Type);
                                //Debug.Log("check" + (CB[from + Left] == (OppColor | Piece.Pawn)) + ","+ CB[from + Left] + "," + OppColor);
                                if (lastmove != null)
                                {
                                    if (CB[from + Left] == (OppColor | Piece.Pawn) && (lastmove.From == from + Left + forward * 2 && lastmove.To == from + Left && lastmove.Type == MoveType.Special00))//en passent left
                                    {
                                        if (AttackerNum == 1)//single check
                                        {
                                            if (CheckLine.Contains(from + Left))
                                            {
                                                possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture | MoveType.Special00));

                                            }
                                        }
                                        else if (AttackerNum == 0) // no check
                                        {
                                            bool enPoss = true;
                                            if (IndexToY[kingLoc] == IndexToY[from])
                                            {
                                                int[] CBCopy = new int[64];
                                                CBCopy = (int[])CB.Clone();
                                                CBCopy[from] = 0;
                                                CBCopy[from + Left] = 0;

                                                for (int j = 1; j < IndexToX[kingLoc] + 1; j++)//left Side
                                                {
                                                    if (CBCopy[kingLoc - j] != 0)
                                                    {
                                                        if (ValueToCol(CBCopy[kingLoc - j]) == Col)
                                                        {
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            if ((ValueToPiece(CBCopy[kingLoc - j]) == Piece.Rook) || (ValueToPiece(CBCopy[kingLoc - j]) == Piece.Queen))
                                                            {
                                                                enPoss = false;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (enPoss)
                                                {
                                                    for (int j = 1; j < 8 - IndexToX[kingLoc]; j++)//right Side
                                                    {
                                                        if (CBCopy[kingLoc + j] != 0)
                                                        {
                                                            if (ValueToCol(CBCopy[kingLoc + j]) == Col)
                                                            {
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                if ((ValueToPiece(CBCopy[kingLoc + j]) == Piece.Rook) || (ValueToPiece(CBCopy[kingLoc + j]) == Piece.Queen))
                                                                {
                                                                    enPoss = false;
                                                                }
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }


                                            if (enPoss)
                                            {
                                                possMoves_List.Add(new Move(from, from + LeftAttack, MoveType.capture | MoveType.Special00));
                                            }

                                        }
                                        //Debug.Log("enpassentAdd");
                                    }
                                }

                            }
                        }
                    }
                    if (IndexToX[from] != RightFile)//add right attack
                    {
                        if ((!isPinned) || (isPinned && pinDirection >= 4 && pinRay.Contains(from + RightAttack)))
                        {
                            if ((CB[from + RightAttack] != 0) && (ValueToCol(CB[from + RightAttack]) != Color))
                            {
                                if (IndexToY[from + RightAttack] == Promotion) //add capture promotion movement
                                {
                                    if (AttackerNum == 1)//single check
                                    {
                                        if (CheckLine.Contains(from + RightAttack))
                                        {
                                            possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture | knightPromo)); // knight promo
                                            possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture | BishopPromo)); // bishop promo
                                            possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture | RookPromo)); // rook promo
                                            possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture | QueenPromo)); // queen promo
                                        }
                                    }
                                    else if (AttackerNum == 0) // no check
                                    {
                                        possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture | knightPromo)); // knight promo
                                        possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture | BishopPromo)); // bishop promo
                                        possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture | RookPromo)); // rook promo
                                        possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture | QueenPromo)); // queen promo
                                    }

                                }
                                else
                                {
                                    if (AttackerNum == 1)//single check
                                    {
                                        if (CheckLine.Contains(from + RightAttack))
                                        {
                                            possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture));

                                        }
                                    }
                                    else if (AttackerNum == 0) // no check
                                    {
                                        possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture));
                                    }

                                }

                            }
                            if (IndexToY[from] == enRank)//CalculateEnpassent
                            {
                                if (lastmove != null)
                                {
                                    //Debug.Log(lastMove.Type == MoveType.Special00);
                                    if (CB[from + Right] == (OppColor | Piece.Pawn) && lastmove.From == from + Right + forward * 2 && lastmove.To == from + Right && lastmove.Type == MoveType.Special00)//en passent right
                                    {
                                        if (AttackerNum == 1)//single check
                                        {
                                            if (CheckLine.Contains(from + Right))
                                            {
                                                possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture | MoveType.Special00));

                                            }
                                        }
                                        else if (AttackerNum == 0) // no check
                                        {
                                            bool enPoss = true;
                                            if (IndexToY[kingLoc] == IndexToY[from])
                                            {
                                                int[] CBCopy = new int[64];
                                                CBCopy = (int[])CB.Clone();
                                                CBCopy[from] = 0;
                                                CBCopy[from + Right] = 0;

                                                for (int j = 1; j < IndexToX[kingLoc] + 1; j++)//left Side
                                                {
                                                    if (CBCopy[kingLoc - j] != 0)
                                                    {
                                                        if (ValueToCol(CBCopy[kingLoc - j]) == Col)
                                                        {
                                                            break;
                                                        }
                                                        else
                                                        {

                                                            if ((ValueToPiece(CBCopy[kingLoc - j]) == Piece.Rook) || (ValueToPiece(CBCopy[kingLoc - j]) == Piece.Queen))
                                                            {
                                                                enPoss = false;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (enPoss)
                                                {
                                                    for (int j = 1; j < 8 - IndexToX[kingLoc]; j++)//right Side
                                                    {
                                                        if (CBCopy[kingLoc + j] != 0)
                                                        {
                                                            if (ValueToCol(CBCopy[kingLoc + j]) == Col)
                                                            {
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                if ((ValueToPiece(CBCopy[kingLoc + j]) == Piece.Rook) || (ValueToPiece(CBCopy[kingLoc + j]) == Piece.Queen))
                                                                {
                                                                    enPoss = false;
                                                                }
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (enPoss)
                                            {
                                                possMoves_List.Add(new Move(from, from + RightAttack, MoveType.capture | MoveType.Special00));
                                            }

                                        }
                                        //Debug.Log("enpassentAdd");

                                    }
                                }


                            }
                        }
                    }



                }
                else if (Type == Piece.Rook)
                {
                    if (!isPinned || pinDirection <= 3)
                    {
                        if (!isPinned || (pinDirection == 1 || pinDirection == 0))
                        {
                            for (int j = 1; j < HowMuchLeft + 1; j++)
                            {

                                if (ValueToCol(CB[from - j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from - j))
                                    {
                                        if (CheckLine.Contains(from - j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from - j]) != Color && CB[from - j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j, MoveType.capture));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from - j]) != Color && CB[from - j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from - j, 0));
                                                }
                                            }

                                        }
                                    }
                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from - j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from - j]) != Color && CB[from - j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from - j]) != Color && CB[from - j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from - j, 0));
                                            }
                                        }
                                    }

                                }
                                if (ValueToCol(CB[from - j]) != Color && CB[from - j] != 0) break;


                            }
                            for (int j = 1; j < HowMuchRight + 1; j++)
                            {
                                if (ValueToCol(CB[from + j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from + j))
                                    {
                                        if (CheckLine.Contains(from + j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from + j]) != Color && CB[from + j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j, MoveType.capture));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from + j]) != Color && CB[from + j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from + j, 0));
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from + j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from + j]) != Color && CB[from + j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from + j]) != Color && CB[from + j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from + j, 0));
                                            }
                                        }
                                    }

                                }
                                if (ValueToCol(CB[from + j]) != Color && CB[from + j] != 0) break;

                            }
                        }



                        if (!isPinned || (pinDirection == 2 || pinDirection == 3))
                        {
                            for (int j = 1; j < HowMuchUp + 1; j++)
                            {
                                if (ValueToCol(CB[from - 8 * j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from - 8 * j))
                                    {
                                        if (CheckLine.Contains(from - 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from - 8 * j]) != Color && CB[from - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - 8 * j, MoveType.capture)); // 
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from - 8 * j]) != Color && CB[from - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - 8 * j, MoveType.capture)); // 
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from - 8 * j, 0)); // 
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from - 8 * j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from - 8 * j]) != Color && CB[from - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - 8 * j, MoveType.capture)); // 
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from - 8 * j]) != Color && CB[from - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - 8 * j, MoveType.capture)); // 
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from - 8 * j, 0)); // 
                                            }
                                        }
                                    }

                                }
                                if (ValueToCol(CB[from - 8 * j]) != Color && CB[from - 8 * j] != 0) break;
                            }
                            for (int j = 1; j < HowMuchDown + 1; j++)
                            {
                                if (ValueToCol(CB[from + 8 * j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from + 8 * j))
                                    {
                                        if (CheckLine.Contains(from + 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from + 8 * j]) != Color && CB[from + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + 8 * j, MoveType.capture));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from + 8 * j]) != Color && CB[from + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + 8 * j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from + 8 * j, 0));
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from + 8 * j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from + 8 * j]) != Color && CB[from + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + 8 * j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from + 8 * j]) != Color && CB[from + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + 8 * j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from + 8 * j, 0));
                                            }
                                        }
                                    }

                                }
                                if (ValueToCol(CB[from + 8 * j]) != Color && CB[from + 8 * j] != 0) break;
                            }


                        }


                    }

                }
                else if (Type == Piece.Bishop)
                {
                    if (!isPinned || pinDirection >= 4)
                    {
                        if (!isPinned || (pinDirection == 6 || pinDirection == 5))
                        {
                            for (int j = 1; j < HowMuchLeftTop + 1; j++)
                            {

                                if (ValueToCol(CB[from - j - 8 * j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from - j - 8 * j))
                                    {
                                        if (CheckLine.Contains(from - j - 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from - j - 8 * j]) != Color && CB[from - j - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j - 8 * j, MoveType.capture));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from - j - 8 * j]) != Color && CB[from - j - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j - 8 * j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from - j - 8 * j, 0));
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from - j - 8 * j))
                                    {

                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from - j - 8 * j]) != Color && CB[from - j - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j - 8 * j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from - j - 8 * j]) != Color && CB[from - j - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j - 8 * j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from - j - 8 * j, 0));
                                            }
                                        }

                                    }

                                }
                                if (ValueToCol(CB[from - j - 8 * j]) != Color && CB[from - j - 8 * j] != 0) break;
                            }
                            for (int j = 1; j < HowMuchRightDown + 1; j++)
                            {
                                if (ValueToCol(CB[from + j + 8 * j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from + j + 8 * j))
                                    {
                                        if (CheckLine.Contains(from + j + 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from + j + 8 * j]) != Color && CB[from + j + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j + 8 * j, MoveType.capture));
                                                    break;
                                                }

                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from + j + 8 * j]) != Color && CB[from + j + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j + 8 * j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from + j + 8 * j, 0));
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from + j + 8 * j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from + j + 8 * j]) != Color && CB[from + j + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j + 8 * j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from + j + 8 * j]) != Color && CB[from + j + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j + 8 * j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from + j + 8 * j, 0));
                                            }
                                        }


                                    }
                                }
                                if (ValueToCol(CB[from + j + 8 * j]) != Color && CB[from + j + 8 * j] != 0) break;
                            }
                        }

                        if (!isPinned || (pinDirection == 4 || pinDirection == 7))
                        {
                            for (int j = 1; j < HowMuchRightTop + 1; j++)
                            {
                                if (ValueToCol(CB[from + j - 8 * j]) == Color) break;

                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from + j - 8 * j))
                                    {
                                        if (CheckLine.Contains(from + j - 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from + j - 8 * j]) != Color && CB[from + j - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j - 8 * j, MoveType.capture));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from + j - 8 * j]) != Color && CB[from + j - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j - 8 * j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from + j - 8 * j, 0));
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from + j - 8 * j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from + j - 8 * j]) != Color && CB[from + j - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j - 8 * j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from + j - 8 * j]) != Color && CB[from + j - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j - 8 * j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from + j - 8 * j, 0));
                                            }
                                        }
                                    }
                                }
                                if (ValueToCol(CB[from + j - 8 * j]) != Color && CB[from + j - 8 * j] != 0) break;
                            }
                            for (int j = 1; j < HowMuchLeftDown + 1; j++)
                            {
                                if (ValueToCol(CB[from - j + 8 * j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from - j + 8 * j))
                                    {
                                        if (CheckLine.Contains(from - j + 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from - j + 8 * j]) != Color && CB[from - j + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j + 8 * j, MoveType.capture)); // 
                                                    break;
                                                }

                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from - j + 8 * j]) != Color && CB[from - j + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j + 8 * j, MoveType.capture)); // 
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from - j + 8 * j, 0)); // 
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from - j + 8 * j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from - j + 8 * j]) != Color && CB[from - j + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j + 8 * j, MoveType.capture)); // 
                                                break;
                                            }

                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from - j + 8 * j]) != Color && CB[from - j + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j + 8 * j, MoveType.capture)); // 
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from - j + 8 * j, 0)); // 
                                            }
                                        }
                                    }
                                }
                                if (ValueToCol(CB[from - j + 8 * j]) != Color && CB[from - j + 8 * j] != 0) break;
                            }
                        }



                    }

                }
                else if (Type == Piece.Queen)
                {
                    if (!isPinned || pinDirection <= 3)
                    {
                        if (!isPinned || (pinDirection == 1 || pinDirection == 0))
                        {
                            for (int j = 1; j < HowMuchLeft + 1; j++)
                            {

                                if (ValueToCol(CB[from - j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from - j))
                                    {
                                        if (CheckLine.Contains(from - j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from - j]) != Color && CB[from - j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j, MoveType.capture));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from - j]) != Color && CB[from - j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from - j, 0));
                                                }
                                            }

                                        }
                                    }
                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from - j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from - j]) != Color && CB[from - j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from - j]) != Color && CB[from - j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from - j, 0));
                                            }
                                        }
                                    }

                                }
                                if (ValueToCol(CB[from - j]) != Color && CB[from - j] != 0) break;


                            }
                            for (int j = 1; j < HowMuchRight + 1; j++)
                            {
                                if (ValueToCol(CB[from + j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from + j))
                                    {
                                        if (CheckLine.Contains(from + j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from + j]) != Color && CB[from + j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j, MoveType.capture));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from + j]) != Color && CB[from + j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from + j, 0));
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from + j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from + j]) != Color && CB[from + j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from + j]) != Color && CB[from + j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from + j, 0));
                                            }
                                        }
                                    }

                                }
                                if (ValueToCol(CB[from + j]) != Color && CB[from + j] != 0) break;

                            }
                        }



                        if (!isPinned || (pinDirection == 2 || pinDirection == 3))
                        {
                            for (int j = 1; j < HowMuchUp + 1; j++)
                            {
                                if (ValueToCol(CB[from - 8 * j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from - 8 * j))
                                    {
                                        if (CheckLine.Contains(from - 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from - 8 * j]) != Color && CB[from - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - 8 * j, MoveType.capture)); // 
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from - 8 * j]) != Color && CB[from - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - 8 * j, MoveType.capture)); // 
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from - 8 * j, 0)); // 
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from - 8 * j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from - 8 * j]) != Color && CB[from - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - 8 * j, MoveType.capture)); // 
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from - 8 * j]) != Color && CB[from - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - 8 * j, MoveType.capture)); // 
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from - 8 * j, 0)); // 
                                            }
                                        }
                                    }

                                }
                                if (ValueToCol(CB[from - 8 * j]) != Color && CB[from - 8 * j] != 0) break;
                            }
                            for (int j = 1; j < HowMuchDown + 1; j++)
                            {
                                if (ValueToCol(CB[from + 8 * j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from + 8 * j))
                                    {
                                        if (CheckLine.Contains(from + 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from + 8 * j]) != Color && CB[from + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + 8 * j, MoveType.capture));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from + 8 * j]) != Color && CB[from + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + 8 * j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from + 8 * j, 0));
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from + 8 * j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from + 8 * j]) != Color && CB[from + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + 8 * j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from + 8 * j]) != Color && CB[from + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + 8 * j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from + 8 * j, 0));
                                            }
                                        }
                                    }

                                }
                                if (ValueToCol(CB[from + 8 * j]) != Color && CB[from + 8 * j] != 0) break;
                            }


                        }


                    }
                    if (!isPinned || pinDirection >= 4)
                    {
                        if (!isPinned || (pinDirection == 6 || pinDirection == 5))
                        {
                            for (int j = 1; j < HowMuchLeftTop + 1; j++)
                            {

                                if (ValueToCol(CB[from - j - 8 * j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from - j - 8 * j))
                                    {
                                        if (CheckLine.Contains(from - j - 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from - j - 8 * j]) != Color && CB[from - j - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j - 8 * j, MoveType.capture));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from - j - 8 * j]) != Color && CB[from - j - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j - 8 * j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from - j - 8 * j, 0));
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from - j - 8 * j))
                                    {

                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from - j - 8 * j]) != Color && CB[from - j - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j - 8 * j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from - j - 8 * j]) != Color && CB[from - j - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j - 8 * j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from - j - 8 * j, 0));
                                            }
                                        }

                                    }

                                }
                                if (ValueToCol(CB[from - j - 8 * j]) != Color && CB[from - j - 8 * j] != 0) break;
                            }
                            for (int j = 1; j < HowMuchRightDown + 1; j++)
                            {
                                if (ValueToCol(CB[from + j + 8 * j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from + j + 8 * j))
                                    {
                                        if (CheckLine.Contains(from + j + 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from + j + 8 * j]) != Color && CB[from + j + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j + 8 * j, MoveType.capture));
                                                    break;
                                                }

                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from + j + 8 * j]) != Color && CB[from + j + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j + 8 * j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from + j + 8 * j, 0));
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from + j + 8 * j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from + j + 8 * j]) != Color && CB[from + j + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j + 8 * j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from + j + 8 * j]) != Color && CB[from + j + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j + 8 * j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from + j + 8 * j, 0));
                                            }
                                        }


                                    }
                                }
                                if (ValueToCol(CB[from + j + 8 * j]) != Color && CB[from + j + 8 * j] != 0) break;
                            }
                        }

                        if (!isPinned || (pinDirection == 4 || pinDirection == 7))
                        {
                            for (int j = 1; j < HowMuchRightTop + 1; j++)
                            {
                                if (ValueToCol(CB[from + j - 8 * j]) == Color) break;

                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from + j - 8 * j))
                                    {
                                        if (CheckLine.Contains(from + j - 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from + j - 8 * j]) != Color && CB[from + j - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j - 8 * j, MoveType.capture));
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from + j - 8 * j]) != Color && CB[from + j - 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from + j - 8 * j, MoveType.capture));
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from + j - 8 * j, 0));
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from + j - 8 * j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from + j - 8 * j]) != Color && CB[from + j - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j - 8 * j, MoveType.capture));
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from + j - 8 * j]) != Color && CB[from + j - 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from + j - 8 * j, MoveType.capture));
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from + j - 8 * j, 0));
                                            }
                                        }
                                    }
                                }
                                if (ValueToCol(CB[from + j - 8 * j]) != Color && CB[from + j - 8 * j] != 0) break;
                            }
                            for (int j = 1; j < HowMuchLeftDown + 1; j++)
                            {
                                if (ValueToCol(CB[from - j + 8 * j]) == Color) break;
                                if (AttackerNum == 1)//single check
                                {
                                    if (!isPinned || pinRay.Contains(from - j + 8 * j))
                                    {
                                        if (CheckLine.Contains(from - j + 8 * j))
                                        {
                                            if (isCapture)
                                            {
                                                if (ValueToCol(CB[from - j + 8 * j]) != Color && CB[from - j + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j + 8 * j, MoveType.capture)); // 
                                                    break;
                                                }

                                            }
                                            else
                                            {
                                                if (ValueToCol(CB[from - j + 8 * j]) != Color && CB[from - j + 8 * j] != 0)
                                                {
                                                    possMoves_List.Add(new Move(from, from - j + 8 * j, MoveType.capture)); // 
                                                    break;
                                                }
                                                else
                                                {
                                                    possMoves_List.Add(new Move(from, from - j + 8 * j, 0)); // 
                                                }
                                            }

                                        }
                                    }

                                }
                                else if (AttackerNum == 0) // no check
                                {
                                    if (!isPinned || pinRay.Contains(from - j + 8 * j))
                                    {
                                        if (isCapture)
                                        {
                                            if (ValueToCol(CB[from - j + 8 * j]) != Color && CB[from - j + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j + 8 * j, MoveType.capture)); // 
                                                break;
                                            }

                                        }
                                        else
                                        {
                                            if (ValueToCol(CB[from - j + 8 * j]) != Color && CB[from - j + 8 * j] != 0)
                                            {
                                                possMoves_List.Add(new Move(from, from - j + 8 * j, MoveType.capture)); // 
                                                break;
                                            }
                                            else
                                            {
                                                possMoves_List.Add(new Move(from, from - j + 8 * j, 0)); // 
                                            }
                                        }
                                    }
                                }
                                if (ValueToCol(CB[from - j + 8 * j]) != Color && CB[from - j + 8 * j] != 0) break;
                            }
                        }



                    }
                }
            }


        }
        if (CheckIfCheck)
        {
            //if (possMoves_List.Any(move => move.To == OppkingLoc))
            //{
            //    IsChecking = true;
            //}
        }




        return (possMoves_List, isCheck, IsChecking);





    }
    List<int> CalculateKingLines(int Color, int KingLoc, int[] CB)
    {
        List<int> possMoves_List = new();
        int posX = IndexToX[KingLoc];
        int posY = IndexToY[KingLoc];
        int HowMuchLeft = posX;
        int HowMuchRight = 7 - posX;
        int HowMuchUp = posY;
        int HowMuchDown = 7 - posY;

        int HowMuchLeftTop = Math.Min(HowMuchLeft, HowMuchUp);
        int HowMuchRightTop = Math.Min(HowMuchRight, HowMuchUp);
        int HowMuchLeftDown = Math.Min(HowMuchLeft, HowMuchDown);
        int HowMuchRightDown = Math.Min(HowMuchRight, HowMuchDown);
        for (int j = 1; j < HowMuchLeft + 1; j++)
        {
            if (ValueToCol(CB[KingLoc - j]) == Color) break;
            if (ValueToCol(CB[KingLoc - j]) != Color && CB[KingLoc - j] != 0)
            {
                possMoves_List.Add(KingLoc - j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc - j);
            }


        }
        for (int j = 1; j < HowMuchRight + 1; j++)
        {
            if (ValueToCol(CB[KingLoc + j]) == Color) break;


            if (ValueToCol(CB[KingLoc + j]) != Color && CB[KingLoc + j] != 0)
            {
                possMoves_List.Add(KingLoc + j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc + j);
            }
        }
        for (int j = 1; j < HowMuchUp + 1; j++)
        {
            if (ValueToCol(CB[KingLoc - 8 * j]) == Color) break;

            if (ValueToCol(CB[KingLoc - 8 * j]) != Color && CB[KingLoc - 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc - 8 * j); // 
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc - 8 * j); // 
            }
        }
        for (int j = 1; j < HowMuchDown + 1; j++)
        {
            if (ValueToCol(CB[KingLoc + 8 * j]) == Color) break;

            if (ValueToCol(CB[KingLoc + 8 * j]) != Color && CB[KingLoc + 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc + 8 * j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc + 8 * j);
            }
        }
        for (int j = 1; j < HowMuchLeftTop + 1; j++)
        {
            if (ValueToCol(CB[KingLoc - j - 8 * j]) == Color) break;
            if (ValueToCol(CB[KingLoc - j - 8 * j]) != Color && CB[KingLoc - j - 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc - j - 8 * j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc - j - 8 * j);
            }


        }
        for (int j = 1; j < HowMuchRightTop + 1; j++)
        {
            if (ValueToCol(CB[KingLoc + j - 8 * j]) == Color) break;


            if (ValueToCol(CB[KingLoc + j - 8 * j]) != Color && CB[KingLoc + j - 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc + j - 8 * j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc + j - 8 * j);
            }
        }
        for (int j = 1; j < HowMuchLeftDown + 1; j++)
        {
            if (ValueToCol(CB[KingLoc - j + 8 * j]) == Color) break;

            if (ValueToCol(CB[KingLoc - j + 8 * j]) != Color && CB[KingLoc - j + 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc - j + 8 * j); // 
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc - j + 8 * j); // 
            }
        }
        for (int j = 1; j < HowMuchRightDown + 1; j++)
        {
            if (ValueToCol(CB[KingLoc + j + 8 * j]) == Color) break;

            if (ValueToCol(CB[KingLoc + j + 8 * j]) != Color && CB[KingLoc + j + 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc + j + 8 * j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc + j + 8 * j);
            }
        }

        return possMoves_List;
    }

    List<int> CalculatePossibleLine(int Color, int KingLoc, int[] CB, bool isDiagonal)
    {
        List<int> possMoves_List = new();
        int posX = IndexToX[KingLoc];
        int posY = IndexToY[KingLoc];
        int HowMuchLeft = posX;
        int HowMuchRight = 7 - posX;
        int HowMuchUp = posY;
        int HowMuchDown = 7 - posY;

        int HowMuchLeftTop = Math.Min(HowMuchLeft, HowMuchUp);
        int HowMuchRightTop = Math.Min(HowMuchRight, HowMuchUp);
        int HowMuchLeftDown = Math.Min(HowMuchLeft, HowMuchDown);
        int HowMuchRightDown = Math.Min(HowMuchRight, HowMuchDown);

        if (isDiagonal)
        {
            HowMuchLeft = 0;
            HowMuchRight = 0;
            HowMuchUp = 0;
            HowMuchDown = 0;
        }
        else
        {
            HowMuchLeftTop = 0;
            HowMuchRightTop = 0;
            HowMuchLeftDown = 0;
            HowMuchRightDown = 0;
        }
        for (int j = 1; j < HowMuchLeft + 1; j++)
        {
            if (ValueToCol(CB[KingLoc - j]) == Color) break;
            if (ValueToCol(CB[KingLoc - j]) != Color && CB[KingLoc - j] != 0)
            {
                possMoves_List.Add(KingLoc - j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc - j);
            }


        }
        for (int j = 1; j < HowMuchRight + 1; j++)
        {
            if (ValueToCol(CB[KingLoc + j]) == Color) break;


            if (ValueToCol(CB[KingLoc + j]) != Color && CB[KingLoc + j] != 0)
            {
                possMoves_List.Add(KingLoc + j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc + j);
            }
        }
        for (int j = 1; j < HowMuchUp + 1; j++)
        {
            if (ValueToCol(CB[KingLoc - 8 * j]) == Color) break;

            if (ValueToCol(CB[KingLoc - 8 * j]) != Color && CB[KingLoc - 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc - 8 * j); // 
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc - 8 * j); // 
            }
        }
        for (int j = 1; j < HowMuchDown + 1; j++)
        {
            if (ValueToCol(CB[KingLoc + 8 * j]) == Color) break;

            if (ValueToCol(CB[KingLoc + 8 * j]) != Color && CB[KingLoc + 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc + 8 * j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc + 8 * j);
            }
        }
        for (int j = 1; j < HowMuchLeftTop + 1; j++)
        {
            if (ValueToCol(CB[KingLoc - j - 8 * j]) == Color) break;
            if (ValueToCol(CB[KingLoc - j - 8 * j]) != Color && CB[KingLoc - j - 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc - j - 8 * j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc - j - 8 * j);
            }


        }
        for (int j = 1; j < HowMuchRightTop + 1; j++)
        {
            if (ValueToCol(CB[KingLoc + j - 8 * j]) == Color) break;


            if (ValueToCol(CB[KingLoc + j - 8 * j]) != Color && CB[KingLoc + j - 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc + j - 8 * j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc + j - 8 * j);
            }
        }
        for (int j = 1; j < HowMuchLeftDown + 1; j++)
        {
            if (ValueToCol(CB[KingLoc - j + 8 * j]) == Color) break;

            if (ValueToCol(CB[KingLoc - j + 8 * j]) != Color && CB[KingLoc - j + 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc - j + 8 * j); // 
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc - j + 8 * j); // 
            }
        }
        for (int j = 1; j < HowMuchRightDown + 1; j++)
        {
            if (ValueToCol(CB[KingLoc + j + 8 * j]) == Color) break;

            if (ValueToCol(CB[KingLoc + j + 8 * j]) != Color && CB[KingLoc + j + 8 * j] != 0)
            {
                possMoves_List.Add(KingLoc + j + 8 * j);
                break;
            }
            else
            {
                possMoves_List.Add(KingLoc + j + 8 * j);
            }
        }

        return possMoves_List;
    }
    (List<int> AttackSquare, List<int> AttackPiece) CalculateAttackedSquare(int color, List<int> BoardPieces, int[] CB)
    {
        List<int> possMoves_List = new();
        List<int> WhichPiece = new();
        for (int i = 0; i < BoardPieces.Count; i++)
        {
            int Color = ValueToCol(CB[BoardPieces[i]]);
            if (Color != color) continue;
            int OppColor = 24 - Color;
            int Type = ValueToPiece(CB[BoardPieces[i]]);
            int from = BoardPieces[i];

            int posX = IndexToX[from];
            int posY = IndexToY[from];
            int HowMuchLeft = posX;
            int HowMuchRight = 7 - posX;
            int HowMuchUp = posY;
            int HowMuchDown = 7 - posY;

            int HowMuchLeftTop = Math.Min(HowMuchLeft, HowMuchUp);
            int HowMuchRightTop = Math.Min(HowMuchRight, HowMuchUp);
            int HowMuchLeftDown = Math.Min(HowMuchLeft, HowMuchDown);
            int HowMuchRightDown = Math.Min(HowMuchRight, HowMuchDown);


            if (Type == Piece.Knight)
            {
                //Debug.Log("n");
                for (int j = 0; j < KnightPreCalc[from].Count; j++)
                {
                    possMoves_List.Add(KnightPreCalc[from][j]);
                    WhichPiece.Add(BoardPieces[i]);

                }

            }
            else if (Type == Piece.Pawn)
            {
                //Debug.Log("p");
                int LeftAttack;
                int RightAttack;
                int LeftFile = 0;
                int RightFile = 7;

                if (Color == Piece.White)
                {
                    LeftAttack = -9;
                    RightAttack = -7;
                }
                else
                {
                    LeftAttack = 7;
                    RightAttack = 9;
                }






                if (IndexToX[from] != LeftFile)//add left attack
                {
                    possMoves_List.Add(from + LeftAttack);
                    WhichPiece.Add(BoardPieces[i]);
                }
                if (IndexToX[from] != RightFile)//add right attack
                {
                    possMoves_List.Add(from + RightAttack);
                    WhichPiece.Add(BoardPieces[i]);
                }



            }
            else if (Type == Piece.Rook)
            {

                for (int j = 1; j < HowMuchLeft + 1; j++)
                {

                    if (CB[from - j] != 0)
                    {
                        possMoves_List.Add(from - j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from - j);
                        WhichPiece.Add(BoardPieces[i]);
                    }


                }
                for (int j = 1; j < HowMuchRight + 1; j++)
                {
                    if (CB[from + j] != 0)
                    {
                        possMoves_List.Add(from + j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from + j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchUp + 1; j++)
                {
                    if (CB[from - 8 * j] != 0)
                    {
                        possMoves_List.Add(from - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchDown + 1; j++)
                {
                    if (CB[from + 8 * j] != 0)
                    {
                        possMoves_List.Add(from + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
            }
            else if (Type == Piece.Bishop)
            {
                for (int j = 1; j < HowMuchLeftTop + 1; j++)
                {
                    if (CB[from - j - 8 * j] != 0)
                    {
                        possMoves_List.Add(from - j - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from - j - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchRightTop + 1; j++)
                {
                    if (CB[from + j - 8 * j] != 0)
                    {
                        possMoves_List.Add(from + j - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from + j - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchLeftDown + 1; j++)
                {
                    if (CB[from - j + 8 * j] != 0)
                    {
                        possMoves_List.Add(from - j + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from - j + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchRightDown + 1; j++)
                {
                    if (CB[from + j + 8 * j] != 0)
                    {
                        possMoves_List.Add(from + j + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from + j + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
            }
            else if (Type == Piece.Queen)
            {

                for (int j = 1; j < HowMuchLeft + 1; j++)
                {

                    if (CB[from - j] != 0)
                    {
                        possMoves_List.Add(from - j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from - j);
                        WhichPiece.Add(BoardPieces[i]);
                    }


                }
                for (int j = 1; j < HowMuchRight + 1; j++)
                {
                    if (CB[from + j] != 0)
                    {
                        possMoves_List.Add(from + j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from + j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchUp + 1; j++)
                {
                    if (CB[from - 8 * j] != 0)
                    {
                        possMoves_List.Add(from - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchDown + 1; j++)
                {
                    if (CB[from + 8 * j] != 0)
                    {
                        possMoves_List.Add(from + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchLeftTop + 1; j++)
                {
                    if (CB[from - j - 8 * j] != 0)
                    {
                        possMoves_List.Add(from - j - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from - j - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchRightTop + 1; j++)
                {
                    if (CB[from + j - 8 * j] != 0)
                    {
                        possMoves_List.Add(from + j - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from + j - 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchLeftDown + 1; j++)
                {
                    if (CB[from - j + 8 * j] != 0)
                    {
                        possMoves_List.Add(from - j + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from - j + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
                for (int j = 1; j < HowMuchRightDown + 1; j++)
                {
                    if (CB[from + j + 8 * j] != 0)
                    {
                        possMoves_List.Add(from + j + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                        break;
                    }
                    else
                    {
                        possMoves_List.Add(from + j + 8 * j);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }
            }
            else if (Type == Piece.King)
            {
                List<int> kingMoves = new List<int>() { 8, -8, 1, -1, 7, 9, -7, -9 };
                List<int> Distance = new List<int>() { HowMuchDown, HowMuchUp, HowMuchRight, HowMuchLeft, HowMuchLeftDown, HowMuchRightDown, HowMuchRightTop, HowMuchLeftTop };
                for (int j = 0; j < kingMoves.Count; j++)
                {
                    if (Distance[j] > 0)
                    {
                        possMoves_List.Add(from + kingMoves[j]);
                        WhichPiece.Add(BoardPieces[i]);
                    }
                }

            }

        }




        return (possMoves_List, WhichPiece);
    }
    int GetTranspositionIndex(UInt64 Zobrish, int HashSize)
    {
        //Debug.Log(TranspositionTable.Length + "," + (int)(Zobrish % (UInt64)HashSize));

        //if (((int)Zobrish % HashSize )< 0 || ((int)Zobrish % HashSize) >= TranspositionTable.Length)
        //{
        //    // Log or handle the out-of-range index here
        //    Console.WriteLine("Index out of range: " + (int)Zobrish % HashSize + "length" + TranspositionTable.Length);
        //}
        return ((int)(Zobrish % (UInt64)HashSize));
    }
    UInt64 Generate_HashKey()
    {
        UInt64 final_key = 0;

        for (int i = 0; i < chessBoardHash.Count; i++)
        {

            final_key ^= piece_Keys[Get_HashIndex(chessBoard[chessBoardHash[i]]), chessBoardHash[i]];
        }

        final_key ^= castle_Keys[CastlingIndex(WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle)];
        return final_key;
    }

    int CastlingIndex(bool WKC, bool WQC, bool BKC, bool BQC)
    {
        int caseNumber = 0;
        caseNumber |= (WKC ? 1 : 0) << 0;
        caseNumber |= (WQC ? 1 : 0) << 1;
        caseNumber |= (BKC ? 1 : 0) << 2;
        caseNumber |= (BQC ? 1 : 0) << 3;
        return caseNumber;
    }
    int ValueToPiece(int num)
    {
        return num & 7;
    }
    int ValueToCol(int num)
    {
        return num & 24;
    }

    string PieceToString(int num)
    {
        if (num == 1)
        {
            return "King";
        }
        else if (num == 2)
        {
            return "Pawn";
        }
        else if (num == 3)
        {
            return "Knight";
        }
        else if (num == 4)
        {
            return "Bishop";
        }
        else if (num == 5)
        {
            return "Rook";
        }
        else if (num == 6)
        {
            return "Queen";
        }
        return "";
    }
    string ColToString(int num)
    {

        if (num == 8)
        {
            return "White";
        }
        else if (num == 16)
        {
            return "Black";
        }
        return "";
    }
    int OppColor(int col)
    {
        return 24 - col;
    }
    public static class Piece
    {
        public const int None = 0;
        public const int King = 1;
        public const int Pawn = 2;
        public const int Knight = 3;
        public const int Bishop = 4;
        public const int Rook = 5;
        public const int Queen = 6;

        public const int White = 8;
        public const int Black = 16;


    }
    public static class MoveType
    {
        public const int promotion = 8;
        public const int capture = 4;
        public const int Special01 = 2;
        public const int Special00 = 1;

    }
    class Move
    {

        public int From { get; set; }
        public int To { get; set; }
        public int Type { get; set; }


        public Move(int from, int to, int type)
        {
            From = from;
            To = to;
            Type = type;

        }
    }

    class TranspositionValue
    {
        public UInt64 Zobrish { get; set; }
        public int Eval { get; set; }
        public int Depth { get; set; }
        public int State { get; set; }

        //public PVLine EngineLine;
        public TranspositionValue(UInt64 zobrish, int eval, int depth, int state)
        {
            Zobrish = zobrish;
            Eval = eval;
            Depth = depth;
            //EngineLine = engineLine;
            State = state;
        }
    }
    class PVLine
    {

        public int Cmove { get; set; }
        public Move[] ArgMove { get; set; }



        public PVLine(int cmove, Move[] argmove)
        {
            Cmove = cmove;
            ArgMove = argmove;

        }
    }
    //class ThreeFoldValue
    //{
    //    public UInt64 Zobrish { get; set; }

    //    public int HowMuchRepetition { get; set; }


    //    public ThreeFoldValue(UInt64 zobrish, int howMuchRepetition)
    //    {
    //        Zobrish = zobrish;
    //        HowMuchRepetition = howMuchRepetition;
    //    }
    //}

    int[] FEN_ToList(string FEN)
    {
        Dictionary<char, int> PieceTypeFormat = new Dictionary<char, int>()
        {
            ['k'] = Piece.King,
            ['p'] = Piece.Pawn,
            ['n'] = Piece.Knight,
            ['b'] = Piece.Bishop,
            ['r'] = Piece.Rook,
            ['q'] = Piece.Queen
        };

        string[] FEN_EachLine = FEN.Split(' ')[0].Split('/');
        int[] chessBoard_FEN = new int[64];
        int index = 0;

        foreach (string line in FEN_EachLine)
        {
            int x = 0;
            foreach (char c in line)
            {
                if (char.IsDigit(c))
                {
                    x += (int)char.GetNumericValue(c);
                }
                else if (char.IsLetter(c))
                {
                    int color = char.IsUpper(c) ? Piece.White : Piece.Black;
                    int piece = PieceTypeFormat[char.ToLower(c)];

                    chessBoard_FEN[8 * index + x] = color | piece;
                    chessBoardHash.Add(8 * index + x);
                    x++;
                }
            }
            index++;
        }

        return chessBoard_FEN;
    }


    void PreCalculateKnightMovement()
    {
        KnightPreCalc.Add(new List<int> { 10, 17 });
        KnightPreCalc.Add(new List<int> { 11, 16, 18 });
        KnightPreCalc.Add(new List<int> { 8, 17, 19, 12 });
        KnightPreCalc.Add(new List<int> { 9, 18, 20, 13 }); //3
        KnightPreCalc.Add(new List<int> { 10, 19, 21, 14 });
        KnightPreCalc.Add(new List<int> { 11, 20, 22, 15 });
        KnightPreCalc.Add(new List<int> { 12, 21, 23 }); // 6
        KnightPreCalc.Add(new List<int> { 13, 22 });

        KnightPreCalc.Add(new List<int> { 2, 18, 25 });
        KnightPreCalc.Add(new List<int> { 3, 19, 26, 24 }); // 9
        KnightPreCalc.Add(new List<int> { 0, 4, 20, 27, 25, 16 });
        KnightPreCalc.Add(new List<int> { 1, 5, 21, 28, 26, 17 });
        KnightPreCalc.Add(new List<int> { 2, 6, 22, 29, 27, 18 });//12
        KnightPreCalc.Add(new List<int> { 3, 7, 23, 30, 28, 19 });
        KnightPreCalc.Add(new List<int> { 4, 20, 29, 31 });
        KnightPreCalc.Add(new List<int> { 5, 21, 30 });

        KnightPreCalc.Add(new List<int> { 1, 10, 26, 33 });
        KnightPreCalc.Add(new List<int> { 0, 2, 11, 27, 34, 32 });
        KnightPreCalc.Add(new List<int> { 1, 3, 12, 28, 35, 33, 24, 8 });
        KnightPreCalc.Add(new List<int> { 2, 4, 13, 29, 36, 34, 25, 9 });//19
        KnightPreCalc.Add(new List<int> { 3, 5, 14, 30, 37, 35, 26, 10 });
        KnightPreCalc.Add(new List<int> { 4, 6, 15, 31, 38, 36, 27, 11 });
        KnightPreCalc.Add(new List<int> { 5, 7, 39, 37, 28, 12 });
        KnightPreCalc.Add(new List<int> { 6, 13, 29, 38 });

        KnightPreCalc.Add(new List<int> { 9, 18, 34, 41 });
        KnightPreCalc.Add(new List<int> { 8, 10, 19, 35, 42, 40 });
        KnightPreCalc.Add(new List<int> { 9, 11, 20, 36, 43, 41, 32, 16 });
        KnightPreCalc.Add(new List<int> { 10, 12, 21, 37, 44, 42, 33, 17 });
        KnightPreCalc.Add(new List<int> { 11, 13, 22, 38, 45, 43, 34, 18 });
        KnightPreCalc.Add(new List<int> { 12, 14, 23, 39, 46, 44, 35, 19 });
        KnightPreCalc.Add(new List<int> { 13, 15, 47, 45, 36, 20 });
        KnightPreCalc.Add(new List<int> { 14, 21, 37, 46 });

        KnightPreCalc.Add(new List<int> { 17, 26, 42, 49 });
        KnightPreCalc.Add(new List<int> { 16, 18, 27, 43, 50, 48 });
        KnightPreCalc.Add(new List<int> { 17, 19, 28, 44, 51, 49, 40, 24 });
        KnightPreCalc.Add(new List<int> { 18, 20, 29, 45, 52, 50, 41, 25 });
        KnightPreCalc.Add(new List<int> { 19, 21, 30, 46, 53, 51, 42, 26 });
        KnightPreCalc.Add(new List<int> { 20, 22, 31, 47, 54, 52, 43, 27 });
        KnightPreCalc.Add(new List<int> { 21, 23, 55, 53, 44, 28 });
        KnightPreCalc.Add(new List<int> { 22, 29, 45, 54 });

        KnightPreCalc.Add(new List<int> { 25, 34, 50, 57 });
        KnightPreCalc.Add(new List<int> { 24, 26, 35, 51, 58, 56 });
        KnightPreCalc.Add(new List<int> { 25, 27, 36, 52, 59, 57, 48, 32 });
        KnightPreCalc.Add(new List<int> { 26, 28, 37, 53, 60, 58, 49, 33 });
        KnightPreCalc.Add(new List<int> { 27, 29, 38, 54, 61, 59, 50, 34 });
        KnightPreCalc.Add(new List<int> { 28, 30, 39, 55, 62, 60, 51, 35 });
        KnightPreCalc.Add(new List<int> { 29, 31, 63, 61, 52, 36 });
        KnightPreCalc.Add(new List<int> { 30, 37, 53, 62 });

        KnightPreCalc.Add(new List<int> { 33, 42, 58 });
        KnightPreCalc.Add(new List<int> { 32, 34, 43, 59 });
        KnightPreCalc.Add(new List<int> { 33, 35, 44, 60, 56, 40 });
        KnightPreCalc.Add(new List<int> { 34, 36, 45, 61, 57, 41 });
        KnightPreCalc.Add(new List<int> { 35, 37, 46, 62, 58, 42 });
        KnightPreCalc.Add(new List<int> { 36, 38, 47, 63, 59, 43 });
        KnightPreCalc.Add(new List<int> { 37, 39, 60, 44 });
        KnightPreCalc.Add(new List<int> { 38, 61, 45 });

        KnightPreCalc.Add(new List<int> { 41, 50 });
        KnightPreCalc.Add(new List<int> { 40, 42, 51 });
        KnightPreCalc.Add(new List<int> { 41, 43, 52, 48 });
        KnightPreCalc.Add(new List<int> { 42, 44, 53, 49 });
        KnightPreCalc.Add(new List<int> { 43, 45, 54, 50 });
        KnightPreCalc.Add(new List<int> { 44, 46, 55, 51 });
        KnightPreCalc.Add(new List<int> { 52, 45, 47 });
        KnightPreCalc.Add(new List<int> { 53, 46 });
    }
    void AddMVVLVA()
    {
        MVVLVA_T[0, 0] = 105;
        MVVLVA_T[1, 0] = 205;
        MVVLVA_T[2, 0] = 305;
        MVVLVA_T[3, 0] = 405;
        MVVLVA_T[4, 0] = 505;
        MVVLVA_T[5, 0] = 605;

        MVVLVA_T[0, 1] = 104;
        MVVLVA_T[1, 1] = 204;
        MVVLVA_T[2, 1] = 304;
        MVVLVA_T[3, 1] = 404;
        MVVLVA_T[4, 1] = 504;
        MVVLVA_T[5, 1] = 604;

        MVVLVA_T[0, 2] = 103;
        MVVLVA_T[1, 2] = 203;
        MVVLVA_T[2, 2] = 303;
        MVVLVA_T[3, 2] = 403;
        MVVLVA_T[4, 2] = 503;
        MVVLVA_T[5, 2] = 603;

        MVVLVA_T[0, 3] = 102;
        MVVLVA_T[1, 3] = 202;
        MVVLVA_T[2, 3] = 302;
        MVVLVA_T[3, 3] = 402;
        MVVLVA_T[4, 3] = 502;
        MVVLVA_T[5, 3] = 602;

        MVVLVA_T[0, 4] = 101;
        MVVLVA_T[1, 4] = 201;
        MVVLVA_T[2, 4] = 301;
        MVVLVA_T[3, 4] = 401;
        MVVLVA_T[4, 4] = 501;
        MVVLVA_T[5, 4] = 601;

        MVVLVA_T[0, 5] = 100;
        MVVLVA_T[1, 5] = 200;
        MVVLVA_T[2, 5] = 300;
        MVVLVA_T[3, 5] = 400;
        MVVLVA_T[4, 5] = 500;
        MVVLVA_T[5, 5] = 600;
    }
}
