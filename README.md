# Webinar.Dynamo
Example use Dynamo in C#

## Links of Interest
- [Implementación de DynamoDB localmente en el equipo](https://docs.aws.amazon.com/es_es/amazondynamodb/latest/developerguide/DynamoDBLocal.DownloadingAndRunning.html)
- [Scan Description](https://docs.aws.amazon.com/cli/latest/reference/dynamodb/scan.html)
- [Amazon DynamoDB Local Integration with AWS Toolkit for Visual Studio](https://aws.amazon.com/es/blogs/developer/amazon-dynamodb-local-integration-with-aws-toolkit-for-visual-studio/)
- [Ejemplos de código .NET](https://docs.aws.amazon.com/es_es/amazondynamodb/latest/developerguide/CodeSamples.DotNet.html)

## Init project
`docker run -p 8000:8000 amazon/dynamodb-local`

## Create Table - Dynamo
`aws dynamodb create-table --table-name State --attribute-definitions AttributeName=Country,AttributeType=S AttributeName=Code,AttributeType=S --key-schema AttributeName=Country,KeyType=HASH AttributeName=Code,KeyType=RANGE --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 --endpoint-url http://localhost:8000`

## List Tables - Dynamo
`aws dynamodb list-tables --endpoint-url http://localhost:8000`

## List Element by Scan
`aws dynamodb scan --table-name State --endpoint-url http://localhost:8000`