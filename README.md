## 🔍 Motivation

This project was created to test the skills of a developer and I decided to use it as a way to improve my skills.

The backend was implemented at Level 8.
The frontend will be implemented soon.

The original challenge was created by [Aprovame](https://aprovame.com/) and ask to use Javascript, but I made it using .NET 😆

## 🚀 Technologies

- NET 8
- Entity Framework Core
- Identity
- PostgreSQL
- Docker / Docker Compose / Podman (I use this.)
- RabbitMQ
- NUnit3

## 💻 The Problem

A client of Bankme requested a new feature related to receivables.

Every day, this client processes multiple receivables, and our operations team was going crazy having to register all this manually!

Receivables are digital representations of a document that simulates a debt to be received. For Bankme, it is important to have this information as part of the commercial flow we have with this client.

### Requirements

- NET 8
- Docker / Podman
- Python 3 (to generate batches)

In docs folder, you can find python script and the requirements.txt to generate batches.

```shell
pip install --upgrade pip
```

### Structure of a Receivable

| FIELD        | TYPE          | DESCRIPTION                             |
|--------------|---------------|-----------------------------------------|
| id           | string (UUID) | The identification of a receivable.     |
| value        | float         | The value of the receivable.            |
| emissionDate | date          | The emission date of the receivable.    |
| assignor     | string (UUID) | The identification of an assignor.      |

### Structure of an Assignor

| FIELD    | TYPE          | DESCRIPTION                             |
|----------|---------------|-----------------------------------------|
| id       | string (UUID) | The identification of an assignor.      |
| document | string(30)    | The CPF or CNPJ document of the assignor.|
| email    | string(140)   | The email of the assignor.              |
| phone    | string(20)    | The phone number of the assignor.       |
| name     | string(140)   | The name or corporate name of the assignor.|

## 💾 Back-end

### Level 1 - Validation

Implement an API using NestJS that receives data of a receivable and an assignor.

The route for this registration is:

`POST /integrations/payable`

This route should receive all information. It is important to ensure the validation of this data:

1. No field can be null;
2. IDs must be of type UUID;
3. Strings cannot have more characters than defined in their structure;

If any field is not filled out correctly, a message should be returned to the user showing which problem was found in which field.

If all data is validated, just return all the data in JSON format.

### Level 2 - Persistence

Use Prisma to include a new SQLite database.

Create the structure according to what was defined.

If the data is valid, register them.

Create 2 new routes:

`GET /integrations/payable/:id`

`GET /integrations/assignor/:id`

To make it possible to return payables and assignors independently.

Also include routes for other operations:

- Edit;
- Delete;
- Register;

### Level 3 - Tests

Create unit tests for each file of the application. For each new implementation, tests must also be created.

### Level 4 - Authentication

Include an authentication system in all routes.

For this, create a new route:

`POST /integrations/auth` that should receive:

```json
{
  "login": "aprovame",
  "password": "aprovame"
}
```

With these credentials, the endpoint should return a JWT with an expiration time of 1 minute.

Rewrite the rules of all other routes so that the JWT is sent as a parameter in the request `Header`.

If the JWT is valid, then the data should be shown; otherwise, a "Not authorized" message should be displayed.

### Level 5 - Permission Management

Now, create a permission management system.

Create a new permissions registration. This registration must store: `login` and `password`.

Refactor the authentication endpoint so that JWTs are always generated if login and password are registered in the database.

### Level 6 - Infra and Documentation

Create a `Dockerfile` for your API.

Create a `docker-compose.yaml` to start your project.

Document everything that has been done so far:

- How to prepare the environment;
- How to install dependencies;
- How to run the project;

### Level 7 - Batches

Create a new feature for batch processing of payables.

The idea is that the client can send a LARGE number of payables at once. This cannot be processed synchronously.

Create a new endpoint:

`POST /integrations/payable/batch`

In this endpoint, it should be possible to receive batches of up to 10,000 payables.

Upon receiving all payables, they should be posted to a queue.

Create a consumer for this queue that should take each payable, create its record in the database, and at the end of the batch processing, send an email of the processed batch, with the number of successes and failures.

### Level 8 - Resilience

If it is not possible to process an item from the batch, put it back in the queue. This should occur up to 4 times. After that, this item should go to a "Dead Queue" and an email should be sent to the operations team.

### Level 9 - Cloud

Create a deployment pipeline of the application in some Cloud structure (AWS, Google, Azure...).

### Level 10 - Infrastructure as Code

Create a structure in Terraform that sets up the desired infrastructure.

## 🖥️ Front-end

### Level 1 - Registration

Create an interface where it is possible to register the payables.

It is important that your interface prevents the registration of empty fields or fields that do not follow the defined rules.

Display the registered payable on a new screen.

### Level 2 - Connecting to the API

Connect your Front-end to the created API, and make the registration of a payable reflect in your API.

Also, create a screen for the registration of the assignor.

Change the initial registration so that the `assignor` field is a `combobox` where it is possible to select an assignor.

### Level 3 - Listing

Now, create a listing system for payables, showing only: `id`, `value`, and `emissionDate`.

For each item in the list, place a link that shows the details of the payable.

Additionally, include options to edit and delete.

In this detail page, include a new link to display the assignor's data.

All data should come from the API.

### Level 4 - Authentication

Now implement the login and password system to access your routes in an authenticated way.

Store the token in your browser's `localStorage`.

If the token expires, redirect the user to the login page.

### Level 5 - Tests

Create tests for your Front-end application.

