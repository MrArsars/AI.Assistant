🤖 AI.Assistant (Gemini Telegram Bot)
AI.Assistant is an intelligent Telegram bot built on .NET 8 and powered by Microsoft Semantic Kernel. The bot leverages the advanced capabilities of the Google Gemini model for natural conversations and Supabase for persistent chat history and long-term user context (memory).

The project is designed following Clean Architecture principles, utilizing Dependency Injection (DI) and a modular service-oriented structure, making it highly scalable and easy to maintain.

✨ Key Features
🧠 Intelligent Dialogues: Natural, context-aware conversations powered by Google Gemini AI.

💾 Long-term Memory: The bot "remembers" important facts about the user (e.g., name, preferences, details) using a dedicated Native Plugin that interfaces with Supabase.

📜 Chat History Management: Automatic saving and loading of chat history per user to maintain conversation flow across sessions.

🔌 Semantic Kernel Plugins: Extends AI capabilities with custom C# functions (Native Functions), allowing the LLM to interact directly with your database.

🏗️ Robust Architecture: Clear separation of concerns between Repositories, Services, and Handlers.

🛠 Tech Stack
Language: C# (.NET 8)

AI Orchestration: Microsoft Semantic Kernel

LLM: Google Gemini 1.5 Flash / Pro

Database & Auth: Supabase (PostgreSQL)

Telegram API: Telegram.Bot

Dependency Injection: Microsoft.Extensions.DependencyInjection

🏗 Architecture Overview
The project is implemented with a focus on modern software engineering practices:

Repository Pattern: Data access logic is isolated in MessagesRepository and ContextRepository, decoupling the database provider from the business logic.

Service Layer: Core business logic is encapsulated within specialized services like ChatService and HistoryService.

Plugin-based AI: AI capabilities are expanded via ContextPlugin, allowing the model to trigger C# methods for data persistence.

Fluent Extensions: Object mapping and repetitive tasks are handled through Extension Methods to keep the main codebase clean and readable.

🚀 Quick Start
1. Clone the repository
Bash
git clone https://github.com/MrArsars/AI.Assistant.git
cd AI.Assistant
2. Configuration
Create an appsettings.json file in the root directory and add your credentials:

JSON
{
  "TelegramBotToken": "YOUR_TELEGRAM_BOT_TOKEN",
  "GeminiApiToken": "YOUR_GEMINI_API_KEY",
  "GeminiModel": "gemini-1.5-flash",
  "SupabaseUrl": "https://your-project.supabase.co",
  "SupabaseApiToken": "YOUR_SUPABASE_ANON_KEY",
  "HistoryMessagesLimit": 10
}
3. Run the application
Bash
dotnet run
📂 Project Structure
Handlers/ — Logic for processing incoming Telegram updates and managing the main bot loop.

Services/ — Business logic for history management and AI interaction.

Repositories/ — Supabase data access layer and database models.

Plugins/ — Native functions for Semantic Kernel (Tool calling).

Models/ — Data Transfer Objects (DTOs) and database schemas.

Extensions/ — Helper methods for mapping and cleaner syntax.

📈 Roadmap
[ ] Proactive Memory: Implement logic where the bot suggests remembering important information autonomously.

[ ] Cold Storage Retrieval: Advanced logic to pull relevant facts from a large knowledge base only when necessary.

[ ] Multimedia Support: Process images and voice messages using Gemini Vision capabilities.

[ ] Structured Logging: Integration with Serilog or Microsoft.Extensions.Logging for professional monitoring.
