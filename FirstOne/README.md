
## Compile and run locally

* Move to the project folder where the `Dockerfile` lives

    ```shell
    cd src/FirstOne
    ```

* Compile and Run

    ```shell
    docker build -t "some_image_name" .
    docker run -it --rm -p 8080:8080 "some_image_name"
    ```

* Consume (Smoke-Test) locally using curl

    ```shell
    curl "http://localhost:8080/2015-03-31/functions/function/invocations" \
    -H "content-type: text/plain" \
    -d '"i am the payload!"' -w "\n%{http_code}\n"
    ```


## Deploying to AWS

* Don't forget to refresh your AWS credentials to really have cloud
  access through the `Amazon.Lambda.Tools` plugin

    ```shell
    aws sso login
    ```

* Again from the project folder `src/AsyncHandler`, use the
  `Amazon.Lambda.Tools` plugin by running the required command to
  all-in-one: `Docker-build + ECR-upload + Lambda-deploy`, like so:

    ```shell
    dotnet lambda deploy-function "SOME_LAMBDA_NAME" \
      --profile "SOME_LOCAL_AWS_PROFILE_NAME" \
      --function-role "SOME_AWS_ROLE_NAME_TO_BIND_WITH_YOUR_LAMBDA"
    ```
