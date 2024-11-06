# WalletWise

**WalletWise** is a personal finance management platform designed to help users make smarter financial decisions regarding spending and investments. Featuring real-time financial analysis, investment recommendations, and an AI-powered chatbot, WalletWise empowers users to take control of their finances.

## Table of Contents
- [Features](#features)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Usage](#usage)
- [License](#license)


## Features
- Financial Dashboard: Track your income, expenses, investments, and overall financial health.
- Real-Time Financial Analysis: Get insights into your spending patterns and receive tailored investment recommendations.
- AI-Powered Chatbot: ARIA, the AI chatbot, provides personalized advice based on user data and responds to financial queries.

## System Requirements
**Unity:** Version 6.0 LTS
**Ollama:** For running the ARIA chatbot model locally


## Installation
1. **Clone the Repository**
To get started with WalletWise, clone the repository:

```
git clone https://github.com/arjunGGGG/WalletWise.git
```

2. **Open the Project in Unity**
- Open Unity Hub.
- Select Open Project and navigate to the cloned WalletWise folder.
- Unity will automatically download dependencies required for the project.

3. **Run ARIA Locally**
Ensure [Ollama]{https://ollama.com/download} is installed on your machine. Start the ARIA model locally:

```
ollama run arjungupta/aria:27b
ollama serve
```

## Usage
- Launch WalletWise: Open the project in Unity and select Play in the editor, or build it for your target platform.
- Interact with the Financial Dashboard: View and track your financial details.
- Use the AI Chatbot: Ask questions about finances, spending, or investments. The chatbot uses the ARIA model to provide personalized responses.


## License
> This project is licensed under the MIT License.