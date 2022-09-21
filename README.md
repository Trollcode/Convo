# Convo
C# Chatbot library for basic conversations

Convo.Abstractions - Base statefull chatbot classes, logic and abstractions
Convo.Telegram  - Implementation of telegram library into the statefull chatbot lib
Convo.Telegram.Chatmenu - Implementation of Telegram Specific menu



CONVO_BOT_SECRET


### Example settings for example
```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet"
    }
}
```

## Notes

Have a look if you can create a custom binding for Telegram instead of injecting the context and calling it manually.