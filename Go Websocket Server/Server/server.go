package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"sync"

	"github.com/gorilla/websocket"
)

// เก็บ WebSocket connections ทั้งหมด
var connections = make(map[*websocket.Conn]bool)
var mu sync.Mutex // Mutex to avoid concurrent access issues

// Create a map (key-value pairs) of platform states
var platformState = map[string]bool{
	"platformer1":  false,
	"platformer2":  false,
	"platformer3":  false,
	"platformer4":  false,
	"platformer5":  false,
	"platformer6":  false,
	"platformer7":  false,
	"platformer8":  false,
	"platformer9":  false,
	"platformer10": false,
}

// Define WebSocket upgrader to handle WebSocket connections
var upgrader = websocket.Upgrader{
	CheckOrigin: func(r *http.Request) bool {
		// Allow all connections (for development purposes)
		return true
	},
}

// WebSocket connection handler
func handleConnection(w http.ResponseWriter, r *http.Request) {
	// Upgrade HTTP connection to WebSocket
	conn, err := upgrader.Upgrade(w, r, nil)
	if err != nil {
		log.Println("Error upgrading connection:", err)
		return
	}
	defer conn.Close() // Ensure connection is properly closed after use

	// เพิ่ม connection ที่เชื่อมต่อใหม่
	connections[conn] = true

	// ตรวจสอบการปิด connection
	defer func() {
		delete(connections, conn) // ลบ connection เมื่อเชื่อมต่อเสร็จ
	}()

	// Lock the connections map to add the new client
	mu.Lock()
	connections[conn] = true
	mu.Unlock()

	sendPlatformStateToAll(conn, websocket.TextMessage)

	// Start a loop to continuously handle messages
	for {
		messageType, message, err := conn.ReadMessage()
		if err != nil {
			log.Println("Error reading message:", err)
			break // Exit the loop on error
		}

		log.Printf("Received: %s\n", string(message))

		// Handle incoming JSON updates
		err = handlePlatformUpdate(message)
		if err != nil {
			log.Println("Error processing update:", err)
			continue
		}

		// Send the updated platform state back to the client
		sendPlatformStateToAll(conn, messageType)
	}
}

// Function to handle incoming platform updates from the client
func handlePlatformUpdate(message []byte) error {
	// Deserialize the JSON message into the platformState map
	var updatedState map[string]bool
	err := json.Unmarshal(message, &updatedState)
	if err != nil {
		return fmt.Errorf("error unmarshaling message: %w", err)
	}

	// Update the global platformState map
	for key, value := range updatedState {
		if _, exists := platformState[key]; exists {
			platformState[key] = value
			log.Printf("Platform '%s' updated to %v", key, value)
		} else {
			log.Printf("Unknown platform key: %s", key)
		}
	}
	return nil
}

// Function to send platform state data over WebSocket
func sendPlatformStateToAll(conn *websocket.Conn, messageType int) {
	// Serialize the platformState map to JSON
	data, err := json.Marshal(platformState)
	if err != nil {
		log.Println("Error marshaling data:", err)
		return
	}

	// Lock the map before accessing it
	mu.Lock()
	defer mu.Unlock()

	// ส่งข้อมูลไปยังทุก client
	for conn := range connections {
		err := conn.WriteMessage(messageType, data)
		if err != nil {
			log.Println("Error sending message:", err)
		}
	}
}

func main() {
	// WebSocket server
	http.HandleFunc("/ws", handleConnection)

	fmt.Println("Server started at ws://localhost:8080")
	log.Fatal(http.ListenAndServe(":8080", nil))
}
