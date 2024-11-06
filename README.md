# WalletWise

**Wallet Wise** is a tool for managing personal finances that aims to give users the ability to make informed decisions about buying and investing. Wallet Wise is accessible as both a mobile application and a browser extension. It utilizes information from multiple sources like income, credit cards, & investments to offer personalized financial guidance. The app analyzes the user's financial well-being and spending habits before they buy something, providing suggestions on the financial feasibility of the expense. Furthermore, it offers investment guidance, including information on stocks and mutual funds. It utilizes user-specific financial information, it will offer tailored responses to financial questions through a chatbot, promoting improved financial sense. This device is created to allow the user to spend wisely, decrease spontaneous purchases, and steer users towards long-term finance planning.

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
- **Unity:** Version 6.0 LTS
- **Ollama:** For running the ARIA chatbot model locally


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
