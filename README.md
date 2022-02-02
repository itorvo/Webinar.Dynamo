# Webinar.Dynamo
Example use Dynamo in C#

## Nugets used
| Nuget | Version | Source |
| - | - | - |
| AWSSDK.DynamoDBv2 | 3.7.0.13 | [nuget.org](https://www.nuget.org/) |
| Microsoft.Extensions.Configuration | 5.0.0 | [nuget.org](https://www.nuget.org/) |
| Newtonsoft.Json | 13.0.1 | [nuget.org](https://www.nuget.org/) |

## Links of Interest
- [Implementación de DynamoDB localmente en el equipo](https://docs.aws.amazon.com/es_es/amazondynamodb/latest/developerguide/DynamoDBLocal.DownloadingAndRunning.html)
- [Scan Description](https://docs.aws.amazon.com/cli/latest/reference/dynamodb/scan.html)
- [Amazon DynamoDB Local Integration with AWS Toolkit for Visual Studio](https://aws.amazon.com/es/blogs/developer/amazon-dynamodb-local-integration-with-aws-toolkit-for-visual-studio/)
- [Ejemplos de código .NET](https://docs.aws.amazon.com/es_es/amazondynamodb/latest/developerguide/CodeSamples.DotNet.html)
- [New – Amazon DynamoDB Transactions](https://aws.amazon.com/es/blogs/aws/new-amazon-dynamodb-transactions/)
- [Ejemplo de transacciones de DynamoDB](https://docs.aws.amazon.com/es_es/amazondynamodb/latest/developerguide/transaction-example.html)
- [UpdateItem](https://docs.aws.amazon.com/es_es/amazondynamodb/latest/APIReference/API_UpdateItem.html)
- [Batch Operations Using the AWS SDK for .NET Object Persistence Model](https://docs.amazonaws.cn/en_us/amazondynamodb/latest/developerguide/DotNetDynamoDBContext.BatchOperations.html)
## Init project
`docker run -p 8000:8000 amazon/dynamodb-local:latest`

### Create Table State
`aws dynamodb create-table --table-name Location --attribute-definitions AttributeName=Country,AttributeType=S AttributeName=Code,AttributeType=S --key-schema AttributeName=Country,KeyType=HASH AttributeName=Code,KeyType=RANGE --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 --endpoint-url http://localhost:8000`

### Create Table Country
`aws dynamodb create-table --table-name Country --attribute-definitions AttributeName=Code,AttributeType=S AttributeName=Name,AttributeType=S --key-schema AttributeName=Code,KeyType=HASH AttributeName=Name,KeyType=RANGE --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 --endpoint-url http://localhost:8000`

### Create Index
`aws dynamodb update-table --table-name State --attribute-definitions AttributeName=Name,AttributeType=S --global-secondary-index-updates file://gsi.json --endpoint-url http://localhost:8000`

## List Tables - Dynamo
`aws dynamodb list-tables --endpoint-url http://localhost:8000`

## List Element by Scan
`aws dynamodb scan --table-name Country --endpoint-url http://localhost:8000`

## Delete table
`aws dynamodb delete-table --table-name State --endpoint-url http://localhost:8000`