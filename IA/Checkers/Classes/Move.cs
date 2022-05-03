namespace IA.Checkers.Classes;

// Um movimento...
public class Move {

    // Tem uma posição inicial...
    public Tuple<int, int> InitialPosition;

    // Tem uma posição final...
    public Tuple<int, int> FinalPosition;

    // Tem uma lista de pulos, ou seja, quem foi capturado...
    public List<Tuple<int, int>> Jumps = new List<Tuple<int, int>>();

    // E finalmente os pontos em que os pulos mudaram de direção... (não sei se fez sentido?)
    public List<Tuple<int, int>> Midpoints = new List<Tuple<int, int>>();

    // Construtor.
    public Move(int InitialX, int InitialY, int FinalX, int FinalY) {
        // Sets nas Posições.
        InitialPosition = Tuple.Create(InitialX, InitialY);
        FinalPosition = Tuple.Create(FinalX, FinalY);
    }
}