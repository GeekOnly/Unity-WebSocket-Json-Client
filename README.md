# WebSocket Client for Unity x GPT

This project demonstrates how to implement a WebSocket client in Unity that communicates with a server, sending platform updates based on user input and receiving platform state updates from the server.
![image](https://github.com/user-attachments/assets/5f8477e9-5765-40c0-a815-12a3b7efa62f)
## Features

- **WebSocket Connection**: Establishes a WebSocket connection to a server.
- **User Input Handling**: Sends updates to the server when the user presses keys (1-0).
- **State Updates**: Receives platform state updates from the server and applies them to the game.
- **Platform State Management**: Sends and receives platform activation states.
- **Real-Time Communication**: Continuously listens for updates and reacts in real time.

## Requirements

- Unity 2020.3 or higher.
- Newtonsoft.Json for JSON serialization/deserialization.
- A WebSocket server running at `ws://localhost:8080/ws`.

## Getting Started

Follow these steps to set up and run the project:

### 1. Clone the Repository

```bash
git clone https://github.com/GeekOnly/Unity-WebSocket-Json-Client.git
cd Unity-WebSocket-Json-Client
