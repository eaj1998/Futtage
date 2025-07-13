# 🎥 Futtage - Concatenação e Upload de Vídeos para YouTube

Futtage é uma aplicação desktop desenvolvida em C# (.NET 8) que permite concatenar múltiplos vídeos MP4 e fazer upload direto para o YouTube com uma interface gráfica intuitiva.

## ✨ Funcionalidades

- **Concatenação de Vídeos**: Junte múltiplos arquivos MP4 em um único vídeo
- **Ordenação Inteligente**: Os vídeos são automaticamente ordenados por data de criação
- **Gerenciamento de Lista**: Adicione, remova e reordene vídeos na lista de concatenação
- **Upload para YouTube**: Upload direto para o YouTube com metadados personalizados
- **Thumbnail Automático**: Aplicação automática de thumbnail personalizado
- **Interface Intuitiva**: Interface gráfica amigável com barras de progresso

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0** - Framework de desenvolvimento
- **Windows Forms** - Interface gráfica
- **Google.Apis.YouTube.v3** - API do YouTube para upload
- **FFmpeg** - Processamento de vídeo
- **DotNetEnv** - Gerenciamento de variáveis de ambiente

## 📋 Pré-requisitos

- Windows 10 ou superior
- .NET 8.0 Runtime
- FFmpeg (incluído no projeto)
- Conta do Google com API do YouTube habilitada

## ⚙️ Configuração

### 1. Configuração da API do YouTube

1. Acesse o [Google Cloud Console](https://console.cloud.google.com/)
2. Crie um novo projeto ou selecione um existente
3. Habilite a API do YouTube Data v3
4. Crie credenciais OAuth 2.0
5. Baixe o arquivo de credenciais

### 2. Configuração das Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto com as seguintes variáveis:

```env
YOUTUBE_CLIENT_ID=seu_client_id_aqui
YOUTUBE_CLIENT_SECRET=seu_client_secret_aqui
```

## 🚀 Como Usar

### 1. Selecionar Vídeos
- Clique em "Selecionar Vídeos" para escolher os arquivos MP4
- Os vídeos serão automaticamente ordenados por data de criação
- Use os botões de seta para reordenar manualmente se necessário

### 2. Concatenar Vídeos
- Selecione pelo menos 2 vídeos
- Clique em "Juntar Vídeos"
- Escolha o local para salvar o vídeo final
- Aguarde o processamento com FFmpeg

### 3. Fazer Upload para YouTube
- Após a concatenação, clique em "Fazer Upload"
- Preencha os detalhes do vídeo:
  - Título (obrigatório)
  - Descrição (opcional)
  - Marque se é conteúdo infantil
- Aguarde o upload e a aplicação da thumbnail

## 📁 Estrutura do Projeto

```
Futtage/
├── Form1.cs                 # Formulário principal
├── FormDetalhesVideo.cs     # Formulário de detalhes do vídeo
├── FormAguarde.cs          # Formulário de progresso
├── Program.cs              # Ponto de entrada da aplicação
├── Resources/              # Recursos (imagens, thumbnails)
└── bin/Debug/              # Arquivos compilados e dependências
```

## 🔧 Compilação

1. Clone o repositório
2. Abra o projeto no Visual Studio ou use o comando:
   ```bash
   dotnet build
   ```
3. Execute a aplicação:
   ```bash
   dotnet run
   ```

## 📝 Notas Importantes

- **FFmpeg**: O projeto inclui o FFmpeg executável necessário para concatenação
- **Autenticação**: Na primeira execução, será necessário autenticar com sua conta Google
- **Thumbnail**: O projeto inclui uma thumbnail padrão que será aplicada automaticamente
- **Limitações**: Apenas arquivos MP4 são suportados para concatenação

## 🐛 Solução de Problemas

### Erro de Upload
- Verifique se as credenciais da API do YouTube estão corretas
- Certifique-se de que a API do YouTube Data v3 está habilitada
- Verifique se o canal tem permissões para upload

### Erro de Concatenação
- Certifique-se de que todos os vídeos são MP4 válidos
- Verifique se há espaço suficiente no disco
- Confirme se o FFmpeg está presente na pasta bin

### Erro de Thumbnail
- O canal precisa ser verificado no YouTube
- Verifique se o recurso de thumbnail está disponível

## 📄 Licença

Este projeto é de uso pessoal e educacional.

## 🤝 Contribuições

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou pull requests.

---

**Desenvolvido com ❤️ para facilitar a criação de conteúdo no YouTube** 