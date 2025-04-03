# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FirstOne.csproj", "FirstOne/"]
RUN dotnet restore "FirstOne/FirstOne.csproj"
WORKDIR "/src/FirstOne"
COPY . .
RUN dotnet build "FirstOne.csproj" --configuration Release --output /app/build
# Change `linux-x64` standard (for lambdas running in cloud servers) ...
    # ... with your required machine's RID if running locally https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
RUN dotnet publish "FirstOne.csproj" \
            --configuration Release \ 
            --runtime linux-x64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true

# Runtime stage
FROM public.ecr.aws/lambda/dotnet:8 AS runtime
# EXTRAS for debugging purpose
ENV AWS_LAMBDA_LOG_LEVEL="DEBUG"
ENV AWS_LAMBDA_LOG_FORMAT="JSON"
WORKDIR /var/task
COPY --from=build /app/publish .
CMD ["FirstOne::FirstOne.Function::FunctionHandler"]
