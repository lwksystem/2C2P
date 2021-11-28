# 2C2P Test
<b>Requirements</b>
<p>
  1. SQL Server 2012 or above <br/>
  2. Visual Studio 2019 Commnunity <br/>
  3. .Net Core 3.1 <br/>
  4. Latest Chrome/Edge/Safari/Firefox browser <br/>
  5. Postman or other API clients. <br/>
  
 </p>
 <br/>
 
<b>Setup</b>
<p>
  1. Run the Scripts/CreateTables.sql in SQL Server's query of new or existing database. <br/>
  2. Change the connection string in appsettings.json in 2C2P.Web's project. <br/>
  
 </p>
  <br/>
<b>Test</b>
<p>
  1. Set 2C2P.Web's project as startup and run debugging.<br/>
  2. To test data import by file upload, select the sample file (testcsv.csv or testxml.xml) from the Scripts folder and click Upload. <br/>
  3. To test get output of transaction, please use postman or other API clients: </b> <br/>
  &nbsp;&nbsp;&nbsp;&nbsp;API Endpoint: https://{url}/api/transactions/search  <br/>
    &nbsp;&nbsp;&nbsp;&nbsp;{url} = localhost:port, get from debuggring browser after run the web application. <br/>
     &nbsp;&nbsp;&nbsp;&nbsp;Request Header: Content-Type= application/json<br/>
     &nbsp;&nbsp;&nbsp;&nbsp;Request Body: <br/>
     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   {<br/>
     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   "CurrencyCode": "USD",<br/>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;    "Status": "",<br/>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;    "TransactionDateFrom": "2019/01/01",<br/>
     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   "TransactionDateTo": "2019/02/26" <br/>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; } 
    

  
  
 </p>
