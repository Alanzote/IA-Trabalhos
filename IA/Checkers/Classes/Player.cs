namespace IA.Checkers.Classes;

// Um jogador de Damas. Funciona como um Scoreboard.
public class Player {

    // Pontuação do jogador.
    public int Score { get; set; }

    // O índice do jogador.
    public int Index { get; private set; }

    // Se este jogador é uma IA.
    public bool IsAI { get; private set; }

    // Construtor.
    public Player(int Index, bool IsAI) {
        // Sets.
        this.Index = Index;
        this.IsAI = IsAI;
    }
}
