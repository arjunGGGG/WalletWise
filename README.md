#WalletWise
##WalletWise is a personal finance management app designed to help users make informed financial decisions. It offers real-time insights, investment recommendations, and an AI-powered chatbot for financial advice.

##Features
###Financial Dashboard:
Track income, expenses, and investments.
###AI Chatbot:
Get financial advice with ARIA, powered by local AI.
Cross-Platform Support: Build for mobile, web, and desktop via Unity.
System Requirements
**Unity**: 6
**Ollama**: For running the ARIA chatbot model locally

##Installation
###Clone the Repo:

```
git clone https://github.com/arjunGGGG/WalletWise.git
cd WalletWise
```
Open in Unity:
Open Unity Hub, select Open Project, and navigate to the WalletWise folder.

##Run ARIA Model: Start ARIA locally with Ollama:

```
ollama run arjungupta/aria:27b
```

Usage
Dashboard: View financial data.
Chatbot: Ask financial questions to ARIA.
Options: Set API endpoint IP and port in the chatbot options screen.
Project Structure
Assets/Scenes: Main scenes for WalletWise.
Assets/Scripts: Key scripts include Chat.cs (Chatbot) and OptionsManager.cs (API settings).
