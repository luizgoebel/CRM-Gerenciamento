# Fase de Base: Imagem ASP.NET para rodar a aplicação final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
# Opcional: Adicione esta linha se o Kestrel não estiver a escutar na porta 80 por padrão
# ENV ASPNETCORE_URLS=http://+:80

# Fase de Build: Imagem SDK para compilar e publicar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Define o diretório de trabalho dentro do contêiner para a fase de build
WORKDIR /src

# Copia todo o conteúdo da pasta 'src' do host para o WORKDIR /src no contêiner.
# Isso garante que a estrutura completa da sua solução (.sln e todos os projetos)
# seja replicada em /src dentro do contêiner.
COPY src/ .

# Restaura as dependências para a solução inteira.
# O .sln agora está em /src/CRM-API.sln dentro do contêiner.
RUN dotnet restore "CRM-API.sln"

# Altera o diretório de trabalho para o diretório do projeto API para o publish.
# O projeto API está em /src/CRM.API/ dentro do contêiner.
WORKDIR /src/CRM.API

# Publica a aplicação.
# O comando 'dotnet publish' será executado do WORKDIR atual (/src/CRM.API),
# e ele encontrará o 'CRM.API.csproj' no diretório atual.
RUN dotnet publish CRM.API.csproj -c Release -o /app/publish --no-restore

# Fase Final: Cria a imagem leve para produção
FROM base AS final
WORKDIR /app
# Copia os arquivos publicados da fase 'build' (de /app/publish) para o diretório de trabalho final (/app).
COPY --from=build /app/publish .
# Define o comando que será executado quando o contêiner iniciar.
ENTRYPOINT ["dotnet", "CRM.API.dll"]
