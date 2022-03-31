// Utilizar a Biblioteca criada durante a matéria de Grafos.
// Originalmente em Java, transformado em C# por que eu tenho maior facilidade.
using GraphUtils;

// Classe para o Trabalho 04.
public static class Trabalho04 {

    // Método Main.
    public static void Main(String[] args) {
        // Cria o Grafo, mesmo do Trabalho 04.
        Graph GN = Trabalho03.CreateGraph(out List<Node> Nodes);

        // Primeiro, vamos demonstrar para o Usuário a disposição dos nossos nós.
        // Para utilizar esse Display, de uma olhada no Readme.md do GraphUtils
        // pois é necessário as DLLs do SDL2 para conseguir rodar.
        // Se não quiser seguir com isso, só comentar essa linha.
        GN.Display();

        /// Assim que fechamos o display, está na hora de resolver as questões práticas.
        /// Sabemos que o Estado Final é o Carrinho estar presente em N0.
        Node NT = Nodes[0];

        // Utilizando a função Display, a heurística foi pré-calculada a mão.
        Dictionary<Node, int> HeuristicValues = new Dictionary<Node, int> {
            { Nodes[0], 0 },
            { Nodes[1], 1 },
            { Nodes[2], 2 },
            { Nodes[3], 2 },
            { Nodes[4], 3 },
            { Nodes[5], 3 },
            { Nodes[6], 3 },
            { Nodes[7], 4 }
        };

        // Para isso, precisamos pedir os estado inicial para o usuário.
        Console.Write("Digite o Nome do Nó que você Quer como Estado Inicial: ");
        Node? NI = GN.FindWithLabel(Console.ReadLine()?.Trim()!);

        // Precisamos de um Nó Válido.
        if (NI == null)
            throw new ArgumentNullException(nameof(NI));

        // Agora, vamos chamar a busca gulosa. Simplesmente a Busca Heurísitica com UseWeights = false.
        // Essa função é nova, implementada neste trabalho. Ela engloba já Limites e o Método A* (UseWeights).
        GN.HeuristicSearch(NI, NT, HeuristicValues, out List<Node> Path);

        // Demonstrar o nosso caminho.
        GraphExtensions.PrintArrayOfNodes(Path, "Caminho de Busca Gulosa");

        // Limpar o Path para a Próxima Busca.
        Path.Clear();

        // Agora, vamos chamar Busca A*. Busca Heurística com UseWeights = true.
        GN.HeuristicSearch(NI, NT, HeuristicValues, out Path, true);

        // Demonstrar o nosso caminho.
        GraphExtensions.PrintArrayOfNodes(Path, "Caminho de Busca A*");

        // Limpar o Path para a Próxima Busca.
        Path.Clear();

        // Pedir o Limite para a Busca de Aprofundamento Iterativo.
        Console.WriteLine();
        Console.Write("Digite a Profundidade Máxima para a Busca A* com Aprofundamento Iterativo: ");
        int Limit = int.Parse(Console.ReadLine()?.Trim()!);

        // Executamos a Busca A* com Aprofundamento Iterativo. Basta passar UseWeights = true para o A* e o Limite.
        // Essa função é nova e foi implementada para esse Trabalho.
        GN.HeuristicIterativeDeepening(NI, NT, HeuristicValues, out Path, Limit, true);

        // Demonstrar o nosso caminho.
        GraphExtensions.PrintArrayOfNodes(Path, "Caminho de Busca A* com Aprofundamento Iterativo");

        // Notificar que estamos esperando analisar os resultados.
        Console.WriteLine();
        Console.Write("Pressione qualquer tecla para terminar...");

        // Esperar qualquer tecla.
        Console.ReadKey();
    }
}