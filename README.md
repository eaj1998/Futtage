<<<<<<< HEAD
# 🎥 Futtage - Concatenação, Corte e Upload de Vídeos para YouTube

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Windows Forms](https://img.shields.io/badge/Windows%20Forms-Desktop%20App-0078D4?logo=windows&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows-blue.svg)](https://www.microsoft.com/windows)

> **Futtage** é uma aplicação desktop desenvolvida em C# (.NET 8) que permite concatenar, cortar e fazer upload de vídeos MP4 para o YouTube, com interface gráfica intuitiva e suporte a thumbnail personalizada.

## 📋 Índice

- [✨ Funcionalidades](#-funcionalidades)
- [🛠️ Tecnologias Utilizadas](#️-tecnologias-utilizadas)
- [📋 Pré-requisitos](#-pré-requisitos)
- [⚙️ Configuração](#️-configuração)
- [🚀 Como Usar](#-como-usar)
- [📁 Estrutura do Projeto](#-estrutura-do-projeto)
- [🔧 Compilação e Instalação](#-compilação-e-instalação)
- [📝 Notas Importantes](#-notas-importantes)
- [🐛 Solução de Problemas](#-solução-de-problemas)
- [🤝 Contribuições](#-contribuições)
- [📄 Licença](#-licença)

## ✨ Funcionalidades

### 🎬 Processamento de Vídeo
- **Concatenação de Vídeos**: Junte múltiplos arquivos MP4 em um único vídeo
- **Ordenação Inteligente**: Os vídeos são automaticamente ordenados por data de criação
- **Gerenciamento de Lista**: Adicione, remova e reordene vídeos na lista de concatenação
- **Corte de Vídeo**: Corte o vídeo concatenado selecionando tempo de início e fim
- **Suporte a MP4**: Processamento otimizado para arquivos MP4

### 📤 Upload para YouTube
- **Upload Direto**: Upload automático para o YouTube com metadados personalizados
- **Thumbnail Personalizada**: Selecione uma imagem personalizada ou use a padrão
- **Metadados Completos**: Título, descrição e configurações de conteúdo infantil
- **Barra de Progresso**: Acompanhe o progresso do upload em tempo real

### 🎨 Interface do Usuário
- **Navegação por Etapas**: Interface baseada em abas para cada etapa do fluxo
- **Feedback Visual**: Mensagens de sucesso/erro e indicadores de progresso
- **Interface Intuitiva**: Design amigável e responsivo
- **Gerenciamento de Estado**: Controle de fluxo entre as diferentes etapas

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Versão | Propósito |
|------------|--------|-----------|
| **.NET 8.0** | 8.0 | Framework de desenvolvimento |
| **Windows Forms** | - | Interface gráfica desktop |
| **Google.Apis.YouTube.v3** | 1.69.0.3764 | API do YouTube para upload |
| **FFmpeg** | Incluído | Processamento de vídeo |
| **DotNetEnv** | 3.1.1 | Gerenciamento de variáveis de ambiente |

## 📋 Pré-requisitos

### Sistema Operacional
- ✅ Windows 10 ou superior
- ❌ Não suporta macOS ou Linux

### Software Necessário
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (para desenvolvimento)
- [FFmpeg](https://ffmpeg.org/) (incluído no projeto)

### Conta e APIs
- Conta do Google com API do YouTube habilitada
- Credenciais OAuth 2.0 configuradas

## ⚙️ Configuração

### 1. Configuração da API do YouTube

#### Passo 1: Criar Projeto no Google Cloud Console
1. Acesse o [Google Cloud Console](https://console.cloud.google.com/)
2. Crie um novo projeto ou selecione um existente
3. Ative a faturação (necessária para APIs)

#### Passo 2: Habilitar API do YouTube
1. No menu lateral, vá para "APIs e Serviços" > "Biblioteca"
2. Procure por "YouTube Data API v3"
3. Clique em "Ativar"

#### Passo 3: Criar Credenciais OAuth 2.0
1. Vá para "APIs e Serviços" > "Credenciais"
2. Clique em "Criar Credenciais" > "ID do Cliente OAuth 2.0"
3. Configure a tela de consentimento OAuth
4. Selecione "Aplicativo Desktop" como tipo de aplicativo
5. Baixe o arquivo JSON de credenciais

### 2. Configuração das Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto com as seguintes variáveis:

```env
# Credenciais da API do YouTube
YOUTUBE_CLIENT_ID=seu_client_id_aqui
YOUTUBE_CLIENT_SECRET=seu_client_secret_aqui

# Configurações opcionais
DEBUG_MODE=false
LOG_LEVEL=INFO
```

> ⚠️ **Importante**: Nunca compartilhe suas credenciais da API. O arquivo `.env` já está no `.gitignore` para segurança.

## 🚀 Como Usar

### 📋 Fluxo Completo da Aplicação

#### Etapa 1: Seleção de Vídeos
1. **Abrir a aplicação** e clicar em "Selecionar Arquivo..."
2. **Escolher arquivos MP4** que deseja concatenar
3. **Verificar a ordem** - os vídeos são ordenados automaticamente por data
4. **Reordenar se necessário** usando os botões de seta
5. **Remover arquivos** indesejados com o botão "X"
6. **Clicar em "Próximo Passo"** para avançar

#### Etapa 2: Concatenação
1. **Verificar se há pelo menos 2 vídeos** selecionados
2. **Clicar em "Juntar Vídeos"**
3. **Escolher local** para salvar o vídeo final
4. **Aguardar processamento** - acompanhar pela barra de progresso
5. **Avançar automaticamente** para a etapa de corte

#### Etapa 3: Corte (Opcional)
1. **Definir tempo de início** no campo correspondente
2. **Definir tempo de fim** no campo correspondente
3. **Clicar em "Cortar Vídeo"** para salvar versão cortada
4. **Ou clicar em "Pular Corte"** para manter o vídeo inteiro

#### Etapa 4: Thumbnail
1. **Verificar thumbnail padrão** que será aplicada
2. **Clicar em "Selecionar Capa"** para escolher imagem personalizada
3. **Selecionar arquivo** (JPG, JPEG, PNG)
4. **Avançar para upload**

#### Etapa 5: Upload para YouTube
1. **Clicar em "Fazer Upload"**
2. **Preencher detalhes do vídeo**:
   - **Título** (obrigatório)
   - **Descrição** (opcional)
   - **Marcar se é conteúdo infantil**
3. **Acompanhar progresso** pela barra de progresso
4. **Aguardar conclusão** - a thumbnail será aplicada automaticamente

### 🎯 Dicas de Uso

- **Formato de vídeo**: Use apenas arquivos MP4 para melhor compatibilidade
- **Tamanho de arquivo**: Considere o limite de upload do YouTube (128GB)
- **Qualidade**: Mantenha a qualidade original dos vídeos para melhor resultado
- **Backup**: Sempre mantenha cópias dos vídeos originais

## 📁 Estrutura do Projeto

```
Futtage/
├── 📄 Futtage.csproj              # Arquivo de projeto .NET
├── 📄 Futtage.sln                 # Solução do Visual Studio
├── 📄 Program.cs                  # Ponto de entrada da aplicação
├── 📄 .env                        # Variáveis de ambiente (não versionado)
├── 📄 .gitignore                  # Arquivos ignorados pelo Git
│
├── 🖼️ Form1.cs                    # Formulário principal da aplicação
├── 🖼️ Form1.Designer.cs           # Designer do formulário principal
├── 🖼️ Form1.resx                  # Recursos do formulário principal
│
├── 🖼️ FormDetalhesVideo.cs        # Formulário de detalhes do vídeo
├── 🖼️ FormDetalhesVideo.Designer.cs
├── 🖼️ FormDetalhesVideo.resx
│
├── 🖼️ FormAguarde.cs              # Formulário de progresso/aguarde
├── 🖼️ FormAguarde.Designer.cs
├── 🖼️ FormAguarde.resx
│
├── 📁 Resources/                  # Recursos da aplicação
│   ├── 🖼️ CAPA.png               # Thumbnail padrão
│   ├── 🖼️ faz-o-simples.ico      # Ícone da aplicação
│   └── 🎬 *.gif                  # GIFs e outros recursos
│
├── 📁 Properties/                 # Propriedades do projeto
├── 📁 bin/                        # Arquivos compilados (não versionado)
├── 📁 obj/                        # Objetos de compilação (não versionado)
└── 📁 .vs/                        # Configurações do Visual Studio (não versionado)
```

## 🔧 Compilação e Instalação

### Opção 1: Visual Studio (Recomendado)

1. **Clone o repositório**:
   ```bash
   git clone https://github.com/seu-usuario/futtage.git
   cd futtage
   ```

2. **Abra no Visual Studio**:
   - Abra o arquivo `Futtage.sln`
   - Aguarde a restauração dos pacotes NuGet

3. **Configure as variáveis de ambiente**:
   - Crie o arquivo `.env` na raiz do projeto
   - Adicione suas credenciais da API do YouTube

4. **Compile e execute**:
   - Pressione `F5` para executar em modo debug
   - Ou `Ctrl+Shift+B` para compilar

### Opção 2: Linha de Comando

1. **Clone e navegue**:
   ```bash
   git clone https://github.com/seu-usuario/futtage.git
   cd futtage
   ```

2. **Restauração de dependências**:
   ```bash
   dotnet restore
   ```

3. **Compilação**:
   ```bash
   dotnet build
   ```

4. **Execução**:
   ```bash
   dotnet run
   ```

### Opção 3: Build de Release

```bash
# Build de release
dotnet publish -c Release -r win-x64 --self-contained true

# O executável estará em: bin/Release/net8.0-windows/win-x64/publish/
```

## 📝 Notas Importantes

### 🔧 Dependências Incluídas
- **FFmpeg**: Executável incluído no projeto para processamento de vídeo
- **Thumbnail Padrão**: Imagem padrão incluída nos recursos
- **Ícones**: Ícones da aplicação incluídos

### 🔐 Segurança e Autenticação
- **Primeira execução**: Será necessário autenticar com sua conta Google
- **Token de acesso**: Salvo localmente para futuras execuções
- **Credenciais**: Nunca compartilhadas ou enviadas para servidores externos

### 📊 Limitações Conhecidas
- **Formatos suportados**: Apenas arquivos MP4 para concatenação e corte
- **Sistema operacional**: Apenas Windows
- **Tamanho de upload**: Limite do YouTube (128GB por vídeo)
- **Thumbnail**: Canal precisa ser verificado no YouTube

### 🔄 Fluxo de Trabalho
- **Navegação sequencial**: As etapas devem ser seguidas em ordem
- **Validações**: Cada etapa valida os dados antes de avançar
- **Feedback**: Mensagens claras de sucesso e erro em cada etapa

## 🐛 Solução de Problemas

### ❌ Erro de Upload para YouTube

**Sintomas**: Erro durante upload ou autenticação
**Soluções**:
1. Verifique se as credenciais da API do YouTube estão corretas no `.env`
2. Certifique-se de que a API do YouTube Data v3 está habilitada
3. Verifique se o canal tem permissões para upload
4. Confirme se a conta Google está logada corretamente

### ❌ Erro de Concatenação ou Corte

**Sintomas**: Erro durante processamento de vídeo
**Soluções**:
1. Certifique-se de que todos os vídeos são MP4 válidos
2. Verifique se há espaço suficiente no disco (pelo menos 2x o tamanho dos vídeos)
3. Confirme se o FFmpeg está presente na pasta `bin`
4. Tente com vídeos menores para testar

### ❌ Erro de Thumbnail

**Sintomas**: Thumbnail não é aplicada após upload
**Soluções**:
1. O canal precisa ser verificado no YouTube
2. Verifique se o recurso de thumbnail está disponível
3. Confirme se a imagem está no formato correto (JPG, PNG)
4. Tente com uma imagem menor (máximo 2MB)

### ❌ Erro de Compilação

**Sintomas**: Erro ao compilar o projeto
**Soluções**:
1. Verifique se o .NET 8.0 SDK está instalado
2. Execute `dotnet restore` para restaurar dependências
3. Limpe a solução: `dotnet clean`
4. Recompile: `dotnet build`

### ❌ Erro de Execução

**Sintomas**: Aplicação não inicia ou trava
**Soluções**:
1. Verifique se o .NET 8.0 Runtime está instalado
2. Execute como administrador se necessário
3. Verifique se o antivírus não está bloqueando
4. Confirme se todas as dependências estão presentes

## 🤝 Contribuições

Contribuições são muito bem-vindas! Aqui estão algumas formas de contribuir:

### 🐛 Reportar Bugs
1. Use a aba "Issues" do GitHub
2. Inclua detalhes sobre o erro
3. Adicione screenshots se possível
4. Descreva os passos para reproduzir

### 💡 Sugerir Melhorias
1. Abra uma issue com a tag "enhancement"
2. Descreva a funcionalidade desejada
3. Explique o benefício para os usuários

### 🔧 Contribuir com Código
1. Faça um fork do projeto
2. Crie uma branch para sua feature: `git checkout -b feature/nova-funcionalidade`
3. Commit suas mudanças: `git commit -m 'Adiciona nova funcionalidade'`
4. Push para a branch: `git push origin feature/nova-funcionalidade`
5. Abra um Pull Request

### 📝 Melhorar Documentação
1. Corrija erros no README
2. Adicione exemplos de uso
3. Traduza para outros idiomas
4. Melhore a estrutura e organização

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

### 📋 Termos de Uso
- **Uso pessoal**: Livre para uso pessoal e educacional
- **Uso comercial**: Entre em contato para discussão
- **Modificações**: Permitidas desde que mantenha a atribuição original

---

## 🙏 Agradecimentos

- **Google**: Pela API do YouTube
- **FFmpeg**: Pelo processamento de vídeo
- **.NET Community**: Pelo framework e ferramentas
- **Contribuidores**: Todos que ajudaram no desenvolvimento

---

**Desenvolvido com ❤️ para facilitar a criação de conteúdo no YouTube**

*Se este projeto te ajudou, considere dar uma ⭐ no repositório!* 
=======
FUTTAGE - Video Editor for YouTube

Version: 2.0.0
Platform: Windows (.NET 8.0)
📋 About

Futtage is a desktop application for Windows that allows you to concatenate, trim, and upload videos directly to YouTube. Built in C# with a modern interface and complete integration with the YouTube API.
✨ Key Features

    Video Concatenation: Merge multiple MP4 videos into a single file
    Video Trimming: Cut videos to specific time ranges
    YouTube Integration: Direct upload to YouTube with metadata
    Modern UI: Clean interface with progress tracking
    Authentication: Secure Google OAuth2 integration
    Thumbnail Support: Custom thumbnail selection for uploads
    Error Handling: Robust error management and logging
    Multiple Formats: Support for MP4, AVI, MOV, MKV, WMV, FLV

🏗️ Project Structure

Futtage/
├── Core/                          # Business logic layer
│   ├── Models/                    # Data models
│   │   ├── VideoInfo.cs          # Video information
│   │   ├── YoutubeUploadRequest.cs # Upload request model
│   │   ├── UserInfo.cs           # User data
│   │   └── ProcessingProgress.cs # Processing progress tracking
│   └── Services/                  # Core services
│       ├── IVideoProcessingService.cs # Video processing interface
│       ├── VideoProcessingService.cs  # Video processing implementation
│       ├── IYoutubeService.cs         # YouTube service interface
│       ├── FuttageYouTubeService.cs   # YouTube service implementation
│       ├── IFileService.cs            # File service interface
│       └── FileService.cs             # File service implementation
├── Infrastructure/                # Infrastructure layer
│   ├── Configuration/             # App configuration
│   ├── Extensions/                # Dependency injection extensions
│   └── Logging/                   # Logging system
├── Presentation/                  # User interface layer
│   ├── Views/                     # Forms and dialogs
│   ├── Presenters/                # MVP pattern presenters
│   └── Common/                    # Shared UI components
├── Resources/                     # Application resources
├── Program.cs                     # Application entry point
├── appsettings.json              # Configuration file
└── ffmpeg.exe                    # FFmpeg binary for video processing

🚀 How to Use
Step 1: Select and Order Videos

    Click "Select Files..." to choose multiple MP4 videos
    Use the arrow buttons to reorder videos as needed
    Remove unwanted videos with the X button
    Login with your Google account for YouTube access

Step 2: Concatenate Videos

    Review your selected videos
    Click "Join Videos" and choose output location
    Wait for processing to complete (progress bar will show status)

Step 3: Trim Video (Optional)

    Set start time in HH:MM:SS format
    Set end time in HH:MM:SS format
    Click "Cut" to trim the video, or "Skip Cut" to proceed

Step 4: Select Thumbnail (Optional)

    A default thumbnail is automatically generated
    Click "Change Default Cover..." to select a custom image
    Supported formats: JPG, PNG, BMP
    Click "Next Step" to continue

Step 5: Upload to YouTube

    Fill in video details:
        Title: Your video title (up to 100 characters)
        Description: Video description (up to 5000 characters)
        Privacy: Choose Private, Unlisted, or Public
        Child-friendly content: Check if appropriate
    Click "Upload to YouTube"
    Wait for upload completion

⚙️ Configuration
appsettings.json Setup

Create or modify the appsettings.json file in the application directory:
json

{
  "YouTube": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret",
    "ApplicationName": "Futtage"
  },
  "Video": {
    "DefaultQuality": "copy",
    "DeleteTempFiles": true
  },
  "UI": {
    "Theme": "Light",
    "ShowTooltips": true,
    "EnableAnimations": true
  }
}

Environment Variables (Alternative)

You can also configure YouTube credentials via environment variables:

    FUTTAGE_YOUTUBE_CLIENTID: Your Google Client ID
    FUTTAGE_YOUTUBE_CLIENTSECRET: Your Google Client Secret

Google API Setup

    Go to Google Cloud Console
    Create a new project or select existing one
    Enable the YouTube Data API v3
    Create OAuth 2.0 credentials (Desktop Application)
    Add your Client ID and Secret to the configuration

🔧 System Requirements

    Operating System: Windows 10 64-bit or higher
    Framework: .NET 8.0 Runtime
    Memory: 4 GB RAM minimum, 8 GB recommended
    Storage: 1 GB free space (more for video processing)
    FFmpeg: Included with the application
    Internet: Required for YouTube uploads
    Google Account: Valid YouTube account for uploads

📁 Supported Formats
Input Video Formats

    MP4 (H.264/H.265)
    AVI
    MOV
    MKV
    WMV
    FLV
    WEBM

Output Format

    MP4 (H.264) - optimized for YouTube

Thumbnail Formats

    JPEG/JPG
    PNG
    BMP
    WEBP

🐛 Troubleshooting
FFmpeg Not Found

Problem: Video processing fails with FFmpeg error Solution:

    Ensure ffmpeg.exe is in the application folder
    Download FFmpeg from: https://www.gyan.dev/ffmpeg/builds/
    Extract ffmpeg.exe to the same folder as Futtage.exe

YouTube Authentication Failed

Problem: Cannot login to Google/YouTube Solutions:

    Verify your Client ID and Secret in appsettings.json
    Check that YouTube Data API v3 is enabled in Google Cloud Console
    Ensure OAuth consent screen is properly configured
    Try logging out and logging back in

Video Processing Errors

Problem: Concatenation or trimming fails Solutions:

    Verify all input videos are not corrupted
    Ensure sufficient disk space is available
    Check that all videos have compatible codecs
    Try processing smaller batches of videos

Upload Failures

Problem: YouTube upload fails or times out Solutions:

    Check your internet connection stability
    Verify video file size (YouTube limit is 256GB or 12 hours)
    Ensure you have sufficient upload quota
    Try uploading during off-peak hours

Log Files

Application logs are stored in: %LOCALAPPDATA%\Futtage\Logs\
🔒 Privacy & Security

    Local Processing: All video processing happens locally on your machine
    Secure Authentication: Uses Google's OAuth 2.0 for secure login
    No Data Collection: Futtage doesn't collect or store personal data
    Temporary Files: Automatically cleaned up after processing
    Credentials: Stored securely using Windows Credential Manager

🚀 Advanced Usage
Command Line Arguments

Currently, Futtage is designed as a GUI application, but future versions may support command-line operations.
Batch Processing

    Select multiple videos at once for efficient processing
    Videos are processed in the order you arrange them
    Use the reorder buttons to change sequence

Custom Thumbnails

    Thumbnails are automatically generated from the first frame
    Custom thumbnails should be 1280x720 pixels for best quality
    JPG format is recommended for smaller file sizes

📞 Support & Contributing
Getting Help

    GitHub Repository: https://github.com/eaj1998/futtage
    Issues: Report bugs via GitHub Issues
    Email: edipo1998@gmail.com

Contributing

Contributions are welcome! Please:

    Fork the repository
    Create a feature branch
    Submit a pull request with detailed description

Building from Source

    Install .NET 8.0 SDK
    Clone the repository
    Download FFmpeg and place in project root
    Configure appsettings.json with your credentials
    Build with dotnet build or Visual Studio

📝 License

Copyright © 2025 EAJ. All rights reserved.

This software is provided "as is" without warranty. Use at your own risk.
>>>>>>> d41ffedcd2d253031af9cc1242882502c952df34
