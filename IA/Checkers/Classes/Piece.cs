namespace IA.Checkers.Classes;

// Esta classe representa uma peça em um jogo de Damas.
public class Piece {

    // O Jogador que é dono desta peça.
    public Player Player { get; private set; }

    // Se a peça é um rei.
    public bool IsKing { get; set; }

    // Construtor.
    public Piece(Player player) {
        // Sets.
        Player = player;
    }
}