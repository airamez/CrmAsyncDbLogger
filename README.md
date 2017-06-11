# Microsoft Dynamics CRM Async Database Logger using NLog for Plugins
The idea of this project is to offer async logging capability saving log entries to a database that can easily be used on Microsoft Dynamic CRM plugins and workflow activities.

The NLog logging API is really flexibel, easy to setup and fast. This is the url to the NLog project: http://nlog-project.org/

The only think I did was to create a Wrapper class that can easily be embebed in Dynamics Microsoft CRM using the NLog API to asynchronously write log entries to a database. The real hard work was made by the NLog creators.
The Logging database objects scripts are in the "CrmSyncDbLogger.DatabaseObjects.sql"

## Steps to test
1. Create a SQL Database and execute the 'DatabaseObjects.sql'
2. Execute the CrmAsyncDbLoggerTest.IntegrationTest.Main_01() test
3. If the data is not saved in the check the exception to figure out the issue
4. WHEN USING IN PRODUCTION NEVER SET THE RAISE EXCEPTION TO TRUE

## Addiging to the Plugins DLL
It is required to add the NLog.dll and CrmAsyncDbLogger.dll to the plugin deployment assembler using the ILMerge tool. https://www.microsoft.com/en-ca/download/details.aspx?id=17630

## Important considerations
The string connection provided to the Init method could be retrieved in two ways:
1. From the plugin step configuration (Secure or Unsecure)
2. From a CRM settings entity (Yes, I know that every single Dynamic CRM project has a entity called Settings to store system settings :P). In this case you could easily create e method to retrieve the connection string using the plugin Organization Service object.
