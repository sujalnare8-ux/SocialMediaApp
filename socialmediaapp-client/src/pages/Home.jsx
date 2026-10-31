import { useNavigate } from "react-router-dom";

export default function Home() {
  const navigate = useNavigate();
  const name = localStorage.getItem("displayName");

  const logout = () => {
    localStorage.clear();
    navigate("/login");
  };

  return (
    <div style={{ maxWidth: 500, margin: "60px auto" }}>
      <h2>Welcome, {name}!</h2>
      <p>You are logged in. The feed will go here soon.</p>
      <button onClick={logout}>Logout</button>
    </div>
  );
}