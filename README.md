# 🎥 Futtage - Concatenação, Corte e Upload de Vídeos para YouTube

Futtage é uma aplicação desktop desenvolvida em C# (.NET 8) que permite concatenar, cortar e fazer upload de vídeos MP4 para o YouTube, com interface gráfica intuitiva e suporte a thumbnail personalizada.

## ✨ Funcionalidades

- **Concatenação de Vídeos**: Junte múltiplos arquivos MP4 em um único vídeo
- **Ordenação Inteligente**: Os vídeos são automaticamente ordenados por data de criação
- **Gerenciamento de Lista**: Adicione, remova e reordene vídeos na lista de concatenação
- **Corte de Vídeo**: Corte o vídeo concatenado selecionando tempo de início e fim
- **Upload para YouTube**: Upload direto para o YouTube com metadados personalizados
- **Thumbnail Personalizada**: Selecione uma imagem personalizada ou use a padrão para o vídeo
- **Barra de Progresso**: Acompanhe o progresso do upload e da concatenação
- **Navegação por Etapas**: Interface baseada em abas para cada etapa do fluxo (seleção, junção, corte, thumbnail, upload)
- **Interface Intuitiva**: Interface gráfica amigável com feedback visual e mensagens de sucesso/erro

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
- Clique em "Selecionar Arquivo..." para escolher os arquivos MP4
- Os vídeos serão automaticamente ordenados por data de criação
- Use os botões de seta para reordenar manualmente se necessário
- Remova arquivos indesejados com o botão "X"
- Avance para a próxima etapa com "Próximo Passo"

### 2. Concatenar Vídeos
- Selecione pelo menos 2 vídeos
- Clique em "Juntar Vídeos"
- Escolha o local para salvar o vídeo final
- Aguarde o processamento com FFmpeg
- Avance para a etapa de corte automaticamente

### 3. Cortar Vídeo (Opcional)
- Defina o tempo de início e fim para cortar o vídeo concatenado
- Clique em "Cortar Vídeo" para salvar uma nova versão cortada
- Ou clique em "Pular Corte" para manter o vídeo inteiro

### 4. Selecionar Thumbnail
- Por padrão, uma thumbnail padrão será usada
- Clique em "Selecionar Capa" para escolher uma imagem personalizada (JPG, JPEG, PNG)
- Avance para a etapa de upload

### 5. Fazer Upload para YouTube
- Clique em "Fazer Upload"
- Preencha os detalhes do vídeo:
  - Título (obrigatório)
  - Descrição (opcional)
  - Marque se é conteúdo infantil
- Acompanhe o progresso do upload pela barra de progresso
- A thumbnail será aplicada automaticamente após o upload

## 📁 Estrutura do Projeto

```
Futtage/
├── Form1.cs                 # Formulário principal
├── FormDetalhesVideo.cs     # Formulário de detalhes do vídeo
├── FormAguarde.cs           # Formulário de progresso
├── Program.cs               # Ponto de entrada da aplicação
├── Resources/               # Recursos (imagens, thumbnails)
└── bin/Debug/               # Arquivos compilados e dependências
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

- **FFmpeg**: O projeto inclui o FFmpeg executável necessário para concatenação e corte
- **Autenticação**: Na primeira execução, será necessário autenticar com sua conta Google
- **Thumbnail**: O projeto inclui uma thumbnail padrão, mas permite selecionar uma personalizada
- **Limitações**: Apenas arquivos MP4 são suportados para concatenação e corte
- **Fluxo em Etapas**: A navegação é feita por abas, guiando o usuário por cada etapa do processo

## 🐛 Solução de Problemas

### Erro de Upload
- Verifique se as credenciais da API do YouTube estão corretas
- Certifique-se de que a API do YouTube Data v3 está habilitada
- Verifique se o canal tem permissões para upload

### Erro de Concatenação ou Corte
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