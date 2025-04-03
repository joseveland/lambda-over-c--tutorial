# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Copying project's sources while compiling/pulish into /app/publish
WORKDIR /src
COPY ["AsyncHandler.csproj", "AsyncHandler/"]
RUN dotnet restore "AsyncHandler/AsyncHandler.csproj"
WORKDIR "/src/AsyncHandler"
COPY . .
RUN dotnet build "AsyncHandler.csproj" --configuration Release --output /app/build
# Change `linux-x64` standard (for lambdas running in cloud servers) ...
# ... with your required machine's RID if running locally https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
RUN dotnet publish "AsyncHandler.csproj" \
            --configuration Release \ 
            --runtime linux-x64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true
# Install all Playwright browser and its dependencies
RUN pwsh /app/publish/playwright.ps1 install
RUN pwsh /app/publish/playwright.ps1 install-deps

# Runtime stage
FROM public.ecr.aws/lambda/dotnet:8 AS runtime
# Copy dependencies from build image knowing `public.ecr.aws/lambda/dotnet:8` has a `LD_LIBRARY_PATH` pointing to `/usr/lib64` and `/lib64`
COPY --from=build /lib/x86_64-linux-gnu/*.so /lib64/
COPY --from=build /lib/x86_64-linux-gnu/*.so* /lib64/
COPY --from=build /usr/share/fonts/ /usr/share/fonts/
COPY --from=build /etc/fonts/ /etc/fonts/
# Copying all Playwright browsers installed in `~/.cache` folder
COPY --from=build /root/.cache/ms-playwright/ /root/.cache/ms-playwright/
# EXTRA for lambda's debugging purpose
ENV AWS_LAMBDA_LOG_LEVEL="DEBUG"
ENV AWS_LAMBDA_LOG_FORMAT="JSON"
# Copying ready to run project's published files
WORKDIR /var/task
COPY --from=build /app/publish .
# Let the AWS RIE know what handler method should be served
CMD ["AsyncHandler::AsyncHandler.IamTheClass::Conversion"]
