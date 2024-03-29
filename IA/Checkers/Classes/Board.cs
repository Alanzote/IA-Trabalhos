﻿namespace IA.Checkers.Classes;

// O tabuleiro do nosso jogo de Damas.
public class Board {

    // O tabuleiro é representado por uma matriz 8x8 de peças.
    public Piece?[,] Representation { get; private set; } = new Piece?[8, 8];

    // Construtor para cópias.
    private Board() {
        // Cria a representação vazia do board.
        Representation = new Piece?[8, 8];
    }

    // Constructor.
    public Board(Player player0, Player player1) {
        // Criar a representação inicial do board.
        Representation = new Piece?[8, 8] {
            { null, new Piece(player0), null, null, null, new Piece(player1), null, new Piece(player1) },
            { new Piece(player0), null, new Piece(player0), null, null, null, new Piece(player1), null },
            { null, new Piece(player0), null, null, null, new Piece(player1), null, new Piece(player1) },
            { new Piece(player0), null, new Piece(player0), null, null, null, new Piece(player1), null },
            { null, new Piece(player0), null, null, null, new Piece(player1), null, new Piece(player1) },
            { new Piece(player0), null, new Piece(player0), null, null, null, new Piece(player1), null },
            { null, new Piece(player0), null, null, null, new Piece(player1), null, new Piece(player1) },
            { new Piece(player0), null, new Piece(player0), null, null, null, new Piece(player1), null },
        };
    }

    // Se uma posição está dentro do tabuleiro.
    public bool InBounds(int x, int y) {
        return x >= 0 && y >= 0 && x < 8 && y < 8;
    }

    // Retorna se um espaço está vazio.
    public bool IsEmpty(int x, int y) {
        // Valida se estamos dentro do tabuleiro.
        if (!InBounds(x, y))
            return false;

        // Valida se existe aluma peça lá.
        return Representation[x, y] == null;
    }

    // Encontra mais pulos.
    private void FindMoreJumps(Move current, int x, int y, Piece piece) {
        // Para cada direção...
        foreach (var Dir in Enum.GetValues<Direction>()) {
            // Ignoramos direções não válidas para aquele índice de jogador.
            if (!Dir.IsForPlayerIndex(piece.Player.Index) && !piece.IsKing)
                continue;

            // Calcula a nova posição.
            Tuple<int, int> NewPosition = DirectionExtensions.IncrementDirection(Tuple.Create(x, y), Dir);

            // Se estamos fora do tabuleiro, ou não tem nada aqui ou já matamos esse peão, ignoramos...
            if (!InBounds(NewPosition.Item1, NewPosition.Item2) || IsEmpty(NewPosition.Item1, NewPosition.Item2) || current.Jumps.Contains(NewPosition))
                continue;

            // Precisamos também validar se a peça que vamos tentar capturar não é nossa.
            if (Representation[NewPosition.Item1, NewPosition.Item2]!.Player == piece.Player)
                continue;

            // Calcula posição final do pulo.
            Tuple<int, int> FinalJump = DirectionExtensions.IncrementDirection(NewPosition, Dir);

            // Se o espaço está vazio, lucro!
            if (IsEmpty(FinalJump.Item1, FinalJump.Item2)) {
                // Adiciona a posição final atual como um Midpoint.
                current.Midpoints.Add(current.FinalPosition);

                // Alteramos a posição final.
                current.FinalPosition = FinalJump;

                // Adicionamos a nossa captura.
                current.Jumps.Add(NewPosition);

                // Tentamos encontrar mais pulos apartir da posição que estamos.
                FindMoreJumps(current, FinalJump.Item1, FinalJump.Item2, piece);
            }
        }
    }

    // Busca todos os movimentos possíveis de uma peça.
    public List<Move>? GetMoves(int x, int y) {
        // Buscamos a peça.
        Piece? CurrentPiece = Representation[x, y];

        // Se não encontramos, ignora ela.
        if (CurrentPiece == null)
            return null;

        // Cria a lista de resultados.
        List<Move> Results = new List<Move>();

        // Para cada direção...
        foreach (var Dir in Enum.GetValues<Direction>()) {
            // Ignoramos direções não válidas para aquele índice de jogador.
            if (!Dir.IsForPlayerIndex(CurrentPiece.Player.Index) && !CurrentPiece.IsKing)
                continue;

            // Calcula a nova posição.
            Tuple<int, int> NewPosition = DirectionExtensions.IncrementDirection(Tuple.Create(x, y), Dir);

            // Temos certeza que está dentro do tabuleiro.
            if (!InBounds(NewPosition.Item1, NewPosition.Item2))
                continue;

            // Validamos se está vazia a posição.
            if (IsEmpty(NewPosition.Item1, NewPosition.Item2)) {
                // Se já temos resultados com capturas, ignora esse movimento.
                if (Results.Any(x => x.Jumps.Any()))
                    continue;

                // Adicionar o movimento.
                Results.Add(new Move(x, y, NewPosition.Item1, NewPosition.Item2));
            } else if (Representation[NewPosition.Item1, NewPosition.Item2]!.Player != CurrentPiece.Player) {
                // Vamos ver a nossa posição final do pulo.
                Tuple<int, int> FinalJump = DirectionExtensions.IncrementDirection(NewPosition, Dir);

                // Se está vazio... lucro!
                if (IsEmpty(FinalJump.Item1, FinalJump.Item2)) {
                    // Criar movimento final.
                    Move M = new Move(x, y, FinalJump.Item1, FinalJump.Item2);

                    // Adicionamos o pulo.
                    M.Jumps.Add(NewPosition);

                    // Filtramos os resultados para só ter aqueles que tem pulos (captura é obrigatória!)
                    Results = Results.Where(x => x.Jumps.Any()).ToList();

                    // Encontramos pulos conexos.
                    FindMoreJumps(M, FinalJump.Item1, FinalJump.Item2, CurrentPiece);

                    // Adiciona o movimento.
                    Results.Add(M);
                }
            }
        }

        // Retornar os resultados.
        return Results;
    }

    // Executa um Movimento.
    public void ExecuteMove(Move move, bool CountPoints = true) {
        // Buscamos a peça.
        Piece CachedPiece = Representation[move.InitialPosition.Item1, move.InitialPosition.Item2]!;

        // Para cada pulo..
        foreach (var x in move.Jumps) {
            // Adicionar ao score.
            if (CountPoints)
                CachedPiece.Player.Score++;

            // Remove a peça.
            Representation[x.Item1, x.Item2] = null;
        }

        // Movemos a peça.
        Representation[move.InitialPosition.Item1, move.InitialPosition.Item2] = null;
        Representation[move.FinalPosition.Item1, move.FinalPosition.Item2] = CachedPiece;

        // Pegamos a posição para virar um rei.
        int KingY = CachedPiece.Player.Index == 0 ? 7 : 0;

        // Alteramos para Rei se estamos nesta posição.
        if (move.FinalPosition.Item2 == KingY)
            CachedPiece.IsKing = true;
    }

    // Calcula todos os movimentos para um jogador.
    public List<Move> CalculateMovements(Player player) {
        // Cria resultado.
        List<Move> Results = new List<Move>();

        // Loop em X no tabuleiro...
        for (int x = 0; x < 8; x++) {
            // Loop em Y no tabuleiro...
            for (int y = 0; y < 8; y++) {
                // Só aceitamos as peças do turno atual.
                if (Representation[x, y]?.Player != player)
                    continue;

                // Calcular movimentos.
                Results.AddRange(GetMoves(x, y) ?? new List<Move>());
            }
        }

        // Retornamos o resultado.
        return Results;
    }

    // Copia um board.
    public Board Copy() {
        // Cria o resultado.
        var Result = new Board();

        // Para cada X...
        for (int x = 0; x < 8; x++) {
            // Para cada Y...
            for (int y = 0; y < 8; y++) {
                // Se não temos peça aqui, ignora.
                if (Representation[x, y] == null)
                    continue;

                // Copiamos a peça.
                Result.Representation[x, y] = Representation[x, y]!.Copy();
			}
		}

        // Retorna o Resultado.
        return Result;
	}

    // Calcula a heurística de um movimento.
    public int Heuristic(Move move, Player turn) {
        // Create result.
        int Result = 0;

        // Amount of Kills is a Nice Heuristic.
        Result += move.Jumps.Count;

        /*
        // Bonus points we are close to the King Location and we aren't a king yet!
        if (!Representation[move.InitialPosition.Item1, move.InitialPosition.Item2]!.IsKing)
            Result += 7 - Math.Abs((turn.Index == 0 ? 7 : 0) - move.FinalPosition.Item2);

        // Distance to the Closest Piece.
        int ClosestPiece = int.MaxValue;

        // Loop board X.
        for (int x = 0; x < 8; x++) {
            // Loop board Y.
            for (int y = 0; y < 8; y++) {
                // Get Piece at position.
                var PieceAtPos = Representation[x, y];

                // If piece is not valid or it is from the current player, ignore.
                if (PieceAtPos == null || PieceAtPos.Player == turn)
                    continue;

                // Calculate Distance.
                int Distance = Math.Abs(move.InitialPosition.Item1 - x) + Math.Abs(move.InitialPosition.Item2 - y);

                // If Distance is greater than closes distance set it.
                if (Distance > ClosestPiece)
                    ClosestPiece = Distance;
			}
		}

        // Append to result the distance inverted.
        Result += (8 - ClosestPiece) * 2;
        */

        // Return result.
        return Result;
	}
}
