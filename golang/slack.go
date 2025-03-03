package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"net/http"
)

const SlackWebhookURL = "https://slack.com/api/chat.postMessage"

func SendSlackMessage(message string, channel string) error {
	payload := map[string]string{"text": message,"channel":channel}
	jsonData, err := json.Marshal(payload)
	if err != nil {
		return fmt.Errorf("failed to encode JSON: %v", err)
	}
	resp, err := http.Post(SlackWebhookURL, "application/json", bytes.NewBuffer(jsonData))
	if err != nil {
		return fmt.Errorf("HTTP request failed: %v", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusNoContent {
		return fmt.Errorf("Slack API returned an error: %d", resp.StatusCode)
	}

	return nil
}

func main() {
	err := SendSlackMessage("Hi slack", "HelloChannel")
	if err != nil {
		fmt.Println("Error sending message:", err)
	} else {
		fmt.Println("Message sent successfully!")
	}
}
