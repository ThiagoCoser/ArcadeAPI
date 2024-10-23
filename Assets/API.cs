using TMPro; // Importar a biblioteca do TextMeshPro
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics; // Para executar e monitorar processos externos

public class ProjectCarousel : MonoBehaviour
{
    [SerializeField] private Transform projectDisplayContainer; // O container para exibir o projeto atual
    [SerializeField] private GameObject projectDisplayPrefab; // Prefab para exibir um único projeto
    [SerializeField] private string projectsPath; // Caminho da pasta de projetos (Jogos)
    [SerializeField] private Sprite defaultProjectImage; // Imagem padrão caso o projeto não tenha uma específica

    private string[] projectFolders; // Lista de pastas de projetos
    private int currentProjectIndex = 0; // Índice do projeto atualmente exibido
    private bool isProjectRunning = false; // Variável de controle para travar a navegação quando o projeto está rodando
    private Process currentProcess = null; // Variável para monitorar o processo do projeto rodando

    private GameObject currentProjectDisplay; // O GameObject que está exibindo o projeto atual

    void Start()
    {
        // Define o caminho para a pasta "Jogos"
        projectsPath = Path.Combine(Application.dataPath, "../Jogos");
        LoadProjects();

        if (projectFolders.Length > 0)
        {
            DisplayCurrentProject();
        }
        else
        {
            UnityEngine.Debug.LogWarning("Nenhum projeto encontrado na pasta especificada.");
        }
    }

    void Update()
    {
        // Checa se o projeto está rodando e ESC foi pressionado
        if (isProjectRunning)
        {
            // Verifica se o processo foi encerrado
            if (currentProcess != null && currentProcess.HasExited)
            {
                isProjectRunning = false;
                currentProcess = null;
                UnityEngine.Debug.Log("Projeto encerrado. Navegação reativada.");
            }

            return; // Bloqueia qualquer outro input enquanto o projeto está rodando
        }

        // Fecha o aplicativo com ESC se não houver um projeto rodando
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); // Fecha o aplicativo
        }

        // Navega para o próximo projeto com a seta direita, se a navegação estiver habilitada
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextProject();
        }

        // Navega para o projeto anterior com a seta esquerda, se a navegação estiver habilitada
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousProject();
        }

        // Abre o projeto selecionado com Enter, se a navegação estiver habilitada
        if (Input.GetKeyDown(KeyCode.Return)) // Return é a tecla Enter
        {
            OpenSelectedProject();
        }
    }

    void LoadProjects()
    {
        // Carrega todas as pastas de projetos
        projectFolders = Directory.GetDirectories(projectsPath);
        UnityEngine.Debug.Log($"Número de projetos encontrados: {projectFolders.Length}");
    }

    void DisplayCurrentProject()
    {
        // Verifica se o índice atual está dentro dos limites do array
        if (projectFolders.Length == 0 || currentProjectIndex < 0 || currentProjectIndex >= projectFolders.Length)
        {
            UnityEngine.Debug.LogWarning("Índice do projeto atual está fora dos limites.");
            return;
        }

        // Remove a exibição do projeto atual, se houver
        if (currentProjectDisplay != null)
        {
            Destroy(currentProjectDisplay);
        }

        // Cria uma nova exibição para o projeto atual
        currentProjectDisplay = Instantiate(projectDisplayPrefab, projectDisplayContainer);

        string folder = projectFolders[currentProjectIndex];
        string projectName = Path.GetFileName(folder);

        // Atualiza o texto com o nome do projeto usando TextMeshProUGUI
        TextMeshProUGUI projectNameText = currentProjectDisplay.GetComponentInChildren<TextMeshProUGUI>();
        if (projectNameText != null)
        {
            string infoPath = Path.Combine(folder, "info.txt");

            if (File.Exists(infoPath))
            {
                // Lê o texto do arquivo
                string projectDescription = File.ReadAllText(infoPath);
                projectNameText.text = projectDescription;
            }
            else
            {
                // Usa o nome da pasta como fallback
                projectNameText.text = projectName;
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("Texto do projeto não encontrado no prefab.");
        }

        // Atualiza a imagem do projeto
        Image projectImage = currentProjectDisplay.GetComponentInChildren<Image>();
        if (projectImage != null)
        {
            string imagePath = Path.Combine(folder, "icon.png");

            if (File.Exists(imagePath))
            {
                // Carrega a imagem se existir
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);

                // Cria um Sprite a partir da textura e aplica
                projectImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                // Usa a imagem padrão se não houver uma imagem específica
                projectImage.sprite = defaultProjectImage;
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("Imagem do projeto não encontrada no prefab.");
        }
    }

    void OpenSelectedProject()
    {
        string folder = projectFolders[currentProjectIndex];
        string exePath = Path.Combine(folder, Path.GetFileName(folder) + ".exe");

        if (File.Exists(exePath))
        {
            // Abre o executável do projeto selecionado e monitora o processo
            currentProcess = new Process();
            currentProcess.StartInfo.FileName = exePath;
            currentProcess.EnableRaisingEvents = true; // Habilita o evento de término do processo
            currentProcess.Exited += OnProjectExited; // Assina o evento
            currentProcess.Start();

            isProjectRunning = true; // Travar a navegação enquanto o projeto está rodando
            UnityEngine.Debug.Log($"Executável {exePath} iniciado. Navegação desativada.");
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Arquivo executável não encontrado em: {exePath}");
        }
    }

    void OnProjectExited(object sender, System.EventArgs e)
    {
        // Este método será chamado automaticamente quando o processo do jogo for encerrado
        isProjectRunning = false;
        currentProcess = null;
        UnityEngine.Debug.Log("Projeto encerrado. Navegação reativada.");
    }

    void NextProject()
    {
        // Verifica se há projetos para navegar
        if (projectFolders.Length > 0)
        {
            // Incrementa o índice, e volta para o primeiro se passar do último
            currentProjectIndex = (currentProjectIndex + 1) % projectFolders.Length;
            DisplayCurrentProject();
        }
    }

    void PreviousProject()
    {
        // Verifica se há projetos para navegar
        if (projectFolders.Length > 0)
        {
            // Decrementa o índice, e volta para o último se for menor que zero
            currentProjectIndex = (currentProjectIndex - 1 + projectFolders.Length) % projectFolders.Length;
            DisplayCurrentProject();
        }
    }
}
