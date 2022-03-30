// Utilizar a Biblioteca criada durante a matéria de Grafos.
// Originalmente em Java, transformado em C# por que eu tenho maior facilidade.
using GraphUtils;

// Criamos um Novo Grafo.
Graph GN = new Graph();

// Criamos os Nós, com as posições para a função Display().
// Utilizado para ver os nós e suas conexões, em sí, esse valor é ignorado
// nas buscas em largura e profundidade.
Node N0 = new Node<Tuple<int, int>>("N0", Tuple.Create(50, 50));
Node N1 = new Node<Tuple<int, int>>("N1", Tuple.Create(100, 100));
Node N2 = new Node<Tuple<int, int>>("N2", Tuple.Create(200, 100));
Node N3 = new Node<Tuple<int, int>>("N3", Tuple.Create(150, 200));
Node N4 = new Node<Tuple<int, int>>("N4", Tuple.Create(150, 130));
Node N5 = new Node<Tuple<int, int>>("N5", Tuple.Create(250, 180));
Node N6 = new Node<Tuple<int, int>>("N6", Tuple.Create(300, 150));
Node N7 = new Node<Tuple<int, int>>("N7", Tuple.Create(100, 200));

// Adicionamos os Nós ao nosso Grafo.
GN.AddNode(N0);
GN.AddNode(N1);
GN.AddNode(N2);
GN.AddNode(N3);
GN.AddNode(N4);
GN.AddNode(N5);
GN.AddNode(N6);
GN.AddNode(N7);

// Conectamos todos os Nós, passando o Custo de cada caminho.
GN.ConnectNode(N0, N1, 1);
GN.ConnectNode(N1, N3, 3);
GN.ConnectNode(N3, N2, 5);
GN.ConnectNode(N2, N1, 2);
GN.ConnectNode(N2, N4, 6);
GN.ConnectNode(N3, N5, 4);
GN.ConnectNode(N5, N6, 2);
GN.ConnectNode(N6, N2, 1);
GN.ConnectNode(N5, N4, 3);
GN.ConnectNode(N4, N7, 5);

// Primeiro, vamos demonstrar para o Usuário a disposição dos nossos nós.
// Para utilizar esse Display, de uma olhada no Readme.md do GraphUtils
// pois é necessário as DLLs do SDL2 para conseguir rodar.
// Se não quiser seguir com isso, só comentar essa linha.
GN.Display();

/// Assim que fechamos o display, está na hora de resolver as questões práticas.
/// Sabemos que o Estado Final é o Carrinho estar presente em N0.
Node NT = N0;

// Para isso, precisamos pedir os estado inicial para o usuário.
Console.Write("Digite o Nome do Nó que você Quer como Estado Inicial: ");
Node? NI = GN.FindWithLabel(Console.ReadLine()?.Trim()!);

// Precisamos de um Nó Válido.
if (NI == null)
    throw new ArgumentNullException(nameof(NI));

// Executamos a Busca em Largura, saindo de NI indo para N0. (Questão 5)
// Essa função eu já tinha implementado para Resolução de Problemas com Grafos.
GN.BreadthSearch(NI, NT, out List<Node> Path);

// Demonstrar o nosso caminho.
GraphExtensions.PrintArrayOfNodes(Path, "Caminho de Busca em Largura Encontrado para N0");

// Limpar o Path para a Próxima Busca.
Path.Clear();

// Executamos a Busca em Profundidade, saindo de NI indo para N0. (Questão 6)
// Essa função eu já tinha implementado para Resolução de Problemas com Grafos.
GN.DepthSearch(NI, NT, out Path);

// Demonstrar o nosso caminho.
GraphExtensions.PrintArrayOfNodes(Path, "Caminho de Busca em Profundidade Encontrado para N0");

// Limpar o Path para a Próxima Busca.
Path.Clear();

// Pedir o Limite para nossa a Busca em Profundidade Limitada.
Console.WriteLine();
Console.Write("Digite a Profundidade Máxima para a Próxima Busca em Profundidade: ");
int Limit = int.Parse(Console.ReadLine()?.Trim()!);

// Executamos a Busca em Profundidade Limitada, saindo de NI indo para N0. (Questão 7)
// Essa função eu adicionei um novo parametro na busca em profundidade para considerar um limite.
GN.DepthSearch(NI, NT, out Path, Limit);

// Demostrar o nosso caminho.
GraphExtensions.PrintArrayOfNodes(Path, "Caminho de Busca em Profundidade Limitada Encontrado para N0");

// Limpar o Path para a Próxima Busca.
Path.Clear();

// Pedir o Limite para a Busca de Aprofundamento Iterativo.
Console.WriteLine();
Console.Write("Digite a Profundidade Máxima para a Busca de Aprofundamento Iterativo: ");
Limit = int.Parse(Console.ReadLine()?.Trim()!);

// Executamos a Busca em Aprofundamento Iterativo, saindo de NI indo para N0. (Questão 8)
// Essa função é nova e foi implementada para esse TDE.
GN.IterativeDeepening(NI, NT, out Path, Limit);

// Demonstrar o nosso caminho.
GraphExtensions.PrintArrayOfNodes(Path, "Caminho de Busca em Aprofundamento Iterativo Encontrado para N0");