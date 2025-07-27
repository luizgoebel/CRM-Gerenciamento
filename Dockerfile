# Fase de Base: Imagem ASP.NET para rodar a aplicação final
# Usa a imagem oficial do ASP.NET 8.0 para o ambiente de execução.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Define o diretório de trabalho dentro do contêiner para a aplicação.
WORKDIR /app
# Expõe a porta 80 do contêiner. O Elastic Beanstalk espera que a aplicação escute nesta porta.
EXPOSE 80
# Garante que o Kestrel (servidor web do ASP.NET Core) escute na porta 80 em todas as interfaces.
ENV ASPNETCORE_URLS=http://+:80

# Fase de Build: Imagem SDK para compilar e publicar a aplicação
# Usa a imagem oficial do SDK do .NET 8.0 para as operações de build.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Define o diretório de trabalho dentro do contêiner para a fase de build.
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
# '-c Release': Publica na configuração de Release.
# '-o /app/publish': Define o diretório de saída para os arquivos publicados.
# '--no-restore': Evita uma restauração duplicada de pacotes.
RUN dotnet publish CRM.API.csproj -c Release -o /app/publish --no-restore

# Fase Final: Cria a imagem leve para produção
# Usa a imagem 'base' (runtime do ASP.NET) para a imagem final de produção, que é menor.
FROM base AS final
# Define o diretório de trabalho dentro do contêiner final.
WORKDIR /app
# Copia os arquivos publicados da fase 'build' (de /app/publish) para o diretório de trabalho final (/app).
COPY --from=build /app/publish .
# Define o comando que será executado quando o contêiner iniciar.
# 'dotnet CRM.API.dll': Inicia a sua aplicação ASP.NET Core.
ENTRYPOINT ["dotnet", "CRM.API.dll"]
