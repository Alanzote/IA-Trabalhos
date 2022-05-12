using GraphUtils;

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
        // Calcular movimentos.
        allTurnMoves = board.CalculateMovements(turn);

        // Se não temos nenhum movimento, o jogo terminou.
        if (!allTurnMoves.Any())
            IsFinished = true;

        // Se temos algum que pode capturar, então só podemos fazer isso.
        if (allTurnMoves.Any(x => x.Jumps.Any()))
            allTurnMoves = allTurnMoves.Where(x => x.Jumps.Any()).ToList();

        // Validamos se somos IA, rodamos ela se for necessário.
        if (turn.IsAI && !IsFinished) {
            // Calculamos o movimento target.
            var TarMove = PlayAI();

            // Limpamos os movimentos.
            allTurnMoves.Clear();

            // Adicionamos o escolhido pela IA.
            allTurnMoves.Add(TarMove);
		}
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

    // Ai Search depth.
    private static readonly int AiDepth = 5;

    // Roda a Nossa IA.
    private Move PlayAI() {
        // Criamos um Grafo para Representar o nosso Estado atual.
        Graph AlphaBetaGraph = new Graph();

        // Criamos o nosso nó inicial.
        Node<int> StartNode = new Node<int>("Init", 0);

        // Adicionamos o nó inicial.
        AlphaBetaGraph.AddNode(StartNode);

        // For Each turn move, build the move tree recursive.
        foreach (var move in allTurnMoves)
            buildRecursive(AlphaBetaGraph, StartNode, move, board, turn, turn == player0 ? player1 : player0, AiDepth);

        // Alpha-beta.
        var alphabeta = GraphExtensions.AlphaBeta(AlphaBetaGraph, StartNode, AiDepth);

        // Cast no nó resultado que tem o movimento salvo também.
        var alphabeta_cast = alphabeta as Node<int, Move>;

        // Se deu ruim no cast, para de rodar (isso aqui não deve acontecer)
        if (alphabeta_cast == null)
            throw new NotImplementedException();

        // Retorna o movimento para rodar.
        return alphabeta_cast.V2;
	}

    // Monta a nossa árvore recursivamente.
    private void buildRecursive(Graph G, Node Parent, Move parentMove, Board start, Player turnCur, Player turnOth, int Depth) {
        // Valida a profundidade.
        if (Depth <= 0)
            return;

        // Cria o nosso nó passando a heurística.
        Node<int, Move> ThisParent = new Node<int, Move>(turnCur.Index.ToString(), start.Heuristic(parentMove, turnCur), parentMove);

        // Adicionamos o Nó.
        G.AddNode(ThisParent);

        // Conectamos eles.
        G.ConnectNode(Parent, ThisParent, new Connection("AlphaBeta", Connection.EDirection.A_to_B, 1));

        // Copiamos o board.
        var NewBoard = start.Copy();

        // Executamos o movimento.
        NewBoard.ExecuteMove(parentMove, false);

        // Calculamos os movimentos.
        var Moves = NewBoard.CalculateMovements(turnOth);

        // Para cada movimento, montar recursivamente.
        foreach (var move in Moves)
            buildRecursive(G, ThisParent, move, NewBoard, turnOth, turnCur, Depth - 1);
	}
}