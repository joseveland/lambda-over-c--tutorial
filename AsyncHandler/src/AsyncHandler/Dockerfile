# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AsyncHandler.csproj", "AsyncHandler/"]
RUN dotnet restore "AsyncHandler/AsyncHandler.csproj"
WORKDIR "/src/AsyncHandler"
COPY . .
RUN dotnet build "AsyncHandler.csproj" --configuration Release --output /app/build
RUN dotnet publish "AsyncHandler.csproj" \
            --configuration Release \ 
            --runtime linux-x64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

# Run stage
FROM public.ecr.aws/lambda/dotnet:8 AS runtime
# EXTRAS for debugging purpose
ENV AWS_LAMBDA_LOG_LEVEL="DEBUG"
ENV AWS_LAMBDA_LOG_FORMAT="JSON"
WORKDIR /var/task
COPY --from=build /app/publish .
CMD ["AsyncHandler::AsyncHandler.IamTheClass::Convertion"]
