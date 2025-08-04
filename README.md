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

## 📞 Suporte

- **Issues**: [GitHub Issues](https://github.com/seu-usuario/futtage/issues)
- **Email**: [seu-email@exemplo.com](mailto:seu-email@exemplo.com)
- **Documentação**: [Wiki do Projeto](https://github.com/seu-usuario/futtage/wiki)

---

**Desenvolvido com ❤️ para facilitar a criação de conteúdo no YouTube**

*Se este projeto te ajudou, considere dar uma ⭐ no repositório!* 