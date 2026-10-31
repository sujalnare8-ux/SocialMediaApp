import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import api from "../api/api";

export default function Register() {
  const [displayName, setDisplayName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    try {
      const res = await api.post("/auth/register", { displayName, email, password });
      localStorage.setItem("token", res.data.token);
      localStorage.setItem("displayName", res.data.displayName);
      navigate("/");
    } catch (err) {
      const data = err.response?.data;
      setError(Array.isArray(data) ? data.join(" ") : "Registration failed.");
    }
  };

  return (
    <div style={{ maxWidth: 350, margin: "60px auto" }}>
      <h2>Register</h2>
      <form onSubmit={handleSubmit}>
        <input placeholder="Display name" value={displayName}
          onChange={(e) => setDisplayName(e.target.value)} required />
        <br /><br />
        <input type="email" placeholder="Email" value={email}
          onChange={(e) => setEmail(e.target.value)} required />
        <br /><br />
        <input type="password" placeholder="Password (min 6)" value={password}
          onChange={(e) => setPassword(e.target.value)} required />
        <br /><br />
        <button type="submit">Create account</button>
      </form>
      {error && <p style={{ color: "red" }}>{error}</p>}
      <p>Already have an account? <Link to="/login">Login</Link></p>
    </div>
  );
}