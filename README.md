# 🤖 AI.Assistant (Gemini Telegram Bot)

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Semantic Kernel](https://img.shields.io/badge/Semantic--Kernel-Microsoft-blue?logo=microsoft)](https://github.com/microsoft/semantic-kernel)
[![Gemini AI](https://img.shields.io/badge/Model-Gemini--2.5--Flash-orange?logo=google-gemini)](https://deepmind.google/technologies/gemini/)
[![Supabase](https://img.shields.io/badge/Database-Supabase-green?logo=supabase)](https://supabase.com/)

An intelligent Telegram bot built with **.NET 8** and **Microsoft Semantic Kernel**. The bot leverages **Google Gemini** for natural conversations and **Supabase** for persistent chat history and long-term user memory.

---

## ✨ Key Features

* **🧠 Intelligent Dialogues** – Seamless context-aware conversations using Gemini 1.5.
* **💾 Long-term Memory** – Remembers user facts (name, hobbies, etc.) using a custom **ContextPlugin**.
* **📜 History Management** – Persists chat history in Supabase, loading it automatically for each session.
* **🔌 Semantic Kernel Plugins** – Built-in support for tool calling (Native Functions).
* **🏗 Clean Architecture** – Professional structure with Repository and Service patterns.

---

## 🛠 Tech Stack

| Component          | Technology                                                                 |
| :----------------- | :------------------------------------------------------------------------- |
| **Framework** | .NET 8.0                                                                   |
| **AI Orchestrator**| Microsoft Semantic Kernel                                                  |
| **LLM** | Google Gemini 2.5 Flash                                                    |
| **Database** | Supabase (PostgreSQL)                                                      |
| **Telegram API** | [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)               |
| **DI Container** | Microsoft.Extensions.DependencyInjection                                   |

---

## 🏗 Project Architecture

The project follows a modular approach to ensure scalability and testability:

* **`Handlers/`** – Manages the Telegram update loop and message routing.
* **`Services/`** – Contains business logic for history orchestration and AI processing.
* **`Repositories/`** – Direct data access layer for Supabase (Messages & Context).
* **`Plugins/`** – Native C# functions that the AI can call (Tool calling).
* **`Extensions/`** – Fluent mapping for Telegram objects and database models.

---

## 🚀 Getting Started

### 1. Prerequisites
* .NET 8 SDK
* Telegram Bot Token (from @BotFather)
* Google AI API Key (Gemini)
* Supabase Project (URL and Anon Key)

### 2. Configuration
Create an `appsettings.json` file in the root directory:

```json
{
  "TelegramBotToken": "YOUR_TELEGRAM_TOKEN",
  "GeminiApiToken": "YOUR_GEMINI_KEY",
  "GeminiModel": "gemini-1.5-flash",
  "SupabaseUrl": "[https://your-project.supabase.co](https://your-project.supabase.co)",
  "SupabaseApiToken": "YOUR_SUPABASE_KEY",
  "HistoryMessagesLimit": 10
}
```
### 3. Build & Run
```bash
dotnet build
dotnet run
```
## 📈 Roadmap

- [ ] **Proactive Memory** – Bot suggests facts to remember autonomously.
- [x] **Repository Pattern** – Abstracted data access.
- [ ] **Cold Storage Retrieval** – Pulling relevant facts from a large DB only when needed.
- [ ] **Multimedia** – Vision support for images and documents.
- [ ] **Logging** – Professional monitoring with Serilog.

---

> Built with ❤️ by [MrArsars](https://github.com/MrArsars)
