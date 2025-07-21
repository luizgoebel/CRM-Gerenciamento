# Fase de Base: Imagem ASP.NET para rodar a aplicação final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
# Define o ambiente para Produção na imagem final.
# A variável de ambiente do Docker Compose ou GitHub Actions pode sobrescrever isso.
ENV ASPNETCORE_ENVIRONMENT=Production

# Fase de Build: Imagem SDK para compilar e publicar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src # Define o diretório de trabalho dentro do contêiner como /src

# Copia o conteúdo da pasta 'src' do host (onde está o .sln e os projetos)
# para o WORKDIR /src no contêiner.
# Isso garante que a estrutura de pastas do seu projeto seja replicada dentro do contêiner.
COPY src/ .

# Restaura as dependências para a solução inteira.
# O .sln agora está em /src/CRM-API.sln dentro do contêiner.
RUN dotnet restore "CRM-API.sln"

# Altera o diretório de trabalho para o diretório do projeto API para o publish
# O projeto API está em /src/CRM.API/ dentro do contêiner.
WORKDIR /src/CRM.API

# Publica a aplicação.
# O --no-restore é usado porque já restauramos as dependências no nível da solução.
RUN dotnet publish CRM.API.csproj -c Release -o /app/publish --no-restore

# Fase Final: Cria a imagem leve para produção
FROM base AS final
WORKDIR /app
# Copia os arquivos publicados do estágio de build para o estágio final
COPY --from=build /app/publish .
# Define o comando que será executado quando o contêiner iniciar
ENTRYPOINT ["dotnet", "CRM.API.dll"]
