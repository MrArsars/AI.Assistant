# 🤖 AI.Assistant (Gemini Telegram Bot)

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512bd4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Semantic Kernel](https://img.shields.io/badge/Semantic--Kernel-Microsoft-blue?logo=microsoft)](https://github.com/microsoft/semantic-kernel)
[![Gemini AI](https://img.shields.io/badge/Model-Gemini--2.5--Flash-orange?logo=google-gemini)](https://deepmind.google/technologies/gemini/)
[![Supabase](https://img.shields.io/badge/Database-Supabase-green?logo=supabase)](https://supabase.com/)
[![Tavily](https://img.shields.io/badge/Search-Tavily-blue?logo=dataai)](https://tavily.com/)
[![AssemblyAI](https://img.shields.io/badge/AI-AssemblyAI-orange?logo=assemblyai)](https://www.assemblyai.com/)

An intelligent Telegram bot built with **.NET 10** and **Microsoft Semantic Kernel**. The bot leverages **Google Gemini** for natural conversations and **Supabase** for persistent chat history and long-term user memory.

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
| **Framework** | .NET 10.0                                                                   |
| **AI Orchestrator**| Microsoft Semantic Kernel                                                  |
| **LLM** | Google Gemini 2.5 Flash                                                    |
| **Database** | Supabase (PostgreSQL)                                                      |
| **Telegram API** | [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)               |
| **DI Container** | Microsoft.Extensions.DependencyInjection                                   |

---


> Built with ❤️ by [MrArsars](https://github.com/MrArsars)
