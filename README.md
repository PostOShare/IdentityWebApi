<p align="center"><img src="https://github.com/PostOShare/IdentityWebApi/assets/17848426/b2a55da5-dc7f-4b33-9de6-e6d0c641a752" alt="logo"></p>

<div align="center">
  
[![GitHub Issues](https://img.shields.io/github/issues/PostOShare/IdentityWebApi.svg)](https://github.com/PostOShare/IdentityWebApi/issues)
[![GitHub license](https://img.shields.io/github/license/PostOShare/IdentityWebApi)](https://github.com/PostOShare/IdentityWebApi/blob/master/LICENSE)
![Static Badge](https://img.shields.io/badge/.net-6)

</div>

IdentityWebApi is a .NET 6 Web API used to manage user data and authentication in PostOShare. It depends on [.NET 6](https://dotnet.microsoft.com/en-us/) and communicates with a [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2022) using [Entity Framework core](https://learn.microsoft.com/en-us/ef/core/).

# Prerequisites

The following need to be available to ensure that the API and the SQL server database can be published

- [.NET 6](https://dotnet.microsoft.com/en-us/)
- [Visual Studio 2022 or higher](https://visualstudio.microsoft.com/downloads/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2022)
- [AWS user account](https://aws.amazon.com/)
  
# Installation

Steps that can be used to setup the API are

```
git clone https://github.com/PostOShare/IdentityWebApi.git

# Restore packages

cd IdentityWebApi\IdentityWebApi
dotnet restore
cd ..\EntityORM
dotnet restore
```

The API and the SQL Server instance need to be published to a cloud provider to ensure that remote connections can call the API. [AWS](https://aws.amazon.com/) is used to publish the API and host the instance. Steps to deploy the API and the instance are

- [Create a VPC to deploy the Identity API](https://github.com/PostOShare/IdentityWebApi/wiki/Create-a-VPC-to-deploy-the-Identity-API)
- [Installation of DB](https://github.com/PostOShare/IdentityWebApi/wiki/Installation-of-DB)
- [Create a Lambda function, HTTP API and deploy the Identity API](https://github.com/PostOShare/IdentityWebApi/wiki/Create-a-Lambda-function,-HTTP-API-and-deploy-the-Identity-API)

# AWS Deployment

The API deployment in AWS is illustrated below:

![Architecture of API deployment](https://github.com/PostOShare/IdentityWebApi/assets/17848426/79c4a4f6-56be-4b6b-9ad1-c18bcb527202)

# Dependencies

## Production dependencies

-  Amazon.Lambda.AspNetCoreServer Version 7.2.0 
-  Amazon.Lambda.AspNetCoreServer.Hosting Version 1.3.1 
-  MailKit Version 4.3.0 
-  Microsoft.AspNetCore.Authentication.JwtBearer Version 6.0.26 
-  Microsoft.AspNetCore.Identity.EntityFrameworkCore Version 6.0.26 
-  Microsoft.AspNetCore.Identity.UI Version 6.0.26 
-  Microsoft.EntityFrameworkCore.SqlServer Version 6.0.26 
-  Microsoft.EntityFrameworkCore.Tools Version 6.0.26

## Development dependencies

- Swashbuckle.AspNetCore Version 6.5.0
- Swashbuckle.AspNetCore.Annotations Version 6.5.0

# Endpoints of the API

## api/v1/auth/login-identity

This endpoint is used to send user login details and check whether they are available.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/login-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "username",
  "password": "password",
  "registeredDate": "2024-02-28T15:01:55.693Z",
  "lastLoginTime": "2024-02-28T15:01:55.693Z",
  "userRole": "userRole",
  "isActive": true
}'
```

### Responses

| Status              | Status code | Reason                                        |
| -----------------   | ----------- | --------------------------------------------- |
|OK                    | 200        | Valid login                                   |
|Bad Request           | 400        | Invalid login or data is not in correct format|
|Internal Server Error | 500        | Server error                                  |

