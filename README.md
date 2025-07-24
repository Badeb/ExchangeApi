# ExchangeApi


📌 Goal
Build a RESTful API using .NET (C#) that allows users to:

Fetch current exchange rates between two currencies using an external public API
https://exchangerate.host/login
freeapi45@mailinator.com
Login to Your Exchangerate Account | Exchangerate API

Save frequently used currency pairs (e.g., USD/EUR) as reusable queries 
Store each exchange result in the database with the time it was fetched 
Retrieve the history of fetched results for each saved query 

 🧱 Features

GET /api/exchange-rate?base=USD&target=EUR
➤ Calls the external API and stores the result in the database. 

POST /api/queries
➤ Saves a currency pair to track regularly. 

GET /api/queries
➤ Lists saved currency pairs. 

GET /api/results?pairId=1
➤ Returns previous fetched results for the selected currency pair. 

 🧰 Tech Requirements

Use .NET 8 Web API  
Use EF Core with Ms Sql Server.  You can download database this database: SQL Server 2022 Express. SQL Server Downloads | Microsoft
Implement Swagger for the application
Structure the project with proper layering (Controllers, Services, Data) 

 🧭 Tips

Use HttpClient to call the external API.  
Use DateTime.UtcNow to timestamp each fetched rate.  
Keep the code clean, modular, and well-documented. 
Optional: Add basic input validation and error handling.
