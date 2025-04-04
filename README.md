## Intro

* Install `dotnet` binary in your PC [matching the dotnet version
you will use on your lambda](https://dotnet.microsoft.com/download/dotnet)
for example `.NET 8.0`

* [Official documentation](https://docs.aws.amazon.com/lambda/latest/dg/csharp-image.html)
to create a project template usable to build/publish as lambda files

* AWS has templates and plugins usable within `dotnet` ecosystem
so install those once yo have the `dotnet` installation in place

    ```shell
    dotnet new install Amazon.Lambda.Templates
    dotnet tool install -g Amazon.Lambda.Tools
    ```
  * Take a look at the [available templates](https://github.com/aws/aws-lambda-dotnet#dotnet-cli-templates)
  * Take a look at [lambda plugin](https://github.com/aws/aws-extensions-for-dotnet-cli?tab=readme-ov-file#aws-extensions-for-net-cli)

* Set up your AWS environment [installing it](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-getting-started.html)
  and refreshing your credentials to be able to make real cloud actions
  against AWS servers

    ```shell
    curl "https://awscli.amazonaws.com/..." -o ...
    sudo ...
    aws configure sso
    aws sso login
    nano ~/.aws/config  # Adding extra AWS profiles in it
    ```


## Project creation

* Simply replace `SOME_NAME` with your project name

    ```shell
    dotnet new lambda.image.EmptyFunction --name SOME_NAME
    ```

* Notice a 1st `SOME_NAME` parent folder will be created, with
a basic project structure under like a `src` and `test` folders
holding a 2nd `SOME_NAME`-like set of folders, but specifically
at `SOME_NAME/src/SOME_NAME` you will find:
<br></br>

  * A `aws-lambda-tools-defaults.json` with the options needed
  for the plugin to user later at compilation, packaging and even
  uploading to AWS stages
<br></br>

  * A `Dockerfile` with a placeholder for building the lambda
  as a container image instead a `.zip` file
<br></br>

  * A `Readme.md` with extra instructions to fill-up that `Dockerfile`
  among others
<br></br>


## Docker build

* Following the instructions at `SOME_NAME/src/SOME_NAME/Readme.md` I'd
better summarized the final `SOME_NAME/src/SOME_NAME/Dockerfile` like:

    ```dockerfile
    # Build stage
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    COPY ["SOME_NAME.csproj", "SOME_NAME/"]
    RUN dotnet restore "SOME_NAME/SOME_NAME.csproj"
    WORKDIR "/src/SOME_NAME"
    COPY . .
    RUN dotnet build "SOME_NAME.csproj" --configuration Release --output /app/build
    # Change `linux-x64` standard (for lambdas running in cloud servers) ...
    # ... with your required machine's RID if running locally https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
    RUN dotnet publish "SOME_NAME.csproj" \
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
    CMD ["SOME_NAME::SOME_NAME.Function::FunctionHandler"]
    ```
    * Where it is using `mcr.microsoft.com/dotnet/sdk:...` image to
    run `dotnet publish ...` in the format that the lambda needs
<br></br>

     * And the `public.ecr.aws/lambda/dotnet:...` image to later run that
     published resultant
<br></br>

     * `SOME_NAME::SOME_NAME.Function::FunctionHandler` refers to the C#
     publish/compilation of a `DLLNAME::NAMESPACE.CLASSNAME::FUNCTIONNAME`
<br></br>

       * Where typically the project name `SOME_NAME` matches the `DLLNAME`
       abd `NAMESPACE` of the project created by the `lambda.image.EmptyFunction`
       template
       * However, **MAKE SURE** to change the values for `CLASSNAME`
       and `FUNCTIONNAME` according to your project's naming convention
<br></br>

* Build that Dockerfile moving to that `SOME_NAME/src/SOME_NAME`
folder and then run (Change `some_image_name` with any name. you want):

  ```shell
  docker build -t "some_image_name" .
  ```


## Docker run

* Use that previous image running a container as it already has
an entrypoint (`/lambda-entrypoint.sh` under the hood of the
[runtime image](https://gallery.ecr.aws/lambda/dotnet) referred as the
[RIE - Runtime Interface Emulator](https://github.com/aws/aws-lambda-runtime-interface-emulator)
) and arguments for that entrypoint (`CMD ["SOME_NAME.Function.FunctionHandler"]`
referred as the `RIE` handler which will be used to serve logic), so:

  ```shell
  docker run -it --rm -p 8080:8080 "some_image_name"
  ```
  **NOTE:** the default port of the `RIE` is the 8080, so I'm
  exposing that to consume it later via HTTP calls. `--rm`
  is just to remove the container when exiting from it


## Consuming the lambda

* Consumes the `RIE` to be specific as another HTTP server (at `8080`
port as seen previously) which is serving a specific function/handler
that `SOME_NAME` project is representing. Respect the
endpoint to consume the `2015-03-31/functions/function/invocations`
MUST be used

  ```shell
  curl "http://localhost:8080/2015-03-31/functions/function/invocations" \
  -H "content-type: text/plain" \
  -d '"i am the payload!"' -w "\n%{http_code}\n"
  ```


## Deploying

* Don't forget to refresh your AWS credentials to really have cloud
access through the `Amazon.Lambda.Tools` plugin

    ```shell
    aws sso login
    ```

* Until this point I haven't used the `aws-lambda-tools-defaults.json`
  at all I inserted into the `Dockerfile`; however this `.json`
  holds information about the lambda deployment against AWS servers
  so make sure the `"image-command"` identifier of the handler matches
  the dockerfile that you tested locally for the plugin to deploy
  that `CMD` into AWS exactly the same:
    ```json
    {
    ...
    "image-command": "SOME_NAME::SOME_NAME.Function::FunctionHandler",
    ...
    }
    ```

* So next thing is using that file to really put something in cloud
with the help of the previously installed plugin by moving the
folder `SOME_NAME/src/SOME_NAME` (where the `.csproj` and `.json` live
together) and then run:

    ```shell
    dotnet lambda deploy-function
    ```
    * This is an approach where you avoid the previous `docker build ...`
    command but adds AWS-ECR upload and AWS-Lambda deployment
<br></br>
    * Good thing it auto-recognizes `SOME_NAME`, publish the
    resultants (`.dll`s and others) under the `./bin/Release/lambda-publish`
    folder and creates a docker image named in lowercase naming
    convention (`some_name`)
<br></br>
      * Only pending running that while exposing the port
      `docker run -it --rm -p 8080:8080 "some_name"`
<br></br>

      * **HOWEVER**, you still need to write down your `Dockerfile`
      for this to really create that docker image and continues
<br></br>

    * This is a quicker approach to build the docker image as
      it auto-recognizes `SOME_NAME` and builds everything
      inside the `./bin/Release/lambda-publish` folder
<br></br>

* Another variation of the previous command is adding/indicating
specific [options as long as available](https://github.com/aws/aws-extensions-for-dotnet-cli?tab=readme-ov-file#deploy-function)

    ```shell
    dotnet lambda deploy-function "SOME_LAMBDA_NAME" \
      --profile "SOME_LOCAL_AWS_PROFILE_NAME" \
      --function-role "SOME_AWS_ROLE_NAME_TO_BIND_WITH_YOUR_LAMBDA"
    ```
    * For example, you might need a `SOME_LOCAL_AWS_PROFILE_NAME` to let
    the plugin know the local project of your AWS setup
    to be used (Ex: `biodashboard-dev`)
    * Or `SOME_AWS_ROLE_NAME_TO_BIND_WITH_YOUR_LAMBDA` role already set
    on your AWS project (Ex: `gdr-agent-role`) that your lambda will use
    when running at AWS servers (Lambda resource to be specific)
    * etc...
