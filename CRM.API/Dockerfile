# Fase de Base: Imagem ASP.NET para rodar a aplicação final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
# Define o ambiente para Produção na imagem final.
# A variável de ambiente do Docker Compose ou GitHub Actions pode sobrescrever isso.
ENV ASPNETCORE_ENVIRONMENT=Production

# Fase de Build: Imagem SDK para compilar e publicar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de solução (.sln) e os arquivos .csproj primeiro.
# Isso permite que o dotnet restore use o cache do Docker se apenas esses arquivos mudarem.
# Assumimos que o .sln está na raiz do contexto de build (que é a raiz do seu repositório)
# e que os projetos estão em subpastas (ex: CRM.API/, CRM.WEB/).
COPY *.sln ./
COPY CRM.API/*.csproj ./CRM.API/ 
# Copia o csproj da API
# Se houver outros projetos que a API dependa, copie os csproj deles também:
# COPY CRM.Infrastructure/*.csproj ./CRM.Infrastructure/
# COPY CRM.Application/*.csproj ./CRM.Application/

# Restaura as dependências para a solução inteira.
# Isso garante que todas as referências de projeto sejam resolvidas.
RUN dotnet restore

# Copia o restante do código-fonte.
# O ponto . significa copiar tudo do contexto de build para o WORKDIR /src.
COPY . .

# Altera o diretório de trabalho para o diretório do projeto API para o publish
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
