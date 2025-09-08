# FUTTAGE - Editor de Vídeo para YouTube

**Versão:** 2.0.0  
**Plataforma:** Windows (.NET 8.0)

## 📋 Sobre

O Futtage é uma aplicação desktop para Windows que permite concatenar, cortar e fazer upload de vídeos diretamente para o YouTube. Desenvolvido em C# com interface moderna e integração completa com a API do YouTube.

## 🏗️ Estrutura do Projeto

```
Futtage/
├── Core/                          # Lógica de negócio
│   ├── Models/                    # Modelos de dados
│   │   ├── VideoInfo.cs          # Informações do vídeo
│   │   ├── YoutubeUploadRequest.cs # Requisição de upload
│   │   ├── UserInfo.cs           # Dados do usuário
│   │   └── ProcessingProgress.cs # Progresso do processamento
│   └── Services/                  # Serviços principais
│       ├── IVideoProcessingService.cs # Interface processamento
│       ├── VideoProcessingService.cs  # Processamento de vídeo
│       ├── IYoutubeService.cs         # Interface YouTube
│       ├── FuttageYouTubeService.cs   # Serviço YouTube
│       ├── IFileService.cs            # Interface arquivos
│       └── FileService.cs             # Serviço arquivos
├── Infrastructure/                # Infraestrutura
│   ├── Configuration/             # Configurações
│   ├── Extensions/                # Extensões DI
│   └── Logging/                   # Sistema de logs
├── Presentation/                  # Interface do usuário
│   ├── Views/                     # Formulários
│   ├── Presenters/                # Lógica de apresentação
│   └── Common/                    # Componentes UI
├── Resources/                     # Recursos (imagens, ícones)
├── Program.cs                     # Ponto de entrada
├── appsettings.json              # Configurações
└── ffmpeg.exe                    # FFmpeg para processamento
```

## 🚀 Como Usar

#### Concatenar Vídeos
1. Selecione múltiplos arquivos de vídeo
2. Configure o nome do arquivo de saída
3. Clique em "Concatenar"
4. Aguarde o processamento (barra de progresso)

#### Cortar Vídeo
1. Selecione um arquivo de vídeo
2. Defina o tempo de início e fim
3. Configure o nome do arquivo de saída
4. Clique em "Cortar"

#### Upload para YouTube
1. Após processar o vídeo, configure:
   - **Título** do vídeo
   - **Descrição**
   - **Thumbnail** (opcional)
   - **Tags** (opcional)
   - **Privacidade** (privado/público/não listado)
2. Clique em "Upload"
3. Aguarde o upload (barra de progresso)

### 4. Recursos Avançados
- **Preview do vídeo** antes do processamento
- **Informações detalhadas** (duração, resolução, tamanho)
- **Logs detalhados** para debugging
- **Interface moderna** com animações
- **Tratamento de erros** robusto

## ⚙️ Configuração

### appsettings.json
```json
{
  "YouTube": {
    "ClientId": "seu-client-id",
    "ClientSecret": "seu-client-secret",
    "ApplicationName": "Futtage"
  },
  "Video": {
    "DefaultQuality": "copy",
    "DeleteTempFiles": true
  }
}
```

### Variáveis de Ambiente
- `FUTTAGE_YOUTUBE_CLIENTID`: Client ID do YouTube
- `FUTTAGE_YOUTUBE_CLIENTSECRET`: Client Secret do YouTube

## 🔧 Requisitos

- **Sistema:** Windows 10 64-bit ou superior
- **Framework:** .NET 8.0 Runtime
- **FFmpeg:** Incluído no pacote
- **Internet:** Para upload no YouTube
- **Conta:** Google/YouTube válida

## 📁 Formatos Suportados

- **Entrada:** MP4, AVI, MOV, MKV, WMV, FLV
- **Saída:** MP4 (H.264)
- **Thumbnails:** JPG, PNG

## 🐛 Solução de Problemas

### FFmpeg não encontrado
- Verifique se `ffmpeg.exe` está na pasta do aplicativo
- Baixe FFmpeg em: https://www.gyan.dev/ffmpeg/builds/

### Erro de autenticação YouTube
- Verifique as credenciais no `appsettings.json`
- Faça logout e login novamente
- Verifique as permissões da conta

### Erro de processamento
- Verifique se o arquivo de vídeo não está corrompido
- Confirme se há espaço suficiente em disco
- Verifique os logs em `%LOCALAPPDATA%\Futtage\Logs\`

## 📞 Suporte

- **GitHub:** https://github.com/eaj1998/futtage
- **Email:** edipo1998@gmail.com
- **Issues:** Use o GitHub Issues para reportar bugs

---

**Copyright © 2025 EAJ. Todos os direitos reservados.**