<p align="center"><img src="https://github.com/PostOShare/IdentityWebApi/assets/17848426/b2a55da5-dc7f-4b33-9de6-e6d0c641a752" alt="logo"></p>

<div align="center">
  
[![GitHub Issues](https://img.shields.io/github/issues/PostOShare/IdentityWebApi.svg)](https://github.com/PostOShare/IdentityWebApi/issues)
[![GitHub license](https://img.shields.io/github/license/PostOShare/IdentityWebApi)](https://github.com/PostOShare/IdentityWebApi/blob/master/LICENSE)
![Static Badge](https://img.shields.io/badge/.net-6-green)

</div>

IdentityWebApi is a .NET 6 Web API used to manage user data and authentication in PostOShare. It depends on [.NET 6](https://dotnet.microsoft.com/en-us/) and communicates with a [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2022) database using [Entity Framework core](https://learn.microsoft.com/en-us/ef/core/).

# Prerequisites

The following need to be available to ensure that the API and the SQL Server database can be setup:

- [.NET 6](https://dotnet.microsoft.com/en-us/)
- [Visual Studio 2022 or higher](https://visualstudio.microsoft.com/downloads/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2022)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)
  
# Installation

Steps that can be used to setup the API are

```cmd
git clone https://github.com/PostOShare/IdentityWebApi.git

cd IdentityWebApi\IdentityWebApi
dotnet restore
cd ..\EntityORM
dotnet restore
```

# Dependencies

## Production dependencies

-  MailKit
-  Microsoft.AspNetCore.Authentication.JwtBearer
-  Microsoft.AspNetCore.Identity.EntityFrameworkCore
-  Microsoft.AspNetCore.Identity.UI
-  Microsoft.EntityFrameworkCore.SqlServer
-  Microsoft.EntityFrameworkCore.Tools
-  Serilog.AspNetCore
-  Serilog.Sinks.File

## Development dependencies

These dependencies are used only in development:

- Swashbuckle.AspNetCore
- Swashbuckle.AspNetCore.Annotations

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
  "password": "password"
}'
```

### Responses

- 200 - User exists
  	
  ```json
  {    
    "result": true,
    "errorDescription": "",
    "errorCode": ""
  }
  ```

- 400 -

  ```json
  {
    "result": false,
    "errorDescription": "Invalid username and/or password",
    "errorCode": "0001"
  }
  ```
  
- 500 -

  ```json
  {
    "result": false,
    "errorDescription": "Internal server error",
    "errorCode": "0008"
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

- 201 -

  ```json
  {
    "result": true,
    "errorDescription": "Success",
    "errorCode": "0000"
  }
  ```
  
- 400 -

  ```json
  {
    "result": true,
    "errorDescription": "Please choose a different username and/or password",
    "errorCode": "0002"
  }
  ```
  
- 500 - 

  ```json
  {
    "result": false,
    "errorDescription": "Internal server error",
    "errorCode": "0008"
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
  "emailAddress": "email@email.com"
}'
```

### Responses

- 200 - User exists
  	
  ```json
  {
    "result": true,
    "errorDescription": "Success",
    "errorCode": "0000"
  }
  ```

- 400 -
  
  ```json
  {
    "result": false,
    "errorDescription": "Invalid username and/or password",
    "errorCode": "0001"
  }
  ```
  
- 500 - An internal error occurred

  ```json
  {
    "result": false,
    "errorDescription": "Internal server error",
    "errorCode": "0008"
  }
  ```

## api/v1/auth/verify-identity

This endpoint is used to generate an OTP, save the OTP to DB and send the OTP to the user's email. Please note that the values for OTP and Password fields are not validated, but should be passed when making a request.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/verify-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user"
}'
```

### Responses

- 201 -

  ```json
  {
    "result": true,
    "errorDescription": "Success",
    "errorCode": "0000"
  }
  ```
  
- 400 -
  
  ```json
  {
    "result": false,
    "errorDescription": "Invalid username and/or password",
    "errorCode": "0001"
  }
  ```
  
- 500 - An internal error occurred 

  ```json
  {
    "result": false,
    "errorDescription": "Internal server error",
    "errorCode": "0008"
  }
  ```
  
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
  "otp": 2367
}'
```

### Responses

- 200 -
  
  ```json
  {
    "result": true,
    "errorDescription": "Success",
    "errorCode": "0000"
  }
  ```
  
- 400 -
  
  ```json
  {
    "result": false,
    "errorDescription": "Invalid username and/or password",
    "errorCode": "0001"
  }
  ```
  
  ```json
  {
    "result": false,
    "errorDescription": "Invalid OTP",
    "errorCode": "0003"
  }
  ```
  
  ```json
  {
    "result": false,
    "errorDescription": "Cannot try more than maximum attempts",
    "errorCode": "0007"
  }
  ```
  
- 500 - An internal error occurred 

  ```json
  {
    "result": false,
    "errorDescription": "Internal server error",
    "errorCode": "0008"
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
  "password": "password"
}'
```

### Responses

- 200 - Key and Salt for the user were updated
 
  ```json
  {
    "result": true,
    "errorDescription": "Success",
    "errorCode": "0000"
  }
  ```
  
- 400 -
  
  ```json
  {
    "result": false,
    "errorDescription": "Invalid username and/or password",
    "errorCode": "0001"
  }
  ```
  
- 500 - An internal error occurred 

  ```json
  {
    "result": false,
    "errorDescription": "Internal server error",
    "errorCode": "0008"
  }
  ```
  
## api/v1/auth/generate-accessToken

This endpoint is used to create an access token based on the user's refresh token. Please note that the access token is not validated, but should be passed when making a request.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/generate-accessToken' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "currentUserId": "user"
}'
```

### Responses

- 201 - Access token was generated

  **Sample Response**
 
  ```json
  {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImdkZmdkIiwibmJmIjoxNzA5NjUwMzgwLCJleHAiOjE3MDk2NTEyODAsImlhdCI6MTcwOTY1MDM4MH0.D-JUimEo_6UDQvGf_ZggyXM_XoXEIaJ6R_RErMK0qa8",
    "result": true,
    "errorDescription": "",
    "errorCode": ""
  }
  ```

- 400 -
  
  ```json
  {
    "result": false,
    "errorDescription": "Invalid username and/or expired token",
    "errorCode": "0006"
  }
  ```
  
- 500 - An internal error occurred 

  ```json
  {
    "result": false,
    "errorDescription": "Internal server error",
    "errorCode": "0008"
  }
  ```

## api/v1/auth/validate-accessToken

This endpoint is used to validate an access token. Please note that the refresh token is not validated, but should be passed when making a request.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/validate-accessToken' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "refreshToken": "w0czWF0pbdd9hB4h2d1YF+I3ctdzpcfUaOmKagmsy10=",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImdkZmdkIiwibmJmIjoxNzA5MzA0ODk5LCJleHAiOjE3MDkzMDU3OTksImlhdCI6MTcwOTMwNDg5OX0.Hw1GmtW4O245qfD11cHOCQtQ91p2inAOlm6cIjL31rU"
}'
```

### Responses

- 200 - Access token is valid

   **Sample Response**
 
  ```json
  {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImdkZmdkIiwibmJmIjoxNzA5NjUwMzgwLCJleHAiOjE3MDk2NTEyODAsImlhdCI6MTcwOTY1MDM4MH0.D-JUimEo_6UDQvGf_ZggyXM_XoXEIaJ6R_RErMK0qa8",
    "result": true,
    "errorDescription": "Success",
    "errorCode": "0000"
  }
  ```
  
- 400 -
  
  ```json
  {
    "result": false,
    "errorDescription": "Invalid access token",
    "errorCode": "0004"
  }
  ```
  
  ```json
  {
    "result": false,
    "errorDescription": "Invalid username and/or expired token",
    "errorCode": "0006"
  }
  ```
  
- 500 - An internal error occurred 

  ```json
  {
    "result": false,
    "errorDescription": "Internal server error",
    "errorCode": "0008"
  }
  ```

## api/v1/user/list-userdata

This endpoint retrieves user information based on username and token.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/user/list-userdata' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImRnZGYiLCJuYmYiOjE3ODAyMzY0NjYsImV4cCI6MTc4MDIzNzM2NiwiaWF0IjoxNzgwMjM2NDY2fQ.6QI1U2Z_CD3r7DkKST6J1kkf2FV2dlHRBK18_eWik4g"
}'
```

### Responses

- 200 - 
  	
  **Sample Response**
  ```json
  {
    "username": "user",
    "refreshToken": "VfEQZ6WrHmhu/dSb+GcFpHsSbDr4XXbdKzHJQDZ3fWQ=",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImRnZGYiLCJuYmYiOjE3ODAyMzY0NjYsImV4cCI6MTc4MDIzNzM2NiwiaWF0IjoxNzgwMjM2NDY2fQ.6QI1U2Z_CD3r7DkKST6J1kkf2FV2dlHRBK18_eWik4g",
    "personalDetail": {
        "birthDate": "2026-05-28T15:27:25.463",
        "gender": "Male",
        "languageOne": "English",
        "languageTwo": "",
        "location": "Gampaha",
        "status": "Single"
    },
    "employmentDetail": [
        {
            "employerName": "Employer2",
            "employerCity": "EmployerCity2",
            "isCurrentEmployer": true,
            "role": "",
            "responsibilities": "Test2",
            "startDate": "2022-05-31T17:37:03",
            "endDate": "2024-05-31T17:37:03"
        },
        {
            "employerName": "Employer1",
            "employerCity": "EmployerCity1",
            "isCurrentEmployer": false,
            "role": "",
            "responsibilities": "Test1",
            "startDate": "2022-02-16T17:37:03",
            "endDate": "2022-05-30T17:37:03"
        }
    ],
    "learnDetail": [
        {
            "institutionName": "Institution1",
            "major": "Major",
            "award": "Award",
            "startYear": 2023,
            "endYear": 2025
        },
        {
            "institutionName": "Institution2",
            "major": "Major",
            "award": "Award",
            "startYear": 2024,
            "endYear": 2025
        }
    ],
    "result": true,
    "error": ""
}
  ```

- 400 -

 ```json
  {
    "result": false,
    "errorDescription": "Invalid username and/or expired token",
    "errorCode": "0006"
  }
  ```

- 500 -

  {
    "result": false,
    "errorCode": "0008",
    "errorDescription": "Internal server error"
  }

## api/v1/user/upsert-userdata

This endpoint saves/updates user information based on username and token.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/user/upsert-userdata' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
    "username": "user1",
    "refreshToken": "VfEQZ6WrHmhu/dSb+GcFpHsSbDr4XXbdKzHJQDZ3fWQ=",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImRnZGYiLCJuYmYiOjE3ODAyMzY0NjYsImV4cCI6MTc4MDIzNzM2NiwiaWF0IjoxNzgwMjM2NDY2fQ.6QI1U2Z_CD3r7DkKST6J1kkf2FV2dlHRBK18_eWik4g",
    "personalDetail": {
        "birthDate": "2026-05-28T15:27:25.463",
        "gender": "Male",
        "languageOne": "English",
        "languageTwo": "",
        "location": "Gampaha",
        "status": "Single"
    },
    "employmentDetail": [
        {
            "employerName": "Employer2",
            "employerCity": "EmployerCity2",
            "isCurrentEmployer": true,
            "role": "",
            "responsibilities": "Test2",
            "startDate": "2022-05-31T17:37:03",
            "endDate": "2024-05-31T17:37:03"
        },
        {
            "employerName": "Employer1",
            "employerCity": "EmployerCity1",
            "isCurrentEmployer": false,
            "role": "",
            "responsibilities": "Test1",
            "startDate": "2022-02-16T17:37:03",
            "endDate": "2022-05-30T17:37:03"
        }
    ],
    "learnDetail": [
        {
            "institutionName": "Institution1",
            "major": "Major",
            "award": "Award",
            "startYear": 2023,
            "endYear": 2025
        },
        {
            "institutionName": "Institution2",
            "major": "Major",
            "award": "Award",
            "startYear": 2024,
            "endYear": 2025
        }
    ]
}'
```

### Responses

- 201 - User information saved
  	
  ```json
  {
    "result": true,
    "errorDescription": "Success",
    "errorCode": "0000"
  }
  ```

- 400 -
  
 ```json
  {
    "result": false,
    "errorDescription": "Invalid username and/or expired token",
    "errorCode": "0006"
  }
  ```

- 500 -

  ```json  
  {
    "result": false,
    "errorCode": "0008",
    "errorDescription": "Internal server error"
  }
  ```
