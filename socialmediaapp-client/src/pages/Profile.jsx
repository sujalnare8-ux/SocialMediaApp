import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import api from "../api/api";

export default function Profile() {
  const { id } = useParams();
  const [profile, setProfile] = useState(null);
  const [posts, setPosts] = useState([]);
  const [editing, setEditing] = useState(false);
  const [displayName, setDisplayName] = useState("");
  const [bio, setBio] = useState("");
  const [error, setError] = useState("");

  const load = async () => {
    try {
      const p = await api.get(`/users/${id}`);
      setProfile(p.data);
      setDisplayName(p.data.displayName);
      setBio(p.data.bio || "");
      const res = await api.get(`/users/${id}/posts`);
      setPosts(res.data);
    } catch (err) {
      setError("Could not load this profile.");
    }
  };

  useEffect(() => {
    load();
  }, [id]);

  const toggleFollow = async () => {
    const res = await api.post(`/users/${id}/follow`);
    setProfile({
      ...profile,
      isFollowing: res.data.following,
      followerCount: res.data.followerCount,
    });
  };

  const saveProfile = async (e) => {
    e.preventDefault();
    try {
      await api.put("/users/me", { displayName, bio });
      localStorage.setItem("displayName", displayName);
      setProfile({ ...profile, displayName, bio });
      setEditing(false);
    } catch (err) {
      setError("Could not save profile.");
    }
  };

  if (error) return <p style={{ color: "red", textAlign: "center" }}>{error}</p>;
  if (!profile) return <p style={{ textAlign: "center" }}>Loading...</p>;

  return (
    <div style={{ maxWidth: 600, margin: "40px auto", padding: 10 }}>
      <Link to="/">← Back to feed</Link>

      <h2>{profile.displayName}</h2>
      <p>{profile.bio || "No bio yet."}</p>
      <p>
        <strong>{profile.postCount}</strong> posts ·{" "}
        <strong>{profile.followerCount}</strong> followers ·{" "}
        <strong>{profile.followingCount}</strong> following
      </p>

      {profile.isMe ? (
        editing ? (
          <form onSubmit={saveProfile}>
            <input
              value={displayName}
              maxLength={50}
              onChange={(e) => setDisplayName(e.target.value)}
              required
            />
            <br /><br />
            <textarea
              rows={3}
              style={{ width: "100%" }}
              placeholder="Write something about yourself"
              value={bio}
              maxLength={300}
              onChange={(e) => setBio(e.target.value)}
            />
            <button type="submit">Save</button>{" "}
            <button type="button" onClick={() => setEditing(false)}>Cancel</button>
          </form>
        ) : (
          <button onClick={() => setEditing(true)}>Edit profile</button>
        )
      ) : (
        <button onClick={toggleFollow}>
          {profile.isFollowing ? "Unfollow" : "Follow"}
        </button>
      )}

      <hr />
      <h3>Posts</h3>
      {posts.length === 0 && <p>No posts yet.</p>}
      {posts.map((p) => (
        <div key={p.id} style={{ border: "1px solid #888", borderRadius: 6, padding: 10, marginBottom: 10 }}>
          <small>{new Date(p.createdAt + "Z").toLocaleString()}</small>
          <p>{p.content}</p>
          <small>{p.likeCount} likes · {p.commentCount} comments</small>
        </div>
      ))}
    </div>
  );
}