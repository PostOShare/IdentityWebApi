<p align="center"><img src="https://github.com/PostOShare/IdentityWebApi/assets/17848426/b2a55da5-dc7f-4b33-9de6-e6d0c641a752" alt="logo"></p>

<div align="center">
  
[![GitHub Issues](https://img.shields.io/github/issues/PostOShare/IdentityWebApi.svg)](https://github.com/PostOShare/IdentityWebApi/issues)
[![GitHub license](https://img.shields.io/github/license/PostOShare/IdentityWebApi)](https://github.com/PostOShare/IdentityWebApi/blob/master/LICENSE)
![Static Badge](https://img.shields.io/badge/.net-6)

</div>

IdentityWebApi is a .NET 6 Web API used to manage user data and authentication in PostOShare. It depends on [.NET 6](https://dotnet.microsoft.com/en-us/) and communicates with a [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2022) database using [Entity Framework core](https://learn.microsoft.com/en-us/ef/core/).

# Prerequisites

The following need to be available to ensure that the API and the SQL Server database can be setup:

- [.NET 6](https://dotnet.microsoft.com/en-us/)
- [Visual Studio 2022 or higher](https://visualstudio.microsoft.com/downloads/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2022)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)
- [AWS user account](https://aws.amazon.com/)
  
# Installation

Steps that can be used to setup the API are

```cmd
git clone https://github.com/PostOShare/IdentityWebApi.git

cd IdentityWebApi\IdentityWebApi
dotnet restore
cd ..\EntityORM
dotnet restore
```

The API and the SQL Server instance need to be published to a cloud provider to ensure that remote connections can call the API. [AWS](https://aws.amazon.com/) is used as the provider to publish the API and host the instance. Steps to deploy the API and the instance are

- [Create a VPC to deploy the Identity API](https://github.com/PostOShare/IdentityWebApi/wiki/Create-a-VPC-to-deploy-the-Identity-API)
- [Installation of DB](https://github.com/PostOShare/IdentityWebApi/wiki/Installation-of-DB)
- [Create a Lambda function, HTTP API and deploy the Identity API](https://github.com/PostOShare/IdentityWebApi/wiki/Create-a-Lambda-function,-HTTP-API-and-deploy-the-Identity-API)

# Deployment

The architecture of the deployment of the API and the SQL Server instance in AWS is illustrated below:

![Architecture of API deployment](https://github.com/PostOShare/IdentityWebApi/assets/17848426/576dbbbd-b03b-4190-8e3c-0345017da049)

# Dependencies

## Production dependencies

-  Amazon.Lambda.AspNetCoreServer                       | Version 7.2.0 
-  Amazon.Lambda.AspNetCoreServer.Hosting               | Version 1.3.1 
-  MailKit                                              | Version 4.3.0 
-  Microsoft.AspNetCore.Authentication.JwtBearer        | Version 6.0.26 
-  Microsoft.AspNetCore.Identity.EntityFrameworkCore    | Version 6.0.26 
-  Microsoft.AspNetCore.Identity.UI                     | Version 6.0.26 
-  Microsoft.EntityFrameworkCore.SqlServer              | Version 6.0.26 
-  Microsoft.EntityFrameworkCore.Tools                  | Version 6.0.26

## Development dependencies

These dependencies are used only in development:
- Swashbuckle.AspNetCore                                | Version 6.5.0
- Swashbuckle.AspNetCore.Annotations                    | Version 6.5.0

# Endpoints of the API

## api/v1/auth/login-identity

This endpoint is used to check whether login details are available.

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

- 200 - User exists
  	
  **Sample Response body**
  ```json
  {
    "refreshToken": "w0czWF0pbdd9hB4h2d1YF+I3ctdzpcfUaOmKagmsy10=",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImdkZmdkIiwibmJmIjoxNzA5MzA0ODk5LCJleHAiOjE3MDkzMDU3OTksImlhdCI6MTcwOTMwNDg5OX0.Hw1GmtW4O245qfD11cHOCQtQ91p2inAOlm6cIjL31rU",
    "result": true,
    "error": ""
  }
  ```

- 400 - Invalid request, Invalid username and/or password (User does not exist)
- 500 - An internal error occurred

  **Response body**
  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An internal error occurred"
  }
  ```

## api/v1/auth/register-identity

This endpoint is used to register the user data with the given username.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/register-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "password": "password",
  "title": "mr.",
  "firstName": "Edwin",
  "lastName": "Doe",
  "suffix": "",
  "emailAddress": "edwar123@outlook.com",
  "phone": "1234561234",
  "userRole": "user"
}'
```

### Responses

- 201 - User created
- 400 - Invalid request, The given account could not be registered (User exists)
- 500 - An error occurred when adding user

  **Response body**
  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An error occurred when adding user"
  }
  ```

## api/v1/auth/search-identity

This endpoint is used to check whether a user exists. Please note that the values for OTP and Password fields are not validated, but should be passed when making a request. 

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/search-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "emailAddress": "edwar123@outlook.com",
  "otp": 0,
  "password": "password"
}'
```

### Responses

- 200 - User exists
  	
  **Sample Response body**
  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": true,
    "error": ""
  }
  ```

- 400 - Invalid username and/or email (User does not exist), Invalid request

## api/v1/auth/verify-identity

This endpoint is used to generate an OTP, save the OTP to DB and send the OTP to the user's email. Please note that the values for OTP and Password fields are not validated, but should be passed when making a request.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/verify-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "emailAddress": "edwar123@outlook.com",
  "otp": 0,
  "password": "password"
}'
```

### Responses

- 201 - Created
- 400 - Invalid username and/or email (User does not exist), Invalid request
- 500 - An error occurred when adding user, InternalServerError (Error when sending email)

## api/v1/auth/validate-passcode-identity

This endpoint is used to check if the OTP response sent when validating a user is valid. Please note that the values for Email and Password fields are not validated, but should be passed when making a request.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/validate-passcode-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "emailAddress": "edwar123@outlook.com",
  "otp": 236784,
  "password": "password"
}'
```

### Responses

- 200 - OTP is valid
- 400 - OTP is invalid, Invalid request
- 500 - An error occurred when validating the OTP

  **Response body**
  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An error occurred when validating the OTP"
  }
  ```

  500 - Cannot try more than maximum attempts

  **Response body**
  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "Cannot try more than maximum attempts"
  }
  ```

## api/v1/auth/change-credentials-identity

This endpoint is used to update key and salt of a user based on password sent in the request. Please note that the values for Email and OTP fields are not validated, but should be passed when making a request.

### Sample request

```
curl -X 'PATCH' \
  'https://localhost:7224/api/v1/auth/change-credentials-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "emailAddress": "edwar123@outlook.com",
  "otp": 0,
  "password": "password"
}'
```

### Responses

- 200 - Key and Salt for the user were updated

```json
{
  "refreshToken": "",
  "accessToken": "",
  "result": true,
  "error": ""
}
```

- 304 - Key and Salt for the user were not updated

```json
{
  "refreshToken": "",
  "accessToken": "",
  "result": false,
  "error": "An error occurred when updating password"
}
```

- 400 - Invalid request
