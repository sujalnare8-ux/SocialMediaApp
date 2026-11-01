import { useEffect, useRef, useState } from "react";
import { Link } from "react-router-dom";
import { HubConnectionBuilder } from "@microsoft/signalr";
import api from "../api/api";

const toDate = (s) => new Date(s.endsWith("Z") ? s : s + "Z");

export default function Chat() {
  const [users, setUsers] = useState([]);
  const [selected, setSelected] = useState(null);
  const [messages, setMessages] = useState([]);
  const [text, setText] = useState("");
  const [status, setStatus] = useState("Connecting...");

  const connRef = useRef(null);
  const selectedRef = useRef(null);
  const bottomRef = useRef(null);

  const myId = JSON.parse(atob(localStorage.getItem("token").split(".")[1]))[
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
  ];

  // Load the list of people to chat with
  useEffect(() => {
    api.get("/chat/users").then((res) => setUsers(res.data));
  }, []);

  // Connect to the SignalR hub once
  useEffect(() => {
    const conn = new HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_URL ?? "http://localhost:5165"}/hubs/chat`, {        accessTokenFactory: () => localStorage.getItem("token"),
      })
      .withAutomaticReconnect()
      .build();

    conn.on("ReceiveMessage", (msg) => {
      const other = selectedRef.current;
      // Show it only if it belongs to the open conversation
      if (other && (msg.senderId === other || msg.receiverId === other)) {
        setMessages((prev) =>
          prev.some((m) => m.id === msg.id) ? prev : [...prev, msg]
        );
      }
    });

    conn
      .start()
      .then(() => setStatus("Connected"))
      .catch(() => {});

    connRef.current = conn;
    return () => {
      conn.stop();
    };
  }, []);

  // Scroll to the newest message
  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const openChat = async (user) => {
    setSelected(user);
    selectedRef.current = user.id;
    const res = await api.get(`/chat/${user.id}`);
    setMessages(res.data);
  };

  const send = async (e) => {
    e.preventDefault();
    if (!text.trim() || !selected) return;
    try {
      await connRef.current.invoke("SendMessage", selected.id, text);
      setText("");
    } catch (err) {
      setStatus("Could not send. Check your connection.");
    }
  };

  return (
    <div style={{ maxWidth: 700, margin: "40px auto", padding: 10 }}>
      <Link to="/">← Back to feed</Link>
      <h2>Chat <small style={{ fontSize: 14 }}>({status})</small></h2>

      <div style={{ display: "flex", gap: 16 }}>
        <div style={{ width: 180 }}>
          <strong>People</strong>
          {users.length === 0 && <p>No other users yet.</p>}
          {users.map((u) => (
            <div key={u.id} style={{ margin: "6px 0" }}>
              <button
                onClick={() => openChat(u)}
                disabled={selected?.id === u.id}
                style={{ width: "100%" }}
              >
                {u.displayName}
              </button>
            </div>
          ))}
        </div>

        <div style={{ flex: 1 }}>
          {!selected && <p>Pick a person to start chatting.</p>}
          {selected && (
            <>
              <strong>Chat with {selected.displayName}</strong>
              <div
                style={{
                  border: "1px solid #888",
                  borderRadius: 6,
                  height: 300,
                  overflowY: "auto",
                  padding: 8,
                  margin: "8px 0",
                }}
              >
                {messages.length === 0 && <small>No messages yet. Say hi!</small>}
                {messages.map((m) => (
                  <div
                    key={m.id}
                    style={{ textAlign: m.senderId === myId ? "right" : "left", margin: "6px 0" }}
                  >
                    <span
                      style={{
                        display: "inline-block",
                        padding: "6px 10px",
                        borderRadius: 12,
                        background: m.senderId === myId ? "#2f6fed" : "#444",
                        color: "white",
                      }}
                    >
                      {m.content}
                    </span>
                    <br />
                    <small>{toDate(m.sentAt).toLocaleTimeString()}</small>
                  </div>
                ))}
                <div ref={bottomRef} />
              </div>

              <form onSubmit={send}>
                <input
                  style={{ width: "75%" }}
                  placeholder="Type a message..."
                  value={text}
                  maxLength={1000}
                  onChange={(e) => setText(e.target.value)}
                />{" "}
                <button type="submit">Send</button>
              </form>
            </>
          )}
        </div>
      </div>
    </div>
  );
}