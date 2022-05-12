using GraphUtils;
using IA.Checkers.Classes;
using SDL2;
using System.Drawing;

namespace IA.Checkers;

// Classe para o Trabalho de Damas.
public static class Checkers {

    // Taxa de Renderização (está para 60 fps aproximadamente)
    private static readonly uint RefreshRate = 16;

    // Tamanho da Janela e o calculado tamanho de slots.
    private static readonly int WindowSize = 512;
    private static readonly int SlotSize = WindowSize / 8;

    // Texturas das Peças.
    private static IntPtr Tex_Piece_Black;
    private static IntPtr Tex_Piece_White;
    private static IntPtr Tex_King_Black;
    private static IntPtr Tex_King_White;

    // Fontes.
    private static IntPtr Font_Arial;

    // Texturas dos Textos.
    private static IntPtr Text_Player0;
    private static IntPtr Text_Player1;

    // Retângulos dos Textos.
    private static SDL.SDL_Rect TextRect_Player0;
    private static SDL.SDL_Rect TextRect_Player1;

    // Onde que estão os Assets de Checkers.
    private static string AssetsFolder = "./Assets/Checkers/";

    // O jogo atual.
    private static Game game;

    // Método main.
    public static void Main(string[] args) {
        // Perguntamos quem vai ser a nossa IA.
        Console.WriteLine("=====     Damas Configuração     =====");
        Console.WriteLine("Quem vai ser a nossa IA?");
        Console.WriteLine("===== Diga Preto, Branco ou Nada =====");
        string Ai = Console.ReadLine()?.ToUpperInvariant() ?? "Nada";

        // Criamos o Jogo.
        game = new Game(Ai switch {
            "PRETO" => 0,
            "BRANCO" => 1,
            _ => -1
        });

        // Inicializa o SDL.
        SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

        // Criamos a janela de Damas.
        var Window = SDL.SDL_CreateWindow("Damas",
            SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
            WindowSize, WindowSize, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

        // Criamos o Renderizador.
        var Renderer = SDL.SDL_CreateRenderer(Window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        // Inicializamos as Fontes.
        SDL_ttf.TTF_Init();

        // Abrimos a Fonte Arial, 12px.
        Font_Arial = SDL_ttf.TTF_OpenFont("./Assets/arial.ttf", 12);

        // Criamos Textos de quem é o Jogador Atual.
        SDL_Utils.SDL_CreateText(Renderer, Font_Arial, "Vez do Jogador Preto", Color.White, out Text_Player0, out TextRect_Player0);
        SDL_Utils.SDL_CreateText(Renderer, Font_Arial, "Vez do Jogador Branco", Color.White, out Text_Player1, out TextRect_Player1);

        // Inicializamos PNGs.
        SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG);

        // Carregamos as Texturas.
        Tex_Piece_Black = SDL_image.IMG_LoadTexture(Renderer, $"{AssetsFolder}Piece_Black.png");
        Tex_Piece_White = SDL_image.IMG_LoadTexture(Renderer, $"{AssetsFolder}Piece_White.png");
        Tex_King_Black = SDL_image.IMG_LoadTexture(Renderer, $"{AssetsFolder}King_Black.png");
        Tex_King_White = SDL_image.IMG_LoadTexture(Renderer, $"{AssetsFolder}King_White.png");

        // Movimentos atuais.
        Tuple<int, int>? SelectedPiece = null;
        List<Move>? PossibleMovements = null;

        // Enquanto estamos rodando...
        while (true) {
            // Se devemos parar de jogar.
            bool Quit = game.IsFinished;

            // Buscamos todos os eventos do SDL.
            while (SDL.SDL_PollEvent(out SDL.SDL_Event evt) != 0) {
                // Switch no tipo de evento.
                switch (evt.type) {
                    // Se for para sair, saimos.
                    case SDL.SDL_EventType.SDL_QUIT: {
                        // We Quit.
                        Quit = true;
                    }
                    break;

                    // Quando clicar na tela.
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN: {

                        // Buscamos a posição do mouse.
                        SDL.SDL_GetMouseState(out int mouseX, out int mouseY);

                        // Transformamos em índices ao campo.
                        int PieceX = mouseX / SlotSize;
                        int PieceY = mouseY / SlotSize;

                        // Validamos se temos uma peça neste lugar.
                        if (game.board.IsEmpty(PieceX, PieceY)) {
                            // Se não temos, e temos um movimento possível para lá, então executamos o movimento.
                            if (PossibleMovements != null && PossibleMovements.FirstOrDefault(m => m.FinalPosition.Item1 == PieceX && m.FinalPosition.Item2 == PieceY) is var found && found != null)
                                game.ExecuteMove(found);

                            // Reiniciar seleção.
                            PossibleMovements = null;
                            SelectedPiece = null;

                            // Parar de rodar.
                            break;
                        }

                        // Se Estamos selecionando uma peça que não é nossa...
                        // Para IA, apenas 1 movimento é demonstrado em tela. Usuário deve selecionar ele.
                        if (game.board.Representation[PieceX, PieceY]!.Player != game.turn) {
                            // Reiniciar seleção.
                            PossibleMovements = null;
                            SelectedPiece = null;

                            // Parar de rodar.
                            break;
                        }

                        // Buscamos os movimentos para esta peça específica.
                        PossibleMovements = game.allTurnMoves.Where(x => x.InitialPosition.Item1 == PieceX && x.InitialPosition.Item2 == PieceY).ToList();
                        SelectedPiece = Tuple.Create(PieceX, PieceY);
                    } break;
                }
            }

            // Valida se devemos sair.
            if (Quit)
                break;

            // Limpamos o vídeo.
            SDL.SDL_RenderClear(Renderer);

            // Loop do tabuleiro em X...
            for (int x = 0; x < 8; x++) {
                // Loop do tabuleiro em Y...
                for (int y = 0; y < 8; y++) {
                    // Cor de fundo do nosso tabuleiro, apartir do posicionamento. (Escuro/Claro)
                    Color BkgColor = (x % 2 + y % 2) % 2 == 0 ? Color.SandyBrown : Color.SaddleBrown;

                    // Se temos algum movimento que sai daqui, mudamos a cor para ficar óbvio as opções.
                    if (game.allTurnMoves.Any(m => m.InitialPosition.Item1 == x && m.InitialPosition.Item2 == y))
                        BkgColor = Color.DarkOliveGreen;

                    // Se este campo tem a nossa peça selecionada, mudamos a cor para ficar óbvio novamente.
                    if (SelectedPiece != null && SelectedPiece.Item1 == x && SelectedPiece.Item2 == y)
                        BkgColor = Color.LightGreen;

                    // Validar movimentos possíveis.
                    if (PossibleMovements != null) {

                        // Mostrar o campo final de movimentos possíveis e seus midpoints com cores diferentes.
                        if (PossibleMovements.Any(m => m.FinalPosition.Item1 == x && m.FinalPosition.Item2 == y))
                            BkgColor = Color.LightBlue;
                        else if (PossibleMovements.Any(m => m.Midpoints.Any(m1 => m1.Item1 == x && m1.Item2 == y)))
                            BkgColor = Color.Ivory;
                    }

                    // Trocamos a cor.
                    SDL_Utils.SDL_SetColor(Renderer, BkgColor);

                    // Cria a slot colorida.
                    SDL.SDL_Rect SlotRect = new SDL.SDL_Rect {
                        x = x * (SlotSize + 1),
                        y = y * (SlotSize + 1),
                        w = SlotSize,
                        h = SlotSize
                    };

                    // Desenhamos a slot.
                    SDL.SDL_RenderFillRect(Renderer, ref SlotRect);

                    // Buscamos uma peça nesta posição.
                    Piece? ThisPiece = game.board.Representation[x, y];

                    // Se não temos peça, continuamos.
                    if (ThisPiece == null)
                        continue;

                    // Se temos, copiamos a peça para renderizar ela, com a textura correta.
                    SDL.SDL_RenderCopy(Renderer, ThisPiece.Player.Index switch {
                        0 => ThisPiece.IsKing ? Tex_King_Black : Tex_Piece_Black,
                        1 => ThisPiece.IsKing ? Tex_King_White : Tex_Piece_White,
                        _ => throw new NotImplementedException()
                    }, IntPtr.Zero, ref SlotRect);
                }
            }

            // Voltar a cor para o Preto de fundo.
            SDL_Utils.SDL_SetColor(Renderer, Color.Black);

            // Copiamos o texto de vez de quem no momento certo.
            if (game.turn == game.player0)
                SDL.SDL_RenderCopy(Renderer, Text_Player0, IntPtr.Zero, ref TextRect_Player0);
            else
                SDL.SDL_RenderCopy(Renderer, Text_Player1, IntPtr.Zero, ref TextRect_Player1);

            // Renderiza o jogo.
            SDL.SDL_RenderPresent(Renderer);

            // Delay até a próxima frame (não queremos matar o cpu e o gpu né?)
            SDL.SDL_Delay(RefreshRate);
        }

        // Liberar texturas da memória.
        SDL.SDL_DestroyTexture(Tex_Piece_Black);
        SDL.SDL_DestroyTexture(Tex_Piece_White);
        SDL.SDL_DestroyTexture(Tex_King_Black);
        SDL.SDL_DestroyTexture(Tex_King_White);
        SDL.SDL_DestroyTexture(Text_Player0);
        SDL.SDL_DestroyTexture(Text_Player1);

        // Fechamos as fontes.
        SDL_ttf.TTF_CloseFont(Font_Arial);

        // Saimos do criador de fontes.
        SDL_ttf.TTF_Quit();

        // Saimos do criador de texturas.
        SDL_image.IMG_Quit();

        // Destruimos o nosso renderizador.
        SDL.SDL_DestroyRenderer(Renderer);

        // Matamos a nossa janela.
        SDL.SDL_DestroyWindow(Window);

        // E Finalmente saimos do SDL.
        SDL.SDL_Quit();

        // Mostrar resultado do jogo.
        Console.WriteLine( "=====            Damas  Resultados            =====");
        Console.WriteLine($"Pontuação do Jogador Preto: {game.player0.Score}");
        Console.WriteLine($"Pontuação do Jogador Branco: {game.player1.Score}");
        Console.WriteLine();
        Console.WriteLine($"{(game.player0.Score > game.player1.Score ? "Preto" : "Branco")} é o Vencedor!");
        Console.WriteLine( "===== Pressione Qualquer Tecla para Finalizar =====");
        Console.ReadKey();
    }
}