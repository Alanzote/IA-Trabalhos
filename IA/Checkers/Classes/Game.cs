namespace IA.Checkers.Classes;

// O Próprio jogo de damas, controla as classes básicas.
public class Game {

    // Jogador 1.
    public Player player0;

    // Jogador 2.
    public Player player1;

    // Turno Atual.
    public Player turn;

    // O Tabuleiro do Jogo.
    public Board board;

    // Lista de todos os movimentos possíveis para o jogador atual.
    public List<Move> allTurnMoves = new List<Move>();

    // Se o jogo finalizou.
    public bool IsFinished { get; private set; }

    // Construtor.
    public Game(int PlayerAiIndex) {
        // Criar jogadores.
        player0 = new Player(0, PlayerAiIndex == 0);
        player1 = new Player(1, PlayerAiIndex == 1);

        // Set turn.
        turn = player0;

        // Criar Classes básicas.
        board = new Board(player0, player1);

        // Calcular todos os movimentos.
        CalculateTurnMoves();
    }

    // Calcula todos os movimentos.
    private void CalculateTurnMoves() {
        // Limpamos todos os movimentos.
        allTurnMoves.Clear();

        // Loop em X no tabuleiro...
        for (int x = 0; x < 8; x++) {
            // Loop em Y no tabuleiro...
            for (int y = 0; y < 8; y++) {
                // Só aceitamos as peças do turno atual.
                if (board.Representation[x, y]?.Player != turn)
                    continue;

                // Calcular os movimentos.
                allTurnMoves.AddRange(board.GetMoves(x, y) ?? new List<Move>());
            }
        }

        // Se não temos nenhum movimento, o jogo terminou.
        if (!allTurnMoves.Any())
            IsFinished = true;

        // Se temos algum que pode capturar, então só podemos fazer isso.
        if (allTurnMoves.Any(x => x.Jumps.Any()))
            allTurnMoves = allTurnMoves.Where(x => x.Jumps.Any()).ToList();
    }

    // Executa um único movimento.
    public void ExecuteMove(Move move) {
        // Executa o movimento.
        board.ExecuteMove(move);

        // Trocamos o turno.
        turn = turn == player0 ? player1 : player0;

        // Calculamos todos os movimentos.
        CalculateTurnMoves();
    }
}