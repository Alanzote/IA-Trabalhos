namespace IA.Checkers.Classes;

// Nossas Direções.
public enum Direction {
    None,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

// Extensões para as direções.
public static class DirectionExtensions {

    // Nossas tuplas de direções.
    private static readonly Dictionary<Direction, Tuple<int, int>> DirectionTuples = new Dictionary<Direction, Tuple<int, int>> {
        { Direction.None, Tuple.Create(0, 0) },
        { Direction.TopLeft, Tuple.Create(-1, -1) },
        { Direction.TopRight, Tuple.Create(1, -1) },
        { Direction.BottomLeft, Tuple.Create(-1, 1) },
        { Direction.BottomRight, Tuple.Create(1, 1) }
    };

    // Busca a direção apartir de 2 posições.
    public static Direction ToDirection(Tuple<int, int> StartPos, Tuple<int, int> EndPos) {
        // Calculamos o offset.
        int x_offset = Math.Clamp(StartPos.Item1 - EndPos.Item1, -1, 1);
        int y_offset = Math.Clamp(StartPos.Item2 - EndPos.Item2, -1, 1);

        // Buscamos a direção.
        return DirectionTuples.FirstOrDefault(x => x.Value.Item1 == x_offset && x.Value.Item2 == y_offset).Key;
    }
        
    // Retorna a tupla de direção de uma direção.
    public static Tuple<int, int> FromDirection(this Direction dir) {
        return DirectionTuples[dir];
    }

    // Incrementa a direção em uma posição.
    public static Tuple<int, int> IncrementDirection(this Tuple<int, int> Position, Direction dir) {
        // Buscamos a direção tupla.
        var FromDir = dir.FromDirection();

        // Criar o resultado.
        return Tuple.Create(Position.Item1 + FromDir.Item1, Position.Item2 + FromDir.Item2);
    }

    // Se uma Direção é válida para um jogador.
    public static bool IsForPlayerIndex(this Direction dir, int Index) {
        return dir switch {
            // Direção 'none' nunca é válida.
            Direction.None => false,

            // Para cima esquerda e direta só é válido para os brancos.
            Direction.TopLeft => Index == 1,
            Direction.TopRight => Index == 1,

            // Para baixo esquerda e direita só é válido para os pretos.
            Direction.BottomLeft => Index == 0,
            Direction.BottomRight => Index == 0,

            // Eita, não é para chegar aqui!
            _ => throw new NotImplementedException()
        };
    }

}
